using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CodexSwitchUI.Controls;

public enum CodexDatePickerChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public sealed class CodexDatePickerSelectedDateChangedEventArgs(
    DateTime? oldDate,
    DateTime? newDate,
    CodexDatePickerChangeSource source = CodexDatePickerChangeSource.Programmatic)
    : EventArgs
{
    public DateTime? OldDate { get; } = oldDate;

    public DateTime? NewDate { get; } = newDate;

    public CodexDatePickerChangeSource Source { get; } = source;
}

public sealed class CodexDatePickerRangeChangedEventArgs(
    DateTime? start,
    DateTime? end,
    CodexDatePickerChangeSource source = CodexDatePickerChangeSource.Programmatic)
    : EventArgs
{
    public DateTime? Start { get; } = start;

    public DateTime? End { get; } = end;

    public CodexDatePickerChangeSource Source { get; } = source;
}

public sealed class CodexDatePickerOpenChangedEventArgs(
    bool isOpen,
    CodexDatePickerChangeSource source = CodexDatePickerChangeSource.Programmatic) : EventArgs
{
    public bool IsOpen { get; } = isOpen;

    public CodexDatePickerChangeSource Source { get; } = source;
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexDatePicker : TemplatedControl
{
    private Button? _trigger;
    private Button? _clear;
    private CodexCalendar? _calendar;
    private CodexDatePickerChangeSource? _pendingChangeSource;

    public static readonly StyledProperty<DateTime?> SelectedDateProperty =
        AvaloniaProperty.Register<CodexDatePicker, DateTime?>(nameof(SelectedDate));

    public static readonly StyledProperty<DateTime?> RangeStartProperty =
        AvaloniaProperty.Register<CodexDatePicker, DateTime?>(nameof(RangeStart));

    public static readonly StyledProperty<DateTime?> RangeEndProperty =
        AvaloniaProperty.Register<CodexDatePicker, DateTime?>(nameof(RangeEnd));

    public static readonly StyledProperty<DateTime> DisplayDateProperty =
        AvaloniaProperty.Register<CodexDatePicker, DateTime>(nameof(DisplayDate), FirstDayOfMonth(DateTime.Today));

    public static readonly StyledProperty<DateTime?> MinDateProperty =
        AvaloniaProperty.Register<CodexDatePicker, DateTime?>(nameof(MinDate));

    public static readonly StyledProperty<DateTime?> MaxDateProperty =
        AvaloniaProperty.Register<CodexDatePicker, DateTime?>(nameof(MaxDate));

    public static readonly StyledProperty<CodexCalendarSelectionMode> SelectionModeProperty =
        AvaloniaProperty.Register<CodexDatePicker, CodexCalendarSelectionMode>(nameof(SelectionMode), CodexCalendarSelectionMode.Single);

    public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty =
        AvaloniaProperty.Register<CodexDatePicker, DayOfWeek>(nameof(FirstDayOfWeek), DayOfWeek.Sunday);

    public static readonly StyledProperty<bool> ShowOutsideDaysProperty =
        AvaloniaProperty.Register<CodexDatePicker, bool>(nameof(ShowOutsideDays), true);

    public static readonly StyledProperty<bool> ShowWeekNumbersProperty =
        AvaloniaProperty.Register<CodexDatePicker, bool>(nameof(ShowWeekNumbers));

    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<CodexDatePicker, string?>(nameof(PlaceholderText), "Pick a date");

    public static readonly StyledProperty<string> DateFormatProperty =
        AvaloniaProperty.Register<CodexDatePicker, string>(nameof(DateFormat), "MMM d, yyyy");

    public static readonly StyledProperty<string> RangeSeparatorProperty =
        AvaloniaProperty.Register<CodexDatePicker, string>(nameof(RangeSeparator), " - ");

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexDatePicker, bool>(nameof(IsOpen));

    public static readonly StyledProperty<bool> CloseOnSelectProperty =
        AvaloniaProperty.Register<CodexDatePicker, bool>(nameof(CloseOnSelect), true);

    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<CodexDatePicker, bool>(nameof(CloseOnEscape), true);

    public static readonly StyledProperty<bool> IsClearVisibleProperty =
        AvaloniaProperty.Register<CodexDatePicker, bool>(nameof(IsClearVisible), true);

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexDatePicker, bool>(nameof(IsLoading));

    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexDatePicker, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexDatePicker, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<double> MaxPopupHeightProperty =
        AvaloniaProperty.Register<CodexDatePicker, double>(nameof(MaxPopupHeight), 380);

    public static readonly StyledProperty<string?> DisplayTextProperty =
        AvaloniaProperty.Register<CodexDatePicker, string?>(nameof(DisplayText));

    public static readonly StyledProperty<bool> HasSelectionProperty =
        AvaloniaProperty.Register<CodexDatePicker, bool>(nameof(HasSelection));

    public static readonly StyledProperty<bool> HasRangeProperty =
        AvaloniaProperty.Register<CodexDatePicker, bool>(nameof(HasRange));

    public static readonly StyledProperty<bool> HasClearButtonProperty =
        AvaloniaProperty.Register<CodexDatePicker, bool>(nameof(HasClearButton));

    static CodexDatePicker()
    {
        SelectedDateProperty.Changed.AddClassHandler<CodexDatePicker>((picker, args) => picker.OnSelectedDateChanged(args));
        RangeStartProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.OnRangeChanged());
        RangeEndProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.OnRangeChanged());
        DisplayDateProperty.Changed.AddClassHandler<CodexDatePicker>((picker, args) => picker.CoerceDisplayDate(args.NewValue is DateTime date ? date : picker.DisplayDate));
        MinDateProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncClasses());
        MaxDateProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncClasses());
        SelectionModeProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.OnSelectionModeChanged());
        FirstDayOfWeekProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncClasses());
        ShowOutsideDaysProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncClasses());
        ShowWeekNumbersProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncClasses());
        PlaceholderTextProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncDisplayText());
        DateFormatProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncDisplayText());
        RangeSeparatorProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncDisplayText());
        IsOpenProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.OnOpenChanged());
        CloseOnSelectProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncClasses());
        CloseOnEscapeProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncClasses());
        IsClearVisibleProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncClasses());
        IntentProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexDatePicker>((picker, _) => picker.SyncClasses());
    }

    public CodexDatePicker()
    {
        SyncDisplayText();
        SyncClasses();
    }

    public event EventHandler<CodexDatePickerSelectedDateChangedEventArgs>? SelectedDateChanged;

    public event EventHandler<CodexDatePickerRangeChangedEventArgs>? RangeChanged;

    public event EventHandler<CodexDatePickerOpenChangedEventArgs>? OpenChanged;

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

    public DateTime DisplayDate
    {
        get => GetValue(DisplayDateProperty);
        set => SetValue(DisplayDateProperty, FirstDayOfMonth(value));
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

    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public string DateFormat
    {
        get => GetValue(DateFormatProperty);
        set => SetValue(DateFormatProperty, value);
    }

    public string RangeSeparator
    {
        get => GetValue(RangeSeparatorProperty);
        set => SetValue(RangeSeparatorProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool CloseOnSelect
    {
        get => GetValue(CloseOnSelectProperty);
        set => SetValue(CloseOnSelectProperty, value);
    }

    public bool CloseOnEscape
    {
        get => GetValue(CloseOnEscapeProperty);
        set => SetValue(CloseOnEscapeProperty, value);
    }

    public bool IsClearVisible
    {
        get => GetValue(IsClearVisibleProperty);
        set => SetValue(IsClearVisibleProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
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

    public double MaxPopupHeight
    {
        get => GetValue(MaxPopupHeightProperty);
        set => SetValue(MaxPopupHeightProperty, value);
    }

    public string? DisplayText => GetValue(DisplayTextProperty);

    public bool HasSelection => GetValue(HasSelectionProperty);

    public bool HasRange => GetValue(HasRangeProperty);

    public bool HasClearButton => GetValue(HasClearButtonProperty);

    public bool Open()
    {
        return Open(CodexDatePickerChangeSource.Programmatic);
    }

    internal bool Open(CodexDatePickerChangeSource source)
    {
        if (!IsEnabled || IsLoading || IsOpen)
        {
            return false;
        }

        RunWithChangeSource(source, () => IsOpen = true);
        return true;
    }

    public bool Close()
    {
        return Close(CodexDatePickerChangeSource.Programmatic);
    }

    internal bool Close(CodexDatePickerChangeSource source)
    {
        if (!IsOpen)
        {
            return false;
        }

        RunWithChangeSource(source, () => IsOpen = false);
        return true;
    }

    public bool TogglePopup()
    {
        return TogglePopup(CodexDatePickerChangeSource.Programmatic);
    }

    internal bool TogglePopup(CodexDatePickerChangeSource source)
    {
        return IsOpen ? Close(source) : Open(source);
    }

    public bool ClearSelection()
    {
        return ClearSelection(CodexDatePickerChangeSource.Programmatic);
    }

    internal bool ClearSelection(CodexDatePickerChangeSource source)
    {
        if (!HasSelection)
        {
            return false;
        }

        RunWithChangeSource(source, () =>
        {
            SetCurrentValue(SelectedDateProperty, null);
            SetCurrentValue(RangeStartProperty, null);
            SetCurrentValue(RangeEndProperty, null);
        });
        SyncDisplayText();
        SyncClasses();
        return true;
    }

    public bool SelectDate(DateTime date)
    {
        return SelectDate(date, CodexDatePickerChangeSource.Programmatic);
    }

    internal bool SelectDate(DateTime date, CodexDatePickerChangeSource source)
    {
        date = date.Date;

        if (!IsEnabled || IsLoading || IsDateUnavailable(date))
        {
            return false;
        }

        RunWithChangeSource(source, () =>
        {
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

                if (CloseOnSelect && RangeStart.HasValue && RangeEnd.HasValue)
                {
                    Close(source);
                }
            }
            else
            {
                SetCurrentValue(SelectedDateProperty, date);

                if (CloseOnSelect)
                {
                    Close(source);
                }
            }
        });

        return true;
    }

    public bool TryHandleInputKey(Key key)
    {
        if (!IsEnabled)
        {
            return false;
        }

        switch (key)
        {
            case Key.Enter:
            case Key.Space:
            case Key.Down:
                return Open(CodexDatePickerChangeSource.Keyboard);
            case Key.Escape:
                return CloseOnEscape && Close(CodexDatePickerChangeSource.Keyboard);
            case Key.Back:
            case Key.Delete:
                return ClearSelection(CodexDatePickerChangeSource.Keyboard);
            default:
                return false;
        }
    }

    internal bool TryHandleTriggerPointerRelease(PointerUpdateKind updateKind)
    {
        return updateKind == PointerUpdateKind.LeftButtonReleased
            && TogglePopup(CodexDatePickerChangeSource.Pointer);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_trigger is not null)
        {
            _trigger.RemoveHandler(InputElement.PointerReleasedEvent, OnTriggerPointerReleased);
            _trigger.KeyDown -= OnTriggerKeyDown;
        }

        if (_clear is not null)
        {
            _clear.Click -= OnClearClick;
        }

        if (_calendar is not null)
        {
            _calendar.PointerReleased -= OnCalendarPointerReleased;
            _calendar.KeyDown -= OnCalendarKeyDown;
        }

        base.OnApplyTemplate(e);

        _trigger = e.NameScope.Find<Button>("PART_Trigger");
        _clear = e.NameScope.Find<Button>("PART_Clear");
        _calendar = e.NameScope.Find<CodexCalendar>("PART_Calendar");

        if (_trigger is not null)
        {
            _trigger.AddHandler(
                InputElement.PointerReleasedEvent,
                OnTriggerPointerReleased,
                RoutingStrategies.Bubble,
                handledEventsToo: true);
            _trigger.KeyDown += OnTriggerKeyDown;
        }

        if (_clear is not null)
        {
            _clear.Click += OnClearClick;
        }

        if (_calendar is not null)
        {
            _calendar.PointerReleased += OnCalendarPointerReleased;
            _calendar.KeyDown += OnCalendarKeyDown;
        }
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, CodexFocusVisible.FromFocusChange(e));
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
    }

    private void OnTriggerPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var updateKind = e.GetCurrentPoint((Control?)_trigger ?? this).Properties.PointerUpdateKind;
        if (TryHandleTriggerPointerRelease(updateKind))
        {
            e.Handled = true;
        }
    }

    private void OnTriggerKeyDown(object? sender, KeyEventArgs e)
    {
        if (TryHandleInputKey(e.Key))
        {
            e.Handled = true;
        }
    }

    private void OnClearClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ClearSelection(CodexDatePickerChangeSource.Pointer))
        {
            e.Handled = true;
        }
    }

    private void OnCalendarPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var relativeTo = _calendar is not null ? (Visual)_calendar : this;
        var updateKind = e.GetCurrentPoint(relativeTo).Properties.PointerUpdateKind;
        if (TryHandleCalendarPointerRelease(updateKind))
        {
            e.Handled = true;
        }
    }

    internal bool TryHandleCalendarPointerRelease(PointerUpdateKind updateKind, CodexCalendar? calendar = null)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased || IsLoading || !IsEnabled)
        {
            return false;
        }

        return SyncFromCalendar(calendar, CodexDatePickerChangeSource.Pointer);
    }

    private void OnCalendarKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && CloseOnEscape && Close())
        {
            e.Handled = true;
        }
    }

    private bool SyncFromCalendar(CodexCalendar? calendar = null, CodexDatePickerChangeSource source = CodexDatePickerChangeSource.Programmatic)
    {
        calendar ??= _calendar;
        if (calendar is null)
        {
            return false;
        }

        RunWithChangeSource(source, () =>
        {
            if (SelectionMode == CodexCalendarSelectionMode.Range)
            {
                SetCurrentValue(RangeStartProperty, calendar.RangeStart);
                SetCurrentValue(RangeEndProperty, calendar.RangeEnd);

                if (CloseOnSelect && RangeStart.HasValue && RangeEnd.HasValue)
                {
                    Close(source);
                }
            }
            else
            {
                SetCurrentValue(SelectedDateProperty, calendar.SelectedDate);

                if (CloseOnSelect && SelectedDate.HasValue)
                {
                    Close(source);
                }
            }
        });

        return true;
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

        SyncDisplayText();
        SelectedDateChanged?.Invoke(
            this,
            new CodexDatePickerSelectedDateChangedEventArgs(DateOnly(args.OldValue as DateTime?), SelectedDate, CurrentChangeSource));
        SyncClasses();
    }

    private void OnRangeChanged()
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

        SyncDisplayText();
        RangeChanged?.Invoke(this, new CodexDatePickerRangeChangedEventArgs(RangeStart, RangeEnd, CurrentChangeSource));
        SyncClasses();
    }

    private void OnSelectionModeChanged()
    {
        if (SelectionMode == CodexCalendarSelectionMode.Range && SelectedDate.HasValue)
        {
            SetCurrentValue(RangeStartProperty, SelectedDate);
            SetCurrentValue(SelectedDateProperty, null);
        }
        else if (SelectionMode == CodexCalendarSelectionMode.Single && RangeStart.HasValue)
        {
            SetCurrentValue(SelectedDateProperty, RangeStart);
            SetCurrentValue(RangeStartProperty, null);
            SetCurrentValue(RangeEndProperty, null);
        }

        SyncDisplayText();
        SyncClasses();
    }

    private void OnOpenChanged()
    {
        OpenChanged?.Invoke(this, new CodexDatePickerOpenChangedEventArgs(IsOpen, CurrentChangeSource));
        SyncClasses();
    }

    private void CoerceDisplayDate(DateTime date)
    {
        var firstDay = FirstDayOfMonth(date);
        if (DisplayDate != firstDay)
        {
            SetCurrentValue(DisplayDateProperty, firstDay);
        }
    }

    private void SyncDisplayText()
    {
        var text = SelectionMode == CodexCalendarSelectionMode.Range
            ? FormatRange()
            : FormatDate(SelectedDate);

        SetValue(DisplayTextProperty, text);
    }

    private string? FormatRange()
    {
        if (!RangeStart.HasValue)
        {
            return null;
        }

        var start = FormatDate(RangeStart) ?? string.Empty;
        if (!RangeEnd.HasValue)
        {
            return start;
        }

        return $"{start}{RangeSeparator}{FormatDate(RangeEnd)}";
    }

    private string? FormatDate(DateTime? date)
    {
        return date.HasValue
            ? date.Value.ToString(DateFormat, CultureInfo.CurrentCulture)
            : null;
    }

    private void SyncClasses()
    {
        var hasRange = RangeStart.HasValue || RangeEnd.HasValue;
        var hasSelection = SelectedDate.HasValue || hasRange;

        SetValue(HasSelectionProperty, hasSelection);
        SetValue(HasRangeProperty, hasRange);
        SetValue(HasClearButtonProperty, IsClearVisible && hasSelection);

        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("date-picker", true);
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("has-selection", hasSelection);
        Classes.Set("placeholder-visible", !hasSelection);
        Classes.Set("range", SelectionMode == CodexCalendarSelectionMode.Range);
        Classes.Set("single", SelectionMode == CodexCalendarSelectionMode.Single);
        Classes.Set("range-complete", RangeStart.HasValue && RangeEnd.HasValue);
        Classes.Set("loading", IsLoading);
        Classes.Set("has-clear", HasClearButton);
        Classes.Set("close-on-select", CloseOnSelect);
        Classes.Set("close-on-escape", CloseOnEscape);
        Classes.Set("week-numbers", ShowWeekNumbers);
    }

    private bool IsDateUnavailable(DateTime date)
    {
        return MinDate.HasValue && date < MinDate.Value.Date
               || MaxDate.HasValue && date > MaxDate.Value.Date;
    }

    private static DateTime? DateOnly(DateTime? value)
    {
        return value?.Date;
    }

    private static DateTime FirstDayOfMonth(DateTime value)
    {
        return new DateTime(value.Year, value.Month, 1);
    }

    private CodexDatePickerChangeSource CurrentChangeSource => _pendingChangeSource ?? CodexDatePickerChangeSource.Programmatic;

    private void RunWithChangeSource(CodexDatePickerChangeSource source, Action action)
    {
        var previousSource = _pendingChangeSource;
        _pendingChangeSource = source;
        try
        {
            action();
        }
        finally
        {
            _pendingChangeSource = previousSource;
        }
    }
}
