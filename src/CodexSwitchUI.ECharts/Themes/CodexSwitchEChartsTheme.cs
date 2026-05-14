using CodexSwitchUI.Themes;

namespace CodexSwitchUI.ECharts.Themes;

public static class CodexSwitchEChartsTheme
{
    public static EChartsThemePalette FromCurrentTheme()
    {
        var manager = CodexSwitchThemeManager.Current;
        return FromOptions(manager.Options, manager.Mode);
    }

    public static EChartsThemePalette FromOptions(
        CodexSwitchThemeOptions options,
        CodexSwitchThemeMode mode = CodexSwitchThemeMode.Light)
    {
        ArgumentNullException.ThrowIfNull(options);

        var palette = options.ResolvePalette(mode);
        return new EChartsThemePalette(
            BackgroundColor: ToCssHex(palette.Background),
            TextColor: ToCssHex(palette.Foreground),
            AxisLineColor: ToCssHex(palette.Border),
            SplitLineColor: ToCssHex(palette.Muted),
            TooltipBackgroundColor: ToCssHex(palette.Popover),
            TooltipTextColor: ToCssHex(palette.PopoverForeground),
            Color:
            [
                ToCssHex(palette.Primary),
                ToCssHex(palette.Success),
                ToCssHex(palette.Warning),
                ToCssHex(palette.Destructive),
                ToCssHex(palette.Secondary),
                ToCssHex(palette.Accent)
            ]);
    }

    public static IReadOnlyDictionary<string, object> ToEChartsThemeObject(EChartsThemePalette palette)
    {
        ArgumentNullException.ThrowIfNull(palette);

        return new Dictionary<string, object>
        {
            ["backgroundColor"] = palette.BackgroundColor,
            ["color"] = palette.Color,
            ["textStyle"] = new Dictionary<string, object>
            {
                ["color"] = palette.TextColor
            },
            ["axisPointer"] = new Dictionary<string, object>
            {
                ["lineStyle"] = new Dictionary<string, object>
                {
                    ["color"] = palette.AxisLineColor
                }
            },
            ["tooltip"] = new Dictionary<string, object>
            {
                ["backgroundColor"] = palette.TooltipBackgroundColor,
                ["textStyle"] = new Dictionary<string, object>
                {
                    ["color"] = palette.TooltipTextColor
                }
            },
            ["categoryAxis"] = Axis(palette),
            ["valueAxis"] = Axis(palette)
        };
    }

    private static IReadOnlyDictionary<string, object> Axis(EChartsThemePalette palette)
    {
        return new Dictionary<string, object>
        {
            ["axisLine"] = new Dictionary<string, object>
            {
                ["lineStyle"] = new Dictionary<string, object>
                {
                    ["color"] = palette.AxisLineColor
                }
            },
            ["axisLabel"] = new Dictionary<string, object>
            {
                ["color"] = palette.TextColor
            },
            ["splitLine"] = new Dictionary<string, object>
            {
                ["lineStyle"] = new Dictionary<string, object>
                {
                    ["color"] = palette.SplitLineColor
                }
            }
        };
    }

    private static string ToCssHex(string argb)
    {
        if (argb.Length == 9 && argb.StartsWith('#'))
        {
            return $"#{argb[3..]}";
        }

        return argb;
    }
}
