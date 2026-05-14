using Xunit;

namespace CodexSwitchUI.Tests;

public class DocsPanelLayoutTests
{
    [Fact]
    public void CoverageMatrixUsesBoundedTwoAxisScrolling()
    {
        var source = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI.Docs", "MainWindow.cs"));
        var method = ExtractMethod(source, "BuildCoverageMatrix");

        Assert.Contains("private const double CoverageMatrixMinHeight", source);
        Assert.Contains("private const double CoverageMatrixMaxHeight", source);
        Assert.Contains("MinHeight = CoverageMatrixMinHeight", method);
        Assert.Contains("MaxHeight = CoverageMatrixMaxHeight", method);
        Assert.Contains("HorizontalScrollBarVisibility = ScrollBarVisibility.Auto", method);
        Assert.Contains("VerticalScrollBarVisibility = ScrollBarVisibility.Auto", method);
        Assert.DoesNotContain("VerticalScrollBarVisibility = ScrollBarVisibility.Disabled", method);
    }

    [Fact]
    public void NavigationMenuDefaultDocsUseHorizontalExample()
    {
        var source = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI.Docs", "MainWindow.cs"));
        var method = ExtractMethod(source, "BuildNavigationMenuPreview");

        Assert.Contains("<controls:CodexNavigationMenu Orientation=\"Horizontal\">", source);
        Assert.Contains("Orientation = Orientation.Horizontal", method);
        Assert.Contains("NavigationMenuHorizontalContent", method);
        Assert.DoesNotContain("Orientation = Orientation.Vertical", method);
    }

    [Fact]
    public void NavigationStateMatrixUsesSingleHorizontalNavigationMenuExample()
    {
        var source = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI.Docs", "MainWindow.cs"));
        var matrix = ExtractMethod(source, "BuildNavigationStateMatrix");
        var preview = ExtractMethod(source, "BuildNavigationMenuStatePreview");

        Assert.Contains("BuildNavigationMenuStatePreview()", matrix);
        Assert.DoesNotContain("BuildNavigationMenuPreview(),", matrix);
        Assert.Contains("Orientation = Orientation.Horizontal", preview);
        Assert.Contains("new CodexNavigationMenuItem { Header = \"Disabled\", IsEnabled = false }", preview);
        Assert.Contains("new CodexNavigationMenuItem { Header = \"Link\" }", preview);
        Assert.DoesNotContain("Orientation = Orientation.Vertical", preview);
    }

    [Fact]
    public void UtilitiesDocsIncludeUsageTrendChartExample()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI.Docs", "MainWindow.cs"));
        var app = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI.Docs", "App.cs"));
        var utilities = ExtractMethod(source, "BuildUtilitiesSection");
        var example = ExtractMethod(source, "BuildEChartsUsageTrendChartExample");

        Assert.Contains("ECharts Usage Trend Chart", utilities);
        Assert.Contains("BuildEChartsUsageTrendChartExample", utilities);
        Assert.Contains("new CsUsageTrendChart", example);
        Assert.Contains("UsageTrendChartGranularity.Hour", example);
        Assert.Contains("CreateUsageTrendChartSample", source);
        Assert.Contains("avares://CodexSwitchUI.ECharts/Themes/UsageTrendChart.axaml", app);
    }

    [Fact]
    public void DataDisplayDocsIncludeRankedBarChartExample()
    {
        var source = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI.Docs", "MainWindow.cs"));
        var dataDisplay = ExtractMethod(source, "BuildDataDisplaySection");
        var example = ExtractMethod(source, "BuildRankedBarChartPreview");

        Assert.Contains("Ranked Bar Chart", dataDisplay);
        Assert.Contains("BuildRankedBarChartPreview", dataDisplay);
        Assert.Contains("new CodexRankedBarChart", example);
        Assert.Contains("new CodexRankedBarChartItem", example);
        Assert.Contains("IsCompact = true", example);
        Assert.Contains("IsEnabled = false", example);
    }

    [Fact]
    public void DocsSidebarUsesCodexSidebarMenuWithoutMenuItemChrome()
    {
        var source = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI.Docs", "MainWindow.cs"));
        var sidebar = ExtractMethod(source, "BuildSidebar");
        var navItem = ExtractMethod(source, "BuildNavCategory");
        var preview = ExtractMethod(source, "BuildDocsSideNavigationPreview");

        Assert.Contains("new CodexSidebarMenu", sidebar);
        Assert.Contains("new CodexSidebarGroup", sidebar);
        Assert.Contains("new CodexSidebarGroupLabel", sidebar);
        Assert.Contains("private CodexSidebarMenuButton BuildNavCategory", source);
        Assert.Contains("new CodexSidebarMenuButton", navItem);
        Assert.DoesNotContain("new CodexMenu", sidebar);
        Assert.DoesNotContain("new CodexMenuGroup", sidebar);
        Assert.DoesNotContain("side-nav", sidebar);
        Assert.DoesNotContain("new CodexMenuItem", navItem);
        Assert.DoesNotContain("new CodexButton", navItem);
        Assert.Contains("new CodexSidebarMenu", preview);
        Assert.Contains("Side Navigation Menu", source);
    }

    [Fact]
    public void DocsSidebarNavigationKeepsExistingScrollViewer()
    {
        var source = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI.Docs", "MainWindow.cs"));
        var navigate = ExtractMethod(source, "Navigate");
        var refresh = ExtractMethod(source, "RefreshSidebarSelection");
        var navItem = ExtractMethod(source, "BuildNavCategory");

        Assert.DoesNotContain("_sidebar.Child = BuildSidebar();", navigate);
        Assert.Contains("RefreshSidebarSelection();", navigate);
        Assert.Contains("_navItemsByCategory[category.Title] = item;", navItem);
        Assert.Contains("item.IsActive = category.Title == _activeCategory;", refresh);
    }

    private static string ExtractMethod(string source, string methodName)
    {
        var start = source.IndexOf($"private Control {methodName}()", StringComparison.Ordinal);
        if (start < 0)
        {
            start = source.IndexOf($"private Control {methodName}(", StringComparison.Ordinal);
        }

        if (start < 0)
        {
            start = source.IndexOf($"private CodexMenuItem {methodName}(", StringComparison.Ordinal);
        }

        if (start < 0)
        {
            start = source.IndexOf($"private CodexSidebarMenuButton {methodName}(", StringComparison.Ordinal);
        }

        if (start < 0)
        {
            start = source.IndexOf($"private void {methodName}(", StringComparison.Ordinal);
        }

        Assert.True(start >= 0, $"Could not find {methodName}.");

        var braceStart = source.IndexOf('{', start);
        Assert.True(braceStart >= 0, $"Could not find opening brace for {methodName}.");

        var depth = 0;
        for (var i = braceStart; i < source.Length; i++)
        {
            if (source[i] == '{')
            {
                depth++;
            }
            else if (source[i] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return source[start..(i + 1)];
                }
            }
        }

        throw new InvalidOperationException($"Could not find closing brace for {methodName}.");
    }

    private static string FindRepositoryRoot()
    {
        return TestRepository.FindRoot();
    }
}
