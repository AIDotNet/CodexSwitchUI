using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Themes;
using Xunit;

namespace CodexSwitchUI.Tests;

public class BreadcrumbRenderedLayoutTests
{
    [Fact]
    public async Task BreadcrumbTextPartsInheritStateForegroundAndRenderInline()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var docs = new CodexBreadcrumbLink
            {
                Content = "Docs",
                Href = "/docs"
            };
            var separator = new CodexBreadcrumbSeparator();
            var ellipsis = new CodexBreadcrumbEllipsis();
            var current = new CodexBreadcrumbPage
            {
                Content = "Breadcrumb"
            };
            var breadcrumb = new CodexBreadcrumb
            {
                Content = new CodexBreadcrumbList
                {
                    Items =
                    {
                        new CodexBreadcrumbItem { Content = docs },
                        separator,
                        new CodexBreadcrumbItem { Content = ellipsis },
                        new CodexBreadcrumbSeparator(),
                        new CodexBreadcrumbItem { IsCurrent = true, Content = current }
                    }
                }
            };
            var window = CreateLayoutWindow(breadcrumb);

            try
            {
                window.Show();
                window.UpdateLayout();
                Dispatcher.UIThread.RunJobs();

                AssertPresenterForeground(docs, "PART_LinkContent", docs.Foreground);
                AssertPresenterForeground(separator, "PART_Separator", separator.Foreground);
                AssertPresenterForeground(ellipsis, "PART_EllipsisContent", ellipsis.Foreground);
                AssertPresenterForeground(current, "PART_PageContent", current.Foreground);
                AssertPlacedToRight(separator, docs, breadcrumb);
                AssertPlacedToRight(current, ellipsis, breadcrumb);

                docs.IsCurrent = true;
                window.UpdateLayout();
                Dispatcher.UIThread.RunJobs();

                AssertPresenterForeground(docs, "PART_LinkContent", docs.Foreground);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    private static Window CreateLayoutWindow(Control content)
    {
        return new Window
        {
            Width = 520,
            Height = 160,
            Content = new Border
            {
                Padding = new Thickness(24),
                Child = content
            }
        };
    }

    private static void AssertPresenterForeground(Control owner, string presenterName, object? expectedForeground)
    {
        var presenter = owner.GetVisualDescendants()
            .OfType<ContentPresenter>()
            .Single(control => control.Name == presenterName);

        Assert.Equal(expectedForeground, presenter.Foreground);
    }

    private static void AssertPlacedToRight(Control right, Control left, Visual root)
    {
        var rightBounds = BoundsInRoot(right, root);
        var leftBounds = BoundsInRoot(left, root);

        Assert.True(
            rightBounds.X > leftBounds.X,
            $"Breadcrumb items should render inline from left to right. Left={leftBounds}, Right={rightBounds}");
        Assert.InRange(Math.Abs(CenterY(rightBounds) - CenterY(leftBounds)), 0, 2);
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
