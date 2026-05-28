using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class CommandDialogInteractionSample
{
    public static Control BuildCommandDialogInteractionPreview()
    {
        var trigger = new CodexButton
        {
            Content = "Open command menu",
            Size = CodexControlSize.Small
        };
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Command dialog is open; selecting an enabled item can close it."
        };
        var closeOnSelectDialog = new CodexCommandDialog
        {
            Trigger = trigger,
            Placeholder = "Search commands...",
            IsOpen = true,
            CloseOnItemSelected = true,
            RestoreFocusElement = trigger,
            Content = new CodexCommandList
            {
                Items =
                {
                    new CodexCommandGroup
                    {
                        Header = "Close on select",
                        Items =
                        {
                            new CodexCommandItem { Content = "Switch provider", Value = "provider", Icon = "P", Shortcut = "Enter", IsActive = true },
                            new CodexCommandItem { Content = "Refresh models", Value = "refresh", Icon = "R", Shortcut = "Cmd+R" },
                            new CodexCommandItem { Content = "Disabled action", Value = "disabled", Icon = "D", IsEnabled = false }
                        }
                    }
                }
            }
        };

        closeOnSelectDialog.OpenChanged += (_, args) =>
        {
            status.Text = args.IsOpen
                ? "Command dialog opened from its trigger."
                : "Command dialog closed and requested focus return to the trigger.";
        };
        closeOnSelectDialog.ItemSelected += (_, args) =>
        {
            status.Text = $"Selected {args.Value} via {args.Source}; close-on-select will dismiss when allowed.";
        };

        var loadingDialog = new CodexCommandDialog
        {
            Trigger = new CodexButton { Content = "Open loading menu", Size = CodexControlSize.Small },
            Placeholder = "Loading command...",
            IsOpen = true,
            IsLoading = true,
            CloseOnItemSelected = true,
            Content = new CodexCommandList
            {
                Items =
                {
                    new CodexCommandLoading { Content = "Loading command results..." },
                    new CodexCommandEmpty { Content = "No commands available while loading." }
                }
            }
        };
        var manualDialog = new CodexCommandDialog
        {
            Trigger = new CodexButton { Content = "Open manual menu", Size = CodexControlSize.Small },
            Placeholder = "Manual close...",
            IsOpen = true,
            CloseOnItemSelected = false,
            Content = new CodexCommandList
            {
                Items =
                {
                    new CodexCommandGroup
                    {
                        Header = "Manual",
                        Items =
                        {
                            new CodexCommandItem { Content = "Copy command", Icon = "C", Shortcut = "Cmd+C" },
                            new CodexCommandItem { Content = "Open logs", Icon = "L" }
                        }
                    }
                }
            }
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                closeOnSelectDialog,
                loadingDialog,
                manualDialog
            }
        };
    }
}
