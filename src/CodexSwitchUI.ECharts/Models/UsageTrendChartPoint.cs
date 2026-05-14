using CodexSwitchUI.ECharts.Abstractions;

namespace CodexSwitchUI.ECharts.Models;

public sealed class UsageTrendChartPoint : IUsageTrendChartPoint
{
    public DateTimeOffset Timestamp { get; init; }

    public long Requests { get; init; }

    public long InputTokens { get; init; }

    public long CachedInputTokens { get; init; }

    public long CacheCreationInputTokens { get; init; }

    public long OutputTokens { get; init; }

    public long ReasoningOutputTokens { get; init; }

    public long OutputDurationMs { get; init; }

    public decimal Cost { get; init; }
}
