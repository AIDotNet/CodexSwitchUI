using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Themes;
using Xunit;

namespace CodexSwitchUI.Tests;

public class CarouselRenderedLayoutTests
{
    [Fact]
    public async Task CarouselUsesConfiguredItemsPanelForHorizontalAndVerticalTracks()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var horizontalFirst = new CodexCarouselItem { Content = "One", Width = 120, MinWidth = 120 };
            var horizontalSecond = new CodexCarouselItem { Content = "Two", Width = 120, MinWidth = 120 };
            var horizontal = new CodexCarousel
            {
                Width = 340,
                Items =
                {
                    horizontalFirst,
                    horizontalSecond
                }
            };

            var verticalFirst = new CodexCarouselItem { Content = "Top", Width = 180, MinWidth = 180, Height = 80, MinHeight = 80 };
            var verticalSecond = new CodexCarouselItem { Content = "Bottom", Width = 180, MinWidth = 180, Height = 80, MinHeight = 80 };
            var vertical = new CodexCarousel
            {
                Width = 240,
                Height = 260,
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

                AssertPresenterForeground(horizontalFirst, horizontalFirst.Foreground);
                AssertPlacedToRight(horizontalSecond, horizontalFirst, horizontal, "Horizontal Carousel items should render side-by-side");
                AssertPlacedBelow(verticalSecond, verticalFirst, vertical, "Vertical Carousel items should render top-to-bottom");
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
            Height = 560,
            Content = new Border
            {
                Padding = new Thickness(24),
                Child = stack
            }
        };
    }

    private static void AssertPresenterForeground(Control owner, object? expectedForeground)
    {
        var presenter = owner.GetVisualDescendants()
            .OfType<ContentPresenter>()
            .Single(control => control.Name == "PART_ItemContent");

        Assert.Equal(expectedForeground, presenter.Foreground);
    }

    private static void AssertPlacedToRight(Control right, Control left, Visual root, string message)
    {
        var rightBounds = BoundsInRoot(right, root);
        var leftBounds = BoundsInRoot(left, root);

        Assert.True(
            rightBounds.X > leftBounds.X,
            $"{message}. Left={leftBounds}, Right={rightBounds}");
        Assert.InRange(Math.Abs(CenterY(rightBounds) - CenterY(leftBounds)), 0, 2);
    }

    private static void AssertPlacedBelow(Control lower, Control upper, Visual root, string message)
    {
        var lowerBounds = BoundsInRoot(lower, root);
        var upperBounds = BoundsInRoot(upper, root);

        Assert.True(
            lowerBounds.Y > upperBounds.Y,
            $"{message}. Upper={upperBounds}, Lower={lowerBounds}");
        Assert.InRange(Math.Abs(lowerBounds.X - upperBounds.X), 0, 2);
    }

    private static Rect BoundsInRoot(Control control, Visual root)
    {
        var origin = control.TranslatePoint(default, root);
        Assert.True(origin.HasValue, $"Could not translate {control} into root coordinates.");
        return new Rect(origin.Value, control.Bounds.Size);
    }

    private static double CenterY(Rect rect)
    {
        return rect.Y + rect.Height / 2d;
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
