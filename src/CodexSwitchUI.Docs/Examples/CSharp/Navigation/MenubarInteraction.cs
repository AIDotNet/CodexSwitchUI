using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class MenubarInteractionSample
{
    public static Control BuildMenubarInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "ItemSelected: ready."
        };

        var newTab = new CodexMenubarItem { Header = "New tab", Shortcut = "Cmd+T" };
        newTab.ItemSelected += (_, args) =>
        {
            status.Text = $"ItemSelected: {args.Header} via {args.Source}.";
        };

        var compact = new CodexMenubarCheckboxItem
        {
            Header = "Compact mode",
            IsChecked = true
        };
        compact.ItemSelected += (_, args) =>
        {
            status.Text = $"ItemSelected: {args.Header} checked={args.IsChecked} close={args.DidCloseOnSelect}.";
        };

        var file = new CodexMenubarMenu
        {
            Header = "File",
            Items =
            {
                newTab,
                new CodexMenubarItem { Header = "New window", Shortcut = "Cmd+N" },
                new CodexMenubarSeparator(),
                new CodexMenubarItem
                {
                    Header = "Share",
                    Items =
                    {
                        new CodexMenubarItem { Header = "Copy link" },
                        new CodexMenubarItem { Header = "Export markdown" }
                    }
                }
            }
        };
        var edit = new CodexMenubarMenu
        {
            Header = "Edit",
            Items =
            {
                new CodexMenubarItem { Header = "Undo", Shortcut = "Cmd+Z" },
                new CodexMenubarItem
                {
                    Header = "Find",
                    Items =
                    {
                        new CodexMenubarItem { Header = "Search the web" },
                        new CodexMenubarSeparator(),
                        new CodexMenubarItem { Header = "Find", Shortcut = "Cmd+F" },
                        new CodexMenubarItem { Header = "Find next", Shortcut = "Cmd+G" }
                    }
                },
                new CodexMenubarSeparator(),
                new CodexMenubarItem { Header = "Cut" },
                new CodexMenubarItem { Header = "Copy" },
                new CodexMenubarItem { Header = "Paste" }
            }
        };
        var view = new CodexMenubarMenu
        {
            Header = "View",
            Items =
            {
                compact,
                new CodexMenubarCheckboxItem { Header = "Show full URLs" },
                new CodexMenubarSeparator(),
                new CodexMenubarItem { Header = "Reload", Shortcut = "Cmd+R" }
            }
        };

        var menubar = new CodexMenubar
        {
            Loop = true,
            Items =
            {
                file,
                edit,
                view
            }
        };
        menubar.ActiveMenuChanged += (_, args) =>
        {
            status.Text = $"ActiveMenuChanged: {args.OldMenu?.Header ?? "none"} -> {args.NewMenu?.Header ?? "none"}.";
        };

        var blocked = new CodexMenubar
        {
            IsLoading = true,
            Items =
            {
                new CodexMenubarMenu
                {
                    Header = "Loading",
                    Items =
                    {
                        new CodexMenubarItem { Header = "Selection suppressed" }
                    }
                },
                new CodexMenubarMenu
                {
                    Header = "View",
                    Items =
                    {
                        new CodexMenubarCheckboxItem { Header = "Locked checked", IsChecked = true }
                    }
                }
            }
        };

        var openView = new CodexButton
        {
            Content = "Open View",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        openView.Click += (_, _) => menubar.OpenMenu(view);

        var dismiss = new CodexButton
        {
            Content = "Dismiss",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        dismiss.Click += (_, _) =>
        {
            if (menubar.Dismiss())
            {
                status.Text = "Dismiss closed the active menu.";
            }
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                menubar,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        openView,
                        dismiss
                    }
                },
                blocked
            }
        };
    }
}
