using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Themes;
using System.Reflection;
using System.Windows.Input;
using Xunit;

namespace CodexSwitchUI.Tests;

public class MenuRenderedLifecycleTests
{
    [Fact]
    public async Task MenuSubMenuRespondsToMountedKeyboardOpenAndClose()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var rename = new CodexMenuItem { Header = "Rename" };
            var move = new CodexMenuItem { Header = "Move" };
            var submenu = new CodexMenuItem { Header = "More" };
            submenu.Items.Add(rename);
            submenu.Items.Add(move);

            var menu = new CodexMenu
            {
                Width = 240,
                Items =
                {
                    new CodexMenuItem { Header = "Open" },
                    submenu
                }
            };
            var window = new Window
            {
                Width = 420,
                Height = 260,
                Content = menu
            };

            try
            {
                window.Show();

                Assert.True(submenu.Focus(NavigationMethod.Tab, KeyModifiers.None));
                Assert.True(submenu.IsFocused);

                RaiseKey(submenu, Key.Right, PhysicalKey.ArrowRight);

                Assert.True(submenu.IsSubMenuOpen);
                Assert.True(rename.IsFocused);

                using var frame = Assert.IsAssignableFrom<Bitmap>(window.CaptureRenderedFrame());
                Assert.True(frame.PixelSize.Width > 0);
                Assert.True(frame.PixelSize.Height > 0);

                RaiseKey(rename, Key.Left, PhysicalKey.ArrowLeft);

                Assert.False(submenu.IsSubMenuOpen);
                Assert.True(submenu.IsFocused);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task MenuSiblingKeysMoveMountedFocusAndSkipInactiveItems()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var commandBlocked = new TestCommand(() => { }) { CanExecuteValue = false };
            var first = new CodexMenuItem { Header = "Open" };
            var disabled = new CodexMenuItem { Header = "Disabled", IsEnabled = false };
            var blocked = new CodexMenuItem { Header = "Blocked", Command = commandBlocked };
            var submenu = new CodexMenuItem { Header = "More" };
            submenu.Items.Add(new CodexMenuItem { Header = "Rename" });
            var last = new CodexMenuItem { Header = "Close" };
            var menu = new CodexMenu
            {
                Width = 240,
                Items =
                {
                    first,
                    disabled,
                    blocked,
                    submenu,
                    last
                }
            };
            var window = new Window
            {
                Width = 420,
                Height = 260,
                Content = menu
            };

            try
            {
                window.Show();

                Assert.True(first.Focus(NavigationMethod.Tab, KeyModifiers.None));
                Assert.True(first.IsFocused);

                RaiseKey(first, Key.Down, PhysicalKey.ArrowDown);

                Assert.False(disabled.IsFocused);
                Assert.False(blocked.IsFocused);
                Assert.True(submenu.IsFocused);

                RaiseKey(submenu, Key.End, PhysicalKey.End);

                Assert.True(last.IsFocused);

                RaiseKey(last, Key.Home, PhysicalKey.Home);

                Assert.True(first.IsFocused);

                menu.IsLoading = true;
                RaiseKey(first, Key.Down, PhysicalKey.ArrowDown);

                Assert.True(first.IsFocused);
                Assert.False(submenu.IsFocused);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task ContextMenuPopupKeysMoveMountedFocusAndLoopWithinOpenSurface()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var commandBlocked = new TestCommand(() => { }) { CanExecuteValue = false };
            var first = new CodexContextMenuItem { Header = "Open" };
            var disabled = new CodexContextMenuItem { Header = "Disabled", IsEnabled = false };
            var blocked = new CodexContextMenuItem { Header = "Blocked", Command = commandBlocked };
            var archive = new CodexContextMenuItem { Header = "Archive" };
            var submenu = new CodexContextMenuItem { Header = "Move to" };
            submenu.Items.Add(archive);
            var last = new CodexContextMenuItem { Header = "Delete" };
            var contextMenu = new CodexContextMenu
            {
                Placement = PlacementMode.Bottom,
                Items =
                {
                    new CodexContextMenuLabel { Content = "Session" },
                    first,
                    new CodexContextMenuSeparator(),
                    disabled,
                    blocked,
                    submenu,
                    last
                }
            };
            var target = new CodexButton
            {
                Width = 160,
                Content = "Context target"
            };
            var window = new Window
            {
                Width = 480,
                Height = 320,
                Content = target
            };

            try
            {
                window.Show();
                contextMenu.Open(target);
                Dispatcher.UIThread.RunJobs();

                Assert.True(contextMenu.IsOpen);
                Assert.Contains("context-menu-open", contextMenu.Classes);
                Assert.True(first.Focus(NavigationMethod.Tab, KeyModifiers.None));
                Assert.True(first.IsFocused);

                RaiseKey(first, Key.Down, PhysicalKey.ArrowDown);

                Assert.False(disabled.IsFocused);
                Assert.False(blocked.IsFocused);
                Assert.True(submenu.IsFocused);

                RaiseKey(submenu, Key.Right, PhysicalKey.ArrowRight);

                Assert.True(submenu.IsSubMenuOpen);
                Assert.True(archive.IsFocused);

                RaiseKey(archive, Key.Left, PhysicalKey.ArrowLeft);

                Assert.False(submenu.IsSubMenuOpen);
                Assert.True(submenu.IsFocused);

                RaiseKey(submenu, Key.End, PhysicalKey.End);

                Assert.True(last.IsFocused);

                RaiseKey(last, Key.Down, PhysicalKey.ArrowDown);

                Assert.True(first.IsFocused);

                RaiseKey(first, Key.Up, PhysicalKey.ArrowUp);

                Assert.True(last.IsFocused);

                contextMenu.IsLoading = true;
                RaiseKey(last, Key.Home, PhysicalKey.Home);

                Assert.True(last.IsFocused);
                Assert.False(first.IsFocused);

                using var frame = Assert.IsAssignableFrom<Bitmap>(window.CaptureRenderedFrame());
                Assert.True(frame.PixelSize.Width > 0);
                Assert.True(frame.PixelSize.Height > 0);
            }
            finally
            {
                contextMenu.Close();
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task ContextMenuLeafSelectionClosesMountedPopupAndSubMenuChain()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var executions = 0;
            var direct = new CodexContextMenuItem
            {
                Header = "Open",
                Command = new TestCommand(() => executions++)
            };
            var archive = new CodexContextMenuItem
            {
                Header = "Archive",
                Command = new TestCommand(() => executions++)
            };
            var submenu = new CodexContextMenuItem { Header = "Move to" };
            submenu.Items.Add(archive);

            var contextMenu = new CodexContextMenu
            {
                Placement = PlacementMode.Bottom,
                Items =
                {
                    direct,
                    submenu
                }
            };
            var target = new CodexButton
            {
                Width = 160,
                Content = "Context target"
            };
            var window = new Window
            {
                Width = 480,
                Height = 320,
                Content = target
            };

            try
            {
                window.Show();
                contextMenu.Open(target);
                Dispatcher.UIThread.RunJobs();

                Assert.True(contextMenu.IsOpen);

                RaiseClick(direct);

                Assert.Equal(1, executions);
                Assert.False(contextMenu.IsOpen);

                contextMenu.Open(target);
                Dispatcher.UIThread.RunJobs();
                submenu.IsSubMenuOpen = true;

                RaiseClick(archive);

                Assert.Equal(2, executions);
                Assert.False(submenu.IsSubMenuOpen);
                Assert.False(contextMenu.IsOpen);

                contextMenu.Open(target);
                Dispatcher.UIThread.RunJobs();
                submenu.IsSubMenuOpen = true;

                Assert.False(submenu.TryCloseOnSelect());
                Assert.True(contextMenu.IsOpen);
                Assert.True(submenu.IsSubMenuOpen);
            }
            finally
            {
                contextMenu.Close();
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task MenuContextMenuAndMenubarLeafSelectionRaiseWebSelectMetadata()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme();

            var menuLeaf = new CodexMenuItem
            {
                Header = "Archive",
                CommandParameter = "archive"
            };
            var menuSubmenu = new CodexMenuItem { Header = "Export" };
            menuSubmenu.Items.Add(new CodexMenuItem { Header = "JSON" });
            var menuEvents = new List<CodexMenuItemSelectedEventArgs>();
            var submenuEvents = new List<CodexMenuItemSelectedEventArgs>();
            menuLeaf.ItemSelected += (_, args) => menuEvents.Add(args);
            menuSubmenu.ItemSelected += (_, args) => submenuEvents.Add(args);

            var menu = new CodexMenu
            {
                Width = 220,
                Items =
                {
                    menuLeaf,
                    menuSubmenu
                }
            };

            var contextLeaf = new CodexContextMenuItem
            {
                Header = "Pin session",
                ToggleType = MenuItemToggleType.CheckBox
            };
            var contextEvents = new List<CodexMenuItemSelectedEventArgs>();
            contextLeaf.ItemSelected += (_, args) => contextEvents.Add(args);
            var contextMenu = new CodexContextMenu
            {
                Items =
                {
                    contextLeaf
                }
            };

            var file = new CodexMenubarMenu { Header = "File" };
            var menubarLeaf = new CodexMenubarItem
            {
                Header = "Close window",
                CommandParameter = "close-window"
            };
            var menubarEvents = new List<CodexMenuItemSelectedEventArgs>();
            menubarLeaf.ItemSelected += (_, args) => menubarEvents.Add(args);
            file.Items.Add(menubarLeaf);
            var menubar = new CodexMenubar
            {
                Items =
                {
                    file
                }
            };

            var target = new CodexButton
            {
                Width = 160,
                Content = "Context target"
            };
            var window = new Window
            {
                Width = 520,
                Height = 360,
                Content = new StackPanel
                {
                    Spacing = 16,
                    Margin = new Thickness(18),
                    Children =
                    {
                        menu,
                        menubar,
                        target
                    }
                }
            };

            try
            {
                window.Show();

                RaiseClick(menuLeaf);
                RaiseClick(menuSubmenu);

                Assert.Single(menuEvents);
                Assert.Empty(submenuEvents);
                Assert.Same(menuLeaf, menuEvents[0].Item);
                Assert.Equal("Archive", menuEvents[0].Header);
                Assert.Equal("archive", menuEvents[0].CommandParameter);
                Assert.Equal(CodexMenuItemSelectSource.Programmatic, menuEvents[0].Source);
                Assert.False(menuEvents[0].DidCloseOnSelect);
                Assert.False(menuEvents[0].HasSubMenu);

                contextMenu.Open(target);
                Dispatcher.UIThread.RunJobs();
                Assert.True(contextMenu.IsOpen);

                RaiseKey(contextLeaf, Key.Enter, PhysicalKey.Enter);

                Assert.Single(contextEvents);
                Assert.Same(contextLeaf, contextEvents[0].Item);
                Assert.Equal("Pin session", contextEvents[0].Header);
                Assert.Equal(MenuItemToggleType.CheckBox, contextEvents[0].ToggleType);
                Assert.True(contextEvents[0].IsChecked);
                Assert.Equal(CodexMenuItemSelectSource.Keyboard, contextEvents[0].Source);
                Assert.True(contextEvents[0].DidCloseOnSelect);
                Assert.False(contextMenu.IsOpen);

                file.IsSubMenuOpen = true;
                RaiseClick(menubarLeaf);

                Assert.Single(menubarEvents);
                Assert.Same(menubarLeaf, menubarEvents[0].Item);
                Assert.Equal("Close window", menubarEvents[0].Header);
                Assert.Equal("close-window", menubarEvents[0].CommandParameter);
                Assert.Equal(CodexMenuItemSelectSource.Programmatic, menubarEvents[0].Source);
                Assert.True(menubarEvents[0].DidCloseOnSelect);
                Assert.False(file.IsSubMenuOpen);
            }
            finally
            {
                contextMenu.Close();
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task MenuAndContextMenuPointerSubMenuRequestsRespectDelayedOpenClose()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);

        Window? window = null;
        CodexContextMenu? contextMenu = null;
        CodexMenuItem? menuSubMenu = null;
        CodexContextMenuItem? contextSubMenu = null;

        try
        {
            await session.Dispatch(() =>
            {
                EnsureCodexTheme();

                menuSubMenu = new CodexMenuItem { Header = "More" };
                menuSubMenu.Items.Add(new CodexMenuItem { Header = "Rename" });

                var menu = new CodexMenu
                {
                    Width = 240,
                    Items =
                    {
                        new CodexMenuItem { Header = "Open" },
                        menuSubMenu
                    }
                };

                contextSubMenu = new CodexContextMenuItem { Header = "Move to" };
                contextSubMenu.Items.Add(new CodexContextMenuItem { Header = "Archive" });

                contextMenu = new CodexContextMenu
                {
                    Placement = PlacementMode.Bottom,
                    Items =
                    {
                        new CodexContextMenuItem { Header = "Copy" },
                        contextSubMenu
                    }
                };
                var contextTarget = new CodexButton
                {
                    Width = 160,
                    Content = "Context target"
                };

                window = new Window
                {
                    Width = 520,
                    Height = 340,
                    Content = new StackPanel
                    {
                        Spacing = 16,
                        Margin = new Thickness(18),
                        Children =
                        {
                            menu,
                            contextTarget
                        }
                    }
                };

                window.Show();
                contextMenu.Open(contextTarget);
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();

                Assert.Equal(TimeSpan.FromMilliseconds(100), CodexMenuActivation.PointerSubMenuOpenDelay);
                Assert.Equal(TimeSpan.FromMilliseconds(300), CodexMenuActivation.PointerSubMenuCloseDelay);

                Assert.True(CodexMenuActivation.RequestPointerSubMenuOpen(menuSubMenu));
                Assert.False(menuSubMenu.IsSubMenuOpen);
                AssertPointerTimerScheduled(menuSubMenu, "_openTimer");
                CodexMenuActivation.CancelPointerSubMenuRequests(menuSubMenu);

                Assert.True(CodexMenuActivation.RequestPointerSubMenuOpen(contextSubMenu));
                Assert.False(contextSubMenu.IsSubMenuOpen);
                AssertPointerTimerScheduled(contextSubMenu, "_openTimer");
                CodexMenuActivation.CancelPointerSubMenuRequests(contextSubMenu);

                Assert.True(CodexMenuActivation.RequestPointerSubMenuOpen(menuSubMenu, TimeSpan.Zero));
                Assert.True(CodexMenuActivation.RequestPointerSubMenuOpen(contextSubMenu, TimeSpan.Zero));
                Dispatcher.UIThread.RunJobs();
                Assert.True(menuSubMenu.IsSubMenuOpen);
                Assert.True(contextSubMenu.IsSubMenuOpen);
                Assert.Contains("submenu-open", contextSubMenu.Classes);
                Assert.True(FindSubMenuSurface(menuSubMenu).IsEffectivelyVisible);
                Assert.True(FindSubMenuSurface(contextSubMenu).IsEffectivelyVisible);

                using (var openFrame = Assert.IsAssignableFrom<Bitmap>(window.CaptureRenderedFrame()))
                {
                    Assert.True(openFrame.PixelSize.Width > 0);
                    Assert.True(openFrame.PixelSize.Height > 0);
                }

                Assert.True(CodexMenuActivation.RequestPointerSubMenuClose(menuSubMenu));
                Assert.True(CodexMenuActivation.RequestPointerSubMenuClose(contextSubMenu));
                Assert.True(menuSubMenu.IsSubMenuOpen);
                Assert.True(contextSubMenu.IsSubMenuOpen);
                AssertPointerTimerScheduled(menuSubMenu, "_closeTimer");
                AssertPointerTimerScheduled(contextSubMenu, "_closeTimer");
                CodexMenuActivation.CancelPointerSubMenuRequests(menuSubMenu);
                CodexMenuActivation.CancelPointerSubMenuRequests(contextSubMenu);

                Assert.True(CodexMenuActivation.RequestPointerSubMenuClose(menuSubMenu, TimeSpan.Zero));
                Assert.True(CodexMenuActivation.RequestPointerSubMenuClose(contextSubMenu, TimeSpan.Zero));
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();

                Assert.False(menuSubMenu.IsSubMenuOpen);
                Assert.False(contextSubMenu.IsSubMenuOpen);
                Assert.DoesNotContain("submenu-open", contextSubMenu.Classes);

                using var frame = Assert.IsAssignableFrom<Bitmap>(window.CaptureRenderedFrame());
                Assert.True(frame.PixelSize.Width > 0);
                Assert.True(frame.PixelSize.Height > 0);
            }, CancellationToken.None);
        }
        finally
        {
            await session.Dispatch(() =>
            {
                contextMenu?.Close();
                window?.Close();
            }, CancellationToken.None);
        }
    }

    private static void AssertPointerTimerScheduled(MenuItem item, string fieldName)
    {
        var pointerStates = typeof(CodexMenuActivation).GetField("PointerStates", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.NotNull(pointerStates);

        var table = pointerStates.GetValue(null);
        Assert.NotNull(table);

        var tryGetValue = table.GetType().GetMethod("TryGetValue");
        Assert.NotNull(tryGetValue);

        var args = new object?[] { item, null };
        var found = Assert.IsType<bool>(tryGetValue.Invoke(table, args));
        Assert.True(found);

        var state = args[1];
        Assert.NotNull(state);

        var timerField = state.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(timerField);
        Assert.IsType<DispatcherTimer>(timerField.GetValue(state));
    }

    private static Border FindSubMenuSurface(MenuItem item)
    {
        var popup = item.GetVisualDescendants()
            .OfType<Popup>()
            .First(control => control.Name == "PART_Popup");
        var surface = Assert.IsType<Border>(popup.Child);
        Assert.Equal("PART_SubMenuSurface", surface.Name);
        return surface;
    }

    private static void RaiseKey(InputElement target, Key key, PhysicalKey physicalKey)
    {
        target.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Source = target,
            Key = key,
            PhysicalKey = physicalKey,
            KeyModifiers = KeyModifiers.None
        });
    }

    private static void RaiseClick(MenuItem item)
    {
        var method = typeof(MenuItem).GetMethod(
            "Avalonia.Input.IClickableControl.RaiseClick",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(method);
        method.Invoke(item, null);
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

        public bool CanExecuteValue { get; set; } = true;

        public bool CanExecute(object? parameter)
        {
            return CanExecuteValue;
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
