using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class SheetInteractionSample
{
    public static Control BuildSheetInteractionPreview()
    {
        var trigger = new CodexButton
        {
            Content = "Toggle sheet",
            Size = CodexControlSize.Small
        };
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Sheet is open on the right edge."
        };
        var sheet = new CodexSheet
        {
            Trigger = trigger,
            Title = "Route filters",
            Description = "Dismiss, Escape, outside pointer, and focus return reuse the dialog contract.",
            Side = CodexSheetSide.Right,
            Width = 360,
            IsOpen = true,
            RestoreFocusElement = trigger,
            Content = new CodexText
            {
                Role = CodexTextRole.Muted,
                Text = "Closing sets IsOpen=false while the sheet stays mounted for exit motion."
            },
            Action = new CodexButton { Content = "Apply", Size = CodexControlSize.Small }
        };

        sheet.RestoreFocusRequested += (_, _) =>
        {
            status.Text = "Sheet dismissed and requested focus restoration to the trigger.";
        };
        sheet.OpenChanged += (_, args) =>
        {
            status.Text = args.IsOpen
                ? $"OpenChanged: sheet opened on the {sheet.Side.ToString().ToLowerInvariant()} edge."
                : "OpenChanged: sheet closed.";
        };

        var dismiss = new CodexButton
        {
            Content = "Dismiss command",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        dismiss.Click += (_, _) =>
        {
            if (sheet.DismissCommand?.CanExecute(null) == true)
            {
                sheet.DismissCommand.Execute(null);
            }
        };

        var cycleSide = new CodexButton
        {
            Content = "Cycle side",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        cycleSide.Click += (_, _) =>
        {
            sheet.Side = sheet.Side switch
            {
                CodexSheetSide.Right => CodexSheetSide.Bottom,
                CodexSheetSide.Bottom => CodexSheetSide.Left,
                CodexSheetSide.Left => CodexSheetSide.Top,
                _ => CodexSheetSide.Right
            };
            status.Text = $"Sheet side changed to {sheet.Side}; edge slide classes updated.";
        };

        var manualPolicy = new CodexButton
        {
            Content = "Manual policy",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        manualPolicy.Click += (_, _) =>
        {
            sheet.CloseOnEscape = !sheet.CloseOnEscape;
            sheet.DismissOnOutsidePointer = sheet.CloseOnEscape;
            status.Text = sheet.CloseOnEscape
                ? "Escape and outside pointer dismissal are enabled."
                : "Escape and outside pointer dismissal are disabled for host-managed close.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                sheet,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        dismiss,
                        cycleSide,
                        manualPolicy
                    }
                }
            }
        };
    }
}
