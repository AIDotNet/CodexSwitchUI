using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class PopoverInteractionSample
{
    public static Control BuildPopoverInteractionPreview()
    {
        var trigger = new CodexButton
        {
            Content = "Usage trigger",
            Size = CodexControlSize.Small
        };
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "OpenChanged: popover starts closed."
        };
        var popover = new CodexPopover
        {
            Trigger = trigger,
            Title = "Usage window",
            Description = "DismissCommand, Escape, and outside pointer share the same close path.",
            IsArrowVisible = true,
            RestoreFocusElement = trigger,
            Content = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new CodexProgress { Minimum = 0, Maximum = 100, Value = 72, Variant = CodexControlVariant.Warning },
                    new CodexText { Role = CodexTextRole.Muted, Text = "72% of the monthly budget is used." }
                }
            },
            Action = new CodexButton { Content = "Open billing", Size = CodexControlSize.Small }
        };
        popover.OpenChanged += (_, args) =>
        {
            status.Text = args.IsOpen
                ? $"OpenChanged: popover opened by {args.Source}."
                : $"OpenChanged: popover closed by {args.Source}.";
        };
        popover.RestoreFocusRequested += (_, _) =>
        {
            status.Text = "Dismissed and requested focus restoration to the trigger.";
        };

        var dismiss = new CodexButton
        {
            Content = "Dismiss command",
            Variant = CodexControlVariant.Secondary,
            Size = CodexControlSize.Small
        };
        dismiss.Click += (_, _) =>
        {
            if (popover.DismissCommand?.CanExecute(null) == true)
            {
                popover.DismissCommand.Execute(null);
            }
        };

        var open = new CodexButton
        {
            Content = "Open",
            Variant = CodexControlVariant.Ghost,
            Size = CodexControlSize.Small
        };
        open.Click += (_, _) => popover.Open();

        var policy = new CodexPopover
        {
            Trigger = new CodexButton
            {
                Content = "Open persistent panel",
                Size = CodexControlSize.Small,
                Variant = CodexControlVariant.Secondary
            },
            Title = "Persistent panel",
            Description = "Escape and outside pointer are disabled for manual host control.",
            CloseOnEscape = false,
            DismissOnOutsidePointer = false,
            IsCloseVisible = false,
            Content = new CodexBadge { Content = "manual", Variant = CodexControlVariant.Outline },
            Action = new CodexButton { Content = "Apply", Size = CodexControlSize.Small }
        };

        var closed = new CodexPopover
        {
            Title = "Closed popover",
            Description = "Closed class stays mounted for exit styling.",
            IsOpen = false,
            Content = new CodexText { Role = CodexTextRole.Muted, Text = "Toggle open state without replacing the surface." }
        };
        var toggleClosed = new CodexButton
        {
            Content = "Toggle closed sample",
            Variant = CodexControlVariant.Secondary,
            Size = CodexControlSize.Small
        };
        toggleClosed.Click += (_, _) => closed.IsOpen = !closed.IsOpen;

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                popover,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        open,
                        dismiss
                    }
                },
                policy,
                closed,
                toggleClosed
            }
        };
    }
}
