using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;
using System.Windows.Input;

public static class NavigationMenuInteractionSample
{
    public static Control BuildNavigationMenuInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "ActiveItemChanged: menu starts closed."
        };

        var overview = new CodexNavigationMenuItem
        {
            Header = "Overview",
            Value = "overview",
            Icon = "O",
            ViewportWidth = 360,
            Content = new CodexNavigationMenuContent
            {
                Header = "Pointer enter",
                Description = "Trigger activation opens the shared viewport.",
                Items =
                {
                    new CodexNavigationMenuLink { Content = "Getting started", Description = "Current route", IsActive = true },
                    new CodexNavigationMenuLink { Content = "Motion", Description = "Duration and easing tokens" }
                }
            }
        };
        var components = new CodexNavigationMenuItem
        {
            Header = "Components",
            Value = "components",
            Icon = "C",
            ViewportWidth = 420,
            Content = new CodexNavigationMenuContent
            {
                Header = "Arrow right",
                Description = "Horizontal arrows activate the next trigger.",
                Items =
                {
                    new CodexNavigationMenuLink { Content = "Forms", Description = "Inputs and actions" },
                    new CodexNavigationMenuLink { Content = "Overlay", Description = "Dialog and popover" }
                }
            }
        };
        var docsLink = new CodexNavigationMenuItem
        {
            Header = "Docs",
            Value = "docs",
            Icon = "D",
            CommandParameter = "docs",
            Command = new SampleCommand(() => status.Text = "Docs command executed from the link trigger.")
        };
        docsLink.Activated += (_, args) =>
        {
            status.Text = $"Activated: link parameter={args.CommandParameter}.";
        };

        var blockedLink = new CodexNavigationMenuItem
        {
            Header = "Blocked",
            Icon = "B",
            Command = new SampleCommand(() => status.Text = "Blocked command executed.", () => false)
        };

        var menu = new CodexNavigationMenu
        {
            ItemsSource = new[] { overview, components, docsLink, blockedLink }
        };
        menu.ActiveItemChanged += (_, args) =>
        {
            status.Text = args.NewItem is null
                ? "ActiveItemChanged: viewport closed."
                : $"ActiveItemChanged: {args.Value} opened at {menu.ViewportWidth:0}px.";
        };
        var verticalSelected = new CodexNavigationMenuItem
        {
            Header = "Selected",
            Value = "selected",
            Content = "Vertical viewport content"
        };
        var vertical = new CodexNavigationMenu
        {
            Orientation = Orientation.Vertical,
            Size = CodexControlSize.Small,
            ItemsSource = new[]
            {
                new CodexNavigationMenuItem { Header = "Up", Value = "up", Content = "Vertical previous item" },
                verticalSelected,
                new CodexNavigationMenuItem { Header = "Disabled", IsEnabled = false, Content = "Skipped item" },
                new CodexNavigationMenuItem { Header = "Down", Value = "down", Content = "Vertical next item" }
            }
        };

        var openComponents = new CodexButton
        {
            Content = "Open components",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        openComponents.Click += (_, _) => menu.ActivateItem(components);

        var openVertical = new CodexButton
        {
            Content = "Open vertical",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        openVertical.Click += (_, _) => vertical.ActivateItem(verticalSelected);

        var close = new CodexButton
        {
            Content = "Close viewport",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        close.Click += (_, _) => menu.CloseViewport();

        var activateLink = new CodexButton
        {
            Content = "Activate link",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        activateLink.Click += (_, _) =>
        {
            if (!docsLink.TryActivateLink())
            {
                status.Text = "Link command is blocked by CanExecute.";
            }
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                menu,
                vertical,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        openComponents,
                        openVertical,
                        close,
                        activateLink
                    }
                }
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
