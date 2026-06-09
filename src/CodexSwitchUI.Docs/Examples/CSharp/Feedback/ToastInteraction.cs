using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System.Windows.Input;

public static class ToastInteractionSample
{
public static Control BuildToastInteractionPreview()
{
    var status = new CodexText
    {
        Role = CodexTextRole.Muted,
        Text = "Toast is closed by default. Use the controls to open and dismiss it."
    };
    var undo = new CodexButton
    {
        Content = "Undo",
        Size = CodexControlSize.Small,
        Variant = CodexControlVariant.Secondary
    };
    var primaryToast = new CodexToast
    {
        IsOpen = false,
        Icon = "i",
        Title = "Provider switched",
        Description = "Close button, Escape, and DismissCommand close the same mounted surface.",
        Action = undo,
        CloseContent = "Dismiss",
        CloseCommand = new SampleCommand(() =>
        {
            status.Text = "CloseCommand executed after the toast moved to the closed state.";
        })
    };

    undo.Click += (_, _) =>
    {
        primaryToast.Variant = CodexControlVariant.Success;
        primaryToast.Title = "Provider restored";
        primaryToast.Description = "Action slot clicked without dismissing the toast.";
        status.Text = "Undo action clicked; toast remains open and switched to success.";
    };

    var dismiss = new CodexButton
    {
        Content = "Dismiss command",
        Size = CodexControlSize.Small,
        Variant = CodexControlVariant.Secondary
    };
    dismiss.Click += (_, _) =>
    {
        if (primaryToast.DismissCommand?.CanExecute(null) == true)
        {
            primaryToast.DismissCommand.Execute(null);
        }
    };

    var reopen = new CodexButton
    {
        Content = "Show toast",
        Size = CodexControlSize.Small,
        Variant = CodexControlVariant.Outline
    };
    reopen.Click += (_, _) =>
    {
        primaryToast.IsOpen = true;
        primaryToast.Variant = CodexControlVariant.Default;
        primaryToast.Title = "Provider switched";
        primaryToast.Description = "Close button, Escape, and DismissCommand close the same mounted surface.";
        status.Text = "Toast opened; open class restored.";
    };

    var manualToast = new CodexToast
    {
        IsOpen = false,
        Icon = "!",
        Title = "Manual dismissal policy",
        Description = "Escape is ignored because host code owns this notification.",
        Variant = CodexControlVariant.Warning,
        CloseOnEscape = false,
        CloseContent = "Manual"
    };
    var manualDismiss = new CodexButton
    {
        Content = "Toggle manual",
        Size = CodexControlSize.Small,
        Variant = CodexControlVariant.Secondary
    };
    manualDismiss.Click += (_, _) =>
    {
        manualToast.IsOpen = !manualToast.IsOpen;
        status.Text = manualToast.IsOpen
            ? "Manual toast opened without changing Escape policy."
            : "Manual toast closed by host-owned state change.";
    };

    var actionToast = new CodexToast
    {
        IsOpen = false,
        Icon = "i",
        Title = "Usage refreshed",
        Description = "Action and close controls are independent slots.",
        Action = new CodexButton
        {
            Content = "View",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        },
        CloseContent = "Close"
    };
    var showAction = new CodexButton
    {
        Content = "Show action toast",
        Size = CodexControlSize.Small,
        Variant = CodexControlVariant.Secondary
    };
    showAction.Click += (_, _) =>
    {
        actionToast.IsOpen = true;
        status.Text = "Action toast opened; action and close slots stay independent.";
    };

    return new StackPanel
    {
        Spacing = 10,
        Children =
        {
            status,
            primaryToast,
            new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                Children =
                {
                    dismiss,
                    reopen
                }
            },
            manualToast,
            manualDismiss,
            actionToast,
            showAction
        }
    };
}

private sealed class SampleCommand(Action execute) : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => execute();
}
}
