using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Themes;
using Xunit;

namespace CodexSwitchUI.Tests;

public class TableRenderedLayoutTests
{
    [Fact]
    public async Task TableHeadAndCellAlignmentRenderAtExpectedColumnPositions()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var providerHeadText = new TextBlock { Text = "Provider" };
            var statusHeadText = new TextBlock { Text = "Status" };
            var spendHeadText = new TextBlock { Text = "Spend" };
            var providerCellText = new TextBlock { Text = "OpenAI" };
            var statusCellText = new TextBlock { Text = "ready" };
            var spendCellText = new TextBlock { Text = "$42.70" };

            var providerHead = new CodexTableHead
            {
                Content = providerHeadText,
                Alignment = CodexTableCellAlignment.Left
            };
            var statusHead = new CodexTableHead
            {
                Content = statusHeadText,
                Alignment = CodexTableCellAlignment.Center
            };
            var spendHead = new CodexTableHead
            {
                Content = spendHeadText,
                Alignment = CodexTableCellAlignment.Right
            };
            var providerCell = new CodexTableCell
            {
                Content = providerCellText,
                Alignment = CodexTableCellAlignment.Left
            };
            var statusCell = new CodexTableCell
            {
                Content = statusCellText,
                Alignment = CodexTableCellAlignment.Center
            };
            var spendCell = new CodexTableCell
            {
                Content = spendCellText,
                Alignment = CodexTableCellAlignment.Right
            };

            var table = new CodexTable
            {
                Width = 520,
                Content = new StackPanel
                {
                    Children =
                    {
                        new CodexTableHeader
                        {
                            Content = new CodexTableRow
                            {
                                Content = TableRowGrid(providerHead, statusHead, spendHead)
                            }
                        },
                        new CodexTableBody
                        {
                            Items =
                            {
                                new CodexTableRow
                                {
                                    Content = TableRowGrid(providerCell, statusCell, spendCell)
                                }
                            }
                        }
                    }
                }
            };
            var window = CreateLayoutWindow(table);

            try
            {
                window.Show();
                window.UpdateLayout();
                Dispatcher.UIThread.RunJobs();

                AssertLeftAligned(providerHead, providerHeadText, window);
                AssertCenterAligned(statusHead, statusHeadText, window);
                AssertRightAligned(spendHead, spendHeadText, window);
                AssertLeftAligned(providerCell, providerCellText, window);
                AssertCenterAligned(statusCell, statusCellText, window);
                AssertRightAligned(spendCell, spendCellText, window);

                using var frame = Assert.IsAssignableFrom<Avalonia.Media.Imaging.Bitmap>(window.CaptureRenderedFrame());
                Assert.True(frame.PixelSize.Width > 0);
                Assert.True(frame.PixelSize.Height > 0);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    private static Grid TableRowGrid(params Control[] cells)
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(160)),
                new ColumnDefinition(new GridLength(160)),
                new ColumnDefinition(new GridLength(160))
            }
        };

        for (var index = 0; index < cells.Length; index++)
        {
            Grid.SetColumn(cells[index], index);
            grid.Children.Add(cells[index]);
        }

        return grid;
    }

    private static Window CreateLayoutWindow(Control content)
    {
        return new Window
        {
            Width = 620,
            Height = 260,
            Content = new Border
            {
                Padding = new Thickness(24),
                Child = content
            }
        };
    }

    private static void AssertLeftAligned(Control cell, Control content, Visual root)
    {
        var cellBounds = BoundsInRoot(FindCellSurface(cell), root);
        var contentBounds = BoundsInRoot(content, root);

        Assert.InRange(contentBounds.Left - cellBounds.Left, 7, 18);
    }

    private static void AssertCenterAligned(Control cell, Control content, Visual root)
    {
        var cellBounds = BoundsInRoot(FindCellSurface(cell), root);
        var contentBounds = BoundsInRoot(content, root);

        Assert.InRange(Math.Abs(CenterX(contentBounds) - CenterX(cellBounds)), 0, 1.5);
    }

    private static void AssertRightAligned(Control cell, Control content, Visual root)
    {
        var cellBounds = BoundsInRoot(FindCellSurface(cell), root);
        var contentBounds = BoundsInRoot(content, root);

        Assert.InRange(cellBounds.Right - contentBounds.Right, 7, 18);
    }

    private static Control FindCellSurface(Control cell)
    {
        return cell.GetVisualDescendants()
            .OfType<Control>()
            .First(control => control.Name is "PART_Head" or "PART_Cell");
    }

    private static Rect BoundsInRoot(Control control, Visual root)
    {
        var origin = control.TranslatePoint(default, root);
        Assert.True(origin.HasValue, $"Could not translate {control} into root coordinates.");
        return new Rect(origin.Value, control.Bounds.Size);
    }

    private static double CenterX(Rect rect)
    {
        return rect.X + rect.Width / 2d;
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
