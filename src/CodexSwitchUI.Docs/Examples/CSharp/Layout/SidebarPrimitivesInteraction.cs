using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class SidebarPrimitivesInteractionSample
{
    public static Control BuildSidebarPrimitivesInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Active route: Active routes."
        };
        var activeRoutes = new CodexSidebarMenuButton
        {
            Content = "Active routes",
            Icon = "P",
            IsActive = true,
            Badge = new CodexSidebarMenuBadge { Content = "12" }
        };
        var usage = new CodexSidebarMenuButton { Content = "Usage", Icon = "U", Badge = "Live" };
        var disabled = new CodexSidebarMenuButton { Content = "Disabled route", Icon = "D", Badge = "Locked", IsEnabled = false };
        var menuButtons = new[] { activeRoutes, usage, disabled };

        void SelectMenu(CodexSidebarMenuButton selected, string name)
        {
            if (!selected.IsEnabled)
            {
                status.Text = "Disabled sidebar rows ignore activation.";
                return;
            }

            foreach (var button in menuButtons)
            {
                button.IsActive = ReferenceEquals(button, selected);
            }

            status.Text = $"Active route: {name}.";
        }

        activeRoutes.Click += (_, _) => SelectMenu(activeRoutes, "Active routes");
        usage.Click += (_, _) => SelectMenu(usage, "Usage");
        disabled.Click += (_, _) => SelectMenu(disabled, "Disabled route");

        var action = new CodexSidebarMenuAction { Content = "...", IsShowOnHover = true };
        action.Click += (_, _) =>
        {
            action.IsActive = !action.IsActive;
            status.Text = action.IsActive ? "Hover action marked active." : "Hover action cleared.";
        };

        var refreshBadge = new CodexSidebarGroupAction { Content = "+" };
        var count = 12;
        refreshBadge.Click += (_, _) =>
        {
            count++;
            activeRoutes.Badge = new CodexSidebarMenuBadge { Content = count.ToString() };
            status.Text = $"Active routes badge refreshed to {count}.";
        };

        var routing = new CodexSidebarMenuSubButton { Content = "Routing", IsActive = true };
        var security = new CodexSidebarMenuSubButton { Content = "Security" };
        var billing = new CodexSidebarMenuSubButton { Content = "Billing", IsEnabled = false };
        var nested = new[] { routing, security, billing };

        void SelectNested(CodexSidebarMenuSubButton selected, string name)
        {
            if (!selected.IsEnabled)
            {
                status.Text = "Disabled nested row ignored activation.";
                return;
            }

            foreach (var button in nested)
            {
                button.IsActive = ReferenceEquals(button, selected);
            }

            status.Text = $"Nested route: {name}.";
        }

        routing.Click += (_, _) => SelectNested(routing, "Routing");
        security.Click += (_, _) => SelectNested(security, "Security");
        billing.Click += (_, _) => SelectNested(billing, "Billing");

        return new CodexSidebar
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
                            Spacing = 8,
                            Children =
                            {
                                refreshBadge,
                                activeRoutes,
                                action,
                                usage,
                                disabled,
                                new CodexSidebarMenuSub
                                {
                                    Items =
                                    {
                                        new CodexSidebarMenuSubItem { Content = routing },
                                        new CodexSidebarMenuSubItem { Content = security },
                                        new CodexSidebarMenuSubItem { Content = billing }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
