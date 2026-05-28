using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class HoverCardInteractionSample
{
    public static Control BuildHoverCardInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "OpenChanged: hover card starts closed."
        };
        var card = new CodexHoverCard
        {
            Trigger = new CodexButton { Content = "Provider details", Size = CodexControlSize.Small },
            Placement = PlacementMode.Right,
            Align = CodexHoverCardAlign.Start,
            OpenDelay = TimeSpan.FromMilliseconds(700),
            CloseDelay = TimeSpan.FromMilliseconds(300),
            Content = ProviderContent("OpenAI", "Primary route", "Healthy")
        };
        card.OpenChanged += (_, args) =>
        {
            status.Text = args.IsOpen
                ? $"OpenChanged: hover card opened by {args.Source}."
                : $"OpenChanged: hover card closed by {args.Source}.";
        };

        var open = new CodexButton
        {
            Content = "Open",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        open.Click += (_, _) => card.Open();

        var dismiss = new CodexButton
        {
            Content = "Dismiss",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        dismiss.Click += (_, _) => card.Dismiss();

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                card,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        open,
                        dismiss
                    }
                },
                new CodexHoverCard
                {
                    Trigger = new CodexButton { Content = "Instant focus", Size = CodexControlSize.Small },
                    IsOpen = true,
                    Placement = PlacementMode.Top,
                    Align = CodexHoverCardAlign.End,
                    OpenDelay = TimeSpan.Zero,
                    CloseDelay = TimeSpan.Zero,
                    IsArrowVisible = false,
                    Content = ProviderContent("Claude", "Fallback route", "Ready")
                },
                new CodexHoverCard
                {
                    Trigger = new CodexButton { Content = "Disabled trigger", Size = CodexControlSize.Small, IsEnabled = false },
                    IsOpen = false,
                    IsEnabled = false,
                    Content = "Disabled hover cards ignore pointer and focus open requests."
                },
                new CodexHoverCard
                {
                    Trigger = new CodexButton { Content = "Closed state", Size = CodexControlSize.Small },
                    IsOpen = false,
                    Placement = PlacementMode.Left,
                    Align = CodexHoverCardAlign.Center,
                    Content = ProviderContent("Local proxy", "Paused route", "Paused")
                }
            }
        };
    }

    private static Control ProviderContent(string name, string route, string state)
    {
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                new CodexText { Role = CodexTextRole.Title, Text = name },
                new CodexText { Role = CodexTextRole.Muted, Text = route },
                new CodexBadge { Content = state, Variant = CodexControlVariant.Success }
            }
        };
    }
}
