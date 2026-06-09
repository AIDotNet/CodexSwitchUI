using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System.Reflection;
using System.Windows.Input;
using Xunit;

namespace CodexSwitchUI.Tests;

public class NavigationDataComponentTests
{
    [Fact]
    public void TabsKeyboardSelectionMirrorsWebRovingTriggers()
    {
        var changes = new List<CodexTabsValueChangedEventArgs>();
        var first = new CodexTabItem { Header = "General", Value = "general" };
        var disabled = new CodexTabItem { Header = "Disabled", Value = "disabled", IsEnabled = false };
        var last = new CodexTabItem { Header = "Advanced", Value = "advanced" };
        var tabs = new CodexTabs
        {
            ItemsSource = new[] { first, disabled, last },
            SelectedIndex = 0
        };
        tabs.ValueChanged += (_, args) => changes.Add(args);

        Assert.True(tabs.TryHandleSelectionKey(Key.Right));
        Assert.Equal(2, tabs.SelectedIndex);
        Assert.Equal("advanced", tabs.SelectedValue);
        Assert.Single(changes);
        Assert.Equal(0, changes[0].OldIndex);
        Assert.Equal(2, changes[0].NewIndex);
        Assert.Equal("general", changes[0].OldValue);
        Assert.Equal("advanced", changes[0].NewValue);
        Assert.Equal(CodexTabsValueChangeSource.Keyboard, changes[0].Source);

        Assert.True(tabs.TryHandleSelectionKey(Key.Right));
        Assert.Equal(0, tabs.SelectedIndex);

        Assert.True(tabs.TryHandleSelectionKey(Key.End));
        Assert.Equal(2, tabs.SelectedIndex);
        tabs.IsLoop = false;
        Assert.False(tabs.TryHandleSelectionKey(Key.Right));
        Assert.Equal(2, tabs.SelectedIndex);
        tabs.IsLoop = true;

        tabs.Orientation = Orientation.Vertical;

        Assert.True(tabs.TryHandleSelectionKey(Key.Up));
        Assert.Equal(0, tabs.SelectedIndex);
        Assert.False(tabs.TryHandleSelectionKey(Key.Left));

        tabs.ActivationMode = CodexTabsActivationMode.Manual;
        changes.Clear();
        Assert.True(tabs.TryHandleSelectionKey(Key.Down));
        Assert.Equal(0, tabs.SelectedIndex);
        Assert.Empty(changes);
        Assert.Contains("activation-manual", tabs.Classes);

        Assert.True(last.TryHandleActivationKey(Key.Space));
        Assert.True(last.IsSelected);
        Assert.False(first.TryHandleActivationKey(Key.Escape));
    }

    [Fact]
    public void TabsValueChangedPublishesSourceMetadataAndPrimaryPointerRelease()
    {
        var changes = new List<CodexTabsValueChangedEventArgs>();
        var preview = new CodexTabItem { Header = "Preview", Value = "preview" };
        var code = new CodexTabItem { Header = "Code", Value = "code" };
        var disabled = new CodexTabItem { Header = "Disabled", Value = "disabled", IsEnabled = false };
        var tabs = new CodexTabs
        {
            SelectedIndex = 0,
            Items =
            {
                preview,
                code,
                disabled
            }
        };
        tabs.ValueChanged += (_, args) => changes.Add(args);

        Assert.False(code.TryHandlePointerActivation(PointerUpdateKind.RightButtonReleased));
        Assert.False(code.TryHandlePointerActivation(PointerUpdateKind.MiddleButtonReleased));
        Assert.Equal(0, tabs.SelectedIndex);

        Assert.True(code.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));

        Assert.Equal(1, tabs.SelectedIndex);
        Assert.Equal("code", tabs.SelectedValue);
        var pointerChange = Assert.Single(changes);
        Assert.Same(preview, pointerChange.OldItem);
        Assert.Same(code, pointerChange.NewItem);
        Assert.Equal(0, pointerChange.OldIndex);
        Assert.Equal(1, pointerChange.NewIndex);
        Assert.Equal("preview", pointerChange.OldValue);
        Assert.Equal("code", pointerChange.NewValue);
        Assert.Equal(CodexTabsValueChangeSource.Pointer, pointerChange.Source);

        changes.Clear();
        Assert.True(preview.TryHandleActivationKey(Key.Enter));

        var keyboardChange = Assert.Single(changes);
        Assert.Equal("code", keyboardChange.OldValue);
        Assert.Equal("preview", keyboardChange.NewValue);
        Assert.Equal(CodexTabsValueChangeSource.Keyboard, keyboardChange.Source);

        changes.Clear();
        tabs.SelectedValue = "code";

        var programmaticChange = Assert.Single(changes);
        Assert.Equal("preview", programmaticChange.OldValue);
        Assert.Equal("code", programmaticChange.NewValue);
        Assert.Equal(CodexTabsValueChangeSource.Programmatic, programmaticChange.Source);

        Assert.False(disabled.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.Equal("code", tabs.SelectedValue);
        Assert.Single(changes);
    }

    [Fact]
    public void CarouselNavigationLoopAndKeyboardMirrorWebSlideSelection()
    {
        var changes = new List<CodexCarouselSelectionChangedEventArgs>();
        var one = new CodexCarouselItem { Content = "One" };
        var two = new CodexCarouselItem { Content = "Two" };
        var three = new CodexCarouselItem { Content = "Three" };
        var carousel = new CodexCarousel
        {
            Items =
            {
                one,
                two,
                three
            }
        };
        carousel.SelectionChanged += (_, args) => changes.Add(args);

        Assert.Contains("carousel", carousel.Classes);
        Assert.Contains("has-items", carousel.Classes);
        Assert.Contains("has-multiple", carousel.Classes);
        Assert.Equal(3, carousel.SlideCount);
        Assert.Equal("Slide 1 of 3", carousel.StatusText);
        Assert.False(carousel.CanGoPrevious);
        Assert.True(carousel.CanGoNext);
        Assert.Contains("at-start", carousel.Classes);
        Assert.DoesNotContain("at-end", carousel.Classes);
        Assert.Contains("previous-disabled", carousel.Classes);
        Assert.DoesNotContain("next-disabled", carousel.Classes);
        Assert.False(carousel.PreviousCommand.CanExecute(null));
        Assert.True(carousel.NextCommand.CanExecute(null));

        Assert.True(carousel.GoNext());
        Assert.Equal(1, carousel.SelectedIndex);
        Assert.Equal("Slide 2 of 3", carousel.StatusText);
        Assert.DoesNotContain("at-start", carousel.Classes);
        Assert.DoesNotContain("at-end", carousel.Classes);
        Assert.DoesNotContain("previous-disabled", carousel.Classes);
        Assert.DoesNotContain("next-disabled", carousel.Classes);
        var nextChange = Assert.Single(changes);
        Assert.Equal(0, nextChange.OldIndex);
        Assert.Equal(1, nextChange.NewIndex);
        Assert.Same(one, nextChange.OldItem);
        Assert.Same(two, nextChange.NewItem);
        Assert.Equal(CodexCarouselSelectionChangeSource.Next, nextChange.Source);

        Assert.True(carousel.TryHandleNavigationKey(Key.Right));
        Assert.Equal(2, carousel.SelectedIndex);
        Assert.Equal(CodexCarouselSelectionChangeSource.Keyboard, changes[1].Source);
        Assert.Same(three, changes[1].NewItem);
        Assert.False(carousel.CanGoNext);
        Assert.Contains("at-end", carousel.Classes);
        Assert.Contains("next-disabled", carousel.Classes);
        Assert.DoesNotContain("previous-disabled", carousel.Classes);

        Assert.False(carousel.GoNext());
        carousel.Loop = true;
        Assert.Contains("at-end", carousel.Classes);
        Assert.DoesNotContain("next-disabled", carousel.Classes);
        Assert.True(carousel.NextCommand.CanExecute(null));
        Assert.True(carousel.GoNext());
        Assert.Equal(0, carousel.SelectedIndex);
        Assert.Equal(CodexCarouselSelectionChangeSource.Next, changes[2].Source);
        Assert.True(carousel.CanGoPrevious);
        Assert.True(carousel.CanGoNext);
        Assert.Contains("at-start", carousel.Classes);
        Assert.DoesNotContain("previous-disabled", carousel.Classes);
        Assert.DoesNotContain("next-disabled", carousel.Classes);

        carousel.Orientation = Orientation.Vertical;
        Assert.True(carousel.TryHandleNavigationKey(Key.Down));
        Assert.Equal(1, carousel.SelectedIndex);
        Assert.Equal(CodexCarouselSelectionChangeSource.Keyboard, changes[3].Source);
        Assert.False(carousel.TryHandleNavigationKey(Key.Right));

        Assert.True(carousel.TryHandleNavigationKey(Key.End));
        Assert.Equal(2, carousel.SelectedIndex);
        Assert.Equal(CodexCarouselSelectionChangeSource.Keyboard, changes[4].Source);
        Assert.True(carousel.TryHandleNavigationKey(Key.Home));
        Assert.Equal(0, carousel.SelectedIndex);
        Assert.Equal(CodexCarouselSelectionChangeSource.Keyboard, changes[5].Source);

        Assert.True(carousel.SelectIndex(2));
        Assert.Equal(CodexCarouselSelectionChangeSource.Programmatic, changes[6].Source);
        Assert.Same(one, changes[6].OldItem);
        Assert.Same(three, changes[6].NewItem);
    }

    [Fact]
    public void ResizablePanelGroupKeyboardAndResizeEventsMirrorWebPanels()
    {
        var changes = new List<IReadOnlyList<double>>();
        var left = new CodexResizablePanel { DefaultSize = 30, MinSize = 20, MaxSize = 60, Content = "Left" };
        var right = new CodexResizablePanel { DefaultSize = 70, MinSize = 35, Content = "Right" };
        var handle = new CodexResizableHandle { WithHandle = true };
        var group = new CodexResizablePanelGroup
        {
            Size = CodexControlSize.Large,
            Children =
            {
                left,
                handle,
                right
            }
        };
        group.LayoutChanged += (_, args) => changes.Add(args.PanelSizes);

        Assert.Contains("resizable-panel-group", group.Classes);
        Assert.Contains("horizontal", group.Classes);
        Assert.Contains("has-handle", group.Classes);
        Assert.True(group.ResizeHandleByPercent(handle, 10));
        Assert.Equal(40, Math.Round(left.PanelSize));
        Assert.Equal(60, Math.Round(right.PanelSize));
        Assert.Equal("40% / 60%", group.LayoutSummary);
        Assert.Contains("resizable-panel", left.Classes);
        Assert.Contains("resizable-handle", handle.Classes);
        Assert.Contains("with-handle", handle.Classes);
        Assert.Single(changes);

        Assert.True(handle.TryHandleResizeKey(Key.Left));
        Assert.Equal(30, Math.Round(left.PanelSize));
        Assert.Equal(70, Math.Round(right.PanelSize));

        group.Orientation = Orientation.Vertical;
        Assert.Contains("vertical", group.Classes);
        Assert.Contains("vertical", handle.Classes);
        Assert.True(handle.TryHandleResizeKey(Key.Down));
        Assert.Equal(40, Math.Round(left.PanelSize));
        Assert.False(handle.TryHandleResizeKey(Key.Right));

        Assert.True(handle.TryHandleResizeKey(Key.End));
        Assert.Equal(60, Math.Round(left.PanelSize));
        Assert.Equal(40, Math.Round(right.PanelSize));
        Assert.True(handle.TryHandleResizeKey(Key.Home));
        Assert.Equal(20, Math.Round(left.PanelSize));
        Assert.Equal(80, Math.Round(right.PanelSize));
        Assert.Contains(changes, sizes => Math.Round(sizes[0]) == 20 && Math.Round(sizes[1]) == 80);
    }

    [Fact]
    public void ResizableHandlePointerDragStartsAndEndsOnlyForPrimaryPointer()
    {
        var left = new CodexResizablePanel { DefaultSize = 30, MinSize = 20, MaxSize = 60, Content = "Left" };
        var right = new CodexResizablePanel { DefaultSize = 70, MinSize = 35, Content = "Right" };
        var handle = new CodexResizableHandle { WithHandle = true };
        var group = new CodexResizablePanelGroup
        {
            Children =
            {
                left,
                handle,
                right
            }
        };

        Assert.False(handle.TryBeginResize(PointerUpdateKind.RightButtonPressed, new Point(10, 0), group));
        Assert.False(group.IsDragging);
        Assert.False(handle.IsDragging);
        Assert.DoesNotContain("dragging", group.Classes);
        Assert.DoesNotContain("dragging", handle.Classes);

        Assert.False(handle.TryBeginResize(PointerUpdateKind.MiddleButtonPressed, new Point(12, 0), group));
        Assert.False(group.IsDragging);
        Assert.False(handle.IsDragging);

        Assert.True(handle.TryBeginResize(PointerUpdateKind.LeftButtonPressed, new Point(16, 0), group));
        Assert.True(group.IsDragging);
        Assert.True(handle.IsDragging);
        Assert.Contains("dragging", group.Classes);
        Assert.Contains("dragging", handle.Classes);

        Assert.False(handle.TryEndResize(PointerUpdateKind.RightButtonReleased, group));
        Assert.True(group.IsDragging);
        Assert.True(handle.IsDragging);

        Assert.False(handle.TryEndResize(PointerUpdateKind.MiddleButtonReleased, group));
        Assert.True(group.IsDragging);
        Assert.True(handle.IsDragging);

        Assert.True(handle.TryEndResize(PointerUpdateKind.LeftButtonReleased, group));
        Assert.False(group.IsDragging);
        Assert.False(handle.IsDragging);
        Assert.DoesNotContain("dragging", group.Classes);
        Assert.DoesNotContain("dragging", handle.Classes);
        Assert.False(handle.TryEndResize(PointerUpdateKind.LeftButtonReleased, group));

        handle.IsEnabled = false;
        Assert.False(handle.TryBeginResize(PointerUpdateKind.LeftButtonPressed, new Point(20, 0), group));
        Assert.False(group.IsDragging);
        Assert.False(handle.IsDragging);
    }

    [Fact]
    public void BreadcrumbCompositionMirrorsWebPathLinksAndCurrentPageGuard()
    {
        var routeCount = 0;
        var homeCommand = new TestCommand(() => routeCount++);
        var home = new CodexBreadcrumbLink
        {
            Content = "Home",
            Href = "/",
            Command = homeCommand
        };
        var currentLink = new CodexBreadcrumbLink
        {
            Content = "Breadcrumb",
            IsCurrent = true,
            Command = new TestCommand(() => routeCount++)
        };
        var page = new CodexBreadcrumbPage
        {
            Content = "Breadcrumb",
            Size = CodexControlSize.Large
        };
        var ellipsis = new CodexBreadcrumbEllipsis { Label = "More sections", Size = CodexControlSize.Small };
        var separator = new CodexBreadcrumbSeparator { Content = "/", Size = CodexControlSize.Small };
        var item = new CodexBreadcrumbItem { IsCurrent = true, Content = page };
        var list = new CodexBreadcrumbList
        {
            Size = CodexControlSize.Small,
            Items =
            {
                new CodexBreadcrumbItem { Content = home },
                separator,
                new CodexBreadcrumbItem { Content = ellipsis },
                new CodexBreadcrumbSeparator(),
                item
            }
        };
        var breadcrumb = new CodexBreadcrumb
        {
            Size = CodexControlSize.Small,
            Label = "Documentation path",
            Content = list
        };
        var currentBreadcrumb = new CodexBreadcrumb
        {
            Content = new CodexBreadcrumbList
            {
                Items =
                {
                    new CodexBreadcrumbItem { IsCurrent = true, Content = currentLink }
                }
            }
        };
        var activations = new List<CodexBreadcrumbLinkActivatedEventArgs>();
        breadcrumb.LinkActivated += (_, args) => activations.Add(args);
        currentBreadcrumb.LinkActivated += (_, args) => activations.Add(args);

        Assert.Contains("breadcrumb", breadcrumb.Classes);
        Assert.Contains("size-sm", breadcrumb.Classes);
        Assert.Equal("Documentation path", breadcrumb.Label);
        Assert.Equal("Documentation path", AutomationProperties.GetName(breadcrumb));
        Assert.Contains("breadcrumb-list", list.Classes);
        Assert.Contains("size-sm", list.Classes);
        Assert.Contains("breadcrumb-item", item.Classes);
        Assert.Contains("current", item.Classes);
        Assert.Contains("breadcrumb-link", home.Classes);
        Assert.Contains("has-href", home.Classes);
        Assert.Contains("can-activate", home.Classes);
        Assert.DoesNotContain("command-blocked", home.Classes);
        Assert.Contains("breadcrumb-page", page.Classes);
        Assert.Contains("current", page.Classes);
        Assert.Contains("size-lg", page.Classes);
        Assert.Contains("breadcrumb-separator", separator.Classes);
        Assert.Contains("size-sm", separator.Classes);
        Assert.Contains("breadcrumb-ellipsis", ellipsis.Classes);
        Assert.Equal("More sections", ellipsis.Label);
        Assert.Equal("More sections", AutomationProperties.GetName(ellipsis));

        Assert.True(home.TryActivate());
        Assert.Equal(1, routeCount);
        var activation = Assert.Single(activations);
        Assert.Same(home, activation.Link);
        Assert.Same(list.Items[0], activation.Item);
        Assert.Equal(0, activation.Index);
        Assert.Equal("/", activation.Href);
        Assert.Equal("Home", activation.Content);
        Assert.Equal(CodexBreadcrumbLinkActivationSource.Programmatic, activation.Source);

        homeCommand.CanExecuteValue = false;
        homeCommand.RaiseCanExecuteChanged();

        Assert.False(home.TryActivate());
        Assert.Equal(1, routeCount);
        Assert.Single(activations);
        Assert.Contains("command-blocked", home.Classes);
        Assert.DoesNotContain("can-activate", home.Classes);

        homeCommand.CanExecuteValue = true;
        homeCommand.RaiseCanExecuteChanged();
        Assert.Contains("can-activate", home.Classes);
        Assert.DoesNotContain("command-blocked", home.Classes);

        Assert.False(currentLink.TryActivate());
        Assert.Equal(1, routeCount);
        Assert.Single(activations);

        home.IsEnabled = false;
        Assert.False(home.TryActivate());
        Assert.Equal(1, routeCount);
        Assert.Single(activations);
    }

