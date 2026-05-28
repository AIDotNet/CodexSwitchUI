using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System.Windows.Input;

public static class AlertDialogInteractionSample
{
public static Control BuildAlertDialogInteractionPreview()
{
    var trigger = new CodexButton
    {
        Content = "Open alert dialog",
        Size = CodexControlSize.Small
    };
    var status = new CodexText
    {
        Role = CodexTextRole.Muted,
        Text = "Cancel and action commands update this status."
    };
    var alertDialog = new CodexAlertDialog
    {
        Trigger = trigger,
        Title = "Delete route?",
        Description = "Escape closes and requests focus return, outside pointer is ignored by default.",
        RestoreFocusElement = trigger,
        Media = "!",
        ActionContent = "Delete route",
        ActionVariant = CodexControlVariant.Destructive,
        CancelCommand = new SampleCommand(() => status.Text = "Cancel command executed."),
        ActionCommand = new SampleCommand(() => status.Text = "Action command executed; dialog closed."),
        Content = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "The least-destructive cancel action receives focus when mounted."
        }
    };

    alertDialog.OpenChanged += (_, args) =>
    {
        status.Text = args.IsOpen
            ? "OpenChanged: alert dialog opened from the trigger."
            : "OpenChanged: alert dialog closed.";
    };
    alertDialog.RestoreFocusRequested += (_, _) =>
    {
        status.Text = "Closed and requested focus restoration to the trigger.";
    };

    var cancel = new CodexButton
    {
        Content = "Run cancel command",
        Size = CodexControlSize.Small,
        Variant = CodexControlVariant.Secondary
    };
    cancel.Click += (_, _) => alertDialog.Cancel();

    var confirm = new CodexButton
    {
        Content = "Run action command",
        Size = CodexControlSize.Small,
        Variant = CodexControlVariant.Destructive
    };
    confirm.Click += (_, _) => alertDialog.Confirm();

    var asyncDialog = new CodexAlertDialog
    {
        IsOpen = true,
        Title = "Async confirmation",
        Description = "CloseOnAction=false keeps the dialog open while the host completes work.",
        CloseOnAction = false,
        IsActionLoading = true,
        ActionContent = "Uploading",
        Content = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "The action command is blocked while loading is true."
        }
    };
    var clearLoading = new CodexButton
    {
        Content = "Clear loading",
        Size = CodexControlSize.Small,
        Variant = CodexControlVariant.Secondary,
        HorizontalAlignment = HorizontalAlignment.Left
    };
    clearLoading.Click += (_, _) =>
    {
        asyncDialog.IsActionLoading = !asyncDialog.IsActionLoading;
    };

    return new StackPanel
    {
        Spacing = 10,
        Children =
        {
            status,
            alertDialog,
            new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                Children =
                {
                    cancel,
                    confirm
                }
            },
            asyncDialog,
            clearLoading
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
