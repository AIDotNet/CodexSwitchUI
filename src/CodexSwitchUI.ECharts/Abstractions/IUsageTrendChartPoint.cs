namespace CodexSwitchUI.ECharts.Abstractions;

public interface IUsageTrendChartPoint
{
    DateTimeOffset Timestamp { get; }

    long Requests { get; }

    long InputTokens { get; }

    long CachedInputTokens { get; }

    long CacheCreationInputTokens { get; }

    long OutputTokens { get; }

    long ReasoningOutputTokens { get; }

    long OutputDurationMs { get; }

    decimal Cost { get; }
}
