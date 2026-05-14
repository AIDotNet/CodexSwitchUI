namespace CodexSwitchUI.ECharts.Themes;

public sealed record EChartsThemePalette(
    string BackgroundColor,
    string TextColor,
    string AxisLineColor,
    string SplitLineColor,
    string TooltipBackgroundColor,
    string TooltipTextColor,
    IReadOnlyList<string> Color);
