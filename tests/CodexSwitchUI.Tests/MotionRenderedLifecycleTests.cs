using System.Reflection;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Themes;
using System.Windows.Input;
using Xunit;

namespace CodexSwitchUI.Tests;

public class MotionRenderedLifecycleTests
{
    [Fact]
    public async Task ReducedMotionRuntimeSurfacesRenderFinalStatesInMountedTree()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme(reducedMotion: true);
            CodexSonnerService.Clear();

            var table = BuildTable();
            var collapsible = BuildCollapsible(isOpen: false);
            var skeleton = new CodexSkeleton { Width = 160, Height = 18 };
            var progress = new CodexProgress { Width = 240, IsIndeterminate = true };
            var chart = BuildUsagePieChart();
            var barChart = BuildBarChart();
            var lineChart = BuildLineChart();
            var sonner = new CodexSonner { VisibleToasts = 2 };
            var window = ShowWindow(table, collapsible, skeleton, progress, chart, barChart, lineChart, sonner);

            try
            {
                Assert.Equal(TimeSpan.Zero, collapsible.AnimationDuration);
                Assert.Equal(TimeSpan.Zero, skeleton.PulseDuration);
                Assert.Equal(TimeSpan.Zero, progress.IndeterminateAnimationDuration);
                Assert.Equal(TimeSpan.Zero, chart.AnimationDuration);
                Assert.Equal(TimeSpan.Zero, barChart.AnimationDuration);
                Assert.Equal(TimeSpan.Zero, lineChart.AnimationDuration);

                table.TransitionKey = "refresh";
                var tableContent = FindVisualDescendant<ContentPresenter>(table, "PART_TableContent");
                Assert.Equal(1, tableContent.Opacity);
                AssertTranslateY(tableContent, 0);

                collapsible.IsOpen = true;
                Assert.True(collapsible.IsContentVisible);
                collapsible.IsOpen = false;
                Assert.False(collapsible.IsContentVisible);
                Assert.Equal(0, collapsible.AnimatedHeight);

                Assert.Equal(1, skeleton.PulseOpacity);
                Assert.Equal(0, skeleton.ShimmerOpacity);
                Assert.Equal(0, ReadPrivateDouble(progress, "_indeterminateProgress"));
                Assert.Null(ReadPrivateNullableField<DispatcherTimer>(progress, "_indeterminateTimer"));
                Assert.Equal(1, ReadPrivateDouble(chart, "_chartProgress"));
                Assert.Equal(1, ReadPrivateDouble(barChart, "_chartProgress"));
                Assert.Equal(1, ReadPrivateDouble(lineChart, "_chartProgress"));

                CodexSonnerService.Success("Saved", new CodexSonnerOptions { Duration = TimeSpan.Zero });
                var toastHost = Assert.IsType<Border>(Assert.Single(sonner.Children));
                Assert.Contains("open", toastHost.Classes);
                Assert.DoesNotContain("entering", toastHost.Classes);

                CodexSonnerService.Dismiss(Assert.Single(CodexSonnerService.Toasts));
                Assert.Empty(CodexSonnerService.Toasts);
                Assert.Empty(sonner.Children);

                CaptureAndAssertFrame(window);
            }
            finally
            {
                CodexSonnerService.Clear();
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task TokenizedRuntimeMotionSurfacesExposeIntermediateStatesWhenMotionIsEnabled()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme(reducedMotion: false);
            CodexSonnerService.Clear();

            var table = BuildTable();
            var collapsible = BuildCollapsible(isOpen: true);
            var skeleton = new CodexSkeleton { Width = 160, Height = 18 };
            var progress = new CodexProgress { Width = 240, IsIndeterminate = true };
            var chart = BuildUsagePieChart();
            var barChart = BuildBarChart();
            var lineChart = BuildLineChart();
            var sonner = new CodexSonner { VisibleToasts = 2 };
            var window = ShowWindow(table, collapsible, skeleton, progress, chart, barChart, lineChart, sonner);

            try
            {
                Assert.True(collapsible.AnimationDuration > TimeSpan.Zero);
                Assert.True(skeleton.PulseDuration > TimeSpan.Zero);
                Assert.True(progress.IndeterminateAnimationDuration > TimeSpan.Zero);
                Assert.NotNull(ReadPrivateNullableField<DispatcherTimer>(progress, "_indeterminateTimer"));
                Assert.True(chart.AnimationDuration > TimeSpan.Zero);
                Assert.True(barChart.AnimationDuration > TimeSpan.Zero);
                Assert.True(lineChart.AnimationDuration > TimeSpan.Zero);

                table.TransitionKey = "refresh";
                var tableContent = FindVisualDescendant<ContentPresenter>(table, "PART_TableContent");
                AssertHasNonZeroOpacityTransition(tableContent);
                AssertHasNonZeroTranslateTransition(tableContent);

                chart.ItemsSource = CreateChartItems("gpt-5", 12, "60%", "gpt-4.1", 8, "40%");
                Assert.Equal(0, ReadPrivateDouble(chart, "_chartProgress"));
                barChart.ItemsSource = CreateBarChartItems("OpenAI", 12, "12", "Anthropic", 28, "28");
                Assert.Equal(0, ReadPrivateDouble(barChart, "_chartProgress"));
                lineChart.ItemsSource = CreateLineChartItems("Mon", 12, "12", "Tue", 28, "28");
                Assert.Equal(0, ReadPrivateDouble(lineChart, "_chartProgress"));

                CodexSonnerService.Success("Saved", new CodexSonnerOptions { Duration = TimeSpan.Zero });
                var toastHost = Assert.IsType<Border>(Assert.Single(sonner.Children));
                Assert.Contains("entering", toastHost.Classes);
                Assert.DoesNotContain("open", toastHost.Classes);

                CaptureAndAssertFrame(window);
            }
            finally
            {
                CodexSonnerService.Clear();
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task HoverCardRenderedDelayStatesMatchWebOpenCloseTiming()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme(reducedMotion: false);

            var hoverCard = new CodexHoverCard
            {
                Width = 240,
                Trigger = "Repository",
                Content = new TextBlock { Text = "Hover card content" }
            };
            var instantHoverCard = new CodexHoverCard
            {
                Width = 240,
                OpenDelay = TimeSpan.Zero,
                CloseDelay = TimeSpan.Zero,
                Trigger = "Instant",
                Content = new TextBlock { Text = "Instant content" }
            };
            var window = ShowWindow(hoverCard, instantHoverCard);

            try
            {
                Assert.Equal(TimeSpan.FromMilliseconds(700), hoverCard.OpenDelay);
                Assert.Equal(TimeSpan.FromMilliseconds(300), hoverCard.CloseDelay);
                Assert.Contains("closed", hoverCard.Classes);

                Assert.True(hoverCard.RequestOpen());
                Assert.False(hoverCard.IsOpen);
                Assert.Contains("closed", hoverCard.Classes);

                hoverCard.Open();
                Assert.True(hoverCard.RequestClose());
                Assert.True(hoverCard.IsOpen);
                Assert.Contains("open", hoverCard.Classes);

                Assert.True(instantHoverCard.RequestOpen());
                Assert.True(instantHoverCard.IsOpen);
                Assert.True(instantHoverCard.RequestClose());
                Assert.False(instantHoverCard.IsOpen);

                CaptureAndAssertFrame(window);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task ScrollAreaRenderedVisibilityStatesUseHoverAndScrollMotionClasses()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme(reducedMotion: false);

            var hoverArea = BuildScrollArea(CodexScrollAreaType.Hover);
            var scrollArea = BuildScrollArea(CodexScrollAreaType.Scroll);
            var window = ShowWindow(hoverArea, scrollArea);

            try
            {
                hoverArea.SyncScrollMetricsForTests(new Vector(0, 0), new Size(260, 480), new Size(180, 120));
                scrollArea.SyncScrollMetricsForTests(new Vector(0, 40), new Size(260, 480), new Size(180, 120));
                scrollArea.SetValue(CodexScrollArea.IsScrollingProperty, true);

                Assert.Contains("type-hover", hoverArea.Classes);
                Assert.Contains("can-scroll-y", hoverArea.Classes);
                Assert.Contains("type-scroll", scrollArea.Classes);
                Assert.Contains("scrolling", scrollArea.Classes);
                Assert.Contains("can-scroll-y", scrollArea.Classes);

                var hoverScrollBar = FindVisualDescendant<ScrollBar>(hoverArea, "PART_VerticalScrollBar");
                var scrollScrollBar = FindVisualDescendant<ScrollBar>(scrollArea, "PART_VerticalScrollBar");
                AssertHasNonZeroOpacityTransition(hoverScrollBar);
                AssertHasNonZeroOpacityTransition(scrollScrollBar);

                CaptureAndAssertFrame(window);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task OverlayMotionSurfacesExposeTokenizedTransitionsAndDismissState()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme(reducedMotion: false);

            var tooltip = new CodexTooltip
            {
                Content = "Usage refreshes every minute.",
                Placement = PlacementMode.Right,
                IsOpen = true,
                IsArrowVisible = true
            };
            var popover = new CodexPopover
            {
                Title = "Usage",
                Description = "Dismissible overlay surface.",
                Content = new TextBlock { Text = "Current window" },
                Action = new CodexButton { Content = "Open usage" },
                IsOpen = true
            };
            var commandDialog = new CodexCommandDialog
            {
                Placeholder = "Search commands...",
                IsOpen = true,
                Content = new CodexCommandList
                {
                    Items =
                    {
                        new CodexCommandItem { Content = "Refresh models", IsActive = true }
                    }
                }
            };
            var window = ShowWindow(tooltip, popover, commandDialog);

            try
            {
                Assert.Contains("open", tooltip.Classes);
                Assert.Contains("has-arrow", tooltip.Classes);
                Assert.Contains("side-right", tooltip.Classes);
                Assert.Contains("open", popover.Classes);
                Assert.Contains("has-action", popover.Classes);
                Assert.Contains("restore-focus", popover.Classes);
                Assert.Contains("open", commandDialog.Classes);
                Assert.Contains("close-on-select", commandDialog.Classes);
                var commandSurface = FindVisualDescendant<Border>(commandDialog, "PART_Surface");

                AssertHasNonZeroOpacityTransition(tooltip);
                AssertHasNonZeroRenderTransformTransition(tooltip);
                AssertHasNonZeroOpacityTransition(popover);
                AssertHasNonZeroRenderTransformTransition(popover);
                AssertHasNonZeroOpacityTransition(commandSurface);
                AssertHasNonZeroRenderTransformTransition(commandSurface);

                Assert.True(tooltip.TryHandleDismissKey(Key.Escape));
                Assert.False(tooltip.IsOpen);
                Assert.Contains("closed", tooltip.Classes);

                Assert.True(popover.TryHandleDismissKey(Key.Escape));
                Assert.False(popover.IsOpen);
                Assert.Contains("closed", popover.Classes);

                commandDialog.IsLoading = true;
                var item = Assert.Single(commandDialog.GetLogicalDescendants().OfType<CodexCommandItem>());
                Assert.False(commandDialog.TryCloseFromCommandItem(item));
                Assert.True(commandDialog.IsOpen);
                Assert.Contains("loading", commandDialog.Classes);

                commandDialog.IsLoading = false;
                Assert.True(commandDialog.TryCloseFromCommandItem(item));
                Assert.False(commandDialog.IsOpen);
                Assert.Contains("closed", commandDialog.Classes);

                CaptureAndAssertFrame(window);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task ReducedMotionOverlaySurfacesResolveZeroDurationTransitions()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme(reducedMotion: true);

            var tooltip = new CodexTooltip
            {
                Content = "Reduced motion tooltip",
                Placement = PlacementMode.Bottom,
                IsOpen = true,
                IsArrowVisible = true
            };
            var popover = new CodexPopover
            {
                Title = "Reduced motion",
                Content = new TextBlock { Text = "Popover" },
                IsOpen = true
            };
            var commandDialog = new CodexCommandDialog
            {
                IsOpen = true,
                Content = new CodexCommandList
                {
                    Items =
                    {
                        new CodexCommandItem { Content = "Open logs" }
                    }
                }
            };
            var window = ShowWindow(tooltip, popover, commandDialog);

            try
            {
                AssertHasZeroOpacityTransition(tooltip);
                AssertHasZeroRenderTransformTransition(tooltip);
                AssertHasZeroOpacityTransition(popover);
                AssertHasZeroRenderTransformTransition(popover);
                var commandSurface = FindVisualDescendant<Border>(commandDialog, "PART_Surface");
                AssertHasZeroOpacityTransition(commandSurface);
                AssertHasZeroRenderTransformTransition(commandSurface);

                CaptureAndAssertFrame(window);

                tooltip.IsOpen = false;
                popover.IsOpen = false;
                commandDialog.IsOpen = false;

                Assert.Contains("closed", tooltip.Classes);
                Assert.Contains("closed", popover.Classes);
                Assert.Contains("closed", commandDialog.Classes);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task DropdownAndSplitButtonPopupMotionAndTriggerStateMatchWebMenuContract()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme(reducedMotion: false);

            var dropdownRestoreTarget = new CodexButton { Content = "Dropdown trigger target" };
            var splitRestoreTarget = new CodexButton { Content = "Split trigger target" };
            var otherFocusTarget = new CodexTextBox { Text = "Temporary focus" };
            var dropdownOpenChanges = new List<bool>();
            var splitOpenChanges = new List<bool>();
            var dropdown = new CodexDropdownButton
            {
                Content = "Provider menu",
                DropDownContent = new StackPanel
                {
                    Children =
                    {
                        new CodexButton { Content = "Open provider", Size = CodexControlSize.Small }
                    }
                },
                RestoreFocusElement = dropdownRestoreTarget,
                IsArrowVisible = true,
                Align = CodexDropdownAlign.Start
            };
            var primaryExecutions = 0;
            var splitButton = new CodexSplitButton
            {
                Content = "Run",
                Command = new TestCommand(() => primaryExecutions++),
                DropDownContent = new StackPanel
                {
                    Children =
                    {
                        new CodexButton { Content = "Schedule run", Size = CodexControlSize.Small }
                    }
                },
                RestoreFocusElement = splitRestoreTarget,
                IsArrowVisible = true,
                Align = CodexDropdownAlign.End
            };
            dropdown.OpenChanged += (_, args) => dropdownOpenChanges.Add(args.IsOpen);
            splitButton.OpenChanged += (_, args) => splitOpenChanges.Add(args.IsOpen);
            var window = ShowWindow(dropdownRestoreTarget, splitRestoreTarget, otherFocusTarget, dropdown, splitButton);

            try
            {
                window.UpdateLayout();

                Assert.True(dropdown.Open());
                Assert.True(splitButton.Open());
                Assert.Equal([true], dropdownOpenChanges);
                Assert.Equal([true], splitOpenChanges);
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();

                Assert.Contains("open", dropdown.Classes);
                Assert.Contains("has-arrow", dropdown.Classes);
                Assert.Contains("align-start", dropdown.Classes);
                Assert.Contains("side-bottom", dropdown.Classes);
                Assert.Contains("open", splitButton.Classes);
                Assert.Contains("has-command", splitButton.Classes);
                Assert.Contains("has-arrow", splitButton.Classes);
                Assert.Contains("can-open-dropdown", splitButton.Classes);
                Assert.Contains("align-end", splitButton.Classes);

                AssertHasNonZeroOpacityTransition(ReadPrivateField<Control>(dropdown, "_surface"));
                AssertHasNonZeroRenderTransformTransition(ReadPrivateField<Control>(dropdown, "_surface"));
                AssertHasNonZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(dropdown, "PART_Chevron"));
                AssertHasNonZeroOpacityTransition(ReadPrivateField<Control>(splitButton, "_surface"));
                AssertHasNonZeroRenderTransformTransition(ReadPrivateField<Control>(splitButton, "_surface"));
                AssertHasNonZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(splitButton, "PART_Chevron"));

                Assert.True(splitButton.TryExecutePrimaryAction());
                Assert.Equal(1, primaryExecutions);

                var action = new CodexButton { Content = "Select item" };

                dropdown.IsLoading = true;
                Assert.False(dropdown.TryCloseFromDropDownAction(action));
                Assert.True(dropdown.IsOpen);
                Assert.Equal([true], dropdownOpenChanges);
                Assert.Contains("loading", dropdown.Classes);

                splitButton.IsLoading = true;
                Assert.False(splitButton.CanOpenDropDown);
                Assert.False(splitButton.IsPrimaryActionAvailable);
                Assert.False(splitButton.TryExecutePrimaryAction());
                Assert.Equal(1, primaryExecutions);
                Assert.False(splitButton.TryCloseFromDropDownAction(action));
                Assert.True(splitButton.IsOpen);
                Assert.Equal([true], splitOpenChanges);
                Assert.Contains("loading", splitButton.Classes);

                dropdown.IsLoading = false;
                splitButton.IsLoading = false;

                Assert.True(otherFocusTarget.Focus(NavigationMethod.Tab, KeyModifiers.None));
                Assert.True(dropdown.TryCloseFromDropDownAction(action));
                Assert.False(dropdown.IsOpen);
                Assert.Equal([true, false], dropdownOpenChanges);
                Assert.True(dropdownRestoreTarget.IsFocused);

                Assert.True(otherFocusTarget.Focus(NavigationMethod.Tab, KeyModifiers.None));
                Assert.True(splitButton.TryCloseFromDropDownAction(action));
                Assert.False(splitButton.IsOpen);
                Assert.Equal([true, false], splitOpenChanges);
                Assert.True(splitRestoreTarget.IsFocused);

                CaptureAndAssertFrame(window);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task ReducedMotionDropdownAndSplitButtonPopupTransitionsResolveZeroDuration()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme(reducedMotion: true);

            var dropdown = new CodexDropdownButton
            {
                Content = "Provider menu",
                DropDownContent = new CodexButton { Content = "Open provider" },
                IsArrowVisible = true
            };
            var splitButton = new CodexSplitButton
            {
                Content = "Run",
                DropDownContent = new CodexButton { Content = "Schedule run" },
                IsArrowVisible = true
            };
            var window = ShowWindow(dropdown, splitButton);

            try
            {
                window.UpdateLayout();

                Assert.True(dropdown.Open());
                Assert.True(splitButton.Open());
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();

                AssertHasZeroOpacityTransition(ReadPrivateField<Control>(dropdown, "_surface"));
                AssertHasZeroRenderTransformTransition(ReadPrivateField<Control>(dropdown, "_surface"));
                AssertHasZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(dropdown, "PART_Chevron"));
                AssertHasZeroOpacityTransition(ReadPrivateField<Control>(splitButton, "_surface"));
                AssertHasZeroRenderTransformTransition(ReadPrivateField<Control>(splitButton, "_surface"));
                AssertHasZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(splitButton, "PART_Chevron"));

                CaptureAndAssertFrame(window);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task SelectNavigationMenuAndCollapsibleChevronMotionSelectorsHitMountedTemplates()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme(reducedMotion: false);

            var select = new CodexSelect
            {
                Width = 240,
                ItemsSource = new[] { "OpenAI", "Claude", "Local" },
                SelectedIndex = 0
            };
            var navigationItem = new CodexNavigationMenuItem
            {
                Header = "Providers",
                Content = new CodexNavigationMenuContent
                {
                    Header = "Providers",
                    Description = "Routing and usage surfaces."
                }
            };
            var navigationMenu = new CodexNavigationMenu
            {
                Width = 420,
                Items =
                {
                    navigationItem,
                    new CodexNavigationMenuItem { Header = "Logs" }
                }
            };
            var collapsible = BuildCollapsible(isOpen: false);
            var window = ShowWindow(select, navigationMenu, collapsible);

            try
            {
                window.UpdateLayout();

                select.IsDropDownOpen = true;
                Dispatcher.UIThread.RunJobs();
                navigationMenu.ActivateItem(navigationItem);
                collapsible.IsOpen = true;
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();

                Assert.True(select.IsDropDownOpen);
                Assert.Contains("popup-open", select.Classes);
                Assert.Contains("open", navigationMenu.Classes);
                Assert.Contains("open", navigationItem.Classes);
                Assert.Contains("open", collapsible.Classes);
                Assert.True(collapsible.IsContentVisible);
                Assert.True(collapsible.AnimatedHeight >= 0);

                AssertHasNonZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(select, "PART_Chevron"));
                AssertHasNonZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(navigationItem, "PART_Chevron"));
                AssertHasNonZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(collapsible, "PART_Chevron"));
                AssertHasNonZeroOpacityTransition(FindVisualDescendant<Border>(navigationMenu, "PART_Viewport"));
                AssertHasNonZeroRenderTransformTransition(FindVisualDescendant<Border>(navigationMenu, "PART_Viewport"));
                AssertHasNonZeroOpacityTransition(FindVisualDescendant<Border>(navigationMenu, "PART_Indicator"));

                CaptureAndAssertFrame(window);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task ReducedMotionChevronDisclosureTransitionsResolveZeroDuration()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme(reducedMotion: true);

            var select = new CodexSelect
            {
                Width = 240,
                ItemsSource = new[] { "OpenAI", "Claude", "Local" },
                SelectedIndex = 0
            };
            var navigationItem = new CodexNavigationMenuItem
            {
                Header = "Providers",
                Content = new TextBlock { Text = "Navigation content" }
            };
            var navigationMenu = new CodexNavigationMenu
            {
                Width = 420,
                Items =
                {
                    navigationItem
                }
            };
            var collapsible = BuildCollapsible(isOpen: false);
            var window = ShowWindow(select, navigationMenu, collapsible);

            try
            {
                window.UpdateLayout();

                select.IsDropDownOpen = true;
                Dispatcher.UIThread.RunJobs();
                navigationMenu.ActivateItem(navigationItem);
                collapsible.IsOpen = true;
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();

                AssertHasZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(select, "PART_Chevron"));
                AssertHasZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(navigationItem, "PART_Chevron"));
                AssertHasZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(collapsible, "PART_Chevron"));
                AssertHasZeroOpacityTransition(FindVisualDescendant<Border>(navigationMenu, "PART_Viewport"));
                AssertHasZeroRenderTransformTransition(FindVisualDescendant<Border>(navigationMenu, "PART_Viewport"));
                AssertHasZeroOpacityTransition(FindVisualDescendant<Border>(navigationMenu, "PART_Indicator"));
                Assert.Equal(TimeSpan.Zero, collapsible.AnimationDuration);

