using CodexSwitchUI.ECharts.Themes;
using CodexSwitchUI.Themes;
using Xunit;

namespace CodexSwitchUI.Tests;

public class EChartsThemeTests
{
    [Fact]
    public void EChartsThemeUsesCodexSwitchTokens()
    {
        var theme = CodexSwitchEChartsTheme.FromOptions(CodexSwitchThemeOptions.ShadcnDefault);
        var themeObject = CodexSwitchEChartsTheme.ToEChartsThemeObject(theme);

        Assert.Equal("#FFFFFF", theme.BackgroundColor);
        Assert.Equal("#18181B", theme.Color[0]);
        Assert.True(themeObject.ContainsKey("tooltip"));
        Assert.True(themeObject.ContainsKey("valueAxis"));
    }
}
