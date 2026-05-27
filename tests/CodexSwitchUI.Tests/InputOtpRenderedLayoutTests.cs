using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Themes;
using Xunit;

namespace CodexSwitchUI.Tests;

public class InputOtpRenderedLayoutTests
{
    [Fact]
    public async Task InputOtpSlotsRenderHorizontallyWithinRootAndGroups()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var firstGroup = new CodexInputOtpGroup
            {
                Items =
                {
                    new CodexInputOtpSlot { Index = 0 },
                    new CodexInputOtpSlot { Index = 1 },
                    new CodexInputOtpSlot { Index = 2 }
                }
            };
            var secondGroup = new CodexInputOtpGroup
            {
                Items =
                {
                    new CodexInputOtpSlot { Index = 3 },
                    new CodexInputOtpSlot { Index = 4 },
                    new CodexInputOtpSlot { Index = 5 }
                }
            };
            var input = new CodexInputOtp
            {
                Text = "123456",
                MaxLength = 6,
                Pattern = CodexInputOtp.DigitsPattern,
                Items =
                {
                    firstGroup,
                    new CodexInputOtpSeparator(),
                    secondGroup
                }
            };
            var window = new Window
            {
                Width = 480,
                Height = 160,
                Content = new Border
                {
                    Padding = new Thickness(16),
                    Child = input
                }
            };

            try
            {
                window.Show();
                window.UpdateLayout();
                Dispatcher.UIThread.RunJobs();

                var groups = input.GetVisualDescendants().OfType<CodexInputOtpGroup>().ToArray();
                Assert.Equal(2, groups.Length);
                AssertPlacedToRight(groups[1], groups[0], "Input OTP groups should render side-by-side");

                foreach (var group in groups)
                {
                    var slots = group.GetVisualDescendants().OfType<CodexInputOtpSlot>().ToArray();
                    Assert.Equal(3, slots.Length);
                    AssertPlacedToRight(slots[1], slots[0], "Input OTP slots should render side-by-side");
                    AssertPlacedToRight(slots[2], slots[1], "Input OTP slots should render side-by-side");
                }
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    private static void AssertPlacedToRight(Control right, Control left, string message)
    {
        Assert.True(
            right.Bounds.X > left.Bounds.X,
            $"{message}. Left={left.Bounds}, Right={right.Bounds}");
        Assert.InRange(
            Math.Abs(right.Bounds.Y - left.Bounds.Y),
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
