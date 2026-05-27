using System.Globalization;
using System.Windows.Input;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public sealed class CodexCalendarSelectedDateChangedEventArgs(DateTime? oldDate, DateTime? newDate)
    : EventArgs
{
    public DateTime? OldDate { get; } = oldDate;

    public DateTime? NewDate { get; } = newDate;
}

public sealed class CodexCalendarRangeChangedEventArgs(
    DateTime? oldStart,
    DateTime? oldEnd,
    DateTime? newStart,
    DateTime? newEnd)
    : EventArgs
{
    public DateTime? OldStart { get; } = oldStart;

    public DateTime? OldEnd { get; } = oldEnd;

    public DateTime? NewStart { get; } = newStart;

    public DateTime? NewEnd { get; } = newEnd;

    public bool IsComplete => NewStart.HasValue && NewEnd.HasValue;
}

public sealed class CodexCalendarDisplayDateChangedEventArgs(DateTime oldDisplayDate, DateTime newDisplayDate, int monthDelta)
    : EventArgs
{
    public DateTime OldDisplayDate { get; } = oldDisplayDate;

    public DateTime NewDisplayDate { get; } = newDisplayDate;

    public int MonthDelta { get; } = monthDelta;
}

public sealed class CodexCalendarActiveDateChangedEventArgs(DateTime? oldDate, DateTime? newDate)
    : EventArgs
{
    public DateTime? OldDate { get; } = oldDate;

    public DateTime? NewDate { get; } = newDate;
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexCalendar : ItemsControl
{
    private readonly CalendarPartCommand _previousMonthCommand;
    private readonly CalendarPartCommand _nextMonthCommand;
    private string _monthTitle = FormatMonthTitle(FirstDayOfMonth(DateTime.Today));
    private bool _isRebuilding;

    public static readonly StyledProperty<DateTime> DisplayDateProperty =
        AvaloniaProperty.Register<CodexCalendar, DateTime>(nameof(DisplayDate), FirstDayOfMonth(DateTime.Today));

    public static readonly DirectProperty<CodexCalendar, string> MonthTitleProperty =
        AvaloniaProperty.RegisterDirect<CodexCalendar, string>(nameof(MonthTitle), calendar => calendar.MonthTitle);

    public static readonly StyledProperty<DateTime?> SelectedDateProperty =
        AvaloniaProperty.Register<CodexCalendar, DateTime?>(nameof(SelectedDate));

    public static readonly StyledProperty<DateTime?> RangeStartProperty =
        AvaloniaProperty.Register<CodexCalendar, DateTime?>(nameof(RangeStart));

    public static readonly StyledProperty<DateTime?> RangeEndProperty =
        AvaloniaProperty.Register<CodexCalendar, DateTime?>(nameof(RangeEnd));

    public static readonly StyledProperty<DateTime?> MinDateProperty =
        AvaloniaProperty.Register<CodexCalendar, DateTime?>(nameof(MinDate));

    public static readonly StyledProperty<DateTime?> MaxDateProperty =
        AvaloniaProperty.Register<CodexCalendar, DateTime?>(nameof(MaxDate));

    public static readonly StyledProperty<IReadOnlyList<DateTime>?> BookedDatesProperty =
        AvaloniaProperty.Register<CodexCalendar, IReadOnlyList<DateTime>?>(nameof(BookedDates));

    public static readonly StyledProperty<DateTime?> ActiveDateProperty =
        AvaloniaProperty.Register<CodexCalendar, DateTime?>(nameof(ActiveDate));

    public static readonly StyledProperty<CodexCalendarSelectionMode> SelectionModeProperty =
        AvaloniaProperty.Register<CodexCalendar, CodexCalendarSelectionMode>(nameof(SelectionMode), CodexCalendarSelectionMode.Single);

    public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty =
        AvaloniaProperty.Register<CodexCalendar, DayOfWeek>(nameof(FirstDayOfWeek), DayOfWeek.Sunday);

    public static readonly StyledProperty<bool> ShowOutsideDaysProperty =
        AvaloniaProperty.Register<CodexCalendar, bool>(nameof(ShowOutsideDays), true);

    public static readonly StyledProperty<bool> ShowWeekNumbersProperty =
        AvaloniaProperty.Register<CodexCalendar, bool>(nameof(ShowWeekNumbers));

    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexCalendar, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexCalendar, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexCalendar()
    {
        DisplayDateProperty.Changed.AddClassHandler<CodexCalendar>((calendar, args) => calendar.OnDisplayDateChanged(args));
        SelectedDateProperty.Changed.AddClassHandler<CodexCalendar>((calendar, args) => calendar.OnSelectedDateChanged(args));
        RangeStartProperty.Changed.AddClassHandler<CodexCalendar>((calendar, args) => calendar.OnRangeStartChanged(args));
        RangeEndProperty.Changed.AddClassHandler<CodexCalendar>((calendar, args) => calendar.OnRangeEndChanged(args));
        MinDateProperty.Changed.AddClassHandler<CodexCalendar>((calendar, _) => calendar.RebuildCalendar());
        MaxDateProperty.Changed.AddClassHandler<CodexCalendar>((calendar, _) => calendar.RebuildCalendar());
        BookedDatesProperty.Changed.AddClassHandler<CodexCalendar>((calendar, _) => calendar.RebuildCalendar());
        ActiveDateProperty.Changed.AddClassHandler<CodexCalendar>((calendar, args) => calendar.OnActiveDateChanged(args));
        SelectionModeProperty.Changed.AddClassHandler<CodexCalendar>((calendar, _) => calendar.OnSelectionModeChanged());
        FirstDayOfWeekProperty.Changed.AddClassHandler<CodexCalendar>((calendar, _) => calendar.RebuildCalendar());
        ShowOutsideDaysProperty.Changed.AddClassHandler<CodexCalendar>((calendar, _) => calendar.RebuildCalendar());
        ShowWeekNumbersProperty.Changed.AddClassHandler<CodexCalendar>((calendar, _) => calendar.RebuildCalendar());
        IntentProperty.Changed.AddClassHandler<CodexCalendar>((calendar, _) => calendar.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexCalendar>((calendar, _) => calendar.RebuildCalendar());
    }

    public CodexCalendar()
    {
        Focusable = true;
        _previousMonthCommand = new CalendarPartCommand(this, calendar => calendar.CanGoPreviousMonth, calendar => calendar.NavigatePreviousMonth());
        _nextMonthCommand = new CalendarPartCommand(this, calendar => calendar.CanGoNextMonth, calendar => calendar.NavigateNextMonth());
        AutomationProperties.SetIsControlElementOverride(this, true);
        RebuildCalendar();
    }

    public event EventHandler<CodexCalendarSelectedDateChangedEventArgs>? SelectedDateChanged;

    public event EventHandler<CodexCalendarRangeChangedEventArgs>? RangeChanged;

    public event EventHandler<CodexCalendarDisplayDateChangedEventArgs>? DisplayDateChanged;

    public event EventHandler<CodexCalendarActiveDateChangedEventArgs>? ActiveDateChanged;

    public DateTime DisplayDate
    {
        get => GetValue(DisplayDateProperty);
        set => SetValue(DisplayDateProperty, FirstDayOfMonth(value));
    }

    public DateTime? SelectedDate
    {
        get => GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, DateOnly(value));
    }

    public DateTime? RangeStart
    {
        get => GetValue(RangeStartProperty);
        set => SetValue(RangeStartProperty, DateOnly(value));
    }

    public DateTime? RangeEnd
    {
        get => GetValue(RangeEndProperty);
        set => SetValue(RangeEndProperty, DateOnly(value));
    }

    public DateTime? MinDate
    {
        get => GetValue(MinDateProperty);
        set => SetValue(MinDateProperty, DateOnly(value));
    }

    public DateTime? MaxDate
    {
        get => GetValue(MaxDateProperty);
        set => SetValue(MaxDateProperty, DateOnly(value));
    }

    public IReadOnlyList<DateTime>? BookedDates
    {
        get => GetValue(BookedDatesProperty);
        set => SetValue(BookedDatesProperty, value?.Select(date => DateOnly(date)).Where(date => date.HasValue).Select(date => date!.Value).ToArray());
    }

    public DateTime? ActiveDate
    {
        get => GetValue(ActiveDateProperty);
        set => SetValue(ActiveDateProperty, DateOnly(value));
    }

    public CodexCalendarSelectionMode SelectionMode
    {
        get => GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    public DayOfWeek FirstDayOfWeek
    {
        get => GetValue(FirstDayOfWeekProperty);
        set => SetValue(FirstDayOfWeekProperty, value);
    }

    public bool ShowOutsideDays
    {
        get => GetValue(ShowOutsideDaysProperty);
        set => SetValue(ShowOutsideDaysProperty, value);
    }

    public bool ShowWeekNumbers
    {
        get => GetValue(ShowWeekNumbersProperty);
        set => SetValue(ShowWeekNumbersProperty, value);
    }

    public CodexControlIntent Intent
    {
        get => GetValue(IntentProperty);
        set => SetValue(IntentProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public string MonthTitle
    {
        get => _monthTitle;
        private set => SetAndRaise(MonthTitleProperty, ref _monthTitle, value);
    }

    public ICommand PreviousMonthCommand => _previousMonthCommand;

    public ICommand NextMonthCommand => _nextMonthCommand;

    public bool CanGoPreviousMonth => !MinDate.HasValue || FirstDayOfMonth(DisplayDate.AddMonths(-1)) >= FirstDayOfMonth(MinDate.Value);

    public bool CanGoNextMonth => !MaxDate.HasValue || FirstDayOfMonth(DisplayDate.AddMonths(1)) <= FirstDayOfMonth(MaxDate.Value);

    public void NavigatePreviousMonth()
    {
        if (!CanGoPreviousMonth)
        {
            return;
        }

        DisplayDate = DisplayDate.AddMonths(-1);
    }

    public void NavigateNextMonth()
    {
        if (!CanGoNextMonth)
        {
            return;
        }

        DisplayDate = DisplayDate.AddMonths(1);
    }

    public void SelectDate(DateTime date)
    {
        date = date.Date;

        if (IsDateUnavailable(date))
        {
            return;
        }

        SetCurrentValue(ActiveDateProperty, date);

        if (SelectionMode == CodexCalendarSelectionMode.Range)
        {
            if (!RangeStart.HasValue || RangeEnd.HasValue)
            {
                SetCurrentValue(RangeStartProperty, date);
                SetCurrentValue(RangeEndProperty, null);
            }
            else if (date < RangeStart.Value)
            {
                SetCurrentValue(RangeEndProperty, RangeStart.Value);
                SetCurrentValue(RangeStartProperty, date);
            }
            else
            {
                SetCurrentValue(RangeEndProperty, date);
            }
        }
        else
        {
            SetCurrentValue(SelectedDateProperty, date);
            SetCurrentValue(RangeStartProperty, null);
            SetCurrentValue(RangeEndProperty, null);
        }

        RebuildCalendar();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        RebuildCalendar();
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, CodexFocusVisible.FromFocusChange(e));
        SetCurrentValue(ActiveDateProperty, ActiveDate ?? SelectedDate ?? RangeStart ?? FirstSelectableDateInMonth());
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        base.OnPointerPressed(e);
        Focus();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        var activeDate = ActiveDate ?? SelectedDate ?? RangeStart ?? FirstSelectableDateInMonth();

        switch (e.Key)
        {
            case Key.Left:
                MoveActiveDate(activeDate.AddDays(-1));
                e.Handled = true;
                break;
            case Key.Right:
                MoveActiveDate(activeDate.AddDays(1));
                e.Handled = true;
                break;
            case Key.Up:
                MoveActiveDate(activeDate.AddDays(-7));
                e.Handled = true;
                break;
            case Key.Down:
                MoveActiveDate(activeDate.AddDays(7));
                e.Handled = true;
                break;
            case Key.Home:
                MoveActiveDate(activeDate.AddDays(-WeekdayOffset(activeDate.DayOfWeek)));
                e.Handled = true;
                break;
            case Key.End:
                MoveActiveDate(activeDate.AddDays(6 - WeekdayOffset(activeDate.DayOfWeek)));
                e.Handled = true;
                break;
            case Key.PageUp:
                NavigatePreviousMonth();
                MoveActiveDate(FirstSelectableDateInMonth());
                e.Handled = true;
                break;
            case Key.PageDown:
                NavigateNextMonth();
                MoveActiveDate(FirstSelectableDateInMonth());
                e.Handled = true;
                break;
            case Key.Enter:
            case Key.Space:
                SelectDate(activeDate);
                e.Handled = true;
                break;
        }

        base.OnKeyDown(e);
    }

    internal void RebuildCalendar()
    {
        if (_isRebuilding)
        {
            return;
        }

        _isRebuilding = true;
        Items.Clear();

        AddWeekdayHeaders();
        AddDayButtons();

        _isRebuilding = false;
        SyncClasses();
        _previousMonthCommand.RaiseCanExecuteChanged();
        _nextMonthCommand.RaiseCanExecuteChanged();
    }

    internal void SyncDayStates()
    {
        foreach (var button in Items.OfType<CodexCalendarDayButton>())
        {
            SyncDayState(button);
        }
    }

    private void AddWeekdayHeaders()
    {
        if (ShowWeekNumbers)
        {
            Items.Add(new CodexCalendarWeekday { Content = "#", IsWeekNumberHeader = true, Size = Size });
        }

        var names = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
        for (var offset = 0; offset < 7; offset++)
        {
            var day = (DayOfWeek)(((int)FirstDayOfWeek + offset) % 7);
            Items.Add(new CodexCalendarWeekday { Content = names[(int)day], Size = Size });
        }
    }

    private void AddDayButtons()
    {
        var firstOfMonth = FirstDayOfMonth(DisplayDate);
        var gridStart = firstOfMonth.AddDays(-WeekdayOffset(firstOfMonth.DayOfWeek));

        for (var week = 0; week < 6; week++)
        {
            if (ShowWeekNumbers)
            {
                var weekDate = gridStart.AddDays(week * 7);
                Items.Add(new CodexCalendarWeekNumber
                {
                    Content = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                        weekDate,
                        CalendarWeekRule.FirstFourDayWeek,
                        FirstDayOfWeek).ToString(CultureInfo.CurrentCulture),
                    Size = Size
                });
            }

            for (var weekday = 0; weekday < 7; weekday++)
            {
                var date = gridStart.AddDays((week * 7) + weekday);
                var outsideMonth = date.Month != DisplayDate.Month || date.Year != DisplayDate.Year;
                var blank = outsideMonth && !ShowOutsideDays;
                var button = new CodexCalendarDayButton
                {
                    Owner = this,
                    Date = date,
                    Content = blank ? string.Empty : date.Day.ToString(CultureInfo.CurrentCulture),
                    Size = Size,
                    IsBlank = blank
                };

                SyncDayState(button);
                Items.Add(button);
            }
        }
    }

    private void SyncDayState(CodexCalendarDayButton button)
    {
        var date = button.Date.Date;
        var outsideMonth = date.Month != DisplayDate.Month || date.Year != DisplayDate.Year;
        var unavailable = button.IsBlank || IsDateUnavailable(date);
        var rangeStart = RangeStart.HasValue && date == RangeStart.Value.Date;
        var rangeEnd = RangeEnd.HasValue && date == RangeEnd.Value.Date;
        var rangeMiddle = RangeStart.HasValue && RangeEnd.HasValue && date > RangeStart.Value.Date && date < RangeEnd.Value.Date;

        button.SetCurrentValue(CodexCalendarDayButton.IsOutsideMonthProperty, outsideMonth);
        button.SetCurrentValue(CodexCalendarDayButton.IsTodayProperty, date == DateTime.Today);
        button.SetCurrentValue(CodexCalendarDayButton.IsSelectedProperty, SelectedDate.HasValue && date == SelectedDate.Value.Date);
        button.SetCurrentValue(CodexCalendarDayButton.IsRangeStartProperty, rangeStart);
        button.SetCurrentValue(CodexCalendarDayButton.IsRangeEndProperty, rangeEnd);
        button.SetCurrentValue(CodexCalendarDayButton.IsRangeMiddleProperty, rangeMiddle);
        button.SetCurrentValue(CodexCalendarDayButton.IsBookedProperty, IsDateBooked(date));
        button.SetCurrentValue(CodexCalendarDayButton.IsUnavailableProperty, unavailable);
        button.SetCurrentValue(CodexCalendarDayButton.IsActiveProperty, ActiveDate.HasValue && date == ActiveDate.Value.Date);
        button.SetCurrentValue(CodexCalendarDayButton.SizeProperty, Size);
        button.SetCurrentValue(InputElement.IsEnabledProperty, !unavailable);
    }

    private void MoveActiveDate(DateTime date)
    {
        date = date.Date;

        if (date.Month != DisplayDate.Month || date.Year != DisplayDate.Year)
        {
            SetCurrentValue(DisplayDateProperty, FirstDayOfMonth(date));
        }

        SetCurrentValue(ActiveDateProperty, date);
        Focus();
    }

    private DateTime FirstSelectableDateInMonth()
    {
        var date = FirstDayOfMonth(DisplayDate);

        for (var index = 0; index < 31; index++)
        {
            var candidate = date.AddDays(index);
            if (candidate.Month != date.Month)
            {
                break;
            }

            if (!IsDateUnavailable(candidate))
            {
                return candidate;
            }
        }

        return date;
    }

    private bool IsDateUnavailable(DateTime date)
    {
        return (MinDate.HasValue && date < MinDate.Value.Date)
               || (MaxDate.HasValue && date > MaxDate.Value.Date)
               || IsDateBooked(date);
    }

    private bool IsDateBooked(DateTime date)
    {
        return BookedDates?.Any(booked => booked.Date == date.Date) == true;
    }

    private int WeekdayOffset(DayOfWeek day)
    {
        return ((int)day - (int)FirstDayOfWeek + 7) % 7;
    }

    private void SyncClasses()
    {
        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("calendar", true);
        Classes.Set("mode-single", SelectionMode == CodexCalendarSelectionMode.Single);
        Classes.Set("mode-range", SelectionMode == CodexCalendarSelectionMode.Range);
        Classes.Set("show-outside-days", ShowOutsideDays);
        Classes.Set("hide-outside-days", !ShowOutsideDays);
        Classes.Set("week-numbers", ShowWeekNumbers);
        Classes.Set("has-selected-date", SelectedDate.HasValue);
        Classes.Set("has-range", RangeStart.HasValue || RangeEnd.HasValue);
        Classes.Set("range-complete", RangeStart.HasValue && RangeEnd.HasValue);
        Classes.Set("has-active-date", ActiveDate.HasValue);
        Classes.Set("can-previous", CanGoPreviousMonth);
        Classes.Set("can-next", CanGoNextMonth);
    }

    private void OnDisplayDateChanged(AvaloniaPropertyChangedEventArgs args)
    {
        var oldDate = FirstDayOfMonth(DateOnlyFromObject(args.OldValue) ?? DisplayDate);
        var newDate = FirstDayOfMonth(DisplayDate);

        if (DisplayDate != newDate)
        {
            SetCurrentValue(DisplayDateProperty, newDate);
            return;
        }

        MonthTitle = FormatMonthTitle(newDate);
        RebuildCalendar();

        if (oldDate != newDate)
        {
            DisplayDateChanged?.Invoke(
                this,
                new CodexCalendarDisplayDateChangedEventArgs(oldDate, newDate, MonthDelta(oldDate, newDate)));
        }
    }

    private void OnSelectedDateChanged(AvaloniaPropertyChangedEventArgs args)
    {
        var newDate = DateOnly(SelectedDate);
        if (SelectedDate != newDate)
        {
            SetCurrentValue(SelectedDateProperty, newDate);
            return;
        }

        if (SelectedDate.HasValue)
        {
            SetCurrentValue(DisplayDateProperty, FirstDayOfMonth(SelectedDate.Value));
            SetCurrentValue(RangeStartProperty, null);
            SetCurrentValue(RangeEndProperty, null);
        }

        RebuildCalendar();
        SelectedDateChanged?.Invoke(
            this,
            new CodexCalendarSelectedDateChangedEventArgs(DateOnlyFromObject(args.OldValue), SelectedDate));
    }

    private void OnRangeStartChanged(AvaloniaPropertyChangedEventArgs args)
    {
        OnRangeChanged(DateOnlyFromObject(args.OldValue), RangeEnd, RangeStart, RangeEnd);
    }

    private void OnRangeEndChanged(AvaloniaPropertyChangedEventArgs args)
    {
        OnRangeChanged(RangeStart, DateOnlyFromObject(args.OldValue), RangeStart, RangeEnd);
    }

    private void OnRangeChanged(DateTime? oldStart, DateTime? oldEnd, DateTime? newStart, DateTime? newEnd)
    {
        if (RangeStart.HasValue && RangeEnd.HasValue && RangeEnd.Value < RangeStart.Value)
        {
            var end = RangeStart.Value;
            SetCurrentValue(RangeStartProperty, RangeEnd.Value);
            SetCurrentValue(RangeEndProperty, end);
            return;
        }

        if (RangeStart.HasValue)
        {
            SetCurrentValue(DisplayDateProperty, FirstDayOfMonth(RangeStart.Value));
            SetCurrentValue(SelectedDateProperty, null);
        }

        RebuildCalendar();
        RangeChanged?.Invoke(this, new CodexCalendarRangeChangedEventArgs(oldStart, oldEnd, newStart, newEnd));
    }

    private void OnActiveDateChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncDayStates();
        SyncClasses();
        ActiveDateChanged?.Invoke(
            this,
            new CodexCalendarActiveDateChangedEventArgs(DateOnlyFromObject(args.OldValue), ActiveDate));
    }

    private void OnSelectionModeChanged()
    {
        RebuildCalendar();
    }

    private static DateTime FirstDayOfMonth(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    private static string FormatMonthTitle(DateTime date)
    {
        return date.ToString("MMMM yyyy", CultureInfo.CurrentCulture);
    }

    private static DateTime? DateOnly(DateTime? date)
    {
        return date?.Date;
    }

    private static DateTime? DateOnlyFromObject(object? value)
    {
        return value is DateTime date ? date.Date : null;
    }

    private static int MonthDelta(DateTime oldDate, DateTime newDate)
    {
        return ((newDate.Year - oldDate.Year) * 12) + newDate.Month - oldDate.Month;
    }

    private sealed class CalendarPartCommand(
        CodexCalendar calendar,
        Func<CodexCalendar, bool> canExecute,
        Action<CodexCalendar> execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecute(calendar);
        }

        public void Execute(object? parameter)
        {
            execute(calendar);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

public enum CodexCalendarSelectionMode
{
    Single,
    Range
}

public class CodexCalendarDayButton : Button
{
    public static readonly StyledProperty<DateTime> DateProperty =
        AvaloniaProperty.Register<CodexCalendarDayButton, DateTime>(nameof(Date), DateTime.Today);

    public static readonly StyledProperty<bool> IsOutsideMonthProperty =
        AvaloniaProperty.Register<CodexCalendarDayButton, bool>(nameof(IsOutsideMonth));

    public static readonly StyledProperty<bool> IsTodayProperty =
        AvaloniaProperty.Register<CodexCalendarDayButton, bool>(nameof(IsToday));

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<CodexCalendarDayButton, bool>(nameof(IsSelected));

    public static readonly StyledProperty<bool> IsRangeStartProperty =
        AvaloniaProperty.Register<CodexCalendarDayButton, bool>(nameof(IsRangeStart));

    public static readonly StyledProperty<bool> IsRangeEndProperty =
        AvaloniaProperty.Register<CodexCalendarDayButton, bool>(nameof(IsRangeEnd));

    public static readonly StyledProperty<bool> IsRangeMiddleProperty =
        AvaloniaProperty.Register<CodexCalendarDayButton, bool>(nameof(IsRangeMiddle));

    public static readonly StyledProperty<bool> IsBookedProperty =
        AvaloniaProperty.Register<CodexCalendarDayButton, bool>(nameof(IsBooked));

    public static readonly StyledProperty<bool> IsUnavailableProperty =
        AvaloniaProperty.Register<CodexCalendarDayButton, bool>(nameof(IsUnavailable));

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexCalendarDayButton, bool>(nameof(IsActive));

    public static readonly StyledProperty<bool> IsBlankProperty =
        AvaloniaProperty.Register<CodexCalendarDayButton, bool>(nameof(IsBlank));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexCalendarDayButton, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexCalendarDayButton()
    {
        DateProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, _) => button.SyncClasses());
        IsOutsideMonthProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, _) => button.SyncClasses());
        IsTodayProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, _) => button.SyncClasses());
        IsSelectedProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, _) => button.SyncClasses());
        IsRangeStartProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, _) => button.SyncClasses());
        IsRangeEndProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, _) => button.SyncClasses());
        IsRangeMiddleProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, _) => button.SyncClasses());
        IsBookedProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, _) => button.SyncClasses());
        IsUnavailableProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, _) => button.SyncClasses());
        IsActiveProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, _) => button.SyncClasses());
        IsBlankProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, _) => button.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexCalendarDayButton>((button, _) => button.SyncClasses());
    }

    public CodexCalendarDayButton()
    {
        SyncClasses();
    }

    public DateTime Date
    {
        get => GetValue(DateProperty);
        set => SetValue(DateProperty, value.Date);
    }

    public bool IsOutsideMonth
    {
        get => GetValue(IsOutsideMonthProperty);
        set => SetValue(IsOutsideMonthProperty, value);
    }

    public bool IsToday
    {
        get => GetValue(IsTodayProperty);
        set => SetValue(IsTodayProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public bool IsRangeStart
    {
        get => GetValue(IsRangeStartProperty);
        set => SetValue(IsRangeStartProperty, value);
    }

    public bool IsRangeEnd
    {
        get => GetValue(IsRangeEndProperty);
        set => SetValue(IsRangeEndProperty, value);
    }

    public bool IsRangeMiddle
    {
        get => GetValue(IsRangeMiddleProperty);
        set => SetValue(IsRangeMiddleProperty, value);
    }

    public bool IsBooked
    {
        get => GetValue(IsBookedProperty);
        set => SetValue(IsBookedProperty, value);
    }

    public bool IsUnavailable
    {
        get => GetValue(IsUnavailableProperty);
        set => SetValue(IsUnavailableProperty, value);
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public bool IsBlank
    {
        get => GetValue(IsBlankProperty);
        set => SetValue(IsBlankProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    internal CodexCalendar? Owner { get; set; }

    protected override void OnClick()
    {
        Owner?.SelectDate(Date);
        Owner?.Focus();
        base.OnClick();
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("calendar-day", true);
        Classes.Set("outside", IsOutsideMonth);
        Classes.Set("today", IsToday);
        Classes.Set("selected", IsSelected);
        Classes.Set("range-start", IsRangeStart);
        Classes.Set("range-end", IsRangeEnd);
        Classes.Set("range-middle", IsRangeMiddle);
        Classes.Set("booked", IsBooked);
        Classes.Set("unavailable", IsUnavailable);
        Classes.Set("active", IsActive);
        Classes.Set("blank", IsBlank);
    }
}

public class CodexCalendarWeekday : ContentControl
{
    public static readonly StyledProperty<bool> IsWeekNumberHeaderProperty =
        AvaloniaProperty.Register<CodexCalendarWeekday, bool>(nameof(IsWeekNumberHeader));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexCalendarWeekday, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexCalendarWeekday()
    {
        IsWeekNumberHeaderProperty.Changed.AddClassHandler<CodexCalendarWeekday>((weekday, _) => weekday.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexCalendarWeekday>((weekday, _) => weekday.SyncClasses());
    }

    public CodexCalendarWeekday()
    {
        Focusable = false;
        IsHitTestVisible = false;
        SyncClasses();
    }

    public bool IsWeekNumberHeader
    {
        get => GetValue(IsWeekNumberHeaderProperty);
        set => SetValue(IsWeekNumberHeaderProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("calendar-weekday", true);
        Classes.Set("week-number-header", IsWeekNumberHeader);
    }
}

public class CodexCalendarWeekNumber : ContentControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexCalendarWeekNumber, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexCalendarWeekNumber()
    {
        SizeProperty.Changed.AddClassHandler<CodexCalendarWeekNumber>((weekNumber, _) => weekNumber.SyncClasses());
    }

    public CodexCalendarWeekNumber()
    {
        Focusable = false;
        IsHitTestVisible = false;
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("calendar-week-number", true);
    }
}
