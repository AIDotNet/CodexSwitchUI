using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using Xunit;

namespace CodexSwitchUI.Tests;

public class NavigationDataComponentTests
{
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
        var rankedBarChart = new CodexRankedBarChart
        {
            IsCompact = true,
            MaxVisibleItems = 4,
            ItemsSource =
            [
                new CodexRankedBarChartItem("OpenAI", 12, "12", "$0.04"),
                new CodexRankedBarChartItem("Anthropic", 8, "8", "$0.02")
            ]
        };
        var usagePieChart = new CodexUsagePieChart
        {
            IsCompact = true,
            TotalLabel = "Requests",
            TotalValue = "20",
            ItemsSource =
            [
                new CodexUsagePieChartItem("gpt-5.5", 12, "60%", "12 requests"),
                new CodexUsagePieChartItem("gpt-5.4", 8, "40%", "8 requests")
            ]
        };
        var avatar = new CodexAvatar { Size = CodexControlSize.Icon };
        var card = new CodexCard { IsInteractive = true, Title = "Metrics", Description = "Updated", Content = "42", Footer = "Footer" };
        var separator = new CodexSeparator { Orientation = Orientation.Vertical, Size = CodexControlSize.Large };
        var text = new CodexText { Role = CodexTextRole.Code };

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
        Assert.Equal(4, rankedBarChart.MaxVisibleItems);
        Assert.NotNull(rankedBarChart.ItemsSource);
        Assert.Contains("compact", usagePieChart.Classes);
        Assert.Equal("Requests", usagePieChart.TotalLabel);
        Assert.Equal("20", usagePieChart.TotalValue);
        Assert.NotNull(usagePieChart.ItemsSource);
        Assert.Contains("size-icon", avatar.Classes);
        Assert.Contains("interactive", card.Classes);
        Assert.True(card.HasHeader);
        Assert.True(card.HasContent);
        Assert.True(card.HasFooter);
        Assert.Contains("vertical", separator.Classes);
        Assert.Contains("size-lg", separator.Classes);
        Assert.Contains("role-code", text.Classes);
    }

    [Theory]
    [InlineData("Tabs.axaml", "PART_Indicator")]
    [InlineData("NavigationMenu.axaml", "PART_Viewport")]
    [InlineData("Menu.axaml", "PART_Surface")]
    [InlineData("ContextMenu.axaml", "PART_Surface")]
    [InlineData("Command.axaml", "PART_Input")]
    [InlineData("Collapsible.axaml", "PART_ContentClip")]
    [InlineData("Table.axaml", "PART_RowRoot")]
    [InlineData("Avatar.axaml", "ControlTemplate")]
    [InlineData("Card.axaml", "ControlTemplate")]
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
    [InlineData("NavigationMenu.axaml", "PART_List", "PART_TriggerRoot", "PART_Viewport", "PART_ContentTransitionHost", "PART_Indicator", "CompositePageTransition", "PageSlide", "CrossFade", "motion-from-start", "motion-from-end", "ViewportWidth", "ViewportMinHeight", "controls|CodexNavigationMenuLink")]
    [InlineData("Menu.axaml", "PART_ItemRoot", "PART_Icon", "PART_Shortcut", "PART_Popup", "PART_SubMenuSurface", "PART_SubMenuItemsPresenter", "PART_SubMenuArrow", "controls|CodexMenuGroup")]
    [InlineData("ContextMenu.axaml", "PART_ItemsPresenter", "PART_ItemRoot", "PART_Icon", "PART_Shortcut", "PART_Popup", "PART_SubMenuSurface", "PART_SubMenuItemsPresenter", "PART_SubMenuArrow", "PART_Radio", "controls|CodexContextMenuGroup", "controls|CodexContextMenuLabel", "controls|CodexContextMenuShortcut")]
    [InlineData("Command.axaml", "PART_InputRoot", "PART_TextPresenter", "PART_Icon", "PART_Shortcut")]
    [InlineData("Collapsible.axaml", "PART_Trigger", "PART_Chevron", "PART_ContentClip", "PART_ContentMeasure", "open", "closed", "TransformOperationsTransition")]
    [InlineData("Table.axaml", "PART_TableSurface", "PART_Head", "PART_Cell", "compact")]
    [InlineData("RankedBarChart.axaml", "MutedForeground", "TrackBrush", "AccentBrush", "SecondaryAccentBrush", "TertiaryAccentBrush", "compact")]
    [InlineData("UsagePieChart.axaml", "MutedForeground", "TrackBrush", "SliceBorderBrush", "CenterFillBrush", "TooltipBackground", "TooltipForeground", "TooltipBorderBrush", "compact")]
    [InlineData("Card.axaml", "PART_Surface", "PART_Header", "PART_Footer", "interactive")]
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
        var rankedBarChart = File.ReadAllText(Path.Combine(root, "RankedBarChart.axaml"));
        var usagePieChart = File.ReadAllText(Path.Combine(root, "UsagePieChart.axaml"));

        Assert.Contains(":selected", tabs);
        Assert.Contains(":pointerover", tabs);
        Assert.Contains(":disabled", tabs);
        Assert.Contains("variant-line", tabs);
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
        Assert.Contains("controls|CodexCommandEmpty", command);
        Assert.Contains("controls|CodexCommandLoading", command);
        Assert.Contains("controls|CodexCommand Separator", command);
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
        Assert.Contains("controls|CodexTableHeader", table);
        Assert.Contains("controls|CodexTable.compact", table);
        Assert.Contains("controls|CodexTable.hoverable controls|CodexTableRow:pointerover", table);
        Assert.Contains("controls|CodexRankedBarChart.compact", rankedBarChart);
        Assert.Contains("CodexSwitch.DisabledOpacity", rankedBarChart);
        Assert.Contains("controls|CodexUsagePieChart.compact", usagePieChart);
        Assert.Contains("CodexSwitch.DisabledOpacity", usagePieChart);

        // Next visual-pass hook: add rendered snapshot coverage for submenu popups and table column alignment.
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
            File.ReadAllText(Path.Combine(controlsRoot, "RankedBarChart.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "UsagePieChart.axaml")),
            File.ReadAllText(Path.Combine(controlsRoot, "Card.axaml")),
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
    public void MenuAndCommandItemsUseShadcnFocusWithoutPressedChrome()
    {
        var root = Path.Combine(FindRepositoryRoot(), "src", "CodexSwitchUI", "Themes", "Controls");
        var menu = File.ReadAllText(Path.Combine(root, "Menu.axaml"));
        var command = File.ReadAllText(Path.Combine(root, "Command.axaml"));

        var menuFocus = ExtractStyleBlock(menu, "controls|CodexMenu MenuItem:focus /template/ Border#PART_ItemRoot");
        var menuPressed = ExtractStyleBlock(menu, "controls|CodexMenu MenuItem:pressed /template/ Border#PART_ItemRoot");
        var commandFocus = ExtractStyleBlock(command, "controls|CodexCommandItem:focus /template/ Border#PART_ItemRoot");
        var commandItem = ExtractStyleBlock(command, "controls|CodexCommandItem");

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
}
