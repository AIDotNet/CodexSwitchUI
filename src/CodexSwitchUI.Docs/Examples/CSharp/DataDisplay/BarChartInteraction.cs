using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class BarChartInteractionSample
{
    public static Control BuildBarChartInteractionPreview()
    {
        var status = Muted("Refresh data, toggle orientation, hide grid/axis labels, and reduce motion.");
        var burst = false;
        var chart = new CodexBarChart
        {
            Width = 560,
            ItemsSource = BarChartItems()
        };
        var normalDuration = chart.AnimationDuration;
        chart.ActiveItemChanged += (_, args) =>
        {
            status.Text = args.NewItem is null
                ? "Active bar cleared."
                : $"Active bar changed to {args.NewItem.Label}: {args.NewItem.ValueText}.";
        };

        var refresh = Button("Refresh data", CodexControlVariant.Secondary);
        refresh.Click += (_, _) =>
        {
            burst = !burst;
            chart.ItemsSource = burst ? BarChartBurstItems() : BarChartItems();
            status.Text = burst ? "Burst usage loaded; bar reveal restarted." : "Baseline usage restored; ordered bars stayed mounted.";
        };

        var orientation = Button("Orientation");
        orientation.Click += (_, _) =>
        {
            chart.Orientation = chart.Orientation == Orientation.Vertical ? Orientation.Horizontal : Orientation.Vertical;
            chart.Width = chart.Orientation == Orientation.Horizontal ? 600 : 560;
            status.Text = chart.Orientation == Orientation.Horizontal
                ? "Horizontal bar mode uses the same ItemsSource and active-item path."
                : "Vertical bar mode restored.";
        };

        var grid = Button("Grid");
        grid.Click += (_, _) =>
        {
            chart.ShowGridLines = !chart.ShowGridLines;
            status.Text = chart.ShowGridLines ? "Grid lines and zero baseline enabled." : "Grid lines hidden; bar hit testing is unchanged.";
        };

        var axis = Button("Axis labels");
        axis.Click += (_, _) =>
        {
            chart.ShowAxisLabels = !chart.ShowAxisLabels;
            status.Text = chart.ShowAxisLabels ? "Axis labels restored." : "Axis labels hidden for a compact dashboard tile.";
        };

        var motion = Button("Reduce motion");
        motion.Click += (_, _) =>
        {
            chart.AnimationDuration = chart.AnimationDuration == TimeSpan.Zero ? normalDuration : TimeSpan.Zero;
            status.Text = chart.AnimationDuration == TimeSpan.Zero
                ? "Reduced motion enabled; refresh renders final bars immediately."
                : "Tokenized bar reveal motion restored.";
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
                    Title = "Interactive bar chart",
                    Description = "Pointer movement updates active bar state while data changes redraw through motion tokens.",
                    Legend = new CodexChartLegend
                    {
                        Items =
                        {
                            new CodexChartLegendItem
                            {
                                Content = "Requests",
                                Value = "live",
                                IndicatorStyle = CodexChartIndicatorStyle.Square
                            }
                        }
                    },
                    Content = chart
                },
                Row(refresh, orientation, grid, axis, motion)
            }
        };
    }

    private static CodexBarChartItem[] BarChartItems()
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

    private static CodexBarChartItem[] BarChartBurstItems()
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
