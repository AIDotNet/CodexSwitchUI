using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

public static class CalendarInteractionSample
{
    public static Control BuildCalendarInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Calendar events: waiting."
        };
        var calendar = BuildCalendar(
            new DateTime(2026, 5, 1),
            selectedDate: new DateTime(2026, 5, 13),
            activeDate: new DateTime(2026, 5, 22));
        calendar.SelectedDateChanged += (_, args) =>
        {
            status.Text = $"SelectedDateChanged: {DateLabel(args.OldDate)} -> {DateLabel(args.NewDate)} (source={args.Source}).";
        };
        calendar.DisplayDateChanged += (_, args) =>
        {
            status.Text = $"DisplayDateChanged: {args.OldDisplayDate:MMM yyyy} -> {args.NewDisplayDate:MMM yyyy} (source={args.Source}).";
        };
        calendar.ActiveDateChanged += (_, args) =>
        {
            status.Text = $"ActiveDateChanged: {DateLabel(args.OldDate)} -> {DateLabel(args.NewDate)} (source={args.Source}).";
        };

        var selectMay25 = new CodexButton { Content = "Select May 25", Size = CodexControlSize.Small, Variant = CodexControlVariant.Outline };
        selectMay25.Click += (_, _) => calendar.SelectDate(new DateTime(2026, 5, 25));

        var nextMonth = new CodexButton { Content = "Next month", Size = CodexControlSize.Small, Variant = CodexControlVariant.Ghost };
        nextMonth.Click += (_, _) => calendar.NavigateNextMonth();

        var moveActive = new CodexButton { Content = "Move active", Size = CodexControlSize.Small, Variant = CodexControlVariant.Ghost };
        moveActive.Click += (_, _) => calendar.ActiveDate = (calendar.ActiveDate ?? new DateTime(2026, 5, 22)).AddDays(1);

        var rangeStatus = new CodexText { Role = CodexTextRole.Muted, Text = "RangeChanged: waiting." };
        var rangeCalendar = BuildCalendar(
            new DateTime(2026, 2, 1),
            rangeStart: new DateTime(2026, 2, 9),
            rangeEnd: new DateTime(2026, 2, 18),
            activeDate: new DateTime(2026, 2, 18),
            selectionMode: CodexCalendarSelectionMode.Range);
        rangeCalendar.RangeChanged += (_, args) =>
        {
            var suffix = args.IsComplete ? "complete" : "open";
            rangeStatus.Text = $"RangeChanged: {DateLabel(args.NewStart)} -> {DateLabel(args.NewEnd)} ({suffix}, source={args.Source}).";
        };

        var startRange = new CodexButton { Content = "Start Mar 6", Size = CodexControlSize.Small, Variant = CodexControlVariant.Outline };
        startRange.Click += (_, _) => rangeCalendar.SelectDate(new DateTime(2026, 3, 6));

        var finishRange = new CodexButton { Content = "Finish Mar 12", Size = CodexControlSize.Small, Variant = CodexControlVariant.Ghost };
        finishRange.Click += (_, _) => rangeCalendar.SelectDate(new DateTime(2026, 3, 12));

        var blockedCalendar = BuildCalendar(
            new DateTime(2026, 8, 1),
            selectedDate: new DateTime(2026, 8, 10),
            activeDate: new DateTime(2026, 8, 17),
            size: CodexControlSize.Small);
        blockedCalendar.AttachedToVisualTree += (_, _) =>
        {
            var blockedDay = blockedCalendar.Items
                .OfType<CodexCalendarDayButton>()
                .SingleOrDefault(button => button.Date == new DateTime(2026, 8, 17));
            if (blockedDay is not null)
            {
                blockedDay.Command = new SampleCommand(() => status.Text = "Blocked day command executed.", () => false);
            }
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                calendar,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { selectMay25, nextMonth, moveActive }
                },
                rangeStatus,
                rangeCalendar,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { startRange, finishRange }
                },
                BuildCalendar(
                    new DateTime(2026, 3, 1),
                    selectedDate: new DateTime(2026, 3, 16),
                    activeDate: new DateTime(2026, 3, 16),
                    minDate: new DateTime(2026, 3, 9),
                    maxDate: new DateTime(2026, 3, 23),
                    intent: CodexControlIntent.Error),
                blockedCalendar
            }
        };
    }

    private static CodexCalendar BuildCalendar(
        DateTime displayDate,
        DateTime? selectedDate = null,
        DateTime? rangeStart = null,
        DateTime? rangeEnd = null,
        DateTime? activeDate = null,
        DateTime? minDate = null,
        DateTime? maxDate = null,
        CodexCalendarSelectionMode selectionMode = CodexCalendarSelectionMode.Single,
        CodexControlIntent intent = CodexControlIntent.Default,
        CodexControlSize size = CodexControlSize.Medium)
    {
        return new CodexCalendar
        {
            DisplayDate = displayDate,
            SelectedDate = selectedDate,
            RangeStart = rangeStart,
            RangeEnd = rangeEnd,
            ActiveDate = activeDate,
            MinDate = minDate,
            MaxDate = maxDate,
            SelectionMode = selectionMode,
            Intent = intent,
            Size = size,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
        };
    }

    private static string DateLabel(DateTime? date)
    {
        return date.HasValue
            ? date.Value.ToString("MMM d, yyyy", CultureInfo.CurrentCulture)
            : "empty";
    }

    private sealed class SampleCommand(Action execute, Func<bool>? canExecute = null) : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => execute();
    }
}
