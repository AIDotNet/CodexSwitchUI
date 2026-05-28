using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;
using System.Windows.Input;

public static class SideNavInteractionSample
{
    public static Control BuildSideNavInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "ValueChanged: sessions selected by host code."
        };

        var nav = new CodexSideNav
        {
            SelectedValue = "sessions",
            Content = new StackPanel
            {
                Spacing = 6,
                Children =
                {
                    new CodexSideNavItem { Value = "providers", Icon = "P", Content = "Providers", Detail = "Previously selected" },
                    new CodexSideNavItem { Value = "sessions", Icon = "S", Content = "Sessions", Detail = "Clicked row" },
                    new CodexSideNavItem { Value = "usage", Icon = "U", Content = "Usage", Detail = "Sibling cleared" },
                    new CodexSideNavItem { Value = "disabled", Icon = "D", Content = "Disabled", Detail = "Cannot select", IsEnabled = false },
                    new CodexSideNavItem
                    {
                        Value = "blocked",
                        Icon = "B",
                        Content = "Command blocked",
                        Detail = "CanExecute=false",
                        Command = new SampleCommand(() => status.Text = "Blocked row executed.", () => false)
                    },
                    new CodexSideNavItem { Value = "dense", Content = "Dense label without icon" }
                }
            }
        };
        nav.ValueChanged += (_, args) =>
        {
            status.Text = $"ValueChanged: {args.OldValue ?? "none"} -> {args.NewValue ?? "none"} (source={args.Source}).";
        };

        var selectProviders = new CodexButton
        {
            Content = "Select providers",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        selectProviders.Click += (_, _) => nav.SelectedValue = "providers";

        var selectUsage = new CodexButton
        {
            Content = "Select usage",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        selectUsage.Click += (_, _) => nav.SelectedValue = "usage";

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                nav,
                status,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        selectProviders,
                        selectUsage
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