                CaptureAndAssertFrame(window);
            }
            finally
            {
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task MenuAndContextMenuSubMenuMotionSelectorsHitMountedTemplates()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme(reducedMotion: false);

            var menuSubMenu = BuildMenuSubMenu();
            var menu = new CodexMenu
            {
                Width = 240,
                Items =
                {
                    new CodexMenuItem { Header = "Open" },
                    menuSubMenu
                }
            };

            var contextSubMenu = BuildContextSubMenu();
            var contextMenu = new CodexContextMenu
            {
                Placement = PlacementMode.Right,
                Items =
                {
                    new CodexContextMenuItem { Header = "Copy" },
                    contextSubMenu
                }
            };
            var contextTarget = new CodexButton { Width = 160, Content = "Context target" };
            var window = ShowWindow(menu, contextTarget);

            try
            {
                window.UpdateLayout();
                menuSubMenu.IsSubMenuOpen = true;
                contextMenu.Open(contextTarget);
                Dispatcher.UIThread.RunJobs();
                contextSubMenu.IsSubMenuOpen = true;
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();

                Assert.True(menuSubMenu.HasSubMenu);
                Assert.True(menuSubMenu.IsSubMenuOpen);
                Assert.Contains("has-submenu", menuSubMenu.Classes);
                Assert.True(contextSubMenu.HasSubMenu);
                Assert.True(contextSubMenu.IsSubMenuOpen);
                Assert.Contains("has-submenu", contextSubMenu.Classes);
                Assert.Contains("submenu-open", contextSubMenu.Classes);

                AssertHasNonZeroOpacityTransition(FindVisualDescendant<PathIcon>(menuSubMenu, "PART_SubMenuArrow"));
                AssertHasNonZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(menuSubMenu, "PART_SubMenuArrow"));
                AssertHasNonZeroOpacityTransition(FindSubMenuSurface(menuSubMenu));
                AssertHasNonZeroRenderTransformTransition(FindSubMenuSurface(menuSubMenu));

                AssertHasNonZeroOpacityTransition(FindVisualDescendant<PathIcon>(contextSubMenu, "PART_SubMenuArrow"));
                AssertHasNonZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(contextSubMenu, "PART_SubMenuArrow"));
                AssertHasNonZeroOpacityTransition(FindSubMenuSurface(contextSubMenu));
                AssertHasNonZeroRenderTransformTransition(FindSubMenuSurface(contextSubMenu));

                CaptureAndAssertFrame(window);
            }
            finally
            {
                contextMenu.Close();
                window.Close();
            }
        }, CancellationToken.None);
    }

    [Fact]
    public async Task ReducedMotionMenuAndContextMenuSubMenuTransitionsResolveZeroDuration()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            EnsureCodexTheme(reducedMotion: true);

            var menuSubMenu = BuildMenuSubMenu();
            var menu = new CodexMenu
            {
                Width = 240,
                Items =
                {
                    menuSubMenu
                }
            };

            var contextSubMenu = BuildContextSubMenu();
            var contextMenu = new CodexContextMenu
            {
                Placement = PlacementMode.Left,
                Items =
                {
                    contextSubMenu
                }
            };
            var contextTarget = new CodexButton { Width = 160, Content = "Context target" };
            var window = ShowWindow(menu, contextTarget);

            try
            {
                window.UpdateLayout();
                menuSubMenu.IsSubMenuOpen = true;
                contextMenu.Open(contextTarget);
                Dispatcher.UIThread.RunJobs();
                contextSubMenu.IsSubMenuOpen = true;
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();

                AssertHasZeroOpacityTransition(FindVisualDescendant<PathIcon>(menuSubMenu, "PART_SubMenuArrow"));
                AssertHasZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(menuSubMenu, "PART_SubMenuArrow"));
                AssertHasZeroOpacityTransition(FindSubMenuSurface(menuSubMenu));
                AssertHasZeroRenderTransformTransition(FindSubMenuSurface(menuSubMenu));

                AssertHasZeroOpacityTransition(FindVisualDescendant<PathIcon>(contextSubMenu, "PART_SubMenuArrow"));
                AssertHasZeroRenderTransformTransition(FindVisualDescendant<PathIcon>(contextSubMenu, "PART_SubMenuArrow"));
                AssertHasZeroOpacityTransition(FindSubMenuSurface(contextSubMenu));
                AssertHasZeroRenderTransformTransition(FindSubMenuSurface(contextSubMenu));

                CaptureAndAssertFrame(window);
            }
            finally
            {
                contextMenu.Close();
                window.Close();
            }
        }, CancellationToken.None);
    }

    private static void EnsureCodexTheme(bool reducedMotion)
    {
        var application = Application.Current;
        Assert.NotNull(application);

        if (!application.Styles.OfType<CodexSwitchTheme>().Any())
        {
            application.Styles.Add(new CodexSwitchTheme());
        }

        CodexSwitchThemeManager.Current.Apply(
            application,
            CodexSwitchThemeMode.Light,
            CodexSwitchThemeOptions.ShadcnDefault with { ReducedMotion = reducedMotion });

        Assert.Equal(
            reducedMotion ? TimeSpan.Zero : CodexSwitchThemeOptions.ShadcnDefault.MotionDurationDefault,
            Assert.IsType<TimeSpan>(application.Resources["CodexSwitch.MotionDurationDefault"]));
    }

    private static Window ShowWindow(params Control[] controls)
    {
        var root = new StackPanel
        {
            Spacing = 16,
            Margin = new Thickness(18)
        };

        foreach (var control in controls)
        {
            root.Children.Add(control);
        }

        var window = new Window
        {
            Width = 720,
            Height = 720,
            Content = root
        };

        window.Show();
        return window;
    }

    private static CodexTable BuildTable()
    {
        return new CodexTable
        {
            Width = 320,
            TransitionOffset = 7,
            Content = new StackPanel
            {
                Children =
                {
                    new CodexTableRow { Content = new TextBlock { Text = "Model" } },
                    new CodexTableRow { Content = new TextBlock { Text = "gpt-5" } }
                }
            }
        };
    }

    private static CodexCollapsible BuildCollapsible(bool isOpen)
    {
        return new CodexCollapsible
        {
            Width = 320,
            Header = "Details",
            IsOpen = isOpen,
            Content = new Border
            {
                Height = 28,
                Background = Brushes.LightGray,
                Child = new TextBlock { Text = "Mounted disclosure content" }
            }
        };
    }

    private static CodexUsagePieChart BuildUsagePieChart()
    {
        return new CodexUsagePieChart
        {
            Width = 320,
            Height = 220,
            TotalLabel = "Requests",
            TotalValue = "20",
            ItemsSource = CreateChartItems("gpt-5", 14, "70%", "gpt-4.1", 6, "30%")
        };
    }

    private static CodexBarChart BuildBarChart()
    {
        return new CodexBarChart
        {
            Width = 320,
            Height = 180,
            ItemsSource = CreateBarChartItems("OpenAI", 8, "8", "Anthropic", 20, "20")
        };
    }

    private static CodexLineChart BuildLineChart()
    {
        return new CodexLineChart
        {
            Width = 320,
            Height = 180,
            ItemsSource = CreateLineChartItems("Mon", 8, "8", "Tue", 20, "20")
        };
    }

    private static CodexScrollArea BuildScrollArea(CodexScrollAreaType type)
    {
        return new CodexScrollArea
        {
            Width = 180,
            Height = 120,
            Type = type,
            Content = new StackPanel
            {
                Children =
                {
                    new TextBlock { Text = "Top" },
                    new Border { Height = 260, Background = Brushes.LightGray },
                    new TextBlock { Text = "Bottom" }
                }
            }
        };
    }

    private static CodexMenuItem BuildMenuSubMenu()
    {
        var subMenu = new CodexMenuItem { Header = "More" };
        subMenu.Items.Add(new CodexMenuItem { Header = "Rename" });
        subMenu.Items.Add(new CodexMenuItem { Header = "Move" });
        return subMenu;
    }

    private static CodexContextMenuItem BuildContextSubMenu()
    {
        var subMenu = new CodexContextMenuItem
        {
            Header = "Move to",
            SubMenuPlacement = PlacementMode.RightEdgeAlignedTop
        };
        subMenu.Items.Add(new CodexContextMenuItem { Header = "Archive" });
        subMenu.Items.Add(new CodexContextMenuItem { Header = "Later" });
        return subMenu;
    }

    private static CodexUsagePieChartItem[] CreateChartItems(
        string firstLabel,
        double firstValue,
        string firstText,
        string secondLabel,
        double secondValue,
        string secondText)
    {
        return
        [
            new CodexUsagePieChartItem(firstLabel, firstValue, firstText),
            new CodexUsagePieChartItem(secondLabel, secondValue, secondText)
        ];
    }

    private static CodexBarChartItem[] CreateBarChartItems(
        string firstLabel,
        double firstValue,
        string firstText,
        string secondLabel,
        double secondValue,
        string secondText)
    {
        return
        [
            new CodexBarChartItem(firstLabel, firstValue, firstText),
            new CodexBarChartItem(secondLabel, secondValue, secondText)
        ];
    }

    private static CodexLineChartPoint[] CreateLineChartItems(
        string firstLabel,
        double firstValue,
        string firstText,
        string secondLabel,
        double secondValue,
        string secondText)
    {
        return
        [
            new CodexLineChartPoint(firstLabel, firstValue, firstText),
            new CodexLineChartPoint(secondLabel, secondValue, secondText)
        ];
    }

    private static T FindVisualDescendant<T>(Control root, string name)
        where T : StyledElement
    {
        var match = root.GetVisualDescendants()
            .OfType<T>()
            .FirstOrDefault(element => element.Name == name);

        Assert.NotNull(match);
        return match;
    }

    private static Border FindSubMenuSurface(MenuItem item)
    {
        var popup = FindVisualDescendant<Popup>(item, "PART_Popup");
        var surface = Assert.IsType<Border>(popup.Child);

        Assert.Equal("PART_SubMenuSurface", surface.Name);
        return surface;
    }

    private static void AssertTranslateY(Control target, double expected)
    {
        var transform = Assert.IsType<TranslateTransform>(target.RenderTransform);
        Assert.Equal(expected, transform.Y, 2);
    }

    private static void AssertHasNonZeroOpacityTransition(Control target)
    {
        var transitions = target.Transitions;
        Assert.NotNull(transitions);
        var transition = Assert.Single(
            transitions.OfType<DoubleTransition>(),
            transition => transition.Property == Visual.OpacityProperty);
        Assert.Equal(Visual.OpacityProperty, transition.Property);
        Assert.True(transition.Duration > TimeSpan.Zero);
    }

    private static void AssertHasZeroOpacityTransition(Control target)
    {
        var transitions = target.Transitions;
        Assert.NotNull(transitions);
        var transition = Assert.Single(
            transitions.OfType<DoubleTransition>(),
            transition => transition.Property == Visual.OpacityProperty);
        Assert.Equal(Visual.OpacityProperty, transition.Property);
        Assert.Equal(TimeSpan.Zero, transition.Duration);
    }

    private static void AssertHasNonZeroRenderTransformTransition(Control target)
    {
        var transitions = target.Transitions;
        Assert.NotNull(transitions);
        var transition = Assert.Single(transitions.OfType<TransformOperationsTransition>());
        Assert.Equal(Visual.RenderTransformProperty, transition.Property);
        Assert.True(transition.Duration > TimeSpan.Zero);
    }

    private static void AssertHasZeroRenderTransformTransition(Control target)
    {
        var transitions = target.Transitions;
        Assert.NotNull(transitions);
        var transition = Assert.Single(transitions.OfType<TransformOperationsTransition>());
        Assert.Equal(Visual.RenderTransformProperty, transition.Property);
        Assert.Equal(TimeSpan.Zero, transition.Duration);
    }

    private static void AssertHasNonZeroTranslateTransition(Control target)
    {
        var transform = Assert.IsType<TranslateTransform>(target.RenderTransform);
        var transitions = transform.Transitions;
        Assert.NotNull(transitions);
        var transition = Assert.Single(transitions.OfType<DoubleTransition>());
        Assert.Equal(TranslateTransform.YProperty, transition.Property);
        Assert.True(transition.Duration > TimeSpan.Zero);
    }

    private static double ReadPrivateDouble(object target, string fieldName)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);

        return Assert.IsType<double>(field.GetValue(target));
    }

    private static T ReadPrivateField<T>(object target, string fieldName)
        where T : class
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);

        return Assert.IsAssignableFrom<T>(field.GetValue(target));
    }

    private static T? ReadPrivateNullableField<T>(object target, string fieldName)
        where T : class
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);

        return field.GetValue(target) as T;
    }

    private static void CaptureAndAssertFrame(Window window)
    {
        using var frame = Assert.IsAssignableFrom<Bitmap>(window.CaptureRenderedFrame());
        Assert.InRange(frame.PixelSize.Width, 680, 760);
        Assert.InRange(frame.PixelSize.Height, 680, 760);

        using var stream = new MemoryStream();
        frame.Save(stream);
        Assert.True(stream.Length > 4096);
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
