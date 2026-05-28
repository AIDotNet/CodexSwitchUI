using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class DialogInteractionSample
{
public static Control BuildDialogInteractionPreview()
{
    var trigger = new CodexButton
    {
        Content = "Open delete dialog",
        Size = CodexControlSize.Small
    };
    var status = new CodexText
    {
        Role = CodexTextRole.Muted,
        Text = "OpenChanged: dialog starts closed."
    };
    var dialog = new CodexDialog
    {
        Trigger = trigger,
        Title = "Delete provider?",
        Description = "DismissCommand, Escape, outside pointer, and close button share the same close path.",
        RestoreFocusElement = trigger,
        Content = new CodexAlert
        {
            Icon = "!",
            Title = "This action affects routing.",
            Description = "The overlay remains mounted so exit classes can animate.",
            Variant = CodexControlVariant.Warning
        },
        Action = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Right,
            Children =
            {
                new CodexButton { Content = "Cancel", Variant = CodexControlVariant.Outline, Size = CodexControlSize.Small },
                new CodexButton { Content = "Delete", Variant = CodexControlVariant.Destructive, Size = CodexControlSize.Small }
            }
        }
    };

    dialog.OpenChanged += (_, args) =>
    {
        status.Text = args.IsOpen
            ? "OpenChanged: dialog opened from the trigger."
            : "OpenChanged: dialog closed.";
    };
    dialog.RestoreFocusRequested += (_, _) =>
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
        if (dialog.DismissCommand?.CanExecute(null) == true)
        {
            dialog.DismissCommand.Execute(null);
        }
    };

    var closedDialog = new CodexDialog
    {
        Title = "Closed exit state",
        Description = "Closed class drives exit styling while the surface remains inspectable.",
        IsOpen = false,
        Content = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Reopen to exercise the open class and action row."
        },
        Action = new CodexButton { Content = "Confirm", Size = CodexControlSize.Small }
    };
    var toggleClosed = new CodexButton
    {
        Content = "Toggle closed sample",
        Variant = CodexControlVariant.Secondary,
        Size = CodexControlSize.Small
    };
    toggleClosed.Click += (_, _) =>
    {
        closedDialog.IsOpen = !closedDialog.IsOpen;
    };

    return new StackPanel
    {
        Spacing = 10,
        Children =
        {
            status,
            dialog,
            dismiss,
            closedDialog,
            toggleClosed
        }
    };
}
}
