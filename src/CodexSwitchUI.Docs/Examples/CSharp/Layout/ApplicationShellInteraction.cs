using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class ApplicationShellInteractionSample
{
    public static Control BuildApplicationShellInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Active page: Providers."
        };
        var providerCount = 12;
        var providers = new CodexSidebarMenuButton { Content = "Providers", IsActive = true, Badge = providerCount.ToString() };
        var sessions = new CodexSidebarMenuButton { Content = "Sessions", Badge = "Live" };
        var usage = new CodexSidebarMenuButton { Content = "Usage" };
        var buttons = new[] { providers, sessions, usage };
        var section = new CodexSection
        {
            Title = "Providers",
            Description = "Sidebar navigation swaps content without rebuilding the shell.",
            Actions = new CodexButton { Content = "Add", Size = CodexControlSize.Small },
            Content = ProviderList()
        };

        void Select(CodexSidebarMenuButton selected, string title, Control content)
        {
            foreach (var button in buttons)
            {
                button.IsActive = ReferenceEquals(button, selected);
            }

            section.Title = title;
            section.Description = $"Active button is {title}; siblings were cleared.";
            section.Content = content;
            status.Text = $"Active page: {title}.";
        }

        providers.Click += (_, _) => Select(providers, "Providers", ProviderList());
        sessions.Click += (_, _) => Select(
            sessions,
            "Sessions",
            new CodexBadge { Content = "3 active", Variant = CodexControlVariant.Success, IsStatusVisible = true });
        usage.Click += (_, _) => Select(
            usage,
            "Usage",
            new CodexProgress { Value = 64, Variant = CodexControlVariant.Success });

        var newProvider = new CodexButton
        {
            Content = "New provider",
            Size = CodexControlSize.Small,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        newProvider.Click += (_, _) =>
        {
            providerCount++;
            providers.Badge = providerCount.ToString();
            status.Text = $"Provider badge incremented to {providerCount}.";
        };

        return new Grid
        {
            Height = 380,
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(260)),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 18,
            Children =
            {
                new CodexSidebar
                {
                    Content = new StackPanel
                    {
                        Children =
                        {
                            new CodexSidebarHeader { Content = status },
                            new CodexSidebarContent
                            {
                                Content = new StackPanel
                                {
                                    Spacing = 6,
                                    Children = { providers, sessions, usage }
                                }
                            },
                            new CodexSidebarFooter { Content = newProvider }
                        }
                    }
                },
                GridCell(section, 0, 1)
            }
        };
    }

    private static Control ProviderList()
    {
        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new CodexCard { Title = "OpenAI", Description = "Primary route" },
                new CodexCard { Title = "Claude", Description = "Fallback route" }
            }
        };
    }

    private static T GridCell<T>(T control, int row, int column)
        where T : Control
    {
        Grid.SetRow(control, row);
        Grid.SetColumn(control, column);
        return control;
    }
}
