using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;
using System.Windows.Input;

public static class MenuInteractionSample
{
    public static Control BuildMenuInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "ItemSelected: choose a leaf menu item."
        };

        var checkedItem = new CodexMenuItem
        {
            Header = "Checked",
            ToggleType = MenuItemToggleType.CheckBox,
            IsChecked = true
        };
        checkedItem.ItemSelected += (_, args) =>
        {
            status.Text = $"ItemSelected: {args.Header} checked={args.IsChecked} source={args.Source}.";
        };

        var radioItem = new CodexMenuItem
        {
            Header = "Radio selected",
            ToggleType = MenuItemToggleType.Radio,
            IsChecked = true
        };
        radioItem.ItemSelected += (_, args) =>
        {
            status.Text = $"ItemSelected: {args.Header} radio={args.IsChecked} close={args.DidCloseOnSelect}.";
        };

        var menu = new CodexMenu
        {
            MinWidth = 230,
            Items =
            {
                new CodexMenuGroup { Header = "Keyboard path" },
                new CodexMenuItem
                {
                    Header = "Focused submenu",
                    Shortcut = "Right",
                    IsActive = true,
                    IsSubMenuOpen = true,
                    Items =
                    {
                        new CodexMenuItem { Header = "Archive" },
                        new CodexMenuItem { Header = "Favorites" }
                    }
                },
                new CodexMenuSeparator(),
                checkedItem,
                radioItem,
                new CodexMenuItem { Header = "Disabled leaf", IsEnabled = false },
                new CodexMenuItem
                {
                    Header = "Command blocked",
                    Command = new SampleCommand(() => status.Text = "Blocked menu command executed.", () => false)
                }
            }
        };

        var loading = new CodexMenu
        {
            MinWidth = 230,
            IsLoading = true,
            Items =
            {
                new CodexMenuGroup { Header = "Loading gate" },
                new CodexMenuItem { Header = "Refresh providers", Shortcut = "Ctrl+R" },
                new CodexMenuItem
                {
                    Header = "Export blocked",
                    Items =
                    {
                        new CodexMenuItem { Header = "JSON" },
                        new CodexMenuItem { Header = "CSV" }
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
                menu,
                loading
            }
        };
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
