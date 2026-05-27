using CodexSwitchUI.ECharts.Abstractions;
using CodexSwitchUI.ECharts.Controls;
using CodexSwitchUI.ECharts.Formatting;
using CodexSwitchUI.ECharts.Models;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Xunit;

namespace CodexSwitchUI.Tests;

public class EChartsUsageTrendChartTests
{
    [Fact]
    public void UsageTrendChartUsesNeutralPointContract()
    {
        var point = new UsageTrendChartPoint
        {
            Timestamp = new DateTimeOffset(2026, 5, 14, 12, 0, 0, TimeSpan.Zero),
            Requests = 2,
            InputTokens = 100,
            CachedInputTokens = 40,
            CacheCreationInputTokens = 10,
            OutputTokens = 80,
            ReasoningOutputTokens = 20,
            OutputDurationMs = 2_000,
            Cost = 0.0123m
        };

        var chart = new CsUsageTrendChart
        {
            ItemsSource = new[] { point },
            Granularity = UsageTrendChartGranularity.Day
        };

        var item = Assert.Single(chart.ItemsSource!);
        Assert.Equal(point.Timestamp, item.Timestamp);
        Assert.Equal(UsageTrendChartGranularity.Day, chart.Granularity);
    }

    [Fact]
    public void UsageTrendChartFormattingMatchesDashboardDisplayRules()
    {
        Assert.Equal("1.5K", UsageChartValueFormatter.FormatTokenCount(1_500));
        Assert.Equal("$0.0123", UsageChartValueFormatter.FormatCost(0.0123m));
        Assert.Equal("26.7%", UsageChartValueFormatter.FormatPercentage(
            UsageChartValueFormatter.CalculateCacheHitRate(100, 40, 10)));
        Assert.Equal("40.0 TPS", UsageChartValueFormatter.FormatTokensPerSecond(
            UsageChartValueFormatter.CalculateOutputTokensPerSecond(80, 2_000)));
    }

    [Fact]
    public void UsageTrendChartStaticRenderResourcesAreImmutableForRenderedDocsSessions()
    {
        var brushFields = typeof(CsUsageTrendChart)
            .GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Where(field => typeof(IBrush).IsAssignableFrom(field.FieldType))
            .ToArray();
        var penFields = typeof(CsUsageTrendChart)
            .GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Where(field => typeof(IPen).IsAssignableFrom(field.FieldType))
            .ToArray();

        Assert.NotEmpty(brushFields);
        Assert.NotEmpty(penFields);
        Assert.All(brushFields, field => Assert.IsType<ImmutableSolidColorBrush>(field.GetValue(null)));
        Assert.All(penFields, field => Assert.IsType<ImmutablePen>(field.GetValue(null)));
    }
}
