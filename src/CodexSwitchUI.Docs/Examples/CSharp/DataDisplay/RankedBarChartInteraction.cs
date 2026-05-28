using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class RankedBarChartInteractionSample
{
    public static Control BuildRankedBarChartInteractionPreview()
    {
        var status = Muted("Move across rows to reveal ActiveItemChanged, or refresh the ranked chart.");
        var burst = false;
        var chart = new CodexRankedBarChart
        {
            Width = 420,
            MaxVisibleItems = 4,
            ItemsSource = RankedBarChartItems()
        };
        chart.ActiveItemChanged += (_, args) =>
        {
            status.Text = args.NewItem is null
                ? "Active ranked row cleared."
                : $"Active ranked row changed to {args.NewItem.Label}: {args.NewItem.ValueText}.";
        };

        var refresh = Button("Refresh data", CodexControlVariant.Secondary);
        refresh.Click += (_, _) =>
        {
            burst = !burst;
            chart.ItemsSource = burst ? RankedBarChartBurstItems() : RankedBarChartItems();
            status.Text = burst
                ? "Burst dataset loaded; row bars remeasured against the new maximum."
                : "Default dataset restored; chart rows rebuilt from ItemsSource.";
        };

        var density = Button("Toggle compact");
        density.Click += (_, _) =>
        {
            chart.IsCompact = !chart.IsCompact;
            chart.RowHeight = chart.IsCompact ? 28 : 34;
            chart.RowSpacing = chart.IsCompact ? 6 : 10;
            status.Text = chart.IsCompact ? "Compact density reduced row height and spacing." : "Default density restored row height and spacing.";
        };

        var maxVisible = Button("Toggle max rows", CodexControlVariant.Secondary);
        maxVisible.Click += (_, _) =>
        {
            chart.MaxVisibleItems = chart.MaxVisibleItems == 4 ? 2 : 4;
            status.Text = $"MaxVisibleItems changed to {chart.MaxVisibleItems}.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                chart,
                Row(refresh, density, maxVisible),
                new CodexRankedBarChart
                {
                    Width = 360,
                    ItemsSource = [],
                    EmptyText = "No ranked usage"
                },
                new CodexRankedBarChart
                {
                    Width = 360,
                    IsCompact = true,
                    MaxVisibleItems = 3,
                    ItemsSource = RankedBarChartItems()
                }
            }
        };
    }

    private static CodexRankedBarChartItem[] RankedBarChartItems()
    {
        return
        [
            new("gpt-5", 42.7, "42.7K", "$0.84"),
            new("claude-sonnet", 18.3, "18.3K", "$0.41"),
            new("o4-mini", 7.1, "7.1K", "$0.09"),
            new("fallback", 3.4, "3.4K", "$0.04")
        ];
    }

    private static CodexRankedBarChartItem[] RankedBarChartBurstItems()
    {
        return
        [
            new("gpt-5", 54.8, "54.8K", "$1.08"),
            new("claude-sonnet", 28.6, "28.6K", "$0.63"),
            new("gemini", 12.4, "12.4K", "$0.18"),
            new("local", 6.8, "6.8K", "$0.00")
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
