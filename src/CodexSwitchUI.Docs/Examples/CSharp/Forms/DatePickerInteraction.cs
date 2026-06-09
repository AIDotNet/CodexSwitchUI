using Avalonia.Controls;
using Avalonia.Input;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System.Globalization;

public static class DatePickerInteractionSample
{
    public static Control BuildDatePickerInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "OpenChanged: picker starts closed; use ArrowDown to open."
        };
        var picker = new CodexDatePicker
        {
            DisplayDate = new DateTime(2026, 5, 1),
            SelectedDate = new DateTime(2026, 5, 13),
            CloseOnSelect = false,
            MinWidth = 280
        };
        picker.OpenChanged += (_, args) =>
        {
            status.Text = $"OpenChanged: {(args.IsOpen ? "open" : "closed")} (source={args.Source}).";
        };
        picker.SelectedDateChanged += (_, args) =>
        {
            status.Text = $"SelectedDateChanged: {DateLabel(args.OldDate)} -> {DateLabel(args.NewDate)} (source={args.Source}).";
        };

        var openByKeyboard = new CodexButton { Content = "ArrowDown", Size = CodexControlSize.Small, Variant = CodexControlVariant.Secondary };
        openByKeyboard.Click += (_, _) => picker.TryHandleInputKey(Key.Down);

        var closeByKeyboard = new CodexButton { Content = "Escape", Size = CodexControlSize.Small, Variant = CodexControlVariant.Ghost };
        closeByKeyboard.Click += (_, _) => picker.TryHandleInputKey(Key.Escape);

        var selectDate = new CodexButton { Content = "Select May 25", Size = CodexControlSize.Small, Variant = CodexControlVariant.Outline };
        selectDate.Click += (_, _) => picker.SelectDate(new DateTime(2026, 5, 25));

        var clearPicker = new CodexDatePicker
        {
            DisplayDate = new DateTime(2026, 7, 1),
            SelectedDate = new DateTime(2026, 7, 9),
            IsClearVisible = true,
            MinWidth = 280
        };
        var clearSelection = new CodexButton { Content = "ClearSelection", Size = CodexControlSize.Small, Variant = CodexControlVariant.Ghost };
        clearSelection.Click += (_, _) => clearPicker.ClearSelection();

        var rangeStatus = new CodexText { Role = CodexTextRole.Muted, Text = "RangeChanged: waiting for the second date." };
        var rangePicker = new CodexDatePicker
        {
            DisplayDate = new DateTime(2026, 2, 1),
            SelectionMode = CodexCalendarSelectionMode.Range,
            RangeStart = new DateTime(2026, 2, 9),
            MinWidth = 280
        };
        rangePicker.RangeChanged += (_, args) =>
        {
            var suffix = args.End.HasValue ? "complete" : "open";
            rangeStatus.Text = $"RangeChanged: {DateLabel(args.Start)} -> {DateLabel(args.End)} ({suffix}, source={args.Source}).";
        };
        var openRange = new CodexButton { Content = "Open range", Size = CodexControlSize.Small, Variant = CodexControlVariant.Secondary };
        openRange.Click += (_, _) => rangePicker.Open();
        var finishRange = new CodexButton { Content = "Finish range", Size = CodexControlSize.Small, Variant = CodexControlVariant.Outline };
        finishRange.Click += (_, _) => rangePicker.SelectDate(new DateTime(2026, 2, 18));

        var guarded = new CodexDatePicker
        {
            DisplayDate = new DateTime(2026, 8, 1),
            SelectedDate = new DateTime(2026, 8, 18),
            MinDate = new DateTime(2026, 8, 10),
            MaxDate = new DateTime(2026, 8, 24),
            IsLoading = true,
            Intent = CodexControlIntent.Error,
            MinWidth = 280
        };
        var tryGuardedSelection = new CodexButton { Content = "Try guarded date", Size = CodexControlSize.Small, Variant = CodexControlVariant.Secondary };
        tryGuardedSelection.Click += (_, _) =>
        {
            var changed = guarded.SelectDate(new DateTime(2026, 8, 26));
            status.Text = changed
                ? "Guarded picker changed unexpectedly."
                : "Guarded picker suppressed loading or out-of-bounds selection.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                picker,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { openByKeyboard, closeByKeyboard, selectDate }
                },
                clearPicker,
                clearSelection,
                rangeStatus,
                rangePicker,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { openRange, finishRange }
                },
                guarded,
                tryGuardedSelection
            }
        };
    }

    private static string DateLabel(DateTime? date)
    {
        return date.HasValue
            ? date.Value.ToString("MMM d, yyyy", CultureInfo.CurrentCulture)
            : "empty";
    }
}
