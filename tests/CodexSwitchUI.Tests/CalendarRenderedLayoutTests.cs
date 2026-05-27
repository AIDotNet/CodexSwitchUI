using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Themes;
using Xunit;

namespace CodexSwitchUI.Tests;

public class CalendarRenderedLayoutTests
{
    [Fact]
    public async Task CalendarRendersDaysInSevenColumnGrid()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var calendar = new CodexCalendar
            {
                Width = 320,
                DisplayDate = new DateTime(2026, 5, 1)
            };
            var window = CreateLayoutWindow(calendar);

            try
            {
                window.Show();
                window.UpdateLayout();
                Dispatcher.UIThread.RunJobs();

                var weekdays = calendar.GetVisualDescendants().OfType<CodexCalendarWeekday>().ToArray();
                var days = calendar.GetVisualDescendants().OfType<CodexCalendarDayButton>().ToArray();

                Assert.Equal(7, weekdays.Length);
                Assert.Equal(42, days.Length);
                AssertSameRow(weekdays);
                AssertIncreasingColumns(weekdays);
                AssertPlacedBelow(days[0], weekdays[0], "Calendar day grid should start below weekday headers");
                AssertSameColumn(days[0], weekdays[0], "Calendar first day cell should align with first weekday column");
                AssertSameRow(days.Take(7).ToArray());
                AssertIncreasingColumns(days.Take(7).ToArray());
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task CalendarWeekNumbersRenderInEightColumnGrid()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var calendar = new CodexCalendar
            {
                Width = 360,
                DisplayDate = new DateTime(2026, 5, 1),
                ShowWeekNumbers = true
            };
            var window = CreateLayoutWindow(calendar);

            try
            {
                window.Show();
                window.UpdateLayout();
                Dispatcher.UIThread.RunJobs();

                var weekdays = calendar.GetVisualDescendants().OfType<CodexCalendarWeekday>().ToArray();
                var weekNumbers = calendar.GetVisualDescendants().OfType<CodexCalendarWeekNumber>().ToArray();
                var days = calendar.GetVisualDescendants().OfType<CodexCalendarDayButton>().ToArray();

                Assert.Equal(8, weekdays.Length);
                Assert.Equal(6, weekNumbers.Length);
                Assert.Equal(42, days.Length);
                AssertSameRow(weekdays);
                AssertIncreasingColumns(weekdays);
                AssertPlacedBelow(weekNumbers[0], weekdays[0], "Calendar week-number rows should start below headers");
                AssertSameColumn(weekNumbers[0], weekdays[0], "Week numbers should occupy the first calendar column");
                AssertPlacedToRight(days[0], weekNumbers[0], "Calendar days should start after the week-number column");
                AssertSameRow(new Control[] { weekNumbers[0] }.Concat(days.Take(7)).ToArray());
                AssertIncreasingColumns(new Control[] { weekNumbers[0] }.Concat(days.Take(7)).ToArray());
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
            Height = 420,
            Content = new Border
            {
                Padding = new Thickness(24),
                Child = content
            }
        };
    }

    private static void AssertSameRow(IReadOnlyList<Control> controls)
    {
        Assert.NotEmpty(controls);
        var y = CenterY(controls[0]);
        Assert.All(controls, control => Assert.InRange(Math.Abs(CenterY(control) - y), 0, 0.5));
    }

    private static void AssertIncreasingColumns(IReadOnlyList<Control> controls)
    {
        for (var index = 1; index < controls.Count; index++)
        {
            AssertPlacedToRight(controls[index], controls[index - 1], "Calendar grid cells should advance by column");
        }
    }

    private static void AssertPlacedToRight(Control right, Control left, string message)
    {
        Assert.True(
            right.Bounds.X > left.Bounds.X,
            $"{message}. Left={left.Bounds}, Right={right.Bounds}");
        Assert.InRange(Math.Abs(CenterY(right) - CenterY(left)), 0, 0.5);
    }

    private static void AssertPlacedBelow(Control lower, Control upper, string message)
    {
        Assert.True(
            lower.Bounds.Y > upper.Bounds.Y,
            $"{message}. Upper={upper.Bounds}, Lower={lower.Bounds}");
    }

    private static void AssertSameColumn(Control lower, Control upper, string message)
    {
        Assert.InRange(
            Math.Abs(lower.Bounds.X - upper.Bounds.X),
            0,
            0.5);
    }

    private static double CenterY(Control control)
    {
        return control.Bounds.Y + control.Bounds.Height / 2d;
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
