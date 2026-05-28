using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.ECharts.Abstractions;
using CodexSwitchUI.ECharts.Controls;
using CodexSwitchUI.ECharts.Models;
using CodexSwitchUI.Primitives;

public static class UsageTrendChartInteractionSample
{
    public static Control BuildUsageTrendChartInteractionPreview()
    {
        var chart = new CsUsageTrendChart
        {
            Width = 620,
            Height = 300,
            Granularity = UsageTrendChartGranularity.Hour,
            ItemsSource = UsageTrendChartItems(),
            RefreshingText = "Refreshing usage"
        };
        var status = Muted("Move across the plot to reveal the pointer marker and detailed tooltip.");
        var showingBurstData = false;

        var refresh = Button("Toggle refresh", CodexControlVariant.Secondary);
        refresh.Click += (_, _) =>
        {
            chart.IsRefreshing = !chart.IsRefreshing;
            status.Text = chart.IsRefreshing
                ? "Refresh overlay is running while data remains mounted."
                : "Refresh overlay stopped without clearing the chart data.";
        };

        var granularity = Button("Switch granularity");
        granularity.Click += (_, _) =>
        {
            chart.Granularity = chart.Granularity == UsageTrendChartGranularity.Hour
                ? UsageTrendChartGranularity.Day
                : UsageTrendChartGranularity.Hour;
            chart.ItemsSource = chart.Granularity == UsageTrendChartGranularity.Day
                ? UsageTrendChartDailyItems()
                : UsageTrendChartItems();
            status.Text = $"{chart.Granularity} granularity rebuilt axis labels and restarted chart motion.";
        };

        var data = Button("Refresh series");
        data.Click += (_, _) =>
        {
            showingBurstData = !showingBurstData;
            chart.ItemsSource = chart.Granularity == UsageTrendChartGranularity.Day
                ? UsageTrendChartDailyItems()
                : showingBurstData
                    ? UsageTrendChartBurstItems()
                    : UsageTrendChartItems();
            status.Text = "ItemsSource changed; series, axes, bands, and cost line were rebuilt.";
        };

        var empty = Button("Empty state");
        empty.Click += (_, _) =>
        {
            chart.IsRefreshing = false;
            chart.ItemsSource = Array.Empty<UsageTrendChartPoint>();
            status.Text = "Empty state is chart-owned and keeps the plot frame mounted.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                chart,
                Row(refresh, granularity, data, empty)
            }
        };
    }

    private static UsageTrendChartPoint[] UsageTrendChartItems()
    {
        var start = new DateTimeOffset(2026, 5, 26, 8, 0, 0, TimeSpan.Zero);
        return
        [
            UsagePoint(start.AddHours(0), 12, 18_400, 7_200, 1_100, 8_600, 2_400, 3_800, 0.21m),
            UsagePoint(start.AddHours(1), 18, 22_900, 8_800, 1_700, 10_200, 3_100, 4_200, 0.28m),
            UsagePoint(start.AddHours(2), 14, 16_300, 6_200, 900, 7_700, 1_800, 3_400, 0.18m),
            UsagePoint(start.AddHours(3), 24, 31_600, 11_400, 2_800, 13_600, 4_100, 4_900, 0.36m)
        ];
    }

    private static UsageTrendChartPoint[] UsageTrendChartBurstItems()
    {
        var start = new DateTimeOffset(2026, 5, 26, 8, 0, 0, TimeSpan.Zero);
        return
        [
            UsagePoint(start.AddHours(0), 16, 21_200, 9_600, 1_300, 9_900, 2_800, 3_700, 0.25m),
            UsagePoint(start.AddHours(1), 31, 42_900, 17_800, 3_900, 19_200, 7_100, 5_800, 0.54m),
            UsagePoint(start.AddHours(2), 27, 36_300, 14_200, 3_400, 15_700, 5_800, 4_900, 0.43m),
            UsagePoint(start.AddHours(3), 38, 51_600, 20_400, 4_800, 23_600, 8_100, 6_900, 0.68m)
        ];
    }

    private static UsageTrendChartPoint[] UsageTrendChartDailyItems()
    {
        var start = new DateTimeOffset(2026, 5, 20, 0, 0, 0, TimeSpan.Zero);
        return
        [
            UsagePoint(start.AddDays(0), 142, 118_400, 57_200, 11_100, 48_600, 12_400, 3_800, 1.84m),
            UsagePoint(start.AddDays(1), 168, 142_900, 68_800, 14_700, 60_200, 18_100, 4_200, 2.28m),
            UsagePoint(start.AddDays(2), 134, 116_300, 56_200, 9_900, 47_700, 11_800, 3_400, 1.78m),
            UsagePoint(start.AddDays(3), 194, 181_600, 91_400, 22_800, 73_600, 24_100, 4_900, 2.96m)
        ];
    }

    private static UsageTrendChartPoint UsagePoint(
        DateTimeOffset timestamp,
        long requests,
        long input,
        long cached,
        long cacheWrite,
        long output,
        long reasoning,
        long outputDurationMs,
        decimal cost)
    {
        return new UsageTrendChartPoint
        {
            Timestamp = timestamp,
            Requests = requests,
            InputTokens = input,
            CachedInputTokens = cached,
            CacheCreationInputTokens = cacheWrite,
            OutputTokens = output,
            ReasoningOutputTokens = reasoning,
            OutputDurationMs = outputDurationMs,
            Cost = cost
        };
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
