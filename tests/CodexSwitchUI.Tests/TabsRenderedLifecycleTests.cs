using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Themes;
using Xunit;

namespace CodexSwitchUI.Tests;

public class TabsRenderedLifecycleTests
{
    [Fact]
    public async Task PlainTabItemsRenderThroughGeneratedCodexContainersWithoutReparentingHeaderVisuals()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var header = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 6
            };
            header.Children.Add(new TextBlock { Text = "Preview" });

            var selectedContent = new TextBlock { Text = "Selected tab content" };
            var tabs = new CodexTabs
            {
                SelectedIndex = 0,
                ItemsSource = new[]
                {
                    new TabItem { Header = header },
                    new TabItem { Header = "Selected", Content = selectedContent },
                    new TabItem { Header = "Disabled", IsEnabled = false }
                }
            };
            var window = new Window
            {
                Width = 640,
                Height = 420,
                Content = tabs
            };

            try
            {
                window.Show();

                var first = Assert.IsType<CodexTabItem>(tabs.ContainerFromIndex(0));
                var second = Assert.IsType<CodexTabItem>(tabs.ContainerFromIndex(1));
                var third = Assert.IsType<CodexTabItem>(tabs.ContainerFromIndex(2));

                Assert.Same(header, first.Header);
                Assert.Null(first.Content);

                tabs.SelectedIndex = 1;

                Assert.Same(selectedContent, second.Content);
                Assert.Same(selectedContent, tabs.SelectedContent);
                Assert.NotSame(tabs.ItemsView[1], tabs.SelectedContent);
                Assert.False(third.IsEnabled);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    private static void EnsureCodexTheme()
    {
        var application = Application.Current;
        Assert.NotNull(application);

        if (!application.Styles.OfType<CodexSwitchTheme>().Any())
        {
            application.Styles.Add(new CodexSwitchTheme());
        }
    }
}
