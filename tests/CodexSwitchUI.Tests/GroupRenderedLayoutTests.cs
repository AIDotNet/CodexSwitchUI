using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Themes;
using Xunit;

namespace CodexSwitchUI.Tests;

public class GroupRenderedLayoutTests
{
    [Fact]
    public async Task ButtonGroupRendersHorizontalAndVerticalItemsWithConfiguredPanel()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var horizontalFirst = new CodexButton { Content = "One" };
            var horizontalSecond = new CodexButton { Content = "Two" };
            var horizontal = new CodexButtonGroup
            {
                Items =
                {
                    horizontalFirst,
                    horizontalSecond
                }
            };

            var verticalFirst = new CodexButton { Content = "Top" };
            var verticalSecond = new CodexButton { Content = "Bottom" };
            var vertical = new CodexButtonGroup
            {
                Orientation = Orientation.Vertical,
                Items =
                {
                    verticalFirst,
                    verticalSecond
                }
            };

            var window = CreateLayoutWindow(horizontal, vertical);

            try
            {
                window.Show();
                window.UpdateLayout();
                Dispatcher.UIThread.RunJobs();

                AssertPlacedToRight(horizontalSecond, horizontalFirst, "ButtonGroup should render horizontal items side-by-side");
                AssertPlacedBelow(verticalSecond, verticalFirst, "Vertical ButtonGroup should render items top-to-bottom");
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task InputGroupRendersInlineAndBlockItemsWithConfiguredPanel()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var inlinePrefix = new CodexInputGroupAddon { Content = "https://" };
            var inlineInput = new CodexInputGroupInput { Text = "api.example.com" };
            var inlineAction = new CodexInputGroupButton { Content = "Copy" };
            var inline = new CodexInputGroup
            {
                Items =
                {
                    inlinePrefix,
                    inlineInput,
                    inlineAction
                }
            };

            var blockPrefix = new CodexInputGroupAddon
            {
                Content = "Endpoint",
                Align = CodexInputGroupAddonAlign.BlockStart
            };
            var blockInput = new CodexInputGroupInput { Text = "/v1/responses" };
            var block = new CodexInputGroup
            {
                Items =
                {
                    blockPrefix,
                    blockInput
                }
            };

            var window = CreateLayoutWindow(inline, block);

            try
            {
                window.Show();
                window.UpdateLayout();
                Dispatcher.UIThread.RunJobs();

                AssertPlacedToRight(inlineInput, inlinePrefix, "InputGroup should render inline items side-by-side");
                AssertPlacedToRight(inlineAction, inlineInput, "InputGroup should render inline items side-by-side", yTolerance: 4);
                AssertPlacedBelow(blockInput, blockPrefix, "Block InputGroup should render items top-to-bottom");
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    private static Window CreateLayoutWindow(params Control[] controls)
    {
        var stack = new StackPanel
        {
            Spacing = 24
        };

        foreach (var control in controls)
        {
            stack.Children.Add(control);
        }

        return new Window
        {
            Width = 640,
            Height = 360,
            Content = new Border
            {
                Padding = new Thickness(24),
                Child = stack
            }
        };
    }

    private static void AssertPlacedToRight(Control right, Control left, string message, double yTolerance = 0.5)
    {
        Assert.True(
            right.Bounds.X > left.Bounds.X,
            $"{message}. Left={left.Bounds}, Right={right.Bounds}");
        Assert.InRange(
            Math.Abs(right.Bounds.Y - left.Bounds.Y),
            0,
            yTolerance);
    }

    private static void AssertPlacedBelow(Control lower, Control upper, string message)
    {
        Assert.True(
            lower.Bounds.Y > upper.Bounds.Y,
            $"{message}. Upper={upper.Bounds}, Lower={lower.Bounds}");
        Assert.InRange(
            Math.Abs(lower.Bounds.X - upper.Bounds.X),
            0,
            0.5);
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
