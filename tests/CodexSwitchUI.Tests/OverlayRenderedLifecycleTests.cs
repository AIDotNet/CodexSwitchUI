using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Skia;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using CodexSwitchUI.Themes;
using System.Windows.Input;
using Xunit;

namespace CodexSwitchUI.Tests;

public class OverlayRenderedLifecycleTests
{
    [Fact]
    public async Task DialogEscapeDismissRestoresActualFocusToTriggerInMountedTree()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var trigger = new CodexButton
            {
                Content = "Open command",
                Name = "Trigger"
            };
            var field = new CodexTextBox
            {
                Text = "Focused inside the dialog",
                Name = "DialogField"
            };
            var dialog = new CodexDialog
            {
                Title = "Mounted dialog",
                IsOpen = true,
                RestoreFocusElement = trigger,
                Content = field
            };
            var root = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    trigger,
                    dialog
                }
            };
            var window = new Window
            {
                Width = 640,
                Height = 420,
                Content = root
            };

            try
            {
                window.Show();

                Assert.True(field.Focus(NavigationMethod.Tab, KeyModifiers.None));
                Assert.True(field.IsFocused);
                Assert.False(trigger.IsFocused);

                Assert.True(dialog.TryHandleDismissKey(Key.Escape));

                Assert.False(dialog.IsOpen);
                Assert.True(trigger.IsFocused);
                Assert.False(field.IsFocused);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task AlertDialogFocusesCancelFirstAndIgnoresOutsidePointerByDefault()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var trigger = new CodexButton
            {
                Content = "Delete route",
                Name = "AlertTrigger"
            };
            var alertDialog = new CodexAlertDialog
            {
                Title = "Delete route?",
                Description = "This requires an explicit response.",
                IsOpen = true,
                RestoreFocusElement = trigger,
                ActionContent = "Delete",
                ActionVariant = CodexControlVariant.Destructive
            };
            var root = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    trigger,
                    alertDialog
                }
            };
            var window = new Window
            {
                Width = 640,
                Height = 420,
                Content = root
            };

            try
            {
                window.Show();
                window.UpdateLayout();
                Dispatcher.UIThread.RunJobs();

                var cancelButton = alertDialog.GetVisualDescendants().OfType<CodexButton>().FirstOrDefault(button => button.Name == "PART_Cancel");
                Assert.NotNull(cancelButton);
                Assert.True(cancelButton.IsFocused);
                Assert.True(alertDialog.IsOpen);
                Assert.False(alertDialog.TryDismissFromOutsidePointer());
                Assert.True(alertDialog.IsOpen);

                Assert.True(alertDialog.TryHandleDismissKey(Key.Escape));

                Assert.False(alertDialog.IsOpen);
                Assert.True(trigger.IsFocused);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task OverlayPointerRoutingDismissesOnlyOutsideMountedContent()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var dismissCount = 0;
            var overlayContent = new Border
            {
                Width = 96,
                Height = 96,
                Background = Brushes.White
            };
            var overlay = new CodexOverlay
            {
                Width = 300,
                Height = 300,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = overlayContent,
                DismissCommand = new TestCommand(() => dismissCount++)
            };
            var window = new Window
            {
                Width = 300,
                Height = 300,
                Content = overlay
            };

            try
            {
                window.Show();

                window.MouseDown(new Point(150, 150), MouseButton.Left, RawInputModifiers.None);
                window.MouseUp(new Point(150, 150), MouseButton.Left, RawInputModifiers.None);

                Assert.True(overlay.IsOpen);
                Assert.Equal(0, dismissCount);

                window.MouseDown(new Point(12, 12), MouseButton.Left, RawInputModifiers.None);
                window.MouseUp(new Point(12, 12), MouseButton.Left, RawInputModifiers.None);

                Assert.False(overlay.IsOpen);
                Assert.Equal(1, dismissCount);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task SheetEscapeDismissRestoresFocusAndKeepsEdgeMotionInMountedTree()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var trigger = new CodexButton
            {
                Content = "Open filters",
                Name = "SheetTrigger"
            };
            var field = new CodexTextBox
            {
                Text = "Focused inside the sheet",
                Name = "SheetField"
            };
            var sheet = new CodexSheet
            {
                Trigger = trigger,
                Title = "Mounted sheet",
                Side = CodexSheetSide.Left,
                IsOpen = true,
                RestoreFocusElement = trigger,
                Content = field
            };
            var root = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    sheet
                }
            };
            var window = new Window
            {
                Width = 700,
                Height = 420,
                Content = root
            };

            try
            {
                window.Show();
                window.UpdateLayout();

                Assert.Contains("side-left", sheet.Classes);
                Assert.DoesNotContain("side-right", sheet.Classes);
                var surface = sheet.GetVisualDescendants().OfType<Border>().FirstOrDefault(border => border.Name == "PART_Surface");
                Assert.NotNull(surface);
                Assert.Contains(surface.Transitions!, transition => transition is TransformOperationsTransition transform && transform.Duration > TimeSpan.Zero);

                Assert.True(field.Focus(NavigationMethod.Tab, KeyModifiers.None));
                Assert.True(field.IsFocused);
                Assert.False(trigger.IsFocused);

                Assert.True(sheet.TryHandleDismissKey(Key.Escape));

                Assert.False(sheet.IsOpen);
                Assert.Contains("closed", sheet.Classes);
                Assert.True(trigger.IsFocused);
                Assert.False(field.IsFocused);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task DrawerHandleDragDismissRestoresFocusAndKeepsDirectionMotionInMountedTree()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var trigger = new CodexButton
            {
                Content = "Open drawer",
                Name = "DrawerTrigger"
            };
            var field = new CodexTextBox
            {
                Text = "Focused inside the drawer",
                Name = "DrawerField"
            };
            var drawer = new CodexDrawer
            {
                Trigger = trigger,
                Title = "Mounted drawer",
                Direction = CodexDrawerDirection.Bottom,
                IsOpen = true,
                RestoreFocusElement = trigger,
                DragDismissThreshold = 96,
                Content = field
            };
            var root = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    drawer
                }
            };
            var window = new Window
            {
                Width = 700,
                Height = 460,
                Content = root
            };

            try
            {
                window.Show();
                window.UpdateLayout();

                Assert.Contains("direction-bottom", drawer.Classes);
                Assert.Contains("has-handle", drawer.Classes);
                Assert.Contains("has-trigger", drawer.Classes);
                var surface = drawer.GetVisualDescendants().OfType<Border>().FirstOrDefault(border => border.Name == "PART_Surface");
                Assert.NotNull(surface);
                Assert.Contains(surface.Transitions!, transition => transition is TransformOperationsTransition transform && transform.Duration > TimeSpan.Zero);

                Assert.True(field.Focus(NavigationMethod.Tab, KeyModifiers.None));
                Assert.True(field.IsFocused);
                Assert.False(trigger.IsFocused);

                Assert.True(drawer.BeginDrag());
                Assert.True(drawer.DragBy(128));
                Assert.True(drawer.IsDragDismissReady);
                Assert.True(drawer.CompleteDrag());

                Assert.False(drawer.IsOpen);
                Assert.Contains("closed", drawer.Classes);
                Assert.True(trigger.IsFocused);
                Assert.False(field.IsFocused);

                drawer.IsOpen = true;
                drawer.Direction = CodexDrawerDirection.Right;
                drawer.CloseOnDragDismiss = false;

                Assert.Contains("direction-right", drawer.Classes);
                Assert.True(drawer.BeginDrag());
                Assert.True(drawer.DragBy(160));
                Assert.True(drawer.IsDragDismissReady);
                Assert.False(drawer.CompleteDrag());
                Assert.True(drawer.IsOpen);
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

    private sealed class TestCommand(Action execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

public sealed class OverlayRenderedLifecycleTestApp : Application
{
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<OverlayRenderedLifecycleTestApp>()
            .UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions
            {
                UseHeadlessDrawing = false
            });
    }
}
