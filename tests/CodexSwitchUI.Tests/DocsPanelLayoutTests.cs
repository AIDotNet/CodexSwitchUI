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

    private static string ExtractMethod(string source, string methodName)
    {
        var start = source.IndexOf($"private Control {methodName}()", StringComparison.Ordinal);
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
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "CodexSwitchUI.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root from test output directory.");
    }
}
