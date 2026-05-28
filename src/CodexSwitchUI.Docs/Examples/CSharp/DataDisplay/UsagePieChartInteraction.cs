using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class UsagePieChartInteractionSample
{
    public static Control BuildUsagePieChartInteractionPreview()
    {
        var chart = new CodexUsagePieChart
        {
            Width = 460,
            Height = 240,
            TotalLabel = "Tokens",
            TotalValue = "71.5K",
            ItemsSource = UsagePieChartItems()
        };
        var normalDuration = chart.AnimationDuration;
        var status = Muted("Move the pointer across slices to reveal ActiveItemChanged and the interpolated tooltip.");
        var useBurstData = false;
        chart.ActiveItemChanged += (_, args) =>
        {
            status.Text = args.NewItem is null
                ? "Active slice cleared."
                : $"Active slice changed to {args.NewItem.Label}: {args.NewItem.ValueText}.";
        };

        var refresh = Button("Refresh data", CodexControlVariant.Secondary);
        refresh.Click += (_, _) =>
        {
            useBurstData = !useBurstData;
            chart.TotalValue = useBurstData ? "89.4K" : "71.5K";
            chart.ItemsSource = useBurstData ? UsagePieChartBurstItems() : UsagePieChartItems();
            status.Text = "ItemsSource changed; the donut redraw animation restarts from the Web-style chart path.";
        };

        var density = Button("Toggle density");
        density.Click += (_, _) =>
        {
            chart.IsCompact = !chart.IsCompact;
            chart.Width = chart.IsCompact ? 360 : 460;
            chart.Height = chart.IsCompact ? 214 : 240;
            status.Text = chart.IsCompact
                ? "Compact density keeps hover and tooltip interpolation active."
                : "Full density restores the legend and center label spacing.";
        };

        var motion = Button("Reduce motion");
        motion.Click += (_, _) =>
        {
            chart.AnimationDuration = chart.AnimationDuration == TimeSpan.Zero ? normalDuration : TimeSpan.Zero;
            motion.Content = chart.AnimationDuration == TimeSpan.Zero ? "Restore motion" : "Reduce motion";
            status.Text = chart.AnimationDuration == TimeSpan.Zero
                ? "Reduced motion jumps refreshes to the final chart state."
                : "Motion restored; collection refreshes animate again.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                chart,
                Row(refresh, density, motion),
                new CodexUsagePieChart
                {
                    Width = 340,
                    Height = 178,
                    IsCompact = true,
                    TotalLabel = "Tokens",
                    TotalValue = "0",
                    ItemsSource = []
                },
                new CodexUsagePieChart
                {
                    Width = 340,
                    Height = 178,
                    IsCompact = true,
                    TotalLabel = "Requests",
                    TotalValue = "1.2K",
                    AnimationDuration = TimeSpan.Zero,
                    ItemsSource = UsagePieChartCompactItems()
                }
            }
        };
    }

    private static CodexUsagePieChartItem[] UsagePieChartItems()
    {
        return
        [
            new("gpt-5", 42.7, "60%", "42.7K tokens"),
            new("claude-sonnet", 18.3, "26%", "18.3K tokens"),
            new("o4-mini", 7.1, "10%", "7.1K tokens"),
            new("fallback", 3.4, "4%", "3.4K tokens")
        ];
    }

    private static CodexUsagePieChartItem[] UsagePieChartBurstItems()
    {
        return
        [
            new("gpt-5", 51.2, "57%", "51.2K tokens"),
            new("claude-sonnet", 21.9, "25%", "21.9K tokens"),
            new("gemini", 10.6, "12%", "10.6K tokens"),
            new("local", 5.7, "6%", "5.7K tokens")
        ];
    }

    private static CodexUsagePieChartItem[] UsagePieChartCompactItems()
    {
        return
        [
            new("streaming", 820, "68%", "820 requests"),
            new("batch", 386, "32%", "386 requests")
        ];
    }

    private static CodexButton Button(string label, CodexControlVariant variant = CodexControlVariant.Ghost)
    {
        return new CodexButton { Content = label, Size = CodexControlSize.Small, Variant = variant };
    }

    private static StackPanel Row(params Control[] children)
    {
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        foreach (var child in children)
            row.Children.Add(child);
        return row;
    }

    private static CodexText Muted(string text)
    {
        return new CodexText { Role = CodexTextRole.Muted, Text = text };
    }
}
