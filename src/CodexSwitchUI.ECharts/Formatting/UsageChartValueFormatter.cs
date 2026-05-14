using System.Globalization;

namespace CodexSwitchUI.ECharts.Formatting;

public static class UsageChartValueFormatter
{
    public static string FormatTokenCount(long value)
    {
        var absolute = Math.Abs(value);
        if (absolute < 1_000)
            return value.ToString("N0", CultureInfo.InvariantCulture);

        return absolute switch
        {
            < 1_000_000 => FormatScaled(value / 1_000d, "K"),
            < 1_000_000_000 => FormatScaled(value / 1_000_000d, "M"),
            _ => FormatScaled(value / 1_000_000_000d, "B")
        };
    }

    public static string FormatCost(decimal value)
    {
        return value == 0m
            ? "$0.0000"
            : value.ToString("$0.0000", CultureInfo.InvariantCulture);
    }

    public static string FormatPercentage(double value)
    {
        var normalized = double.IsFinite(value) ? Math.Clamp(value, 0d, 1d) : 0d;
        return normalized.ToString("0.0%", CultureInfo.InvariantCulture);
    }

    public static string FormatTokensPerSecond(double value)
    {
        var normalized = double.IsFinite(value) ? Math.Max(0d, value) : 0d;
        if (normalized < 1_000d)
        {
            var format = normalized >= 100d ? "0" : "0.0";
            return normalized.ToString(format, CultureInfo.InvariantCulture) + " TPS";
        }

        return normalized switch
        {
            < 1_000_000d => FormatScaled(normalized / 1_000d, "K TPS"),
            < 1_000_000_000d => FormatScaled(normalized / 1_000_000d, "M TPS"),
            _ => FormatScaled(normalized / 1_000_000_000d, "B TPS")
        };
    }

    public static double CalculateCacheHitRate(
        long inputTokens,
        long cachedInputTokens,
        long cacheCreationInputTokens)
    {
        var input = Math.Max(0d, inputTokens);
        var cached = Math.Max(0d, cachedInputTokens);
        var cacheCreation = Math.Max(0d, cacheCreationInputTokens);
        var totalInput = input + cached + cacheCreation;
        return totalInput <= 0d ? 0d : cached / totalInput;
    }

    public static double CalculateOutputTokensPerSecond(long outputTokens, long durationMs)
    {
        var output = Math.Max(0d, outputTokens);
        var seconds = Math.Max(0d, durationMs) / 1_000d;
        return seconds <= 0d ? 0d : output / seconds;
    }

    private static string FormatScaled(double value, string suffix)
    {
        var format = Math.Abs(value) >= 100 ? "0" : "0.0";
        return value.ToString(format, CultureInfo.InvariantCulture) + suffix;
    }
}
