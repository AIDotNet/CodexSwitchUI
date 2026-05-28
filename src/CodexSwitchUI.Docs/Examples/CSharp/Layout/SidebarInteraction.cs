using Avalonia.Controls;
using Avalonia.Input;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System.Windows.Input;

public static class SidebarInteractionSample
{
    public static Control BuildSidebarInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Sidebar is expanded."
        };
        var provider = new CodexSidebarProvider();
        provider.OpenChanged += (_, args) =>
        {
            status.Text = args.IsOpen ? "Sidebar is expanded." : "Sidebar is collapsed.";
        };

        var trigger = new CodexSidebarTrigger { Content = "Toggle" };
        var shortcut = new CodexButton
        {
            Content = "Run Ctrl+B",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Outline
        };
        shortcut.Click += (_, _) => provider.TryHandleShortcut(Key.B, KeyModifiers.Control);

        var blockedTrigger = new CodexSidebarTrigger
        {
            Content = "Blocked trigger",
            Command = new SampleCommand(() => status.Text = "Blocked trigger executed.", () => false)
        };
        var blockedRail = new CodexSidebarRail
        {
            Command = new SampleCommand(() => status.Text = "Blocked rail executed.", () => false)
        };

        provider.Content = new Grid
        {
            Height = 320,
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(240)),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10,
            Children =
            {
                new CodexSidebar
                {
                    Variant = CodexSidebarVariant.Inset,
                    Collapsible = CodexSidebarCollapsible.Icon,
                    Content = new StackPanel
                    {
                        Children =
                        {
                            new CodexSidebarHeader { Content = "Interactive" },
                            new CodexSidebarContent
                            {
                                Content = new CodexSidebarMenuButton { Content = "Providers", IsActive = true, Badge = "12" }
                            }
                        }
                    }
                },
                GridCell(new CodexSidebarRail(), 0, 1),
                GridCell(new CodexSection
                {
                    Title = "Interaction contract",
                    Description = "Trigger, rail, and shortcut publish one provider state change.",
                    Actions = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 8,
                        Children = { trigger, shortcut }
                    },
                    Content = new StackPanel
                    {
                        Spacing = 8,
                        Children = { status, blockedTrigger, blockedRail }
                    }
                }, 0, 2)
            }
        };

        return provider;
    }

    private static T GridCell<T>(T control, int row, int column)
        where T : Control
    {
        Grid.SetRow(control, row);
        Grid.SetColumn(control, column);
        return control;
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
