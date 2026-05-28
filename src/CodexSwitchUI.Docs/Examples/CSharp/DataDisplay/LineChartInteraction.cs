using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class LineChartInteractionSample
{
    public static Control BuildLineChartInteractionPreview()
    {
        var status = Muted("Refresh data, toggle density, switch line/area rendering, and reduce motion.");
        var burst = false;
        var chart = new CodexLineChart
        {
            Width = 560,
            ItemsSource = LineChartItems()
        };
        var normalDuration = chart.AnimationDuration;
        chart.ActivePointChanged += (_, args) =>
        {
            status.Text = args.NewPoint is null
                ? "Active point cleared."
                : $"Active point changed to {args.NewPoint.Label}: {args.NewPoint.ValueText}.";
        };

        var refresh = Button("Refresh data", CodexControlVariant.Secondary);
        refresh.Click += (_, _) =>
        {
            burst = !burst;
            chart.ItemsSource = burst ? LineChartBurstItems() : LineChartItems();
            status.Text = burst ? "Burst trend loaded; line and area reveal restarted." : "Baseline trend restored; ordered points stayed mounted.";
        };

        var density = Button("Density");
        density.Click += (_, _) =>
        {
            chart.IsCompact = !chart.IsCompact;
            chart.Width = chart.IsCompact ? 420 : 560;
            status.Text = chart.IsCompact ? "Compact density hides axis labels and tightens the plot." : "Default line chart density restored.";
        };

        var area = Button("Area");
        area.Click += (_, _) =>
        {
            chart.ShowArea = !chart.ShowArea;
            status.Text = chart.ShowArea ? "Area fill enabled under the smoothed line." : "Line-only mode enabled without changing active-point hit testing.";
        };

        var dots = Button("Dots");
        dots.Click += (_, _) =>
        {
            chart.ShowDots = !chart.ShowDots;
            status.Text = chart.ShowDots ? "Point markers restored." : "Point markers hidden while hover remains active.";
        };

        var motion = Button("Reduce motion");
        motion.Click += (_, _) =>
        {
            chart.AnimationDuration = chart.AnimationDuration == TimeSpan.Zero ? normalDuration : TimeSpan.Zero;
            motion.Content = chart.AnimationDuration == TimeSpan.Zero ? "Restore motion" : "Reduce motion";
            status.Text = chart.AnimationDuration == TimeSpan.Zero
                ? "Reduced motion jumps line refreshes to the final plot state."
                : "Tokenized line reveal motion restored.";
        };

        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                status,
                new CodexChartContainer
                {
                    Width = 650,
                    Title = "Interactive line chart",
                    Description = "Pointer movement updates active point state while data changes redraw through motion tokens.",
                    Legend = new CodexChartLegend
                    {
                        Items =
                        {
                            new CodexChartLegendItem
                            {
                                Content = "Requests",
                                Value = "live",
                                IndicatorStyle = CodexChartIndicatorStyle.Line
                            }
                        }
                    },
                    Content = chart
                },
                Row(refresh, density, area, dots, motion)
            }
        };
    }

    private static CodexLineChartPoint[] LineChartItems()
    {
        return
        [
            new("Jan", 186, "186", "Baseline requests"),
            new("Feb", 305, "305", "Routing enabled"),
            new("Mar", 237, "237", "Cache warmup"),
            new("Apr", 428, "428", "Batch imports"),
            new("May", 512, "512", "Peak traffic"),
            new("Jun", 468, "468", "Stable rollout")
        ];
    }

    private static CodexLineChartPoint[] LineChartBurstItems()
    {
        return
        [
            new("Jan", 226, "226", "Baseline"),
            new("Feb", 372, "372", "Routing"),
            new("Mar", 318, "318", "Cache"),
            new("Apr", 604, "604", "Burst"),
            new("May", 688, "688", "Peak"),
            new("Jun", 631, "631", "Stable")
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
