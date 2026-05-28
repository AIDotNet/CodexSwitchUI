using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;
using System.Windows.Input;

public static class BreadcrumbInteractionSample
{
    public static Control BuildBreadcrumbInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "LinkActivated: waiting for an ancestor route."
        };

        var docsLink = new CodexBreadcrumbLink
        {
            Content = "Docs",
            Href = "/docs"
        };
        var currentLink = new CodexBreadcrumbLink
        {
            Content = "Breadcrumb",
            IsCurrent = true,
            Command = new SampleCommand(() => status.Text = "Current page command should stay suppressed.")
        };
        var blockedLink = new CodexBreadcrumbLink
        {
            Content = "Settings",
            Href = "/settings",
            Command = new SampleCommand(() => status.Text = "Blocked route executed.", () => false)
        };

        var breadcrumb = new CodexBreadcrumb
        {
            Label = "Route breadcrumb",
            Content = new CodexBreadcrumbList
            {
                Items =
                {
                    new CodexBreadcrumbItem { Content = new CodexBreadcrumbLink { Content = "Home", Href = "/" } },
                    new CodexBreadcrumbSeparator(),
                    new CodexBreadcrumbItem { Content = docsLink },
                    new CodexBreadcrumbSeparator(),
                    new CodexBreadcrumbItem { Content = blockedLink },
                    new CodexBreadcrumbSeparator(),
                    new CodexBreadcrumbItem { IsCurrent = true, Content = currentLink }
                }
            }
        };
        breadcrumb.LinkActivated += (_, args) =>
        {
            status.Text = $"LinkActivated: index {args.Index}, href {args.Href ?? "none"}, source={args.Source}.";
        };

        var activateDocs = new CodexButton
        {
            Content = "Activate Docs",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        activateDocs.Click += (_, _) => docsLink.TryActivate();

        var activateCurrent = new CodexButton
        {
            Content = "Try current",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        activateCurrent.Click += (_, _) =>
        {
            status.Text = currentLink.TryActivate()
                ? "Current page activated."
                : "Current page activation was suppressed.";
        };

        var collapsedTrail = new CodexBreadcrumb
        {
            Label = "Collapsed breadcrumb",
            Content = new CodexBreadcrumbList
            {
                Items =
                {
                    new CodexBreadcrumbItem { Content = new CodexBreadcrumbLink { Content = "Home" } },
                    new CodexBreadcrumbSeparator(),
                    new CodexBreadcrumbItem
                    {
                        Content = new CodexDropdownButton
                        {
                            Content = "...",
                            IsOpen = true,
                            Size = CodexControlSize.Small,
                            Variant = CodexControlVariant.Ghost,
                            IsArrowVisible = false,
                            DropDownContent = new StackPanel
                            {
                                Width = 168,
                                Children =
                                {
                                    new CodexButton { Content = "Components", Variant = CodexControlVariant.Ghost, HorizontalAlignment = HorizontalAlignment.Stretch },
                                    new CodexButton { Content = "Navigation", Variant = CodexControlVariant.Ghost, HorizontalAlignment = HorizontalAlignment.Stretch }
                                }
                            }
                        }
                    },
                    new CodexBreadcrumbSeparator(),
                    new CodexBreadcrumbItem { IsCurrent = true, Content = new CodexBreadcrumbPage { Content = "Breadcrumb" } }
                }
            }
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                breadcrumb,
                status,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        activateDocs,
                        activateCurrent
                    }
                },
                collapsedTrail
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