    [Fact]
    public void BreadcrumbLinkActivationUsesPrimaryReleaseAndSourceMetadata()
    {
        var routeCount = 0;
        var docs = new CodexBreadcrumbLink
        {
            Content = "Docs",
            Href = "/docs",
            Command = new TestCommand(() => routeCount++)
        };
        var components = new CodexBreadcrumbLink
        {
            Content = "Components",
            Href = "/components",
            Command = new TestCommand(() => routeCount++)
        };
        var api = new CodexBreadcrumbLink
        {
            Content = "API",
            Href = "/api",
            Command = new TestCommand(() => routeCount++)
        };
        var current = new CodexBreadcrumbLink
        {
            Content = "Breadcrumb",
            IsCurrent = true,
            Command = new TestCommand(() => routeCount++)
        };
        var blockedCommand = new TestCommand(() => routeCount++)
        {
            CanExecuteValue = false
        };
        var blocked = new CodexBreadcrumbLink
        {
            Content = "Blocked",
            Href = "/blocked",
            Command = blockedCommand
        };
        var breadcrumb = new CodexBreadcrumb
        {
            Content = new CodexBreadcrumbList
            {
                Items =
                {
                    new CodexBreadcrumbItem { Content = docs },
                    new CodexBreadcrumbSeparator(),
                    new CodexBreadcrumbItem { Content = components },
                    new CodexBreadcrumbSeparator(),
                    new CodexBreadcrumbItem { Content = api },
                    new CodexBreadcrumbSeparator(),
                    new CodexBreadcrumbItem { Content = blocked },
                    new CodexBreadcrumbSeparator(),
                    new CodexBreadcrumbItem { IsCurrent = true, Content = current }
                }
            }
        };
        var activations = new List<CodexBreadcrumbLinkActivatedEventArgs>();
        breadcrumb.LinkActivated += (_, args) => activations.Add(args);

        Assert.False(docs.TryHandlePointerActivation(PointerUpdateKind.RightButtonReleased));
        Assert.False(docs.TryHandlePointerActivation(PointerUpdateKind.MiddleButtonReleased));
        Assert.Equal(0, routeCount);
        Assert.Empty(activations);

        Assert.True(docs.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));

        Assert.Equal(1, routeCount);
        var pointerActivation = Assert.Single(activations);
        Assert.Same(docs, pointerActivation.Link);
        Assert.Equal("/docs", pointerActivation.Href);
        Assert.Equal(CodexBreadcrumbLinkActivationSource.Pointer, pointerActivation.Source);

        InvokeButtonClick(components);

        Assert.Equal(2, routeCount);
        Assert.Equal(2, activations.Count);
        Assert.Same(components, activations[1].Link);
        Assert.Equal(CodexBreadcrumbLinkActivationSource.Keyboard, activations[1].Source);

        Assert.True(api.TryActivate());

        Assert.Equal(3, routeCount);
        Assert.Equal(3, activations.Count);
        Assert.Same(api, activations[2].Link);
        Assert.Equal(CodexBreadcrumbLinkActivationSource.Programmatic, activations[2].Source);

