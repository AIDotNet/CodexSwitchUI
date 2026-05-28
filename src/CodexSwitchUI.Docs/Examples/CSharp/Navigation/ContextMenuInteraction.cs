using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;
using System.Windows.Input;

public static class ContextMenuInteractionSample
{
    public static Control BuildContextMenuInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "ItemSelected: choose a context leaf."
        };

        var openSession = new CodexContextMenuItem
        {
            Header = "Open session",
            Shortcut = "Cmd+O",
            IsActive = true
        };
        openSession.ItemSelected += (_, args) =>
        {
            status.Text = $"ItemSelected: {args.Header} source={args.Source} close={args.DidCloseOnSelect}.";
        };

        var pinned = new CodexContextMenuItem
        {
            Header = "Pinned",
            ToggleType = MenuItemToggleType.CheckBox,
            IsChecked = true
        };
        pinned.ItemSelected += (_, args) =>
        {
            status.Text = $"ItemSelected: {args.Header} checked={args.IsChecked} source={args.Source}.";
        };

        var compact = new CodexContextMenuItem
        {
            Header = "Compact mode",
            ToggleType = MenuItemToggleType.Radio,
            IsChecked = true,
            IsInset = true
        };
        compact.ItemSelected += (_, args) =>
        {
            status.Text = $"ItemSelected: {args.Header} radio={args.IsChecked} close={args.DidCloseOnSelect}.";
        };

        var contextMenu = new CodexContextMenu
        {
            MinWidth = 240,
            Placement = PlacementMode.Right,
            Items =
            {
                new CodexContextMenuLabel { Content = "Right side" },
                openSession,
                new CodexContextMenuItem
                {
                    Header = "Move to",
                    IsSubMenuOpen = true,
                    SubMenuPlacement = PlacementMode.RightEdgeAlignedTop,
                    Items =
                    {
                        new CodexContextMenuItem { Header = "Archive" },
                        new CodexContextMenuItem { Header = "Favorites" }
                    }
                },
                new CodexContextMenuSeparator(),
                pinned,
                compact,
                new CodexContextMenuItem { Header = "Disabled leaf", IsEnabled = false },
                new CodexContextMenuItem
                {
                    Header = "Command blocked",
                    Command = new SampleCommand(() => status.Text = "Blocked context command executed.", () => false)
                }
            }
        };
        contextMenu.Classes.Add("context-menu-open");

        var leftMenu = new CodexContextMenu
        {
            MinWidth = 220,
            Placement = PlacementMode.Left,
            IsLoading = true,
            Items =
            {
                new CodexContextMenuLabel { Content = "Loading gate", IsInset = true },
                new CodexContextMenuItem
                {
                    Header = "Move left",
                    IsSubMenuOpen = true,
                    SubMenuPlacement = PlacementMode.LeftEdgeAlignedTop,
                    Items =
                    {
                        new CodexContextMenuItem { Header = "Inbox" },
                        new CodexContextMenuItem { Header = "Backlog" }
                    }
                }
            }
        };
        leftMenu.Classes.Add("context-menu-open");

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                contextMenu,
                leftMenu
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