        Assert.False(current.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.False(blocked.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.Equal(3, routeCount);
        Assert.Equal(3, activations.Count);
        Assert.Contains("command-blocked", blocked.Classes);
    }

    [Fact]
    public void KbdGroupMirrorsWebGroupedShortcutComposition()
    {
        var group = new CodexKbdGroup
        {
            Size = CodexControlSize.Small,
            Items =
            {
                new CodexKbd { Content = "Ctrl" },
                new CodexKbd { Content = "B" }
            }
        };

        Assert.Contains("kbd-group", group.Classes);
        Assert.Contains("size-sm", group.Classes);
        Assert.Contains("has-items", group.Classes);
        Assert.DoesNotContain("empty", group.Classes);

        group.Size = CodexControlSize.Large;
        Assert.Contains("size-lg", group.Classes);
        Assert.DoesNotContain("size-sm", group.Classes);

        group.Items.Clear();
        Assert.Contains("empty", group.Classes);
        Assert.DoesNotContain("has-items", group.Classes);
    }

    [Fact]
    public void NavigationMenuKeyboardActivationMovementAndEscapeMirrorWebTriggers()
    {
        var changes = new List<(CodexNavigationMenuItem? OldItem, CodexNavigationMenuItem? NewItem, string? Value)>();
        var overview = new CodexNavigationMenuItem { Header = "Overview", Value = "overview", Content = "Overview panel", ViewportWidth = 320 };
        var disabled = new CodexNavigationMenuItem { Header = "Disabled", Content = "Disabled panel", IsEnabled = false };
        var advanced = new CodexNavigationMenuItem { Header = "Advanced", Value = "advanced", Content = "Advanced panel", ViewportWidth = 480 };
        var topLevelLinkActivations = 0;
        var topLevelLinkParameters = new List<object?>();
        var docs = new CodexNavigationMenuItem
        {
            Header = "Docs",
            Value = "docs",
            Command = new TestCommand(() => topLevelLinkActivations++),
            CommandParameter = "docs-route"
        };
        docs.Activated += (_, args) => topLevelLinkParameters.Add(args.CommandParameter);
        var menu = new CodexNavigationMenu
        {
            ItemsSource = new[] { overview, disabled, advanced, docs }
        };
        menu.ActiveItemChanged += (_, args) => changes.Add((args.OldItem, args.NewItem, args.Value));

        Assert.True(overview.TryHandleActivationKey(Key.Enter, menu));
        Assert.True(menu.IsViewportOpen);
        Assert.Same(overview, menu.ActiveItem);
        Assert.Equal("overview", menu.ActiveValue);
        Assert.Equal("Overview panel", menu.ViewportContent);
        Assert.True(overview.IsOpen);
        Assert.Contains("open", menu.Classes);
        Assert.Contains("open", overview.Classes);
        var firstChange = Assert.Single(changes);
        Assert.Null(firstChange.OldItem);
        Assert.Same(overview, firstChange.NewItem);
        Assert.Equal("overview", firstChange.Value);

        Assert.True(menu.TryHandleItemNavigationKey(overview, Key.Right, moveFocus: false));
        Assert.Same(advanced, menu.ActiveItem);
        Assert.Equal("advanced", menu.ActiveValue);
        Assert.False(overview.IsOpen);
        Assert.True(advanced.IsOpen);
        Assert.False(menu.IsMotionReversed);
        Assert.Equal(480, menu.ViewportWidth);

        Assert.True(menu.TryHandleItemNavigationKey(advanced, Key.Left, moveFocus: false));
        Assert.Same(overview, menu.ActiveItem);
        Assert.True(menu.IsMotionReversed);
        Assert.False(menu.TryHandleItemNavigationKey(overview, Key.Down, moveFocus: false));

        menu.ActiveValue = "advanced";
        Assert.Same(advanced, menu.ActiveItem);
        Assert.Equal("advanced", menu.ActiveValue);

        Assert.True(docs.TryHandleActivationKey(Key.Enter, menu));
        Assert.Equal(1, topLevelLinkActivations);
        Assert.Equal(["docs-route"], topLevelLinkParameters);
        Assert.Contains("can-activate", docs.Classes);
        Assert.False(menu.IsViewportOpen);

        menu.ActivateItem(overview);
        Assert.True(menu.TryHandleNavigationKey(Key.Escape));
        Assert.False(menu.IsViewportOpen);
        Assert.Null(menu.ActiveItem);
        Assert.Null(menu.ActiveValue);
        Assert.False(overview.IsOpen);
        Assert.Contains("closed", menu.Classes);

        var linkActivations = 0;
        var linkParameters = new List<object?>();
        var contentLink = new CodexNavigationMenuLink
        {
            Content = "Forms",
            Command = new TestCommand(() => linkActivations++),
            CommandParameter = "forms-route"
        };
        contentLink.Activated += (_, args) => linkParameters.Add(args.CommandParameter);

        Assert.True(contentLink.TryActivate());
        Assert.Equal(1, linkActivations);
        Assert.Equal(["forms-route"], linkParameters);
        Assert.Contains("can-activate", contentLink.Classes);

        var top = new CodexNavigationMenuItem { Header = "Top", Content = "Top panel" };
        var bottom = new CodexNavigationMenuItem { Header = "Bottom", Content = "Bottom panel" };
        var verticalMenu = new CodexNavigationMenu
        {
            Orientation = Orientation.Vertical,
            ItemsSource = new[] { top, bottom }
        };

        verticalMenu.ActivateItem(top);
        Assert.True(verticalMenu.TryHandleItemNavigationKey(top, Key.Down, moveFocus: false));
        Assert.Same(bottom, verticalMenu.ActiveItem);
        Assert.True(verticalMenu.TryHandleItemNavigationKey(bottom, Key.Home, moveFocus: false));
        Assert.Same(top, verticalMenu.ActiveItem);
    }

    [Fact]
    public void NavigationMenuTopLevelPointerActivationUsesPrimaryRelease()
    {
        var activations = 0;
        var parameters = new List<object?>();
        var overview = new CodexNavigationMenuItem { Header = "Overview", Value = "overview", Content = "Overview panel" };
        var command = new TestCommand(() => activations++);
        var docs = new CodexNavigationMenuItem
        {
            Header = "Docs",
            Value = "docs",
            Command = command,
            CommandParameter = "docs-route"
        };
        docs.Activated += (_, args) => parameters.Add(args.CommandParameter);
        var disabled = new CodexNavigationMenuItem { Header = "Disabled", Content = "Disabled panel", IsEnabled = false };
        var menu = new CodexNavigationMenu
        {
            ItemsSource = new[] { overview, docs, disabled }
        };

        menu.ActivateItem(overview);
        Assert.True(menu.IsViewportOpen);

        Assert.False(docs.TryHandlePointerRelease(PointerUpdateKind.RightButtonReleased, menu));
        Assert.False(docs.TryHandlePointerRelease(PointerUpdateKind.MiddleButtonReleased, menu));
        Assert.Equal(0, activations);
        Assert.Empty(parameters);
        Assert.True(menu.IsViewportOpen);
        Assert.Same(overview, menu.ActiveItem);

        Assert.True(docs.TryHandlePointerRelease(PointerUpdateKind.LeftButtonReleased, menu));
        Assert.Equal(1, activations);
        Assert.Equal(["docs-route"], parameters);
        Assert.False(menu.IsViewportOpen);
        Assert.Null(menu.ActiveItem);

        Assert.True(overview.TryHandlePointerRelease(PointerUpdateKind.LeftButtonReleased, menu));
        Assert.True(menu.IsViewportOpen);
        Assert.Same(overview, menu.ActiveItem);
        Assert.Equal("overview", menu.ActiveValue);

        command.CanExecuteValue = false;
        command.RaiseCanExecuteChanged();

        Assert.Contains("command-blocked", docs.Classes);
        Assert.DoesNotContain("can-activate", docs.Classes);
        Assert.False(docs.TryHandlePointerRelease(PointerUpdateKind.LeftButtonReleased, menu));
        Assert.Equal(1, activations);
        Assert.Equal(["docs-route"], parameters);
        Assert.True(menu.IsViewportOpen);
        Assert.Same(overview, menu.ActiveItem);
        Assert.False(docs.TryHandleActivationKey(Key.Enter, menu));
        Assert.Same(overview, menu.ActiveItem);

        Assert.False(disabled.TryHandlePointerRelease(PointerUpdateKind.LeftButtonReleased, menu));
        Assert.Same(overview, menu.ActiveItem);
    }

    [Fact]
    public void NavigationMenuContentLinkPointerActivationUsesPrimaryRelease()
    {
        var activations = 0;
        var parameters = new List<object?>();
        var command = new TestCommand(() => activations++);
        var link = new CodexNavigationMenuLink
        {
            Content = "Forms",
            Command = command,
            CommandParameter = "forms-route"
        };
        link.Activated += (_, args) => parameters.Add(args.CommandParameter);

        Assert.False(link.TryHandlePointerActivation(PointerUpdateKind.RightButtonReleased));
        Assert.False(link.TryHandlePointerActivation(PointerUpdateKind.MiddleButtonReleased));
        Assert.Equal(0, activations);
        Assert.Empty(parameters);

        Assert.True(link.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.Equal(1, activations);
        Assert.Equal(["forms-route"], parameters);
        Assert.Contains("can-activate", link.Classes);

        command.CanExecuteValue = false;
        command.RaiseCanExecuteChanged();

        Assert.False(link.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.Equal(1, activations);
        Assert.Equal(["forms-route"], parameters);
        Assert.Contains("command-blocked", link.Classes);

        link.Command = null;
        Assert.True(link.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.Equal(["forms-route", "forms-route"], parameters);

        link.IsEnabled = false;
        Assert.False(link.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.Equal(["forms-route", "forms-route"], parameters);
    }

    [Fact]
    public void MenubarTriggerNavigationAndMenuStatesMirrorWebMenubar()
    {
        var opened = new List<(CodexMenubarItem? OldMenu, CodexMenubarItem? NewMenu)>();
        var file = new CodexMenubarMenu
        {
            Header = "File",
            Items =
            {
                new CodexMenubarItem { Header = "New tab", Shortcut = "Cmd+T", IsActive = true },
                new CodexMenubarCheckboxItem { Header = "Compact mode", IsChecked = true },
                new CodexMenubarRadioItem { Header = "Balanced", IsChecked = true },
                new CodexMenubarItem
                {
                    Header = "Share",
                    Items =
                    {
                        new CodexMenubarItem { Header = "Copy link" }
                    }
                }
            }
        };
        var disabled = new CodexMenubarMenu
        {
            Header = "Disabled",
            IsEnabled = false,
            Items =
            {
                new CodexMenubarItem { Header = "Locked" }
            }
        };
        var view = new CodexMenubarMenu
        {
            Header = "View",
            Items =
            {
                new CodexMenubarItem { Header = "Reload" }
            }
        };
        var menubar = new CodexMenubar
        {
            Loop = true,
            Items =
            {
                file,
                disabled,
                view
            }
        };
        menubar.ActiveMenuChanged += (_, args) => opened.Add((args.OldMenu, args.NewMenu));

        Assert.Contains("menubar", menubar.Classes);
        Assert.Contains("horizontal", menubar.Classes);
        Assert.Contains("loop", menubar.Classes);
        Assert.Contains("top-level", file.Classes);
        Assert.Contains("menubar-menu", file.Classes);
        Assert.Contains("menubar-checkbox-item", file.Items.OfType<CodexMenubarCheckboxItem>().Single().Classes);
        Assert.Contains("is-checked", file.Items.OfType<CodexMenubarCheckboxItem>().Single().Classes);
        Assert.Contains("menubar-radio-item", file.Items.OfType<CodexMenubarRadioItem>().Single().Classes);
        Assert.Contains("has-items", file.Items.OfType<CodexMenubarItem>().Last().Classes);

        Assert.True(menubar.OpenMenu(file));
        Assert.True(menubar.IsOpen);
        Assert.True(file.IsSubMenuOpen);
        Assert.Same(file, menubar.ActiveMenu);
        Assert.Contains("open", menubar.Classes);
        Assert.Contains("active-menu", file.Classes);

        Assert.True(menubar.TryHandleTopLevelNavigationKey(file, Key.Right, moveFocus: false));
        Assert.Same(view, menubar.ActiveMenu);
        Assert.False(file.IsSubMenuOpen);
        Assert.True(view.IsSubMenuOpen);

        Assert.True(menubar.TryHandleTopLevelNavigationKey(view, Key.Left, moveFocus: false));
        Assert.Same(file, menubar.ActiveMenu);

        Assert.True(menubar.TryHandleTopLevelNavigationKey(file, Key.End, moveFocus: false));
        Assert.Same(view, menubar.ActiveMenu);
        Assert.True(menubar.TryHandleTopLevelNavigationKey(view, Key.Escape, moveFocus: false));
        Assert.False(menubar.IsOpen);
        Assert.Null(menubar.ActiveMenu);
        Assert.Contains("closed", menubar.Classes);
        Assert.Contains(opened, change => ReferenceEquals(change.NewMenu, file));
        Assert.Contains(opened, change => change.NewMenu is null);

        var pointerFile = new CodexMenubarMenu
        {
            Header = "File",
            Items =
            {
                new CodexMenubarItem { Header = "New tab" }
            }
        };
        var pointerMenu = new CodexMenubar
        {
            Items =
            {
                pointerFile
            }
        };

        Assert.False(pointerMenu.TryHandleTopLevelPointerRelease(pointerFile, PointerUpdateKind.RightButtonReleased));
        Assert.False(pointerMenu.IsOpen);
        Assert.False(pointerFile.IsSubMenuOpen);

        Assert.False(pointerMenu.TryHandleTopLevelPointerRelease(pointerFile, PointerUpdateKind.MiddleButtonReleased));
        Assert.False(pointerMenu.IsOpen);
        Assert.False(pointerFile.IsSubMenuOpen);

        Assert.True(pointerMenu.TryHandleTopLevelPointerRelease(pointerFile, PointerUpdateKind.LeftButtonReleased));
        Assert.True(pointerMenu.IsOpen);
        Assert.True(pointerFile.IsSubMenuOpen);

        Assert.False(pointerMenu.TryHandleTopLevelPointerRelease(pointerFile, PointerUpdateKind.RightButtonReleased));
        Assert.True(pointerMenu.IsOpen);
        Assert.True(pointerFile.IsSubMenuOpen);

        Assert.True(pointerMenu.TryHandleTopLevelPointerRelease(pointerFile, PointerUpdateKind.LeftButtonReleased));
        Assert.False(pointerMenu.IsOpen);
        Assert.False(pointerFile.IsSubMenuOpen);

        pointerMenu.IsLoading = true;
        Assert.False(pointerMenu.TryHandleTopLevelPointerRelease(pointerFile, PointerUpdateKind.LeftButtonReleased));
        Assert.False(pointerMenu.IsOpen);

        var top = new CodexMenubarMenu
        {
            Header = "Top",
            Items =
            {
                new CodexMenubarItem { Header = "First" }
            }
        };
        var bottom = new CodexMenubarMenu
        {
            Header = "Bottom",
            Items =
            {
                new CodexMenubarItem { Header = "Second" }
            }
        };
        var vertical = new CodexMenubar
        {
            Orientation = Orientation.Vertical,
            Items =
            {
                top,
                bottom
            }
        };

        Assert.True(vertical.OpenMenu(top));
        Assert.True(vertical.TryHandleTopLevelNavigationKey(top, Key.Down, moveFocus: false));
        Assert.Same(bottom, vertical.ActiveMenu);
        Assert.False(vertical.TryHandleTopLevelNavigationKey(bottom, Key.Left, moveFocus: false));

        var loadingFile = new CodexMenubarMenu
        {
            Header = "File",
            Items =
            {
                new CodexMenubarItem { Header = "Blocked" }
            }
        };
        var loading = new CodexMenubar
        {
            IsLoading = true,
            Items =
            {
                loadingFile
            }
        };

        Assert.False(loading.OpenMenu(loadingFile));
        Assert.True(loading.TryHandleTopLevelNavigationKey(loadingFile, Key.Enter, moveFocus: false));
        Assert.False(loading.IsOpen);
        Assert.Contains("loading", loading.Classes);
    }

    [Fact]
    public void DropdownButtonTriggerKeysOpenLikeWebMenuTrigger()
    {
        var changes = new List<bool>();
        var dropdown = new CodexDropdownButton
        {
            Content = "Provider actions",
            DropDownContent = new CodexButton { Content = "Rename" },
            IsArrowVisible = true
        };
        dropdown.OpenChanged += (_, args) => changes.Add(args.IsOpen);

        Assert.True(dropdown.HasDropDownContent);
        Assert.Contains("closed", dropdown.Classes);
        Assert.True(dropdown.TryHandleTriggerKey(Key.Enter));
        Assert.True(dropdown.IsOpen);
        Assert.Equal([true], changes);

        Assert.True(dropdown.TryHandleDismissKey(Key.Escape));
        Assert.False(dropdown.IsOpen);
        Assert.Equal([true, false], changes);

        Assert.True(dropdown.TryHandleTriggerKey(Key.Space));
        Assert.True(dropdown.IsOpen);
        Assert.Equal([true, false, true], changes);

        Assert.True(dropdown.TryHandleDismissKey(Key.Escape));
        Assert.True(dropdown.TryHandleTriggerKey(Key.Down));
        Assert.True(dropdown.IsOpen);
        Assert.Equal([true, false, true, false, true], changes);

        Assert.False(dropdown.TryHandleTriggerKey(Key.Up));
        Assert.True(dropdown.IsOpen);

        Assert.True(dropdown.TryHandleDismissKey(Key.Escape));
        Assert.False(dropdown.TryHandleTriggerPointerRelease(PointerUpdateKind.RightButtonReleased));
        Assert.False(dropdown.TryHandleTriggerPointerRelease(PointerUpdateKind.MiddleButtonReleased));
        Assert.False(dropdown.IsOpen);

        Assert.True(dropdown.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(dropdown.IsOpen);

        dropdown.IsLoading = true;
        dropdown.IsOpen = false;
        Assert.False(dropdown.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.False(dropdown.TryHandleTriggerKey(Key.Enter));
        Assert.False(dropdown.IsOpen);
    }

    [Fact]
    public void CollapsibleTriggerKeysToggleOpenStateLikeWebDisclosureTrigger()
    {
        var changes = new List<(bool IsOpen, CodexCollapsibleOpenChangeSource Source)>();
        var collapsible = new CodexCollapsible
        {
            Header = "Repository",
            Content = "Branches",
            AnimationDuration = TimeSpan.Zero
        };
        collapsible.OpenChanged += (_, args) => changes.Add((args.IsOpen, args.Source));

        Assert.False(collapsible.IsOpen);
        Assert.True(collapsible.TryHandleTriggerKey(Key.Enter));
        Assert.True(collapsible.IsOpen);
        Assert.Equal([(true, CodexCollapsibleOpenChangeSource.Keyboard)], changes);
        Assert.Contains("open", collapsible.Classes);

        Assert.True(collapsible.TryHandleTriggerKey(Key.Space));
        Assert.False(collapsible.IsOpen);
        Assert.Equal(
            [
                (true, CodexCollapsibleOpenChangeSource.Keyboard),
                (false, CodexCollapsibleOpenChangeSource.Keyboard)
            ],
            changes);
        Assert.Contains("closed", collapsible.Classes);
        Assert.False(collapsible.TryHandleTriggerKey(Key.Escape));
        Assert.Equal(
            [
                (true, CodexCollapsibleOpenChangeSource.Keyboard),
                (false, CodexCollapsibleOpenChangeSource.Keyboard)
            ],
            changes);

        collapsible.IsOpen = true;
        Assert.Equal(
            [
                (true, CodexCollapsibleOpenChangeSource.Keyboard),
                (false, CodexCollapsibleOpenChangeSource.Keyboard),
                (true, CodexCollapsibleOpenChangeSource.Programmatic)
            ],
            changes);

        collapsible.IsEnabled = false;
        collapsible.IsOpen = false;
        Assert.Equal(
            [
                (true, CodexCollapsibleOpenChangeSource.Keyboard),
                (false, CodexCollapsibleOpenChangeSource.Keyboard),
                (true, CodexCollapsibleOpenChangeSource.Programmatic),
                (false, CodexCollapsibleOpenChangeSource.Programmatic)
            ],
            changes);
        Assert.False(collapsible.TryHandleTriggerKey(Key.Enter));
        Assert.False(collapsible.IsOpen);
        Assert.Equal(
            [
                (true, CodexCollapsibleOpenChangeSource.Keyboard),
                (false, CodexCollapsibleOpenChangeSource.Keyboard),
                (true, CodexCollapsibleOpenChangeSource.Programmatic),
                (false, CodexCollapsibleOpenChangeSource.Programmatic)
            ],
            changes);
    }

    [Fact]
    public void CollapsibleAndAccordionTriggersOnlyUsePrimaryPointerRelease()
    {
        var changes = new List<(bool IsOpen, CodexCollapsibleOpenChangeSource Source)>();
        var collapsible = new CodexCollapsible
        {
            Header = "Repository",
            Content = "Branches",
            AnimationDuration = TimeSpan.Zero
        };
        collapsible.OpenChanged += (_, args) => changes.Add((args.IsOpen, args.Source));

        Assert.False(collapsible.TryHandleTriggerPointerRelease(PointerUpdateKind.RightButtonReleased));
        Assert.False(collapsible.IsOpen);
        Assert.Empty(changes);

        Assert.False(collapsible.TryHandleTriggerPointerRelease(PointerUpdateKind.MiddleButtonReleased));
        Assert.False(collapsible.IsOpen);
        Assert.Empty(changes);

        Assert.True(collapsible.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(collapsible.IsOpen);
        Assert.Equal([(true, CodexCollapsibleOpenChangeSource.Pointer)], changes);

        collapsible.IsEnabled = false;
        Assert.False(collapsible.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(collapsible.IsOpen);
        Assert.Equal([(true, CodexCollapsibleOpenChangeSource.Pointer)], changes);

        var accordionChanges = new List<CodexAccordionValueChangedEventArgs>();
        var routing = new CodexAccordionItem { Value = "routing", Header = "Routing" };
        var billing = new CodexAccordionItem { Value = "billing", Header = "Billing" };
        var accordion = new CodexAccordion
        {
            IsCollapsible = true,
            Items =
            {
                routing,
                billing
            }
        };
        accordion.ValueChanged += (_, args) => accordionChanges.Add(args);

        Assert.False(billing.TryHandleTriggerPointerRelease(PointerUpdateKind.RightButtonReleased));
        Assert.False(billing.IsOpen);
        Assert.Empty(accordionChanges);

        Assert.True(billing.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(billing.IsOpen);
        Assert.Single(accordionChanges);
        Assert.Equal(CodexAccordionValueChangeSource.Trigger, accordionChanges[0].Source);
        Assert.Same(billing, accordionChanges[0].ChangedItem);

        accordion.IsEnabled = false;
        Assert.False(billing.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(billing.IsOpen);
        Assert.Single(accordionChanges);
    }

    [Fact]
    public void AccordionSingleMultipleAndRovingKeysMirrorWebAccordion()
    {
        var changes = new List<CodexAccordionValueChangedEventArgs>();
        var routing = new CodexAccordionItem { Value = "routing", Header = "Routing" };
        var billing = new CodexAccordionItem { Value = "billing", Header = "Billing" };
        var disabled = new CodexAccordionItem { Value = "audit", Header = "Audit", IsEnabled = false };
        var accordion = new CodexAccordion
        {
            IsCollapsible = false,
            Items =
            {
                routing,
                billing,
                disabled
            }
        };
        routing.IsOpen = true;
        Assert.Equal(["routing"], accordion.OpenValues);

        accordion.ValueChanged += (_, args) => changes.Add(args);

        Assert.True(billing.TryHandleTriggerKey(Key.Enter));
        Assert.False(routing.IsOpen);
        Assert.True(billing.IsOpen);
        Assert.Equal(["billing"], accordion.OpenValues);
        Assert.Contains("type-single", accordion.Classes);
        Assert.Contains("non-collapsible", accordion.Classes);
        Assert.Contains("open", billing.Classes);
        Assert.Contains("closed", routing.Classes);
        Assert.Single(changes);
        Assert.Equal(["routing"], changes[0].OldValues);
        Assert.Equal(["billing"], changes[0].NewValues);
        Assert.Equal("routing", changes[0].OldValue);
        Assert.Equal("billing", changes[0].NewValue);
        Assert.Same(billing, changes[0].ChangedItem);
        Assert.Equal("billing", changes[0].ChangedValue);
        Assert.Equal(1, changes[0].ChangedIndex);
        Assert.Equal(CodexAccordionValueChangeSource.Keyboard, changes[0].Source);
        Assert.True(changes[0].IsOpen);

        Assert.True(billing.TryHandleTriggerKey(Key.Space));
        Assert.True(billing.IsOpen);
        Assert.Equal(["billing"], accordion.OpenValues);
        Assert.Single(changes);

        accordion.IsCollapsible = true;
        Assert.True(billing.TryHandleTriggerKey(Key.Space));
        Assert.False(billing.IsOpen);
        Assert.Empty(accordion.OpenValues);
        Assert.Contains("collapsible", accordion.Classes);
        Assert.Equal(2, changes.Count);
        Assert.Equal(["billing"], changes[1].OldValues);
        Assert.Empty(changes[1].NewValues);
        Assert.Same(billing, changes[1].ChangedItem);
        Assert.Equal("billing", changes[1].ChangedValue);
        Assert.Equal(CodexAccordionValueChangeSource.Keyboard, changes[1].Source);
        Assert.False(changes[1].IsOpen);

        routing.IsOpen = true;
        Assert.Equal(["routing"], accordion.OpenValues);
        Assert.Equal(3, changes.Count);
        Assert.Same(routing, changes[2].ChangedItem);
        Assert.Equal("routing", changes[2].ChangedValue);
        Assert.Equal(0, changes[2].ChangedIndex);
        Assert.Equal(CodexAccordionValueChangeSource.Programmatic, changes[2].Source);
        Assert.True(changes[2].IsOpen);

        Assert.True(accordion.TryHandleItemNavigationKey(routing, Key.Down, moveFocus: false));
        Assert.True(accordion.TryHandleItemNavigationKey(billing, Key.Up, moveFocus: false));
        Assert.True(accordion.TryHandleItemNavigationKey(routing, Key.End, moveFocus: false));
        Assert.False(accordion.TryHandleItemNavigationKey(routing, Key.Left, moveFocus: false));

        accordion.Orientation = Orientation.Horizontal;
        Assert.True(accordion.TryHandleItemNavigationKey(routing, Key.Right, moveFocus: false));
        Assert.False(accordion.TryHandleItemNavigationKey(routing, Key.Down, moveFocus: false));
        Assert.Contains("horizontal", accordion.Classes);

        Assert.False(disabled.TryHandleTriggerKey(Key.Enter));
        Assert.False(disabled.IsOpen);

        accordion.IsEnabled = false;
        Assert.False(routing.TryHandleTriggerKey(Key.Enter));
        Assert.True(routing.IsOpen);
        accordion.IsEnabled = true;

        var multipleA = new CodexAccordionItem { Value = "a", Header = "A" };
        var multipleB = new CodexAccordionItem { Value = "b", Header = "B" };
        var multiple = new CodexAccordion
        {
            Type = CodexAccordionType.Multiple,
            Items =
            {
                multipleA,
                multipleB
            }
        };
        var multipleChanges = new List<CodexAccordionValueChangedEventArgs>();
        multiple.ValueChanged += (_, args) => multipleChanges.Add(args);

        Assert.True(multipleA.TryHandleTriggerKey(Key.Enter));
        Assert.True(multipleB.TryHandleTriggerKey(Key.Enter));
        Assert.True(multipleA.IsOpen);
        Assert.True(multipleB.IsOpen);
        Assert.Equal(["a", "b"], multiple.OpenValues);
        Assert.Contains("type-multiple", multiple.Classes);
        Assert.Equal(2, multipleChanges.Count);
        Assert.Equal(CodexAccordionValueChangeSource.Keyboard, multipleChanges[0].Source);
        Assert.Same(multipleA, multipleChanges[0].ChangedItem);
        Assert.Equal("a", multipleChanges[0].ChangedValue);
        Assert.True(multipleChanges[0].IsOpen);
        Assert.Equal(["a"], multipleChanges[1].OldValues);
        Assert.Equal(["a", "b"], multipleChanges[1].NewValues);
        Assert.Same(multipleB, multipleChanges[1].ChangedItem);
        Assert.Equal("b", multipleChanges[1].ChangedValue);
        Assert.True(multipleChanges[1].IsOpen);
    }

    [Fact]
    public void PaginationMirrorsWebPageSelectionAndBoundaryNavigation()
    {
        var changed = new List<CodexPaginationPageChangedEventArgs>();
        var pagination = new CodexPagination
        {
            Page = 4,
            PageCount = 9,
            SiblingCount = 1,
            BoundaryCount = 1,
            Size = CodexControlSize.Small
        };
        pagination.PageChanged += (_, args) => changed.Add(args);

        Assert.Equal(4, pagination.Page);
        Assert.Contains("size-sm", pagination.Classes);
        Assert.Contains("has-ellipsis", pagination.Classes);
        Assert.Contains("can-previous", pagination.Classes);
        Assert.Contains("can-next", pagination.Classes);
        Assert.Contains(pagination.PageItems, item => item.IsCurrent && item.Page == 4);
        Assert.Contains(pagination.PageItems, item => item.IsEllipsis);

        Assert.True(pagination.GoNext());
        Assert.Equal(5, pagination.Page);
        Assert.True(pagination.GoPrevious());
        Assert.Equal(4, pagination.Page);
        Assert.True(pagination.TryHandleNavigationKey(Key.End));
        Assert.Equal(9, pagination.Page);
        Assert.Contains("last-page", pagination.Classes);
        Assert.False(pagination.GoNext());

        Assert.True(pagination.TryHandleNavigationKey(Key.Home));
        Assert.Equal(1, pagination.Page);
        Assert.Contains("first-page", pagination.Classes);
        Assert.False(pagination.GoPrevious());

        Assert.False(pagination.SelectPage(1));
        Assert.True(pagination.SelectPage(6));
        Assert.Equal(6, pagination.Page);
        Assert.True(pagination.SelectPage(7, CodexPaginationPageChangeSource.PageItem));
        Assert.Equal(7, pagination.Page);

        AssertPageChanged(changed[0], 4, 5, CodexPaginationPageChangeSource.Next);
        AssertPageChanged(changed[1], 5, 4, CodexPaginationPageChangeSource.Previous);
        AssertPageChanged(changed[2], 4, 9, CodexPaginationPageChangeSource.Keyboard);
        AssertPageChanged(changed[3], 9, 1, CodexPaginationPageChangeSource.Keyboard);
        AssertPageChanged(changed[4], 1, 6, CodexPaginationPageChangeSource.Programmatic);
        AssertPageChanged(changed[5], 6, 7, CodexPaginationPageChangeSource.PageItem);

        pagination.IsLoading = true;
        Assert.False(pagination.SelectPage(8));
        Assert.False(pagination.TryHandleNavigationKey(Key.Right));
        Assert.Equal(7, pagination.Page);
        Assert.Contains("loading", pagination.Classes);
        Assert.Equal(6, changed.Count);
    }

    [Fact]
    public void PaginationActionPointerReleaseUsesPrimaryButtonOnly()
    {
        var changed = new List<CodexPaginationPageChangedEventArgs>();
        var pagination = new CodexPagination
        {
            Page = 3,
            PageCount = 5
        };
        pagination.PageChanged += (_, args) => changed.Add(args);

        Assert.False(pagination.TryHandleActionPointerRelease(PointerUpdateKind.RightButtonReleased, CodexPaginationPageChangeSource.Next));
        Assert.False(pagination.TryHandleActionPointerRelease(PointerUpdateKind.MiddleButtonReleased, CodexPaginationPageChangeSource.Next));
        Assert.Equal(3, pagination.Page);
        Assert.Empty(changed);

        Assert.True(pagination.TryHandleActionPointerRelease(PointerUpdateKind.LeftButtonReleased, CodexPaginationPageChangeSource.Next));
        Assert.Equal(4, pagination.Page);
        Assert.True(pagination.TryHandleActionPointerRelease(PointerUpdateKind.LeftButtonReleased, CodexPaginationPageChangeSource.Previous));
        Assert.Equal(3, pagination.Page);
        Assert.True(pagination.TryHandleActionPointerRelease(PointerUpdateKind.LeftButtonReleased, CodexPaginationPageChangeSource.First));
        Assert.Equal(1, pagination.Page);
        Assert.True(pagination.TryHandleActionPointerRelease(PointerUpdateKind.LeftButtonReleased, CodexPaginationPageChangeSource.Last));
        Assert.Equal(5, pagination.Page);

        AssertPageChanged(changed[0], 3, 4, CodexPaginationPageChangeSource.Next);
        AssertPageChanged(changed[1], 4, 3, CodexPaginationPageChangeSource.Previous);
        AssertPageChanged(changed[2], 3, 1, CodexPaginationPageChangeSource.First);
        AssertPageChanged(changed[3], 1, 5, CodexPaginationPageChangeSource.Last);

        Assert.False(pagination.TryHandleActionPointerRelease(PointerUpdateKind.LeftButtonReleased, CodexPaginationPageChangeSource.Next));
        Assert.False(pagination.TryHandleActionPointerRelease(PointerUpdateKind.LeftButtonReleased, CodexPaginationPageChangeSource.Last));
        Assert.Equal(5, pagination.Page);

        pagination.IsLoading = true;

        Assert.False(pagination.TryHandleActionPointerRelease(PointerUpdateKind.LeftButtonReleased, CodexPaginationPageChangeSource.Previous));
        Assert.Equal(5, pagination.Page);
        Assert.Equal(4, changed.Count);
    }

    [Fact]
    public void PaginationActionKeyboardActivationUsesEnterAndSpaceOnly()
    {
        var changed = new List<CodexPaginationPageChangedEventArgs>();
        var pagination = new CodexPagination
        {
            Page = 2,
            PageCount = 4
        };
        pagination.PageChanged += (_, args) => changed.Add(args);

        Assert.False(pagination.TryHandleActionKey(Key.Tab, CodexPaginationPageChangeSource.Next));
        Assert.Equal(2, pagination.Page);

        Assert.True(pagination.TryHandleActionKey(Key.Enter, CodexPaginationPageChangeSource.Next));
        Assert.Equal(3, pagination.Page);
        Assert.True(pagination.TryHandleActionKey(Key.Space, CodexPaginationPageChangeSource.Previous));
        Assert.Equal(2, pagination.Page);

        pagination.IsEnabled = false;

        Assert.False(pagination.TryHandleActionKey(Key.Enter, CodexPaginationPageChangeSource.Next));
        Assert.Equal(2, pagination.Page);

        AssertPageChanged(changed[0], 2, 3, CodexPaginationPageChangeSource.Next);
        AssertPageChanged(changed[1], 3, 2, CodexPaginationPageChangeSource.Previous);
        Assert.Equal(2, changed.Count);
    }

    [Fact]
    public void CommandItemsExposeCommandBlockedStateAndSkipSelection()
    {
        var executions = 0;
        var selected = new List<CodexCommandItem>();
        var blockedCommand = new TestCommand(() => executions++)
        {
            CanExecuteValue = false
        };
        var blocked = new CodexCommandItem { Content = "Blocked action", Icon = "B" };
        var fallback = new CodexCommandItem { Content = "Fallback action", Icon = "F" };
        var palette = new CodexCommand
        {
            Content = new CodexCommandList
            {
                Items =
                {
                    new CodexCommandGroup
                    {
                        Header = "Actions",
                        Items =
                        {
                            blocked,
                            fallback
                        }
                    }
                }
            }
        };
        palette.ItemSelected += (_, args) => selected.Add(args.Item);
        blocked.Command = blockedCommand;

        Assert.False(blocked.CanSelect());
        Assert.Contains("command-blocked", blocked.Classes);
        Assert.DoesNotContain("can-select", blocked.Classes);

        InvokeButtonClick(blocked);

        Assert.Equal(0, executions);
        Assert.Empty(selected);
        Assert.Null(palette.SelectedItem);

        blocked.IsActive = true;
        fallback.IsActive = false;

        Assert.True(palette.TrySelectActiveItem());
        Assert.Same(fallback, palette.SelectedItem);
        Assert.Single(selected);
        Assert.Same(fallback, selected[0]);

        blocked.IsActive = true;
        fallback.IsActive = false;

        Assert.True(palette.TryHandleNavigationKey(Key.Home));
        Assert.False(blocked.IsActive);
        Assert.True(fallback.IsActive);

        blockedCommand.CanExecuteValue = true;
        blockedCommand.RaiseCanExecuteChanged();

        Assert.True(blocked.CanSelect());
        Assert.Contains("can-select", blocked.Classes);
        Assert.DoesNotContain("command-blocked", blocked.Classes);

        palette.IsLoading = true;

        Assert.False(blocked.CanSelect());
        Assert.DoesNotContain("can-select", blocked.Classes);
        Assert.DoesNotContain("command-blocked", blocked.Classes);
    }

    [Fact]
    public void PaginationPageButtonUsesCommandAndLoadingGuardsBeforeActivation()
    {
        var commandExecutions = 0;
        var command = new TestCommand(() => commandExecutions++);
        var pageButton = new CodexPaginationPageButton
        {
            Content = "2",
            Page = 2,
            Command = command
        };

        Assert.True(pageButton.CanActivate);
        Assert.Contains("can-activate", pageButton.Classes);

        InvokeButtonClick(pageButton);

        Assert.Equal(1, commandExecutions);

        command.CanExecuteValue = false;
        command.RaiseCanExecuteChanged();

        Assert.False(pageButton.CanActivate);
        Assert.Contains("command-blocked", pageButton.Classes);
        Assert.DoesNotContain("can-activate", pageButton.Classes);

        InvokeButtonClick(pageButton);

        Assert.Equal(1, commandExecutions);

        command.CanExecuteValue = true;
        command.RaiseCanExecuteChanged();
        pageButton.IsLoading = true;

        Assert.False(pageButton.CanActivate);

        InvokeButtonClick(pageButton);

        Assert.Equal(1, commandExecutions);

        pageButton.IsLoading = false;
        pageButton.IsCurrent = true;

        Assert.False(pageButton.CanActivate);

        InvokeButtonClick(pageButton);

        Assert.Equal(1, commandExecutions);

        var pagination = new CodexPagination { Page = 2, PageCount = 5 };

        Assert.True(pagination.CanSelectPageItem(3));

        pagination.IsLoading = true;

        Assert.False(pagination.CanSelectPageItem(3));
    }

    [Fact]
    public void PaginationNormalizesPageRangeAndEmptyState()
    {
        var pagination = new CodexPagination
        {
            PageCount = 0,
            Page = 10,
            SiblingCount = -1,
            BoundaryCount = -2
        };

        Assert.Equal(0, pagination.Page);
        Assert.Equal(0, pagination.SiblingCount);
        Assert.Equal(0, pagination.BoundaryCount);
        Assert.Empty(pagination.PageItems);
        Assert.False(pagination.CanGoPrevious);
        Assert.False(pagination.CanGoNext);
        Assert.Contains("empty", pagination.Classes);

        pagination.PageCount = 3;
        pagination.Page = 99;

        Assert.Equal(3, pagination.Page);
        Assert.Equal(3, pagination.PageItems.Count);
        Assert.Contains("last-page", pagination.Classes);
    }

    [Fact]
    public void ScrollAreaMirrorsWebViewportScrollbarAndBoundaryState()
    {
        var scrollArea = new CodexScrollArea
        {
            Type = CodexScrollAreaType.Hover,
            Size = CodexControlSize.Large,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
            IsInsetContent = true
        };

        scrollArea.SyncScrollMetricsForTests(
            new Vector(12, 24),
            new Size(320, 720),
            new Size(220, 300));

        Assert.Contains("type-hover", scrollArea.Classes);
        Assert.Contains("size-lg", scrollArea.Classes);
        Assert.Contains("horizontal-auto", scrollArea.Classes);
        Assert.Contains("vertical-visible", scrollArea.Classes);
        Assert.Contains("inset-content", scrollArea.Classes);
        Assert.Contains("can-scroll-x", scrollArea.Classes);
        Assert.Contains("can-scroll-y", scrollArea.Classes);
        Assert.DoesNotContain("at-start", scrollArea.Classes);
        Assert.DoesNotContain("at-top", scrollArea.Classes);
        Assert.False(scrollArea.IsAtBottom);

        scrollArea.SyncScrollMetricsForTests(
            new Vector(100, 420),
            new Size(320, 720),
            new Size(220, 300));

        Assert.True(scrollArea.IsAtEnd);
        Assert.True(scrollArea.IsAtBottom);
        Assert.Contains("at-end", scrollArea.Classes);
        Assert.Contains("at-bottom", scrollArea.Classes);

        scrollArea.Type = CodexScrollAreaType.Scroll;
        Assert.Contains("type-scroll", scrollArea.Classes);
        Assert.DoesNotContain("type-hover", scrollArea.Classes);
    }

    [Fact]
    public void NavigationAndDataControlsSyncStateClasses()
    {
        var tabs = new CodexTabs { Size = CodexControlSize.Large, Orientation = Orientation.Vertical, Variant = CodexTabsVariant.Line };
        var navigationMenu = new CodexNavigationMenu { Size = CodexControlSize.Small, Orientation = Orientation.Vertical };
        var navigationMenuItem = new CodexNavigationMenuItem { Header = "Docs", Content = "Panel", Icon = ">", IsOpen = true };
        var navigationMenuContent = new CodexNavigationMenuContent { Header = "Docs", Description = "Guides" };
        var navigationMenuLink = new CodexNavigationMenuLink { Content = "Introduction", Description = "Start here", Icon = ">", IsActive = true };
        var menu = new CodexMenu { Size = CodexControlSize.Small, IsLoading = true };
        var menuItem = new CodexMenuItem { IsActive = true, Shortcut = "Ctrl+N" };
        var contextMenu = new CodexContextMenu { Size = CodexControlSize.Small, IsLoading = true, Placement = PlacementMode.Right };
        var contextMenuItem = new CodexContextMenuItem
        {
            IsActive = true,
            Shortcut = "Ctrl+Shift+P",
            IsInset = true,
            ToggleType = MenuItemToggleType.Radio,
            IsChecked = true,
            SubMenuPlacement = PlacementMode.LeftEdgeAlignedTop
        };
        var contextMenuLabel = new CodexContextMenuLabel { IsInset = true };
        var contextMenuGroup = new CodexContextMenuGroup { Header = "View" };
        var menuGroup = new CodexMenuGroup { Header = "Components" };
        var command = new CodexCommand { IsLoading = true };
        var commandItem = new CodexCommandItem { IsActive = true, Icon = ">", Shortcut = "Ctrl+K" };
        var accordionItem = new CodexAccordionItem { Value = "repository", Header = "Repository", Content = "Branches" };
        var accordion = new CodexAccordion
        {
            Type = CodexAccordionType.Multiple,
            IsCollapsible = true,
            Size = CodexControlSize.Small,
            Items =
            {
                accordionItem
            }
        };
        accordionItem.IsOpen = true;
        var collapsible = new CodexCollapsible { Header = "Repository", Content = "Branches", IsOpen = true, Size = CodexControlSize.Large };
        var sidebarMenuButton = new CodexSidebarMenuButton
        {
            Content = "Home",
            Icon = ">",
            Badge = "3",
            IsActive = true,
            Size = CodexControlSize.Small
        };
        var sidebarMenuAction = new CodexSidebarMenuAction { IsActive = true, IsShowOnHover = true };
        var sidebarSubButton = new CodexSidebarMenuSubButton { IsActive = true };
        var table = new CodexTable { IsHoverable = false, IsStriped = true, IsCompact = true };
        var tableHeader = new CodexTableHeader();
        var tableBody = new CodexTableBody();
        var tableFooter = new CodexTableFooter();
        var row = new CodexTableRow { IsSelected = true };
        var head = new CodexTableHead { Alignment = CodexTableCellAlignment.Right };
        var cell = new CodexTableCell { Alignment = CodexTableCellAlignment.Center };
        var rankedBarChartChanges = new List<(int OldIndex, int NewIndex)>();
        var rankedBarChart = new CodexRankedBarChart
        {
            IsCompact = true,
            MaxVisibleItems = 4,
            ActiveIndex = 1,
            ItemsSource =
            [
                new CodexRankedBarChartItem("OpenAI", 12, "12", "$0.04"),
                new CodexRankedBarChartItem("Anthropic", 8, "8", "$0.02")
            ]
        };
        rankedBarChart.ActiveItemChanged += (_, args) => rankedBarChartChanges.Add((args.OldIndex, args.NewIndex));
        rankedBarChart.ActiveIndex = 1;
        var barChartChanges = new List<(int OldIndex, int NewIndex)>();
        var barChart = new CodexBarChart
        {
            IsCompact = true,
            Orientation = Orientation.Horizontal,
            ShowGridLines = false,
            ShowAxisLabels = false,
            ActiveIndex = 1,
            ItemsSource =
            [
                new CodexBarChartItem("OpenAI", 12, "12", "baseline"),
                new CodexBarChartItem("Anthropic", -4, "-4", "rollback")
            ]
        };
        barChart.ActiveItemChanged += (_, args) => barChartChanges.Add((args.OldIndex, args.NewIndex));
        barChart.ActiveIndex = 1;
        var lineChartChanges = new List<(int OldIndex, int NewIndex)>();
        var lineChart = new CodexLineChart
        {
            IsCompact = true,
            ShowArea = false,
            ShowDots = false,
            ActiveIndex = 1,
            ItemsSource =
            [
                new CodexLineChartPoint("Mon", 12, "12", "baseline"),
                new CodexLineChartPoint("Tue", 24, "24", "peak")
            ]
        };
        lineChart.ActivePointChanged += (_, args) => lineChartChanges.Add((args.OldIndex, args.NewIndex));
        lineChart.ActiveIndex = 1;
        var usagePieChartChanges = new List<(int OldIndex, int NewIndex)>();
        var usagePieChart = new CodexUsagePieChart
        {
            IsCompact = true,
            ActiveIndex = 1,
            TotalLabel = "Requests",
            TotalValue = "20",
            ItemsSource =
            [
                new CodexUsagePieChartItem("gpt-5.5", 12, "60%", "12 requests"),
                new CodexUsagePieChartItem("gpt-5.4", 8, "40%", "8 requests")
            ]
        };
        usagePieChart.ActiveItemChanged += (_, args) => usagePieChartChanges.Add((args.OldIndex, args.NewIndex));
        usagePieChart.ActiveIndex = 1;
        var avatar = new CodexAvatar { Size = CodexControlSize.Icon };
        var aspectRatio = new CodexAspectRatio
        {
            Ratio = 1,
            Size = CodexControlSize.Large,
            Content = "Square preview"
        };
        var card = new CodexCard { IsInteractive = true, Title = "Metrics", Description = "Updated", Content = "42", Footer = "Footer" };
        var itemRow = new CodexItem
        {
            IsInteractive = true,
            IsSelected = true,
            Title = "Primary route",
            Description = "OpenAI",
            Media = new CodexItemMedia { Content = "AI", Size = CodexControlSize.Small },
            Actions = new CodexButton { Content = "Open", Size = CodexControlSize.Small },
            Footer = "Active now"
        };
        var itemGroup = new CodexItemGroup { Variant = CodexControlVariant.Outline, IsInset = true, Items = { itemRow, new CodexItemSeparator() } };
        var separator = new CodexSeparator { Orientation = Orientation.Vertical, Size = CodexControlSize.Large };
        var text = new CodexText { Role = CodexTextRole.Code };
        var breadcrumb = new CodexBreadcrumb { Size = CodexControlSize.Large, Content = new CodexBreadcrumbList() };
        var breadcrumbLink = new CodexBreadcrumbLink { Content = "Docs", Href = "/docs" };
        var breadcrumbPage = new CodexBreadcrumbPage { Content = "Current" };
        var breadcrumbSeparator = new CodexBreadcrumbSeparator();
        var breadcrumbEllipsis = new CodexBreadcrumbEllipsis();
        var pagination = new CodexPagination { Page = 2, PageCount = 5, IsCompact = true, ShowFirstLast = false };
        var scrollArea = new CodexScrollArea { Type = CodexScrollAreaType.Always, IsInsetContent = true, Size = CodexControlSize.Small };
        var carousel = new CodexCarousel
        {
            Size = CodexControlSize.Large,
            Loop = true,
            SelectedIndex = 1,
            Items =
            {
                new CodexCarouselItem { Content = "One" },
                new CodexCarouselItem { Content = "Two" }
            }
        };
        var resizableHandle = new CodexResizableHandle { WithHandle = true };
        var resizable = new CodexResizablePanelGroup
        {
            Size = CodexControlSize.Small,
            Children =
            {
                new CodexResizablePanel { DefaultSize = 35, MinSize = 20 },
                resizableHandle,
                new CodexResizablePanel { DefaultSize = 65, MinSize = 30 }
            }
        };
        resizable.ResizeHandleByPercent(resizableHandle, 5);

        Assert.Contains("size-lg", tabs.Classes);
        Assert.Contains("vertical", tabs.Classes);
        Assert.Contains("variant-line", tabs.Classes);
        Assert.Contains("size-sm", navigationMenu.Classes);
        Assert.Contains("vertical", navigationMenu.Classes);
        Assert.Contains("open", navigationMenuItem.Classes);
        Assert.Contains("has-content", navigationMenuItem.Classes);
        Assert.Contains("has-icon", navigationMenuItem.Classes);
        Assert.Contains("has-header", navigationMenuContent.Classes);
        Assert.Contains("has-description", navigationMenuContent.Classes);
        Assert.Contains("active", navigationMenuLink.Classes);
        Assert.Contains("has-icon", navigationMenuLink.Classes);
        Assert.Contains("size-sm", menu.Classes);
        Assert.Contains("loading", menu.Classes);
        Assert.Contains("active", menuItem.Classes);
        Assert.True(menuItem.HasShortcut);
        Assert.Contains("size-sm", contextMenu.Classes);
        Assert.Contains("loading", contextMenu.Classes);
        Assert.Contains("side-right", contextMenu.Classes);
        Assert.Contains("active", contextMenuItem.Classes);
        Assert.Contains("inset", contextMenuItem.Classes);
        Assert.Contains("is-radio", contextMenuItem.Classes);
        Assert.Contains("submenu-side-left", contextMenuItem.Classes);
        Assert.True(contextMenuItem.HasShortcut);
        Assert.Contains("inset", contextMenuLabel.Classes);
        Assert.True(contextMenuGroup.HasHeader);
        Assert.Contains("has-header", contextMenuGroup.Classes);
        Assert.IsAssignableFrom<MenuItem>(menuGroup);
        Assert.False(menuGroup.Focusable);
        Assert.Equal("Components", menuGroup.Header);
        Assert.Contains("loading", command.Classes);
        Assert.Contains("active", commandItem.Classes);
        Assert.Contains("has-icon", commandItem.Classes);
        Assert.True(commandItem.HasShortcut);
        Assert.Equal("Ctrl+K", commandItem.Shortcut);
        Assert.Contains("type-multiple", accordion.Classes);
        Assert.Contains("collapsible", accordion.Classes);
        Assert.Contains("size-sm", accordion.Classes);
        Assert.Contains("open", accordionItem.Classes);
        Assert.Contains("has-header", accordionItem.Classes);
        Assert.Contains("has-content", accordionItem.Classes);
        Assert.Contains("repository", accordion.OpenValues);
        Assert.Contains("open", collapsible.Classes);
        Assert.Contains("size-lg", collapsible.Classes);
        Assert.Contains("has-header", collapsible.Classes);
        Assert.Contains("has-content", collapsible.Classes);
        Assert.True(collapsible.IsContentVisible);
        Assert.Contains("active", sidebarMenuButton.Classes);
        Assert.Contains("has-icon", sidebarMenuButton.Classes);
        Assert.Contains("has-badge", sidebarMenuButton.Classes);
        Assert.Contains("size-sm", sidebarMenuButton.Classes);
        Assert.Contains("active", sidebarMenuAction.Classes);
        Assert.Contains("show-on-hover", sidebarMenuAction.Classes);
        Assert.Contains("active", sidebarSubButton.Classes);
        Assert.IsAssignableFrom<Button>(sidebarMenuButton);
        Assert.IsAssignableFrom<Button>(sidebarMenuAction);
        Assert.IsAssignableFrom<Button>(sidebarSubButton);
        Assert.False(typeof(MenuItem).IsAssignableFrom(sidebarMenuButton.GetType()));
        Assert.False(typeof(MenuItem).IsAssignableFrom(sidebarMenuAction.GetType()));
        Assert.False(typeof(MenuItem).IsAssignableFrom(sidebarSubButton.GetType()));
        Assert.DoesNotContain("hoverable", table.Classes);
        Assert.Contains("striped", table.Classes);
        Assert.Contains("compact", table.Classes);
        Assert.IsAssignableFrom<ContentControl>(table);
        Assert.IsAssignableFrom<ContentControl>(tableHeader);
        Assert.IsAssignableFrom<ItemsControl>(tableBody);
        Assert.IsAssignableFrom<ContentControl>(tableFooter);
        Assert.IsAssignableFrom<ContentControl>(row);
        Assert.Contains("selected", row.Classes);
        Assert.Contains("align-right", head.Classes);
        Assert.Contains("align-center", cell.Classes);
        Assert.Contains("compact", rankedBarChart.Classes);
        Assert.Contains("has-active-row", rankedBarChart.Classes);
        Assert.Equal(4, rankedBarChart.MaxVisibleItems);
        Assert.Equal("Anthropic", rankedBarChart.ActiveItem?.Label);
        rankedBarChart.ActiveIndex = 0;
        Assert.Contains((1, 0), rankedBarChartChanges);
        Assert.NotNull(rankedBarChart.ItemsSource);
        Assert.Contains("compact", barChart.Classes);
        Assert.Contains("horizontal", barChart.Classes);
        Assert.Contains("no-grid", barChart.Classes);
        Assert.Contains("no-axis-labels", barChart.Classes);
        Assert.Contains("has-negative", barChart.Classes);
        Assert.Contains("has-active-bar", barChart.Classes);
        Assert.Equal("Anthropic", barChart.ActiveItem?.Label);
        barChart.ActiveIndex = 0;
        Assert.Contains((1, 0), barChartChanges);
        Assert.Contains("compact", lineChart.Classes);
        Assert.Contains("line-only", lineChart.Classes);
        Assert.Contains("no-dots", lineChart.Classes);
        Assert.Contains("has-active-point", lineChart.Classes);
        Assert.Equal("Tue", lineChart.ActivePoint?.Label);
        lineChart.ActiveIndex = 0;
        Assert.Contains((1, 0), lineChartChanges);
        Assert.Contains("compact", usagePieChart.Classes);
        Assert.Contains("has-active-slice", usagePieChart.Classes);
        Assert.Equal("Requests", usagePieChart.TotalLabel);
        Assert.Equal("20", usagePieChart.TotalValue);
        Assert.Equal("gpt-5.4", usagePieChart.ActiveItem?.Label);
        usagePieChart.ActiveIndex = 0;
        Assert.Contains((1, 0), usagePieChartChanges);
        Assert.NotNull(usagePieChart.ItemsSource);
        Assert.Contains("size-icon", avatar.Classes);
        Assert.Contains("aspect-ratio", aspectRatio.Classes);
        Assert.Contains("ratio-square", aspectRatio.Classes);
        Assert.Contains("has-content", aspectRatio.Classes);
        Assert.Contains("fit-width", aspectRatio.Classes);
        Assert.Contains("size-lg", aspectRatio.Classes);
        Assert.Equal("1:1", aspectRatio.RatioText);
        Assert.Contains("interactive", card.Classes);
        Assert.True(card.HasHeader);
        Assert.True(card.HasContent);
        Assert.True(card.HasFooter);
        Assert.Contains("interactive", itemRow.Classes);
        Assert.Contains("selected", itemRow.Classes);
        Assert.True(itemRow.HasMedia);
        Assert.True(itemRow.HasActions);
        Assert.True(itemRow.HasFooter);
        Assert.Contains("item-media", ((CodexItemMedia)itemRow.Media).Classes);
        Assert.Contains("item-group", itemGroup.Classes);
        Assert.Contains("inset", itemGroup.Classes);
        Assert.Contains("vertical", separator.Classes);
        Assert.Contains("size-lg", separator.Classes);
        Assert.Contains("role-code", text.Classes);
        Assert.Contains("size-lg", breadcrumb.Classes);
        Assert.Contains("breadcrumb-link", breadcrumbLink.Classes);
        Assert.Contains("has-href", breadcrumbLink.Classes);
        Assert.Contains("breadcrumb-page", breadcrumbPage.Classes);
        Assert.Contains("current", breadcrumbPage.Classes);
        Assert.Contains("breadcrumb-separator", breadcrumbSeparator.Classes);
        Assert.Contains("breadcrumb-ellipsis", breadcrumbEllipsis.Classes);
        Assert.Contains("compact", pagination.Classes);
        Assert.DoesNotContain("show-first-last", pagination.Classes);
        Assert.Equal(5, pagination.PageCount);
        Assert.Contains(pagination.PageItems, item => item.IsCurrent && item.Page == 2);
        Assert.Contains("type-always", scrollArea.Classes);
        Assert.Contains("inset-content", scrollArea.Classes);
        Assert.Contains("size-sm", scrollArea.Classes);
        Assert.Contains("carousel", carousel.Classes);
        Assert.Contains("loop", carousel.Classes);
        Assert.Contains("size-lg", carousel.Classes);
        Assert.Contains("can-previous", carousel.Classes);
        Assert.Contains("can-next", carousel.Classes);
        Assert.DoesNotContain("previous-disabled", carousel.Classes);
        Assert.DoesNotContain("next-disabled", carousel.Classes);
        Assert.Equal("Slide 2 of 2", carousel.StatusText);
        Assert.Contains("resizable-panel-group", resizable.Classes);
        Assert.Contains("size-sm", resizable.Classes);
        Assert.Contains("has-handle", resizable.Classes);
        Assert.Contains("with-handle", resizableHandle.Classes);
        Assert.Equal("40% / 60%", resizable.LayoutSummary);
    }

    [Fact]
    public async Task ImageIconPathLifecycleMirrorsWebImageLoadAndErrorEvents()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(OverlayRenderedLifecycleTestApp), AvaloniaTestIsolationLevel.PerTest);
        await session.Dispatch(() =>
        {
            var appRoot = Path.GetFullPath(Path.Combine(FindRepositoryRoot(), "..", "CodexSwitch"));
            var iconPath = Path.Combine(appRoot, "Assets", "icons", "openai.png");
            var missingPath = Path.Combine(appRoot, "Assets", "icons", "missing-provider.png");
            var loaded = new List<CodexImageIconLoadedEventArgs>();
            var failed = new List<CodexImageIconLoadFailedEventArgs>();
            var icon = new CodexImageIcon();

            icon.ImageLoaded += (_, args) => loaded.Add(args);
            icon.ImageLoadFailed += (_, args) => failed.Add(args);

            Assert.True(File.Exists(iconPath));
            Assert.Contains("image-icon", icon.Classes);
            Assert.True(icon.IsEmpty);
            Assert.False(icon.HasSource);
            Assert.False(icon.IsMissing);
            Assert.Contains("empty-source", icon.Classes);

            icon.Path = iconPath;

            Assert.True(icon.HasSource);
            Assert.False(icon.IsEmpty);
            Assert.False(icon.IsMissing);
            Assert.Null(icon.LastLoadError);
            Assert.Contains("has-source", icon.Classes);
            Assert.DoesNotContain("empty-source", icon.Classes);
            var load = Assert.Single(loaded);
            Assert.Equal(iconPath, load.Path);
            Assert.Null(load.OldPath);
            Assert.Same(icon.Source, load.Source);

            icon.Path = missingPath;

            Assert.False(icon.HasSource);
            Assert.True(icon.IsEmpty);
            Assert.True(icon.IsMissing);
            Assert.Contains("missing-source", icon.Classes);
            Assert.Contains("empty-source", icon.Classes);
            Assert.DoesNotContain("has-source", icon.Classes);
            var failure = Assert.Single(failed);
            Assert.Equal(missingPath, failure.Path);
            Assert.Equal(iconPath, failure.OldPath);
            Assert.Equal(icon.LastLoadError, failure.ErrorMessage);

            icon.Path = "";

            Assert.False(icon.HasSource);
            Assert.True(icon.IsEmpty);
            Assert.False(icon.IsMissing);
            Assert.Null(icon.LastLoadError);
            Assert.Same(failure, Assert.Single(failed));
            Assert.Contains("empty-source", icon.Classes);
            Assert.DoesNotContain("missing-source", icon.Classes);

            icon.Source = load.Source;

            Assert.True(icon.HasSource);
            Assert.False(icon.IsEmpty);
            Assert.False(icon.IsMissing);
            Assert.Contains("has-source", icon.Classes);
        }, CancellationToken.None);
    }

    [Fact]
    public void ItemActivationAndSlotsMirrorWebItemRows()
    {
        var activations = 0;
        var command = new TestCommand(() => activations++);
        var item = new CodexItem
        {
            IsInteractive = true,
            Title = "Fallback provider",
            Description = "Warm standby route",
            Media = new CodexItemMedia { Content = "CL", Variant = CodexControlVariant.Secondary },
            Actions = new CodexButton { Content = "Configure", Size = CodexControlSize.Small },
            Footer = "Last checked 2m ago",
            ActivateCommand = command,
            ActivateCommandParameter = "fallback"
        };
        var eventParameters = new List<object?>();
        var eventSources = new List<CodexItemActivationSource>();
        item.Activated += (_, args) =>
        {
            eventParameters.Add(args.CommandParameter);
            eventSources.Add(args.Source);
        };

        Assert.Contains("item", item.Classes);
        Assert.Contains("interactive", item.Classes);
        Assert.Contains("has-title", item.Classes);
        Assert.Contains("has-description", item.Classes);
        Assert.Contains("has-media", item.Classes);
        Assert.Contains("has-actions", item.Classes);
        Assert.Contains("has-footer", item.Classes);

        Assert.True(item.TryActivate());
        Assert.True(item.TryHandleActivationKey(Key.Enter));
        Assert.True(item.TryHandleActivationKey(Key.Space));
        Assert.Equal(3, activations);
        Assert.Equal(["fallback", "fallback", "fallback"], eventParameters);
        Assert.Equal(
            [
                CodexItemActivationSource.Programmatic,
                CodexItemActivationSource.Keyboard,
                CodexItemActivationSource.Keyboard
            ],
            eventSources);

        command.CanExecuteValue = false;
        command.RaiseCanExecuteChanged();
        Assert.False(item.CanActivate);
        Assert.DoesNotContain("can-activate", item.Classes);
        Assert.Contains("command-blocked", item.Classes);
        Assert.False(item.TryActivate());
        Assert.Equal(3, activations);

        command.CanExecuteValue = true;
        command.RaiseCanExecuteChanged();
        Assert.True(item.CanActivate);
        Assert.Contains("can-activate", item.Classes);
        Assert.DoesNotContain("command-blocked", item.Classes);
        item.IsLoading = true;
        Assert.Contains("loading", item.Classes);
        Assert.DoesNotContain("command-blocked", item.Classes);
        Assert.False(item.TryActivate());
        Assert.Equal(3, activations);

        item.IsLoading = false;
        item.IsEnabled = false;
        Assert.False(item.CanActivate);
        Assert.False(item.TryHandleActivationKey(Key.Enter));
    }

    [Fact]
    public void ItemPointerActivationUsesPrimaryReleaseAndIgnoresNestedActions()
    {
        var activations = 0;
        var command = new TestCommand(() => activations++);
        var item = new CodexItem
        {
            IsInteractive = true,
            Title = "Fallback provider",
            ActivateCommand = command,
            ActivateCommandParameter = "fallback"
        };
        var eventParameters = new List<object?>();
        var eventSources = new List<CodexItemActivationSource>();
        item.Activated += (_, args) =>
        {
            eventParameters.Add(args.CommandParameter);
            eventSources.Add(args.Source);
        };

        Assert.False(item.TryHandlePointerActivation(PointerUpdateKind.RightButtonReleased));
        Assert.False(item.TryHandlePointerActivation(PointerUpdateKind.MiddleButtonReleased));
        Assert.Equal(0, activations);
        Assert.Empty(eventParameters);

        Assert.False(item.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased, new CodexButton { Content = "Configure" }));
        Assert.False(item.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased, new CodexBadge { Content = "Action", IsInteractive = true }));
        Assert.Equal(0, activations);
        Assert.Empty(eventParameters);

        Assert.True(item.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.Equal(1, activations);
        Assert.Equal(["fallback"], eventParameters);
        Assert.Equal([CodexItemActivationSource.Pointer], eventSources);

        item.IsLoading = true;
        Assert.False(item.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.Equal(1, activations);
        Assert.Equal(["fallback"], eventParameters);
        Assert.Equal([CodexItemActivationSource.Pointer], eventSources);
    }

    [Fact]
    public void AspectRatioMeasuresRatioModesAndNormalizesInvalidInput()
    {
        var changes = new List<(double OldRatio, double NewRatio, string RatioText)>();
        var aspectRatio = new CodexAspectRatio
        {
            Width = 360,
            Ratio = 16d / 9d
        };
        aspectRatio.RatioChanged += (_, args) => changes.Add((args.OldRatio, args.NewRatio, args.RatioText));

        Assert.Contains("ratio-video", aspectRatio.Classes);
        Assert.Equal("16:9", aspectRatio.RatioText);
        Assert.Equal(
            new Size(360, 202.5),
            CodexAspectRatio.CalculateRatioSize(aspectRatio.Ratio, aspectRatio.FitMode, new Size(800, 600), aspectRatio.Width));

        aspectRatio.Ratio = 9d / 16d;

        Assert.Contains("ratio-portrait", aspectRatio.Classes);
        Assert.Equal("9:16", aspectRatio.RatioText);
        Assert.Contains(changes, change => change.RatioText == "9:16");

        aspectRatio.Ratio = 0;

        Assert.Equal(1, aspectRatio.Ratio);
        Assert.Contains("ratio-square", aspectRatio.Classes);
        Assert.Equal("1:1", aspectRatio.RatioText);

        var heightFit = CodexAspectRatio.CalculateRatioSize(
            4d / 3d,
            CodexAspectRatioFitMode.Height,
            new Size(double.PositiveInfinity, 180));
        Assert.Equal(new Size(240, 180), heightFit);

        var contained = CodexAspectRatio.CalculateRatioSize(
            16d / 9d,
            CodexAspectRatioFitMode.Contain,
            new Size(320, 120));
        Assert.Equal(new Size(213.33333333333331, 120), contained);
    }

    [Fact]
    public void MenuItemsSuppressDisabledLoadingAndCommandBlockedActivation()
    {
        var menuExecutions = 0;
        var menuCommand = new TestCommand(() => menuExecutions++);
        var menu = new CodexMenu();
        var menuItem = new CodexMenuItem
        {
            Header = "Open",
            Command = menuCommand
        };
        menu.Items.Add(menuItem);

        RaiseClick(menuItem);
        Assert.Equal(1, menuExecutions);

        menuItem.IsEnabled = false;
        RaiseClick(menuItem);
        Assert.Equal(1, menuExecutions);

        menuItem.IsEnabled = true;
        menuCommand.CanExecuteValue = false;
        menuCommand.RaiseCanExecuteChanged();
        RaiseClick(menuItem);
        Assert.Equal(1, menuExecutions);

        menuCommand.CanExecuteValue = true;
        menuCommand.RaiseCanExecuteChanged();
        menu.IsLoading = true;
        RaiseClick(menuItem);
        Assert.Equal(1, menuExecutions);

        menu.IsLoading = false;
        RaiseClick(menuItem);
        Assert.Equal(2, menuExecutions);
    }

    [Fact]
    public void MenuItemPointerSelectionUsesPrimaryReleaseOnly()
    {
        var executions = 0;
        var command = new TestCommand(() => executions++);
        var menu = new CodexMenu();
        var submenu = new CodexMenuItem { Header = "Export", IsSubMenuOpen = true };
        var leaf = new CodexMenuItem
        {
            Header = "JSON",
            Command = command
        };
        var selected = new List<CodexMenuItemSelectedEventArgs>();
        leaf.ItemSelected += (_, args) => selected.Add(args);
        submenu.Items.Add(leaf);
        menu.Items.Add(submenu);

        Assert.False(leaf.TryHandlePointerSelection(PointerUpdateKind.RightButtonReleased));
        Assert.True(submenu.IsSubMenuOpen);
        Assert.Empty(selected);
        Assert.Equal(0, executions);

        Assert.False(leaf.TryHandlePointerSelection(PointerUpdateKind.MiddleButtonReleased));
        Assert.True(submenu.IsSubMenuOpen);
        Assert.Empty(selected);
        Assert.Equal(0, executions);

        Assert.True(leaf.TryHandlePointerSelection(PointerUpdateKind.LeftButtonReleased));
        RaiseClick(leaf);

        Assert.Equal(1, executions);
        var selection = Assert.Single(selected);
        Assert.Equal(CodexMenuItemSelectSource.Pointer, selection.Source);
        Assert.True(selection.DidCloseOnSelect);
        Assert.False(submenu.IsSubMenuOpen);

        submenu.IsSubMenuOpen = true;
        menu.IsLoading = true;
        Assert.False(leaf.TryHandlePointerSelection(PointerUpdateKind.LeftButtonReleased));
        Assert.True(submenu.IsSubMenuOpen);
        Assert.Single(selected);
        Assert.Equal(1, executions);
    }

    [Fact]
    public void ContextMenuItemsSuppressDisabledLoadingAndCommandBlockedActivation()
    {
        var executions = 0;
        var command = new TestCommand(() => executions++);
        var contextMenu = new CodexContextMenu();
        var item = new CodexContextMenuItem
        {
            Header = "Open",
            Command = command
        };
        contextMenu.Items.Add(item);

        RaiseClick(item);
        Assert.Equal(1, executions);

        item.IsEnabled = false;
        RaiseClick(item);
        Assert.Equal(1, executions);

        item.IsEnabled = true;
        command.CanExecuteValue = false;
        command.RaiseCanExecuteChanged();
        RaiseClick(item);
        Assert.Equal(1, executions);

        command.CanExecuteValue = true;
        command.RaiseCanExecuteChanged();
        contextMenu.IsLoading = true;
        RaiseClick(item);
        Assert.Equal(1, executions);

        contextMenu.IsLoading = false;
        RaiseClick(item);
        Assert.Equal(2, executions);
    }

    [Fact]
    public void ContextMenuItemPointerSelectionUsesPrimaryReleaseOnly()
    {
        var contextMenu = new CodexContextMenu();
        var submenu = new CodexContextMenuItem { Header = "Move to", IsSubMenuOpen = true };
        var leaf = new CodexContextMenuItem { Header = "Archive" };
        var selected = new List<CodexMenuItemSelectedEventArgs>();
        leaf.ItemSelected += (_, args) => selected.Add(args);
        submenu.Items.Add(leaf);
        contextMenu.Items.Add(submenu);

        Assert.False(leaf.TryHandlePointerSelection(PointerUpdateKind.RightButtonReleased));
        Assert.True(submenu.IsSubMenuOpen);
        Assert.Empty(selected);

        Assert.False(leaf.TryHandlePointerSelection(PointerUpdateKind.MiddleButtonReleased));
        Assert.True(submenu.IsSubMenuOpen);
        Assert.Empty(selected);

        Assert.True(leaf.TryHandlePointerSelection(PointerUpdateKind.LeftButtonReleased));
        RaiseClick(leaf);

        var selection = Assert.Single(selected);
        Assert.Equal(CodexMenuItemSelectSource.Pointer, selection.Source);
        Assert.True(selection.DidCloseOnSelect);
        Assert.False(submenu.IsSubMenuOpen);

        submenu.IsSubMenuOpen = true;
        contextMenu.IsLoading = true;
        Assert.False(leaf.TryHandlePointerSelection(PointerUpdateKind.LeftButtonReleased));
        Assert.True(submenu.IsSubMenuOpen);
        Assert.Single(selected);
    }

    [Fact]
    public void MenuLeafSelectionClosesOpenSubMenuChainAndKeepsBlockedMenusOpen()
    {
        var executions = 0;
        var command = new TestCommand(() => executions++);
        var menu = new CodexMenu();
        var submenu = new CodexMenuItem { Header = "Export", IsSubMenuOpen = true };
        var leaf = new CodexMenuItem
        {
            Header = "JSON",
            Command = command
        };
        submenu.Items.Add(leaf);
        menu.Items.Add(submenu);

        Assert.False(submenu.TryCloseOnSelect());
        Assert.True(submenu.IsSubMenuOpen);

        RaiseClick(leaf);

        Assert.Equal(1, executions);
        Assert.False(submenu.IsSubMenuOpen);

        submenu.IsSubMenuOpen = true;
        menu.IsLoading = true;
        RaiseClick(leaf);

        Assert.Equal(1, executions);
        Assert.True(submenu.IsSubMenuOpen);

        menu.IsLoading = false;
        command.CanExecuteValue = false;
        command.RaiseCanExecuteChanged();
        RaiseClick(leaf);

        Assert.Equal(1, executions);
        Assert.True(submenu.IsSubMenuOpen);
    }

    [Fact]
    public void ContextMenuLeafSelectionClosesOpenSubMenuChainAndSkipsSubMenuTriggers()
    {
        var contextMenu = new CodexContextMenu();
        var submenu = new CodexContextMenuItem { Header = "Move to", IsSubMenuOpen = true };
        var leaf = new CodexContextMenuItem { Header = "Archive" };
        submenu.Items.Add(leaf);
        contextMenu.Items.Add(submenu);

        Assert.False(submenu.TryCloseOnSelect());
        Assert.True(submenu.IsSubMenuOpen);

        Assert.True(leaf.TryCloseOnSelect());
        Assert.False(submenu.IsSubMenuOpen);

        submenu.IsSubMenuOpen = true;
        contextMenu.IsLoading = true;
        Assert.False(leaf.TryCloseOnSelect());
        Assert.True(submenu.IsSubMenuOpen);
    }

    [Fact]
    public void MenuSubMenuKeysOpenCloseAndRespectActivationGate()
    {
        var menu = new CodexMenu();
        var item = new CodexMenuItem { Header = "More" };
        item.Items.Add(new CodexMenuItem { Header = "Rename" });
        menu.Items.Add(item);

        Assert.True(item.HasSubMenu);
        Assert.True(item.TryHandleSubMenuKey(Key.Right));
        Assert.True(item.IsSubMenuOpen);

        Assert.True(item.TryHandleSubMenuKey(Key.Left));
        Assert.False(item.IsSubMenuOpen);

        Assert.True(item.TryHandleSubMenuKey(Key.Down));
        Assert.True(item.IsSubMenuOpen);

        Assert.True(item.TryHandleSubMenuKey(Key.Escape));
        Assert.False(item.IsSubMenuOpen);

        Assert.True(item.TryHandleSubMenuKey(Key.Enter));
        Assert.True(item.IsSubMenuOpen);

        item.IsSubMenuOpen = false;
        Assert.False(item.TryHandleSubMenuKey(Key.Up));

        menu.IsLoading = true;
        Assert.True(item.TryHandleSubMenuKey(Key.Right));
        Assert.False(item.IsSubMenuOpen);

        menu.IsLoading = false;
        item.IsEnabled = false;
        Assert.True(item.TryHandleSubMenuKey(Key.Right));
        Assert.False(item.IsSubMenuOpen);

        item.IsEnabled = true;
        var command = new TestCommand(() => { }) { CanExecuteValue = false };
        item.Command = command;
        Assert.True(item.TryHandleSubMenuKey(Key.Space));
        Assert.False(item.IsSubMenuOpen);
    }

    [Fact]
    public void ContextMenuSubMenuKeysOpenCloseSyncClassesAndRespectActivationGate()
    {
        var contextMenu = new CodexContextMenu();
        var item = new CodexContextMenuItem { Header = "More" };
        item.Items.Add(new CodexContextMenuItem { Header = "Move left" });
        contextMenu.Items.Add(item);

        Assert.True(item.HasSubMenu);
        Assert.True(item.TryHandleSubMenuKey(Key.Right));
        Assert.True(item.IsSubMenuOpen);

        Dispatcher.UIThread.RunJobs();
        Assert.Contains("submenu-open", item.Classes);

        Assert.True(item.TryHandleSubMenuKey(Key.Left));
        Assert.False(item.IsSubMenuOpen);
        Assert.DoesNotContain("submenu-open", item.Classes);

        Assert.True(item.TryHandleSubMenuKey(Key.Enter));
        Assert.True(item.IsSubMenuOpen);

        item.IsSubMenuOpen = false;
        Assert.False(item.TryHandleSubMenuKey(Key.Down));

        contextMenu.IsLoading = true;
        Assert.True(item.TryHandleSubMenuKey(Key.Right));
        Assert.False(item.IsSubMenuOpen);

        contextMenu.IsLoading = false;
        item.IsEnabled = false;
        Assert.True(item.TryHandleSubMenuKey(Key.Right));
        Assert.False(item.IsSubMenuOpen);

        item.IsEnabled = true;
        var command = new TestCommand(() => { }) { CanExecuteValue = false };
        item.Command = command;
        Assert.True(item.TryHandleSubMenuKey(Key.Space));
        Assert.False(item.IsSubMenuOpen);
    }

    [Theory]
    [InlineData("Tabs.axaml", "PART_Indicator")]
    [InlineData("NavigationMenu.axaml", "PART_Viewport")]
    [InlineData("Menu.axaml", "PART_Surface")]
    [InlineData("ContextMenu.axaml", "PART_Surface")]
    [InlineData("Command.axaml", "PART_Input")]
    [InlineData("Collapsible.axaml", "PART_ContentClip")]
    [InlineData("Table.axaml", "PART_RowRoot")]
    [InlineData("Pagination.axaml", "PART_Items")]
    [InlineData("ScrollArea.axaml", "PART_Viewport")]
    [InlineData("Avatar.axaml", "ControlTemplate")]
    [InlineData("Card.axaml", "ControlTemplate")]
    [InlineData("Item.axaml", "PART_Surface")]
    [InlineData("Carousel.axaml", "PART_Viewport")]
    [InlineData("Resizable.axaml", "PART_HandleRoot")]
    [InlineData("AspectRatio.axaml", "PART_Viewport")]
    [InlineData("Separator.axaml", "PART_Line")]
    public void ComponentStylesDeclareTemplatesAndTransitions(string fileName, string expectedTemplatePart)
    {
        var style = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Themes", "Controls", fileName));

        Assert.Contains("Transitions", style);
        Assert.Contains("ControlTemplate", style);
        Assert.Contains(expectedTemplatePart, style);
        Assert.Contains("CodexSwitch.", style);
    }

    [Theory]
    [InlineData("Tabs.axaml", "PART_List", "PART_TriggerRoot", "PART_Content", "PART_ContentTransitionHost", "PART_FocusRing", "PART_VerticalIndicator", "TransitioningContentControl", "CrossFade", "BoxShadowsTransition", "variant-line", "vertical")]
    [InlineData("NavigationMenu.axaml", "PART_List", "PART_TriggerRoot", "PART_Viewport", "PART_ContentTransitionHost", "PART_Indicator", "CompositePageTransition", "PageSlide", "CrossFade", "motion-from-start", "motion-from-end", "ViewportWidth", "ViewportMinHeight", "controls|CodexNavigationMenuLink", "controls|CodexNavigationMenuLink.active")]
    [InlineData("Menu.axaml", "PART_ItemRoot", "PART_Icon", "PART_Shortcut", "PART_Popup", "PART_SubMenuSurface", "PART_SubMenuItemsPresenter", "PART_SubMenuArrow", "controls|CodexMenuGroup")]
    [InlineData("ContextMenu.axaml", "PART_ItemsPresenter", "PART_ItemRoot", "PART_Icon", "PART_Shortcut", "PART_Popup", "PART_SubMenuSurface", "PART_SubMenuItemsPresenter", "PART_SubMenuArrow", "PART_Radio", "controls|CodexContextMenuGroup", "controls|CodexContextMenuLabel", "controls|CodexContextMenuShortcut")]
    [InlineData("Command.axaml", "PART_InputRoot", "PART_TextPresenter", "PART_Icon", "PART_Shortcut")]
    [InlineData("Collapsible.axaml", "PART_Trigger", "PART_Chevron", "PART_ContentClip", "PART_ContentMeasure", "open", "closed", "TransformOperationsTransition")]
    [InlineData("Table.axaml", "PART_TableSurface", "PART_Head", "PART_Cell", "compact")]
    [InlineData("Pagination.axaml", "PART_FirstButton", "PART_PreviousButton", "PART_Items", "PART_NextButton", "PART_LastButton", "current", "ellipsis", "first-page", "last-page", "compact")]
    [InlineData("ScrollArea.axaml", "PART_Viewport", "PART_ContentPresenter", "PART_HorizontalScrollBar", "PART_VerticalScrollBar", "PART_ThumbSurface", "type-hover", "type-scroll", "scrolling", "inset-content")]
    [InlineData("BarChart.axaml", "BarBrush", "ActiveBarBrush", "GridBrush", "TooltipBackground", "TooltipForeground", "TooltipBorderBrush", "AnimationDuration", "compact", "horizontal", "no-grid", "has-active-bar", "has-negative")]
    [InlineData("LineChart.axaml", "LineBrush", "AreaBrush", "GridBrush", "DotBrush", "ActiveDotBrush", "TooltipBackground", "TooltipForeground", "TooltipBorderBrush", "AnimationDuration", "compact", "line-only", "no-grid", "has-active-point")]
    [InlineData("RankedBarChart.axaml", "MutedForeground", "TrackBrush", "AccentBrush", "SecondaryAccentBrush", "TertiaryAccentBrush", "compact", "has-active-row", "empty")]
    [InlineData("UsagePieChart.axaml", "MutedForeground", "TrackBrush", "SliceBorderBrush", "CenterFillBrush", "TooltipBackground", "TooltipForeground", "TooltipBorderBrush", "AnimationDuration", "compact", "has-active-slice", "empty")]
    [InlineData("Card.axaml", "PART_Surface", "PART_Header", "PART_Footer", "interactive")]
    [InlineData("Item.axaml", "PART_Surface", "PART_Header", "PART_Body", "PART_Media", "PART_Title", "PART_Description", "PART_Content", "PART_Actions", "PART_Footer", "PART_GroupSurface", "PART_MediaRoot", "interactive", "selected", "loading", "can-activate")]
    [InlineData("Carousel.axaml", "PART_Viewport", "PART_PreviousButton", "PART_NextButton", "PART_Status", "PART_ItemRoot", "selected", "loop", "can-previous", "can-next", "at-start", "at-end", "previous-disabled", "next-disabled", "vertical")]
    [InlineData("Resizable.axaml", "PART_PanelRoot", "PART_HandleRoot", "PART_HandleGrip", "PART_FocusRing", "with-handle", "dragging", "horizontal", "vertical")]
    [InlineData("AspectRatio.axaml", "PART_Root", "PART_Viewport", "PART_ContentHost", "PART_Empty", "ratio-video", "ratio-portrait", "fit-contain", "TransformOperationsTransition")]
    [InlineData("Separator.axaml", "PART_Line", "horizontal", "vertical", "size-lg")]
    public void StylesExposeExpectedTemplatePartsAndStateHooks(string fileName, params string[] expectedFragments)
    {
        var style = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Themes", "Controls", fileName));

        foreach (var fragment in expectedFragments)
        {
            Assert.Contains(fragment, style);
        }
    }

    [Fact]
    public void TabsUseOneSelectedContentHostWhileAnimatingVisibleContent()
    {
        var style = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Themes", "Controls", "Tabs.axaml"));

        Assert.Equal(2, Count(style, "Content=\"{TemplateBinding SelectedContent}\""));
        Assert.Equal(2, Count(style, "ContentTemplate=\"{TemplateBinding SelectedContentTemplate}\""));
        Assert.DoesNotContain("PART_SelectedContentHost", style);
        Assert.Equal(2, Count(style, "x:Name=\"PART_ContentTransitionHost\""));
        Assert.Equal(2, Count(style, "x:Name=\"PART_ItemsPresenter\""));
        Assert.Contains("TransitioningContentControl", style);
        Assert.Contains("CrossFade", style);
    }

    [Fact]
    public void ShadcnStateStylesArePresentForNavigationDataAndUtilityComponents()
    {
        var root = Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Themes", "Controls");
        var appShell = File.ReadAllText(Path.Combine(root, "ApplicationShell.axaml"));
        var tabs = File.ReadAllText(Path.Combine(root, "Tabs.axaml"));
        var navigationMenu = File.ReadAllText(Path.Combine(root, "NavigationMenu.axaml"));
        var menu = File.ReadAllText(Path.Combine(root, "Menu.axaml"));
        var contextMenu = File.ReadAllText(Path.Combine(root, "ContextMenu.axaml"));
        var command = File.ReadAllText(Path.Combine(root, "Command.axaml"));
        var collapsible = File.ReadAllText(Path.Combine(root, "Collapsible.axaml"));
        var table = File.ReadAllText(Path.Combine(root, "Table.axaml"));
        var pagination = File.ReadAllText(Path.Combine(root, "Pagination.axaml"));
        var scrollArea = File.ReadAllText(Path.Combine(root, "ScrollArea.axaml"));
        var barChart = File.ReadAllText(Path.Combine(root, "BarChart.axaml"));
        var lineChart = File.ReadAllText(Path.Combine(root, "LineChart.axaml"));
        var rankedBarChart = File.ReadAllText(Path.Combine(root, "RankedBarChart.axaml"));
        var usagePieChart = File.ReadAllText(Path.Combine(root, "UsagePieChart.axaml"));
        var carousel = File.ReadAllText(Path.Combine(root, "Carousel.axaml"));
        var resizable = File.ReadAllText(Path.Combine(root, "Resizable.axaml"));
        var aspectRatio = File.ReadAllText(Path.Combine(root, "AspectRatio.axaml"));

        Assert.Contains(":selected", tabs);
        Assert.Contains(":pointerover", tabs);
        Assert.Contains(":disabled", tabs);
        Assert.Contains("variant-line", tabs);
        Assert.Contains("activation-manual", tabs);
        Assert.Contains("controls|CodexTabs.vertical", tabs);
        Assert.Contains("controls|CodexNavigationMenu.open", navigationMenu);
        Assert.Contains("controls|CodexNavigationMenu.motion-from-start", navigationMenu);
        Assert.Contains("controls|CodexNavigationMenu.motion-from-end", navigationMenu);
        Assert.Contains("Width=\"{TemplateBinding ViewportWidth}\"", navigationMenu);
        Assert.Contains("MinHeight=\"{TemplateBinding ViewportMinHeight}\"", navigationMenu);
        Assert.Contains("ItemsPanel=\"{TemplateBinding ItemsPanel}\"", navigationMenu);
        Assert.Contains("IsTransitionReversed=\"{TemplateBinding IsMotionReversed}\"", navigationMenu);
        Assert.Contains("controls|CodexNavigationMenuItem.open", navigationMenu);
        Assert.Contains("controls|CodexNavigationMenuLink.active", navigationMenu);
        Assert.Contains("controls|CodexMenuItem.active", menu);
        Assert.Contains("controls|CodexMenu.side-nav", menu);
        Assert.Contains("controls|CodexMenu.side-nav controls|CodexMenuItem:pressed", menu);
        Assert.Contains("RenderTransform\" Value=\"none\"", menu);
        Assert.Contains("MenuItem:open", menu);
        Assert.Contains("MenuItem:checked", menu);
        Assert.Contains("Placement=\"RightEdgeAlignedTop\"", menu);
        Assert.Contains("Placement\" Value=\"BottomEdgeAlignedLeft\"", menu);
        Assert.Contains("controls|CodexMenuEmpty", menu);
        Assert.Contains("controls|CodexMenuLoading", menu);
        Assert.Contains(":disabled", menu);
        Assert.Contains("controls|CodexContextMenu.context-menu-open", contextMenu);
        Assert.Contains("controls|CodexContextMenu.side-bottom", contextMenu);
        Assert.Contains("controls|CodexContextMenu.side-left", contextMenu);
        Assert.Contains("controls|CodexContextMenu.side-right", contextMenu);
        Assert.Contains("controls|CodexContextMenu.side-top", contextMenu);
        Assert.Contains("controls|CodexContextMenuItem.submenu-open", contextMenu);
        Assert.Contains("controls|CodexContextMenuItem.submenu-side-left", contextMenu);
        Assert.Contains("controls|CodexContextMenuItem.is-radio:checked", contextMenu);
        Assert.Contains("RenderTransformOrigin", contextMenu);
        Assert.Contains("controls|CodexCommandItem.active", command);
        Assert.Contains("controls|CodexCommandItem.can-select", command);
        Assert.Contains("controls|CodexCommandItem.command-blocked", command);
        Assert.Contains("controls|CodexCommandEmpty", command);
        Assert.Contains("controls|CodexCommandLoading", command);
        Assert.Contains("controls|CodexCommandShortcut", command);
        Assert.Contains("controls|CodexCommandSeparator", command);
        Assert.Contains("controls|CodexCommand Separator", command);
        Assert.Contains("controls|CodexCommandItem.filtered-out", command);
        Assert.Contains("controls|CodexCommandGroup.filtered-out", command);
        Assert.Contains("MaxHeight", command);
        Assert.Contains("ScrollViewer", command);
        Assert.Contains("controls|CodexCollapsible.open", collapsible);
        Assert.Contains("controls|CodexCollapsible.closed", collapsible);
        Assert.Contains(":pointerover", collapsible);
        Assert.Contains(":disabled", collapsible);
        Assert.Contains("controls|CodexSidebarMenuButton.active", appShell);
        Assert.Contains("controls|CodexSidebarMenuButton:pointerover", appShell);
        Assert.Contains("controls|CodexSidebarMenuButton:pointerover /template/ Border#PART_MenuButtonRoot", appShell);
        Assert.Contains("controls|CodexSidebarMenuButton.active /template/ Border#PART_MenuButtonRoot", appShell);
        Assert.Contains("controls|CodexSidebarMenuButton:focus /template/ Border#PART_FocusRing", appShell);
        Assert.Contains("controls|CodexSidebarMenuAction.show-on-hover", appShell);
        Assert.Contains("controls|CodexSidebarMenuSubButton.active", appShell);
        Assert.Contains("controls|CodexImageIcon.has-source", appShell);
        Assert.Contains("controls|CodexImageIcon.empty-source", appShell);
        Assert.Contains("controls|CodexImageIcon.missing-source", appShell);
        Assert.Contains("controls|CodexTableHeader", table);
        Assert.Contains("controls|CodexTable.compact", table);
        Assert.Contains("controls|CodexTable.hoverable controls|CodexTableRow:pointerover", table);
        Assert.Contains("controls|CodexPaginationPageButton.current", pagination);
        Assert.Contains("controls|CodexPaginationPageButton.ellipsis", pagination);
        Assert.Contains("controls|CodexPagination.compact", pagination);
        Assert.Contains("CodexSwitch.DisabledOpacity", pagination);
        Assert.Contains("controls|CodexScrollArea.type-hover", scrollArea);
        Assert.Contains("controls|CodexScrollArea.type-scroll.scrolling", scrollArea);
        Assert.Contains("ScrollGestureRecognizer", scrollArea);
        Assert.Contains("CodexSwitch.DisabledOpacity", scrollArea);
        Assert.Contains("controls|CodexBarChart.has-active-bar", barChart);
        Assert.Contains("controls|CodexBarChart.horizontal", barChart);
        Assert.Contains("controls|CodexBarChart.no-grid", barChart);
        Assert.Contains("controls|CodexBarChart.has-negative", barChart);
        Assert.Contains("CodexSwitch.MotionDurationSlow", barChart);
        Assert.Contains("CodexSwitch.DisabledOpacity", barChart);
        Assert.Contains("controls|CodexLineChart.has-active-point", lineChart);
        Assert.Contains("controls|CodexLineChart.line-only", lineChart);
        Assert.Contains("controls|CodexLineChart.no-grid", lineChart);
        Assert.Contains("CodexSwitch.MotionDurationSlow", lineChart);
        Assert.Contains("CodexSwitch.DisabledOpacity", lineChart);
        Assert.Contains("controls|CodexRankedBarChart.compact", rankedBarChart);
        Assert.Contains("controls|CodexRankedBarChart.has-active-row", rankedBarChart);
        Assert.Contains("CodexSwitch.DisabledOpacity", rankedBarChart);
        Assert.Contains("controls|CodexUsagePieChart.compact", usagePieChart);
        Assert.Contains("controls|CodexUsagePieChart.has-active-slice", usagePieChart);
        Assert.Contains("CodexSwitch.DisabledOpacity", usagePieChart);
        Assert.Contains("controls|CodexAspectRatio.ratio-video", aspectRatio);
        Assert.Contains("controls|CodexAspectRatio.ratio-portrait", aspectRatio);
        Assert.Contains("controls|CodexAspectRatio.fit-contain", aspectRatio);
        Assert.Contains("controls|CodexAspectRatio:pointerover", aspectRatio);
        Assert.Contains("controls|CodexCarousel.loop", carousel);
        Assert.Contains("controls|CodexCarousel.can-previous", carousel);
        Assert.Contains("controls|CodexCarousel.can-next", carousel);
        Assert.Contains("controls|CodexCarousel.at-start.previous-disabled", carousel);
        Assert.Contains("controls|CodexCarousel.at-end.next-disabled", carousel);
        Assert.Contains("controls|CodexCarousel.vertical", carousel);
        Assert.Contains("controls|CodexCarouselItem.selected", carousel);
        Assert.Contains("controls|CodexCarouselItem:pointerover", carousel);
        Assert.Contains("TransformOperationsTransition", carousel);
        Assert.Contains("controls|CodexResizablePanelGroup.dragging", resizable);
        Assert.Contains("controls|CodexResizableHandle.with-handle", resizable);
        Assert.Contains("controls|CodexResizableHandle:focus-visible", resizable);
        Assert.Contains("controls|CodexResizableHandle.vertical", resizable);
        Assert.Contains("CodexSwitch.DisabledOpacity", resizable);

        // Rendered lifecycle tests cover submenu popups and table column alignment.
    }

    [Fact]
    public void NavigationDataStylesGuardAgainstDefaultTemplateLeakage()
    {
        var root = Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI");
        var controlsRoot = Path.Combine(root, "Themes", "Controls");
        var primitiveRoot = Path.Combine(root, "Themes", "Primitives");
        var styles = new[]
        {
            File.ReadAllText(Path.Combine(controlsRoot, "Tabs.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "NavigationMenu.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "Menu.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "ContextMenu.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "Command.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "Collapsible.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "Table.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "Pagination.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "ScrollArea.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "BarChart.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "RankedBarChart.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "UsagePieChart.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "AspectRatio.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "Card.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "Carousel.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "Resizable.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "Separator.axaml")),
            File.ReadAllText(Path.Combine(primitiveRoot, "Typography.axaml"))
        };

        foreach (var style in styles)
        {
            Assert.DoesNotContain("Fluent", style);
            Assert.DoesNotContain("BasedOn", style);
            Assert.Contains("CodexSwitch.", style);
        }

        Assert.Contains("FocusAdorner", styles[1]);
        Assert.Contains("FocusAdorner", styles[2]);
        Assert.Contains("FocusAdorner", styles[3]);
        Assert.Contains("FocusAdorner", styles[4]);
        Assert.Contains("FocusAdorner", styles[5]);
    }

    [Fact]
    public void MenuAndCommandItemsUseShadcnFocusVisibleWithoutPressedChrome()
    {
        var root = Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Themes", "Controls");
        var menu = File.ReadAllText(Path.Combine(root, "Menu.axaml"));
        var command = File.ReadAllText(Path.Combine(root, "Command.axaml"));

        var menuFocus = ExtractStyleBlock(menu, "controls|CodexMenu controls|CodexMenuItem:focus-visible /template/ Panel Border#PART_ItemRoot");
        var menuPressed = ExtractStyleBlock(menu, "controls|CodexMenu controls|CodexMenuItem:pressed /template/ Panel Border#PART_ItemRoot");
        var commandFocus = ExtractStyleBlock(command, "controls|CodexCommandItem:focus-visible /template/ Border#PART_ItemRoot");
        var commandItem = ExtractStyleBlock(command, "controls|CodexCommandItem");
        var commandBlocked = ExtractStyleBlock(command, "controls|CodexCommandItem.command-blocked");
        var commandShortcut = ExtractStyleBlock(command, "controls|CodexCommandShortcut");

        Assert.Contains("CodexSwitch.AccentBrush", menuFocus);
        Assert.DoesNotContain("CodexSwitch.RingBrush", menuFocus);
        Assert.Contains("Value=\"Transparent\"", menuFocus);
        Assert.Contains("Value=\"none\"", menuPressed);
        Assert.DoesNotContain("scale(", menuPressed);

        Assert.Contains("FocusAdorner\" Value=\"{x:Null}", commandItem);
        Assert.Contains("Focusable\" Value=\"True\"", commandItem);
        Assert.Contains("CodexSwitch.AccentBrush", commandFocus);
        Assert.DoesNotContain("CodexSwitch.RingBrush", commandFocus);
        Assert.Contains("Value=\"Transparent\"", commandFocus);
        Assert.Contains("CodexSwitch.DisabledOpacity", commandBlocked);
        Assert.Contains("CodexSwitch.MutedForegroundBrush", commandBlocked);
        Assert.Contains("CodexSwitch.MutedForegroundBrush", commandShortcut);
    }

    [Fact]
    public void MenuContextMenuAndMenubarUseWebMenuSpacing()
    {
        var root = Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Themes", "Controls");
        var menu = File.ReadAllText(Path.Combine(root, "Menu.axaml"));
        var contextMenu = File.ReadAllText(Path.Combine(root, "ContextMenu.axaml"));
        var menubar = File.ReadAllText(Path.Combine(root, "Menubar.axaml"));

        Assert.Contains("Property=\"Padding\" Value=\"8,6\"", ExtractStyleBlock(menu, "controls|CodexMenu MenuItem"));
        Assert.Contains("Property=\"Padding\" Value=\"8,6\"", ExtractStyleBlock(contextMenu, "controls|CodexContextMenu MenuItem"));
        Assert.Contains("Property=\"Padding\" Value=\"8,6\"", ExtractStyleBlock(menubar, "controls|CodexMenubar MenuItem"));

        Assert.Contains("Property=\"Padding\" Value=\"8,4\"", ExtractStyleBlock(contextMenu, "controls|CodexContextMenu.size-sm controls|CodexContextMenuItem"));
        Assert.Contains("Property=\"Padding\" Value=\"10,8\"", ExtractStyleBlock(contextMenu, "controls|CodexContextMenu.size-lg controls|CodexContextMenuItem"));
        Assert.Contains("Property=\"Padding\" Value=\"8,4\"", ExtractStyleBlock(menubar, "controls|CodexMenubar.size-sm controls|CodexMenubarItem"));
        Assert.Contains("Property=\"Padding\" Value=\"10,8\"", ExtractStyleBlock(menubar, "controls|CodexMenubar.size-lg controls|CodexMenubarItem"));
        Assert.Contains("Property=\"Padding\" Value=\"32,6,8,6\"", ExtractStyleBlock(contextMenu, "controls|CodexContextMenu controls|CodexContextMenuItem.inset /template/ Panel Border#PART_ItemRoot"));

        Assert.Contains("Property=\"Margin\" Value=\"-4,4\"", ExtractStyleBlock(menu, "controls|CodexMenu Separator"));
        Assert.Contains("Property=\"Margin\" Value=\"-4,4\"", ExtractStyleBlock(menu, "controls|CodexMenu controls|CodexMenuSeparator"));
        Assert.Contains("Property=\"Margin\" Value=\"-4,4\"", ExtractStyleBlock(contextMenu, "controls|CodexContextMenu Separator"));
        Assert.Contains("Property=\"Margin\" Value=\"-4,4\"", ExtractStyleBlock(contextMenu, "controls|CodexContextMenu controls|CodexContextMenuSeparator"));
        Assert.Contains("Property=\"Margin\" Value=\"-4,4\"", ExtractStyleBlock(menubar, "controls|CodexMenubar controls|CodexMenubarSeparator"));
    }

    [Fact]
    public void ButtonsAndNavigationLinksUseWebActionRowSpacing()
    {
        var root = Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Themes", "Controls");
        var button = File.ReadAllText(Path.Combine(root, "Button.axaml"));
        var buttonGroup = File.ReadAllText(Path.Combine(root, "ButtonGroup.axaml"));
        var navigationMenu = File.ReadAllText(Path.Combine(root, "NavigationMenu.axaml"));
        var applicationShell = File.ReadAllText(Path.Combine(root, "ApplicationShell.axaml"));

        var buttonDefault = ExtractStyleBlock(button, "controls|CodexButton");
        Assert.Contains("Property=\"MinHeight\" Value=\"44\"", buttonDefault);
        Assert.Contains("Property=\"MinWidth\" Value=\"44\"", buttonDefault);
        Assert.Contains("Property=\"Padding\" Value=\"20,0\"", buttonDefault);
        Assert.Contains("Spacing=\"8\"", buttonDefault);

        var buttonIcon = ExtractStyleBlock(button, "controls|CodexButton.size-icon");
        Assert.Contains("Property=\"Width\" Value=\"44\"", buttonIcon);
        Assert.Contains("Property=\"Height\" Value=\"44\"", buttonIcon);
        Assert.Contains("Property=\"MinWidth\" Value=\"44\"", buttonIcon);
        Assert.Contains("Property=\"MinHeight\" Value=\"44\"", buttonIcon);

        var buttonGroupText = ExtractStyleBlock(buttonGroup, "controls|CodexButtonGroupText");
        Assert.Contains("Property=\"MinHeight\" Value=\"44\"", buttonGroupText);
        Assert.Contains("Property=\"Padding\" Value=\"20,0\"", buttonGroupText);
        Assert.Contains("Property=\"MinWidth\" Value=\"44\"", ExtractStyleBlock(buttonGroup, "controls|CodexButtonGroupText.size-icon"));

        var navigationLink = ExtractStyleBlock(navigationMenu, "controls|CodexNavigationMenuLink");
        Assert.Contains("Property=\"MinHeight\" Value=\"44\"", navigationLink);
        Assert.Contains("Property=\"Padding\" Value=\"16,12\"", navigationLink);
        Assert.Contains("ColumnSpacing=\"8\"", navigationLink);
        Assert.Contains("Width=\"16\"", navigationLink);
        Assert.Contains("Height=\"16\"", navigationLink);

        var sideNavItem = ExtractStyleBlock(applicationShell, "controls|CodexSideNavItem");
        Assert.Contains("Property=\"MinHeight\" Value=\"44\"", sideNavItem);
        Assert.Contains("Property=\"Padding\" Value=\"16,12\"", sideNavItem);
        Assert.Contains("ColumnSpacing=\"8\"", sideNavItem);
    }

    [Fact]
    public void CardEmptyStateFieldAndSheetUseWebPanelSpacing()
    {
        var root = Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Themes", "Controls");
        var card = File.ReadAllText(Path.Combine(root, "Card.axaml"));
        var emptyState = File.ReadAllText(Path.Combine(root, "EmptyState.axaml"));
        var field = File.ReadAllText(Path.Combine(root, "Field.axaml"));
        var dialog = File.ReadAllText(Path.Combine(root, "Dialog.axaml"));
        var sheet = File.ReadAllText(Path.Combine(root, "Sheet.axaml"));

        Assert.Contains("Property=\"Padding\" Value=\"20\"", ExtractStyleBlock(card, "controls|CodexCard"));

        var emptyStateDefault = ExtractStyleBlock(emptyState, "controls|CodexEmptyState");
        Assert.Contains("Property=\"Padding\" Value=\"20\"", emptyStateDefault);
        Assert.Contains("Property=\"MinHeight\" Value=\"160\"", emptyStateDefault);
        Assert.Contains("Property=\"Padding\" Value=\"16\"", ExtractStyleBlock(emptyState, "controls|CodexEmptyState.size-sm"));
        Assert.Contains("Property=\"MinHeight\" Value=\"128\"", ExtractStyleBlock(emptyState, "controls|CodexEmptyState.size-sm"));
        Assert.Contains("Property=\"Padding\" Value=\"24\"", ExtractStyleBlock(emptyState, "controls|CodexEmptyState.size-lg"));
        Assert.Contains("Property=\"MinHeight\" Value=\"200\"", ExtractStyleBlock(emptyState, "controls|CodexEmptyState.size-lg"));

        Assert.Contains("RowSpacing=\"8\"", ExtractStyleBlock(field, "controls|CodexField"));
        Assert.Contains("Property=\"RowSpacing\" Value=\"6\"", ExtractStyleBlock(field, "controls|CodexField.size-sm /template/ Grid#PART_Layout"));
        Assert.Contains("Property=\"RowSpacing\" Value=\"4\"", ExtractStyleBlock(field, "controls|CodexField.size-icon /template/ Grid#PART_Layout"));
        Assert.Contains("Property=\"RowSpacing\" Value=\"10\"", ExtractStyleBlock(field, "controls|CodexField.size-lg /template/ Grid#PART_Layout"));
        Assert.Contains("StackPanel Orientation=\"Vertical\" Spacing=\"16\"", ExtractStyleBlock(field, "controls|CodexFieldGroup"));
        Assert.Contains("StackPanel Orientation=\"Vertical\" Spacing=\"16\"", ExtractStyleBlock(field, "controls|CodexFieldSet"));
        Assert.Contains("Spacing=\"16\"", ExtractStyleBlock(field, "controls|CodexFieldSet"));
        Assert.Contains("Property=\"Spacing\" Value=\"12\"", ExtractStyleBlock(field, "controls|CodexFieldSet.size-sm /template/ StackPanel#PART_Root"));
        Assert.Contains("Property=\"Spacing\" Value=\"20\"", ExtractStyleBlock(field, "controls|CodexFieldSet.size-lg /template/ StackPanel#PART_Root"));

        Assert.Contains("Property=\"Padding\" Value=\"24\"", ExtractStyleBlock(dialog, "controls|CodexDialog"));
        Assert.Contains("RowSpacing=\"16\"", ExtractStyleBlock(dialog, "controls|CodexDialog"));
        Assert.Contains("Property=\"Padding\" Value=\"24\"", ExtractStyleBlock(sheet, "controls|CodexSheet"));
        Assert.Contains("RowSpacing=\"16\"", ExtractStyleBlock(sheet, "controls|CodexSheet"));
    }

    [Fact]
    public void PopupDisclosureSurfacesUseWebSideOffsetSpacing()
    {
        var root = Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Themes", "Controls");
        var dropdown = File.ReadAllText(Path.Combine(root, "DropdownButton.axaml"));
        var splitButton = File.ReadAllText(Path.Combine(root, "SplitButton.axaml"));
        var select = File.ReadAllText(Path.Combine(root, "Select.axaml"));
        var combobox = File.ReadAllText(Path.Combine(root, "Combobox.axaml"));
        var datePicker = File.ReadAllText(Path.Combine(root, "DatePicker.axaml"));
        var menu = File.ReadAllText(Path.Combine(root, "Menu.axaml"));
        var menubar = File.ReadAllText(Path.Combine(root, "Menubar.axaml"));

        Assert.Contains("Margin=\"0,4,0,0\"", dropdown);
        Assert.Contains("Property=\"Margin\" Value=\"0,4,0,0\"", ExtractStyleBlock(dropdown, "controls|CodexDropdownButton.side-bottom /template/ Border#PART_Surface"));
        Assert.Contains("Property=\"Margin\" Value=\"0,0,0,4\"", ExtractStyleBlock(dropdown, "controls|CodexDropdownButton.side-top /template/ Border#PART_Surface"));
        Assert.Contains("Property=\"Margin\" Value=\"0,0,4,0\"", ExtractStyleBlock(dropdown, "controls|CodexDropdownButton.side-left /template/ Border#PART_Surface"));
        Assert.Contains("Property=\"Margin\" Value=\"4,0,0,0\"", ExtractStyleBlock(dropdown, "controls|CodexDropdownButton.side-right /template/ Border#PART_Surface"));
        Assert.DoesNotContain("Margin=\"0,6,0,0\"", dropdown);
        Assert.DoesNotContain("Property=\"Margin\" Value=\"0,6,0,0\"", dropdown);

        Assert.Contains("Margin=\"0,4,0,0\"", splitButton);
        Assert.Contains("Property=\"Margin\" Value=\"0,4,0,0\"", ExtractStyleBlock(splitButton, "controls|CodexSplitButton.side-bottom /template/ Border#PART_Surface"));
        Assert.Contains("Property=\"Margin\" Value=\"0,0,0,4\"", ExtractStyleBlock(splitButton, "controls|CodexSplitButton.side-top /template/ Border#PART_Surface"));
        Assert.Contains("Property=\"Margin\" Value=\"0,0,4,0\"", ExtractStyleBlock(splitButton, "controls|CodexSplitButton.side-left /template/ Border#PART_Surface"));
        Assert.Contains("Property=\"Margin\" Value=\"4,0,0,0\"", ExtractStyleBlock(splitButton, "controls|CodexSplitButton.side-right /template/ Border#PART_Surface"));
        Assert.DoesNotContain("Margin=\"0,6,0,0\"", splitButton);
        Assert.DoesNotContain("Property=\"Margin\" Value=\"0,6,0,0\"", splitButton);

        Assert.Contains("Margin=\"0,4,0,0\"", select);
        Assert.Contains("Margin=\"0,4,0,0\"", combobox);
        Assert.Contains("Margin=\"0,4,0,0\"", datePicker);
        Assert.DoesNotContain("Margin=\"0,6,0,0\"", select);
        Assert.DoesNotContain("Margin=\"0,6,0,0\"", combobox);
        Assert.DoesNotContain("Margin=\"0,6,0,0\"", datePicker);

        Assert.Contains("Margin=\"4,0,0,0\"", ExtractStyleBlock(menu, "controls|CodexMenu MenuItem"));
        Assert.Contains("Margin=\"4,0,0,0\"", ExtractStyleBlock(menu, "controls|CodexMenu controls|CodexMenuItem"));
        Assert.Contains("Property=\"Margin\" Value=\"0,4,0,0\"", ExtractStyleBlock(menu, "controls|CodexMenu controls|CodexMenuItem:top /template/ Panel Popup#PART_Popup Border#PART_SubMenuSurface"));

        var menubarMenu = ExtractStyleBlock(menubar, "controls|CodexMenubar controls|CodexMenubarItem");
        Assert.Contains("Margin=\"4,0,0,0\"", menubarMenu);
        Assert.Contains("translate(-4px, 0px) scale(0.96)", menubarMenu);

        var menubarTopMenu = ExtractStyleBlock(menubar, "controls|CodexMenubar controls|CodexMenubarItem.top-level /template/ Panel Popup#PART_Popup Border#PART_MenuSurface");
        Assert.Contains("Property=\"Margin\" Value=\"0,4,0,0\"", menubarTopMenu);
        Assert.Contains("translate(0px, -4px) scale(0.96)", menubarTopMenu);
        Assert.DoesNotContain("-6px", menubarTopMenu);

        var menubarVerticalMenu = ExtractStyleBlock(menubar, "controls|CodexMenubar.vertical controls|CodexMenubarItem.top-level /template/ Panel Popup#PART_Popup Border#PART_MenuSurface");
        Assert.Contains("Property=\"Margin\" Value=\"4,0,0,0\"", menubarVerticalMenu);
        Assert.Contains("translate(-4px, 0px) scale(0.96)", menubarVerticalMenu);
        Assert.DoesNotContain("-6px", menubarVerticalMenu);
        Assert.DoesNotContain("-6px", menubar);
    }

    [Fact]
    public void NavigationDisclosureAndMenuComponentsUseFocusVisibleAndTokenMotion()
    {
        var root = FindRepositoryRoot();
        var controlsRoot = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var stylesRoot = Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls");
        var styleFiles = new[]
        {
            "Tabs",
            "NavigationMenu",
            "Menu",
            "ContextMenu",
            "Command",
            "Collapsible"
        };

        foreach (var component in styleFiles)
        {
            var style = File.ReadAllText(Path.Combine(stylesRoot, $"{component}.axaml"));

            Assert.Contains(":focus-visible", style);
            Assert.DoesNotContain(":focus /template", style);
            Assert.DoesNotContain(":focus\"", style);
            AssertNoHardcodedTokenizableTransitionDurations(component, style);
            Assert.Contains("CodexSwitch.MotionDuration", style);
            Assert.Contains("CodexSwitch.MotionEaseOut", style);
        }

        AssertFocusVisibleSource(Path.Combine(controlsRoot, "CodexTabs.cs"), "CodexTabItem", "CreateContainerForItemOverride", "NeedsContainerOverride");
        AssertFocusVisibleSource(Path.Combine(controlsRoot, "CodexMenu.cs"), "CodexMenuItem", "CreateContainerForItemOverride", "NeedsContainerOverride");
        AssertFocusVisibleSource(Path.Combine(controlsRoot, "CodexContextMenu.cs"), "CodexContextMenuItem", "CreateContainerForItemOverride", "NeedsContainerOverride");
        AssertFocusVisibleSource(Path.Combine(controlsRoot, "CodexCommand.cs"), "CodexCommandInput", "CodexCommandItem");
        AssertFocusVisibleSource(Path.Combine(controlsRoot, "CodexNavigationMenu.cs"), "CodexNavigationMenuItem", "CodexNavigationMenuLink");

        var menuActivation = File.ReadAllText(Path.Combine(controlsRoot, "CodexMenuActivation.cs"));
        var menuSource = File.ReadAllText(Path.Combine(controlsRoot, "CodexMenu.cs"));
        var contextMenuSource = File.ReadAllText(Path.Combine(controlsRoot, "CodexContextMenu.cs"));
        var collapsible = File.ReadAllText(Path.Combine(controlsRoot, "CodexCollapsible.cs"));
        var collapsibleStyle = File.ReadAllText(Path.Combine(stylesRoot, "Collapsible.axaml"));

        Assert.Contains("TryHandleSubMenuKey", menuActivation);
        Assert.Contains("TryHandleSiblingNavigationKey", menuActivation);
        Assert.Contains("Key.Home", menuActivation);
        Assert.Contains("Key.End", menuActivation);
        Assert.Contains("NextNavigableIndex", menuActivation);
        Assert.Contains("CanActivate(item)", menuActivation);
        Assert.Contains("RegisterOwner", menuActivation);
        Assert.Contains("FindRegisteredOwner", menuActivation);
        Assert.Contains("owner.Items", menuActivation);
        Assert.Contains("NavigationMethod.Tab", menuActivation);
        Assert.Contains("FocusFirstSubMenuItem", menuActivation);
        Assert.Contains("TryCloseOwnerSubMenu", menuActivation);
        Assert.Contains("PointerSubMenuOpenDelay", menuActivation);
        Assert.Contains("PointerSubMenuCloseDelay", menuActivation);
        Assert.Contains("RequestPointerSubMenuOpen", menuActivation);
        Assert.Contains("RequestPointerSubMenuClose", menuActivation);
        Assert.Contains("CancelOwnerCloseRequest", menuActivation);
        Assert.Contains("PointerSubMenuState", menuActivation);
        Assert.Contains("DispatcherTimer", menuActivation);
        Assert.Contains("OpenSubMenu(item, focusFirstChild: false)", menuActivation);
        Assert.Contains("ShouldCloseOnSelect", menuActivation);
        Assert.Contains("TryCloseOnSelect", menuActivation);
        Assert.Contains("contextMenu.Close();", menuActivation);
        Assert.Contains("OnPointerEntered(PointerEventArgs e)", menuSource);
        Assert.Contains("CodexMenuActivation.RequestPointerSubMenuOpen(this);", menuSource);
        Assert.Contains("OnPointerExited(PointerEventArgs e)", menuSource);
        Assert.Contains("CodexMenuActivation.RequestPointerSubMenuClose(this);", menuSource);
        Assert.Contains("CodexMenuActivation.CancelPointerSubMenuRequests(this);", menuSource);
        Assert.Contains("CodexMenuActivation.TryCloseOnSelect(this);", menuSource);
        Assert.Contains("OnPointerEntered(PointerEventArgs e)", contextMenuSource);
        Assert.Contains("CodexMenuActivation.RequestPointerSubMenuOpen(this);", contextMenuSource);
        Assert.Contains("OnPointerExited(PointerEventArgs e)", contextMenuSource);
        Assert.Contains("CodexMenuActivation.RequestPointerSubMenuClose(this);", contextMenuSource);
        Assert.Contains("CodexMenuActivation.CancelPointerSubMenuRequests(this);", contextMenuSource);
        Assert.Contains("CodexMenuActivation.TryCloseOnSelect(this);", contextMenuSource);
        Assert.Contains("Focus(NavigationMethod.Pointer, KeyModifiers.None)", collapsible);
        Assert.Contains("public event EventHandler<CodexCollapsibleOpenChangedEventArgs>? OpenChanged;", collapsible);
        Assert.Contains("controls|CodexButton#PART_Trigger:focus-visible", collapsibleStyle);
    }

    [Fact]
    public void MenuGroupUsesOwnContainerWithoutParentHoverChrome()
    {
        var root = FindRepositoryRoot();
        var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Menu.axaml"));
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexMenu.cs"));
        var groupStyle = ExtractStyleBlock(style, "controls|CodexMenuGroup");

        Assert.Contains("public class CodexMenuGroup : MenuItem", source);
        Assert.DoesNotContain("public class CodexMenuGroup : ItemsControl", source);
        Assert.Contains("Focusable = false", source);
        Assert.Contains("TargetType=\"controls:CodexMenuGroup\"", groupStyle);
        Assert.Contains("PART_Group", groupStyle);
        Assert.Contains("PART_GroupItems", groupStyle);
        Assert.Contains("ItemsPanel=\"{TemplateBinding ItemsPanel}\"", groupStyle);
        Assert.Contains("Cursor\" Value=\"Arrow\"", groupStyle);
        Assert.DoesNotContain("PART_ItemRoot", groupStyle);
    }

    [Fact]
    public void SidebarMenuPrimitivesOwnTemplatesWithoutAvaloniaMenuChrome()
    {
        var root = FindRepositoryRoot();
        var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "ApplicationShell.axaml"));
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexApplicationShell.cs"));

        Assert.Contains("public class CodexSidebarMenuButton : Button", source);
        Assert.Contains("public class CodexSidebarMenuAction : Button", source);
        Assert.Contains("public class CodexSidebarMenuSubButton : Button", source);
        Assert.DoesNotContain("CodexSidebarMenuItem : MenuItem", source);
        Assert.DoesNotContain("CodexSidebarMenuButton : MenuItem", source);

        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSidebarMenuButton\"", style);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSidebarMenuAction\"", style);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSidebarMenuSubButton\"", style);
        Assert.Contains("PART_MenuButtonRoot", style);
        Assert.Contains("PART_ActionRoot", style);
        Assert.Contains("PART_SubButtonRoot", style);
        Assert.Contains("PART_FocusRing", style);
        Assert.Contains("FocusAdorner\" Value=\"{x:Null}", style);
        Assert.DoesNotContain("BasedOn=", style);
        Assert.DoesNotContain("FluentTheme", style);
        Assert.DoesNotContain("Avalonia.Themes.Fluent", style);
    }

    [Fact]
    public void SidebarProviderTriggerRailAndInsetMirrorWebOpenState()
    {
        var changes = new List<bool>();
        var provider = new CodexSidebarProvider();
        var sidebar = new CodexSidebar
        {
            Variant = CodexSidebarVariant.Inset,
            Collapsible = CodexSidebarCollapsible.Icon,
            Side = CodexSidebarSide.Right
        };
        var trigger = new CodexSidebarTrigger();
        var rail = new CodexSidebarRail();
        var inset = new CodexSidebarInset();

        provider.OpenChanged += (_, args) => changes.Add(args.IsOpen);
        provider.Content = new StackPanel
        {
            Children =
            {
                sidebar,
                trigger,
                rail,
                inset
            }
        };
        provider.SyncDescendantState();

        Assert.Contains("sidebar-provider", provider.Classes);
        Assert.Contains("state-expanded", sidebar.Classes);
        Assert.Contains("variant-inset", trigger.Classes);
        Assert.Contains("side-right", rail.Classes);
        Assert.Contains("state-expanded", inset.Classes);

        InvokeButtonClick(trigger);

        Assert.False(provider.IsOpen);
        Assert.False(sidebar.IsOpen);
        Assert.Contains(false, changes);
        Assert.Contains("state-collapsed", sidebar.Classes);
        Assert.Contains("icon", sidebar.Classes);
        Assert.Contains("state-collapsed", trigger.Classes);
        Assert.Contains("state-collapsed", rail.Classes);
        Assert.Contains("state-collapsed", inset.Classes);

        InvokeButtonClick(rail);

        Assert.True(provider.IsOpen);
        Assert.True(sidebar.IsOpen);
        Assert.Contains(true, changes);
        Assert.Contains("state-expanded", trigger.Classes);

        Assert.True(provider.TryHandleShortcut(Key.B, KeyModifiers.Control));
        Assert.False(provider.IsOpen);
        Assert.True(provider.TryHandleShortcut(Key.B, KeyModifiers.Meta));
        Assert.True(provider.IsOpen);
        Assert.False(provider.TryHandleShortcut(Key.B, KeyModifiers.None));
    }

    [Fact]
    public void SidebarTriggerAndRailRespectCommandCanExecuteBeforeToggle()
    {
        var triggerExecutions = 0;
        var provider = new CodexSidebarProvider();
        var sidebar = new CodexSidebar();
        var triggerCommand = new TestCommand(() => triggerExecutions++)
        {
            CanExecuteValue = false
        };
        var trigger = new CodexSidebarTrigger
        {
            Command = triggerCommand
        };
        provider.Content = new StackPanel
        {
            Children =
            {
                sidebar,
                trigger
            }
        };
        provider.SyncDescendantState();

        Assert.False(trigger.CanToggle);
        Assert.Contains("command-blocked", trigger.Classes);

        InvokeButtonClick(trigger);

        Assert.True(provider.IsOpen);
        Assert.True(sidebar.IsOpen);
        Assert.Equal(0, triggerExecutions);

        triggerCommand.CanExecuteValue = true;
        triggerCommand.RaiseCanExecuteChanged();

        Assert.True(trigger.CanToggle);
        Assert.Contains("can-toggle", trigger.Classes);
        Assert.DoesNotContain("command-blocked", trigger.Classes);

        InvokeButtonClick(trigger);

        Assert.False(provider.IsOpen);
        Assert.False(sidebar.IsOpen);
        Assert.Equal(1, triggerExecutions);

        var railExecutions = 0;
        var railProvider = new CodexSidebarProvider();
        var railSidebar = new CodexSidebar();
        var railCommand = new TestCommand(() => railExecutions++)
        {
            CanExecuteValue = false
        };
        var rail = new CodexSidebarRail
        {
            Command = railCommand
        };
        railProvider.Content = new StackPanel
        {
            Children =
            {
                railSidebar,
                rail
            }
        };
        railProvider.SyncDescendantState();

        Assert.False(rail.CanToggle);
        Assert.Contains("command-blocked", rail.Classes);

        InvokeButtonClick(rail);

        Assert.True(railProvider.IsOpen);
        Assert.True(railSidebar.IsOpen);
        Assert.Equal(0, railExecutions);

        railCommand.CanExecuteValue = true;
        railCommand.RaiseCanExecuteChanged();

        InvokeButtonClick(rail);

        Assert.False(railProvider.IsOpen);
        Assert.False(railSidebar.IsOpen);
        Assert.Equal(1, railExecutions);
    }

    [Fact]
    public void SidebarProviderSurfaceDeclaresShadcnStateSelectors()
    {
        var root = FindRepositoryRoot();
        var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "ApplicationShell.axaml"));
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexApplicationShell.cs"));

        Assert.Contains("public class CodexSidebarProvider : CodexFrame", source);
        Assert.Contains("public class CodexSidebarTrigger : CodexButton", source);
        Assert.Contains("public class CodexSidebarRail : Button", source);
        Assert.Contains("public class CodexSidebarInset : CodexFrame", source);
        Assert.Contains("CodexSidebarCollapsible", source);
        Assert.Contains("CodexSidebarVariant", source);
        Assert.Contains("CodexSidebarSide", source);
        Assert.Contains("TryHandleShortcut(Key key, KeyModifiers modifiers)", source);
        Assert.Contains("OpenChanged", source);
        Assert.Contains("internal bool CanToggle => IsEnabled", source);
        Assert.Contains("CommandProperty.Changed.AddClassHandler<CodexSidebarTrigger>((trigger, args) => trigger.OnCommandChanged", source);
        Assert.Contains("CommandProperty.Changed.AddClassHandler<CodexSidebarRail>((rail, args) => rail.OnCommandChanged", source);
        Assert.Contains("Classes.Set(\"can-toggle\", CanToggle);", source);
        Assert.Contains("Classes.Set(\"command-blocked\", Command is not null && IsEnabled && !IsLoading && !CanToggle);", source);
        Assert.Contains("Classes.Set(\"command-blocked\", Command is not null && IsEnabled && !CanToggle);", source);

        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSidebarProvider\"", style);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSidebarTrigger\"", style);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSidebarRail\"", style);
        Assert.Contains("<ControlTemplate TargetType=\"controls:CodexSidebarInset\"", style);
        Assert.Contains("state-expanded", style);
        Assert.Contains("state-collapsed", style);
        Assert.Contains("collapsible-offcanvas", style);
        Assert.Contains("collapsible-icon", style);
        Assert.Contains("variant-inset", style);
        Assert.Contains("side-right", style);
        Assert.Contains("PART_TriggerRoot", style);
        Assert.Contains("PART_RailLine", style);
        Assert.Contains("controls|CodexSidebarTrigger.command-blocked", style);
        Assert.Contains("controls|CodexSidebarRail.command-blocked", style);
        Assert.Contains("TransformOperationsTransition", style);
    }

    [Fact]
    public void SidebarMenuHoverBackgroundAnimatesOnlyTemplateSurface()
    {
        var style = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Themes", "Controls", "ApplicationShell.axaml"));

        var buttonHover = ExtractStyleBlock(style, "controls|CodexSidebarMenuButton:pointerover");
        var buttonHoverRoot = ExtractStyleBlock(style, "controls|CodexSidebarMenuButton:pointerover /template/ Border#PART_MenuButtonRoot");
        var buttonActive = ExtractStyleBlock(style, "controls|CodexSidebarMenuButton.active");
        var buttonActiveRoot = ExtractStyleBlock(style, "controls|CodexSidebarMenuButton.active /template/ Border#PART_MenuButtonRoot");

        Assert.DoesNotContain("Property=\"Background\"", buttonHover);
        Assert.DoesNotContain("Property=\"Background\"", buttonActive);
        Assert.Contains("Property=\"Background\"", buttonHoverRoot);
        Assert.Contains("Property=\"Background\"", buttonActiveRoot);
    }

    [Fact]
    public void ContextMenuMirrorsRadixContentAnimationHooks()
    {
        var style = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Themes", "Controls", "ContextMenu.axaml"));
        var source = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Controls", "CodexContextMenu.cs"));

        Assert.Contains("Opacity=\"0\"", style);
        Assert.Contains("translate(0px, -8px) scale(0.95)", style);
        Assert.Contains("translate(8px, 0px) scale(0.95)", style);
        Assert.Contains("translate(-8px, 0px) scale(0.95)", style);
        Assert.Contains("translate(0px, 8px) scale(0.95)", style);
        Assert.Contains("translate(0px, 0px) scale(1)", style);
        Assert.Contains("TransformOperationsTransition", style);
        Assert.Contains("RenderTransformOrigin", style);
        Assert.Contains("Dispatcher.UIThread.Post", source);
        Assert.Contains("context-menu-open", source);
        Assert.Contains("submenu-open", source);
    }

    [Fact]
    public void CollapsibleDefersUnmountUntilHeightAnimationCompletes()
    {
        var source = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Controls", "CodexCollapsible.cs"));

        Assert.Contains("MeasureContentHeight()", source);
        Assert.Contains("StartHeightAnimation(fromHeight, 0, collapseWhenDone: true)", source);
        Assert.Contains("if (_collapseWhenAnimationCompletes && !IsOpen)", source);
        Assert.Contains("SetValue(IsContentVisibleProperty, false)", source);
        Assert.Contains("CssEaseOut", source);
    }

    [Theory]
    [InlineData(100, 100, 24, 14, 44, 12)]
    [InlineData(78.5, 64.5, 31.2, 16.8, 58.4, 10.6)]
    [InlineData(120, 96, 48, 18, 76, 13)]
    public void UsagePieChartCentersValueAndLabelGroup(
        double centerX,
        double centerY,
        double valueWidth,
        double valueHeight,
        double labelWidth,
        double labelHeight)
    {
        const double spacing = 2d;

        var layout = CodexUsagePieChart.CalculateCenterLabelLayout(
            new Point(centerX, centerY),
            valueWidth,
            valueHeight,
            labelWidth,
            labelHeight,
            spacing);
        var groupBottom = layout.LabelOrigin.Y + labelHeight;
        var groupCenter = layout.ValueOrigin.Y + (groupBottom - layout.ValueOrigin.Y) / 2d;

        Assert.InRange(Math.Abs(groupCenter - centerY), 0d, 0.5d);
        Assert.Equal(centerX - valueWidth / 2d, layout.ValueOrigin.X, 6);
        Assert.Equal(centerX - labelWidth / 2d, layout.LabelOrigin.X, 6);
        Assert.Equal(valueHeight + spacing + labelHeight, layout.CombinedHeight, 6);
    }

    private static void AssertFocusVisibleSource(string sourcePath, params string[] expectedFragments)
    {
        var source = File.ReadAllText(sourcePath);

        foreach (var fragment in expectedFragments)
        {
            Assert.Contains(fragment, source);
        }

        Assert.Contains("[PseudoClasses(CodexFocusVisible.PseudoClass)]", source);
        Assert.Contains("OnGotFocus(FocusChangedEventArgs e)", source);
        Assert.Contains("CodexFocusVisible.FromFocusChange(e)", source);
        Assert.Contains("OnPointerPressed(PointerPressedEventArgs e)", source);
        Assert.Contains("PseudoClasses.Set(CodexFocusVisible.PseudoClass, false)", source);
    }

    private static void AssertPageChanged(
        CodexPaginationPageChangedEventArgs args,
        int oldPage,
        int newPage,
        CodexPaginationPageChangeSource source)
    {
        Assert.Equal(oldPage, args.OldPage);
        Assert.Equal(newPage, args.NewPage);
        Assert.Equal(source, args.Source);
    }

    private static void AssertNoHardcodedTokenizableTransitionDurations(string component, string style)
    {
        var hardcodedTransitionLines = style.Split(Environment.NewLine)
            .Where(line => line.Contains("Transition", StringComparison.Ordinal)
                           && !line.Contains("CrossFade", StringComparison.Ordinal)
                           && !line.Contains("PageSlide", StringComparison.Ordinal)
                           && line.Contains("Duration=\"0:0:0.", StringComparison.Ordinal))
            .ToArray();

        Assert.True(
            hardcodedTransitionLines.Length == 0,
            $"{component} contains hard-coded tokenizable transition durations: {string.Join(", ", hardcodedTransitionLines)}");
    }

    private static int Count(string text, string value)
    {
        return text.Split(value, StringSplitOptions.None).Length - 1;
    }

    private static string ExtractStyleBlock(string style, string selector)
    {
        var open = $"<Style Selector=\"{selector}\"";
        var start = style.IndexOf(open, StringComparison.Ordinal);

        Assert.True(start >= 0, $"Missing style selector '{selector}'.");

        var end = style.IndexOf("</Style>", start, StringComparison.Ordinal);

        Assert.True(end >= 0, $"Style selector '{selector}' is not closed.");

        return style[start..(end + "</Style>".Length)];
    }

    private static string FindRepositoryRoot()
    {
        return TestRepository.FindRoot();
    }

    private static void RaiseClick(MenuItem item)
    {
        var method = typeof(MenuItem).GetMethod(
            "Avalonia.Input.IClickableControl.RaiseClick",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(method);
        method.Invoke(item, null);
    }

    private static void InvokeButtonClick(Button button)
    {
        var method = button.GetType().GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);
        method.Invoke(button, null);
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
