using Avalonia.Controls;
using Avalonia.Input;
using CodexSwitchUI.Controls;
using Avalonia.Layout;
using System.Collections.Generic;
using System.Windows.Input;
using Xunit;

namespace CodexSwitchUI.Tests;

public class ControlStateTests
{
    private static readonly VariantCase[] VariantCases =
    [
        new("Button", "controls|CodexButton", () => new CodexButton(), (control, variant) => ((CodexButton)control).Variant = variant),
        new("Alert", "controls|CodexAlert", () => new CodexAlert(), (control, variant) => ((CodexAlert)control).Variant = variant),
        new("Badge", "controls|CodexBadge", () => new CodexBadge(), (control, variant) => ((CodexBadge)control).Variant = variant),
        new("Item", "controls|CodexItem", () => new CodexItem(), (control, variant) => ((CodexItem)control).Variant = variant),
        new("EmptyState", "controls|CodexEmptyState", () => new CodexEmptyState(), (control, variant) => ((CodexEmptyState)control).Variant = variant),
        new("Toast", "controls|CodexToast", () => new CodexToast(), (control, variant) => ((CodexToast)control).Variant = variant)
    ];

    private static readonly SizeCase[] SizeCases =
    [
        new("Button", "controls|CodexButton", () => new CodexButton(), (control, size) => ((CodexButton)control).Size = size),
        new("DropdownButton", "controls|CodexDropdownButton", () => new CodexDropdownButton(), (control, size) => ((CodexDropdownButton)control).Size = size),
        new("SplitButton", "controls|CodexSplitButton", () => new CodexSplitButton(), (control, size) => ((CodexSplitButton)control).Size = size),
        new("Field", "controls|CodexField", () => new CodexField(), (control, size) => ((CodexField)control).Size = size),
        new("Input", "controls|CodexTextBox", () => new CodexTextBox(), (control, size) => ((CodexTextBox)control).Size = size),
        new("Textarea", "controls|CodexTextarea", () => new CodexTextarea(), (control, size) => ((CodexTextarea)control).Size = size),
        new("InputOtp", "controls|CodexInputOtp", () => new CodexInputOtp(), (control, size) => ((CodexInputOtp)control).Size = size),
        new("Select", "controls|CodexSelect", () => new CodexSelect(), (control, size) => ((CodexSelect)control).Size = size),
        new("Combobox", "controls|CodexCombobox", () => new CodexCombobox(), (control, size) => ((CodexCombobox)control).Size = size),
        new("NativeSelect", "controls|CodexNativeSelect", () => new CodexNativeSelect(), (control, size) => ((CodexNativeSelect)control).Size = size),
        new("Calendar", "controls|CodexCalendar", () => new CodexCalendar(), (control, size) => ((CodexCalendar)control).Size = size),
        new("DatePicker", "controls|CodexDatePicker", () => new CodexDatePicker(), (control, size) => ((CodexDatePicker)control).Size = size),
        new("Checkbox", "controls|CodexCheckBox", () => new CodexCheckBox(), (control, size) => ((CodexCheckBox)control).Size = size),
        new("Radio", "controls|CodexRadio", () => new CodexRadio(), (control, size) => ((CodexRadio)control).Size = size),
        new("RadioGroup", "controls|CodexRadioGroup", () => new CodexRadioGroup(), (control, size) => ((CodexRadioGroup)control).Size = size),
        new("Switch", "controls|CodexSwitch", () => new CodexSwitch(), (control, size) => ((CodexSwitch)control).Size = size),
        new("Slider", "controls|CodexSlider", () => new CodexSlider(), (control, size) => ((CodexSlider)control).Size = size),
        new("Pagination", "controls|CodexPagination", () => new CodexPagination(), (control, size) => ((CodexPagination)control).Size = size),
        new("ScrollArea", "controls|CodexScrollArea", () => new CodexScrollArea(), (control, size) => ((CodexScrollArea)control).Size = size),
        new("EmptyState", "controls|CodexEmptyState", () => new CodexEmptyState(), (control, size) => ((CodexEmptyState)control).Size = size),
        new("Chart", "controls|CodexChartContainer", () => new CodexChartContainer(), (control, size) => ((CodexChartContainer)control).Size = size),
        new("BarChart", "controls|CodexBarChart", () => new CodexBarChart(), (control, size) => ((CodexBarChart)control).Size = size),
        new("LineChart", "controls|CodexLineChart", () => new CodexLineChart(), (control, size) => ((CodexLineChart)control).Size = size),
        new("Item", "controls|CodexItem", () => new CodexItem(), (control, size) => ((CodexItem)control).Size = size),
        new("AspectRatio", "controls|CodexAspectRatio", () => new CodexAspectRatio(), (control, size) => ((CodexAspectRatio)control).Size = size),
        new("Carousel", "controls|CodexCarousel", () => new CodexCarousel(), (control, size) => ((CodexCarousel)control).Size = size),
        new("Resizable", "controls|CodexResizablePanelGroup", () => new CodexResizablePanelGroup(), (control, size) => ((CodexResizablePanelGroup)control).Size = size),
        new("Tabs", "controls|CodexTabs", () => new CodexTabs(), (control, size) => ((CodexTabs)control).Size = size),
        new("Tooltip", "controls|CodexTooltip", () => new CodexTooltip(), (control, size) => ((CodexTooltip)control).Size = size),
        new("HoverCard", "controls|CodexHoverCard", () => new CodexHoverCard(), (control, size) => ((CodexHoverCard)control).Size = size),
        new("AlertDialog", "controls|CodexAlertDialog", () => new CodexAlertDialog(), (control, size) => ((CodexAlertDialog)control).Size = size),
        new("Drawer", "controls|CodexDrawer", () => new CodexDrawer(), (control, size) => ((CodexDrawer)control).Size = size),
        new("Menubar", "controls|CodexMenubar", () => new CodexMenubar(), (control, size) => ((CodexMenubar)control).Size = size),
        new("Menu", "controls|CodexMenu", () => new CodexMenu(), (control, size) => ((CodexMenu)control).Size = size),
        new("ContextMenu", "controls|CodexContextMenu", () => new CodexContextMenu(), (control, size) => ((CodexContextMenu)control).Size = size),
        new("Accordion", "controls|CodexAccordion", () => new CodexAccordion(), (control, size) => ((CodexAccordion)control).Size = size),
        new("Collapsible", "controls|CodexCollapsible", () => new CodexCollapsible(), (control, size) => ((CodexCollapsible)control).Size = size),
        new("Avatar", "controls|CodexAvatar", () => new CodexAvatar(), (control, size) => ((CodexAvatar)control).Size = size),
        new("Avatar", "controls|CodexAvatarGroup", () => new CodexAvatarGroup(), (control, size) => ((CodexAvatarGroup)control).Size = size),
        new("Avatar", "controls|CodexAvatarGroupCount", () => new CodexAvatarGroupCount(), (control, size) => ((CodexAvatarGroupCount)control).Size = size),
        new("Separator", "controls|CodexSeparator", () => new CodexSeparator(), (control, size) => ((CodexSeparator)control).Size = size),
        new("Kbd", "controls|CodexKbd", () => new CodexKbd(), (control, size) => ((CodexKbd)control).Size = size)
    ];

    private static readonly IntentCase[] IntentCases =
    [
        new("Field", "controls|CodexField", () => new CodexField(), (control, intent) => ((CodexField)control).Intent = intent),
        new("Input", "controls|CodexTextBox", () => new CodexTextBox(), (control, intent) => ((CodexTextBox)control).Intent = intent),
        new("Textarea", "controls|CodexTextarea", () => new CodexTextarea(), (control, intent) => ((CodexTextarea)control).Intent = intent),
        new("InputOtp", "controls|CodexInputOtp", () => new CodexInputOtp(), (control, intent) => ((CodexInputOtp)control).Intent = intent),
        new("Select", "controls|CodexSelect", () => new CodexSelect(), (control, intent) => ((CodexSelect)control).Intent = intent),
        new("Combobox", "controls|CodexCombobox", () => new CodexCombobox(), (control, intent) => ((CodexCombobox)control).Intent = intent),
        new("NativeSelect", "controls|CodexNativeSelect", () => new CodexNativeSelect(), (control, intent) => ((CodexNativeSelect)control).Intent = intent),
        new("Calendar", "controls|CodexCalendar", () => new CodexCalendar(), (control, intent) => ((CodexCalendar)control).Intent = intent),
        new("DatePicker", "controls|CodexDatePicker", () => new CodexDatePicker(), (control, intent) => ((CodexDatePicker)control).Intent = intent),
        new("Checkbox", "controls|CodexCheckBox", () => new CodexCheckBox(), (control, intent) => ((CodexCheckBox)control).Intent = intent),
        new("Radio", "controls|CodexRadio", () => new CodexRadio(), (control, intent) => ((CodexRadio)control).Intent = intent),
        new("RadioGroup", "controls|CodexRadioGroup", () => new CodexRadioGroup(), (control, intent) => ((CodexRadioGroup)control).Intent = intent),
        new("Switch", "controls|CodexSwitch", () => new CodexSwitch(), (control, intent) => ((CodexSwitch)control).Intent = intent),
        new("Slider", "controls|CodexSlider", () => new CodexSlider(), (control, intent) => ((CodexSlider)control).Intent = intent)
    ];

    [Fact]
    public void ButtonSyncsVariantAndSizeClasses()
    {
        var button = new CodexButton
        {
            Variant = CodexControlVariant.Secondary,
            Size = CodexControlSize.Icon
        };

        Assert.Contains("variant-secondary", button.Classes);
        Assert.Contains("size-icon", button.Classes);
        Assert.DoesNotContain("variant-default", button.Classes);
    }

    [Fact]
    public void TextBoxSyncsIntentClasses()
    {
        var textBox = new CodexTextBox
        {
            Intent = CodexControlIntent.Error
        };

        Assert.Contains("intent-error", textBox.Classes);
        Assert.DoesNotContain("intent-default", textBox.Classes);
    }

    [Fact]
    public void BadgeSyncsVariantClasses()
    {
        var badge = new CodexBadge
        {
            Variant = CodexControlVariant.Success
        };

        Assert.Contains("variant-success", badge.Classes);
        Assert.DoesNotContain("variant-default", badge.Classes);
    }

    [Fact]
    public void SeparatorSyncsOrientationClasses()
    {
        var separator = new CodexSeparator
        {
            Orientation = Orientation.Vertical
        };

        Assert.Contains("vertical", separator.Classes);
        Assert.DoesNotContain("horizontal", separator.Classes);
    }

    [Fact]
    public void TabsSyncsVariantClasses()
    {
        var tabs = new CodexTabs
        {
            Variant = CodexTabsVariant.Line
        };

        Assert.Contains("variant-line", tabs.Classes);
        Assert.DoesNotContain("variant-default", tabs.Classes);
    }

    [Fact]
    public void DirectionSyncsFlowDirectionAndClasses()
    {
        var direction = new CodexDirection();

        Assert.Equal(Avalonia.Media.FlowDirection.LeftToRight, direction.FlowDirection);
        Assert.Contains("direction-ltr", direction.Classes);
        Assert.DoesNotContain("direction-rtl", direction.Classes);

        var raised = false;
        direction.DirectionChanged += (_, args) =>
        {
            raised = true;
            Assert.Equal(CodexDirectionMode.LeftToRight, args.OldDirection);
            Assert.Equal(CodexDirectionMode.RightToLeft, args.NewDirection);
            Assert.Equal(Avalonia.Media.FlowDirection.RightToLeft, args.FlowDirection);
        };

        direction.Direction = CodexDirectionMode.RightToLeft;

        Assert.True(raised);
        Assert.True(direction.IsRightToLeft);
        Assert.Equal(Avalonia.Media.FlowDirection.RightToLeft, direction.FlowDirection);
        Assert.Contains("direction-rtl", direction.Classes);
        Assert.DoesNotContain("direction-ltr", direction.Classes);
    }

    [Fact]
    public void SidebarAndSegmentedButtonsSelectExclusivelyOnClick()
    {
        var home = new CodexSideNavItem { Content = "Home", IsSelected = true };
        var logs = new CodexSideNavItem { Content = "Logs" };
        _ = new StackPanel
        {
            Children =
            {
                home,
                logs
            }
        };

        InvokeClick(logs);

        Assert.False(home.IsSelected);
        Assert.True(logs.IsSelected);
        Assert.DoesNotContain("selected", home.Classes);
        Assert.Contains("selected", logs.Classes);

        var daily = new CodexSegmentedButton { Content = "24h", IsSelected = true };
        var weekly = new CodexSegmentedButton { Content = "7d" };
        var monthly = new CodexSegmentedButton { Content = "30d" };
        _ = new StackPanel
        {
            Children =
            {
                daily,
                weekly,
                monthly
            }
        };

        InvokeClick(monthly);

        Assert.False(daily.IsSelected);
        Assert.False(weekly.IsSelected);
        Assert.True(monthly.IsSelected);
        Assert.DoesNotContain("selected", daily.Classes);
        Assert.DoesNotContain("selected", weekly.Classes);
        Assert.Contains("selected", monthly.Classes);
    }

    [Fact]
    public void SideNavPublishesWebStyleValueChangedOnSelection()
    {
        var root = FindRepositoryRoot();
        var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "ApplicationShell.axaml"));
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexNavigationPrimitives.cs"));
        var changes = new List<CodexSideNavValueChangedEventArgs>();
        var home = new CodexSideNavItem { Content = "Home", Value = "home" };
        var sessions = new CodexSideNavItem { Content = "Sessions", Value = "sessions" };
        var disabled = new CodexSideNavItem { Content = "Disabled", Value = "disabled", IsEnabled = false };
        var nav = new CodexSideNav
        {
            Content = new StackPanel
            {
                Children =
                {
                    home,
                    sessions,
                    disabled
                }
            }
        };
        nav.SelectedValue = "home";
        nav.ValueChanged += (_, args) => changes.Add(args);

        Assert.True(home.IsSelected);
        Assert.False(sessions.IsSelected);
        Assert.Contains("public class CodexSideNav : ContentControl", source);
        Assert.Contains("SelectedValueProperty", source);
        Assert.Contains("public event EventHandler<CodexSideNavValueChangedEventArgs>? ValueChanged;", source);
        Assert.Contains("public static readonly StyledProperty<string?> ValueProperty", source);
        Assert.Contains("ControlTemplate TargetType=\"controls:CodexSideNav\"", style);

        InvokeClick(sessions);

        Assert.False(home.IsSelected);
        Assert.True(sessions.IsSelected);
        Assert.Equal("sessions", nav.SelectedValue);
        var clickChange = Assert.Single(changes);
        Assert.Same(home, clickChange.OldItem);
        Assert.Same(sessions, clickChange.NewItem);
        Assert.Equal(0, clickChange.OldIndex);
        Assert.Equal(1, clickChange.NewIndex);
        Assert.Equal("home", clickChange.OldValue);
        Assert.Equal("sessions", clickChange.NewValue);

        InvokeClick(disabled);

        Assert.True(sessions.IsSelected);
        Assert.Equal("sessions", nav.SelectedValue);
        Assert.Single(changes);

        nav.SelectedValue = "home";

        Assert.True(home.IsSelected);
        Assert.False(sessions.IsSelected);
        Assert.Equal(2, changes.Count);
        Assert.Same(sessions, changes[1].OldItem);
        Assert.Same(home, changes[1].NewItem);
        Assert.Equal("sessions", changes[1].OldValue);
        Assert.Equal("home", changes[1].NewValue);
    }

    [Fact]
    public void SegmentedControlOwnsAnimatedSelectionIndicator()
    {
        var root = FindRepositoryRoot();
        var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "ApplicationShell.axaml"));
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexNavigationPrimitives.cs"));
        var segmentedControl = new CodexSegmentedControl();

        Assert.False(segmentedControl.IsIndicatorVisible);
        Assert.Equal(0, segmentedControl.IndicatorWidth);
        Assert.Equal(0, segmentedControl.IndicatorHeight);
        Assert.Equal(default, segmentedControl.IndicatorMargin);

        Assert.Contains("IndicatorWidthProperty", source);
        Assert.Contains("IndicatorHeightProperty", source);
        Assert.Contains("IndicatorMarginProperty", source);
        Assert.Contains("SelectedValueProperty", source);
        Assert.Contains("public event EventHandler<CodexSegmentedControlValueChangedEventArgs>? ValueChanged;", source);
        Assert.Contains("public static readonly StyledProperty<string?> ValueProperty", source);
        Assert.Contains("UpdateSelectionIndicator()", source);
        Assert.Contains("QueueSelectionIndicatorUpdate()", source);
        Assert.Contains("PART_IndicatorHost", source);
        Assert.Contains("TranslatePoint(new Point(0, 0), _indicatorHost)", source);
        Assert.DoesNotContain("TranslatePoint(new Point(0, 0), this)", source);
        Assert.Contains("PART_Indicator", style);
        Assert.Contains("<Canvas x:Name=\"PART_IndicatorHost\"", style);
        Assert.Contains("IsHitTestVisible=\"False\"", style);
        Assert.Contains("Width=\"{TemplateBinding IndicatorWidth}\"", style);
        Assert.Contains("Height=\"{TemplateBinding IndicatorHeight}\"", style);
        Assert.Contains("Margin=\"{TemplateBinding IndicatorMargin}\"", style);
        Assert.Contains("ThicknessTransition Property=\"Margin\"", style);
        Assert.Contains("DoubleTransition Property=\"Width\"", style);
        Assert.Contains("DoubleTransition Property=\"Height\"", style);
        Assert.Contains("<Setter Property=\"Background\" Value=\"Transparent\" />", style);
    }

    [Fact]
    public void SegmentedControlPublishesWebStyleValueChangedOnSelection()
    {
        var changes = new List<CodexSegmentedControlValueChangedEventArgs>();
        var preview = new CodexSegmentedButton { Content = "Preview", Value = "preview" };
        var code = new CodexSegmentedButton { Content = "Code", Value = "code", IsSelected = true };
        var events = new CodexSegmentedButton { Content = "Events", Value = "events" };
        var control = new CodexSegmentedControl
        {
            SelectedValue = "code",
            Content = new StackPanel
            {
                Children =
                {
                    preview,
                    code,
                    events
                }
            }
        };
        control.ValueChanged += (_, args) => changes.Add(args);

        InvokeClick(events);

        Assert.False(preview.IsSelected);
        Assert.False(code.IsSelected);
        Assert.True(events.IsSelected);
        Assert.Equal("events", control.SelectedValue);
        var change = Assert.Single(changes);
        Assert.Same(code, change.OldItem);
        Assert.Same(events, change.NewItem);
        Assert.Equal(1, change.OldIndex);
        Assert.Equal(2, change.NewIndex);
        Assert.Equal("code", change.OldValue);
        Assert.Equal("events", change.NewValue);

        control.SelectedValue = "preview";

        Assert.True(preview.IsSelected);
        Assert.False(events.IsSelected);
        Assert.Equal(2, changes.Count);
        Assert.Same(events, changes[1].OldItem);
        Assert.Same(preview, changes[1].NewItem);
        Assert.Equal("events", changes[1].OldValue);
        Assert.Equal("preview", changes[1].NewValue);
    }

    [Fact]
    public void SegmentedButtonsWithCommandsUseControlledSelection()
    {
        var executed = false;
        var current = new CodexSegmentedButton { Content = "Current", IsSelected = true };
        var controlled = new CodexSegmentedButton
        {
            Content = "Controlled",
            Command = new TestCommand(() => executed = true)
        };
        _ = new StackPanel
        {
            Children =
            {
                current,
                controlled
            }
        };

        InvokeClick(controlled);

        Assert.True(executed);
        Assert.True(current.IsSelected);
        Assert.False(controlled.IsSelected);
    }

    [Fact]
    public void CommandItemsAndProviderCardsSelectExclusivelyOnClick()
    {
        var open = new CodexCommandItem { Content = "Open", IsActive = true };
        var switchTheme = new CodexCommandItem { Content = "Switch theme" };
        _ = new StackPanel
        {
            Children =
            {
                open,
                switchTheme
            }
        };

        Assert.IsAssignableFrom<Button>(switchTheme);

        InvokeClick(switchTheme);

        Assert.False(open.IsActive);
        Assert.True(switchTheme.IsActive);
        Assert.DoesNotContain("active", open.Classes);
        Assert.Contains("active", switchTheme.Classes);

        var openAi = new CodexProviderCard { Header = "OpenAI", IsActive = true };
        var anthropic = new CodexProviderCard { Header = "Anthropic" };
        _ = new StackPanel
        {
            Children =
            {
                openAi,
                anthropic
            }
        };

        InvokeClick(anthropic);

        Assert.False(openAi.IsActive);
        Assert.True(anthropic.IsActive);
        Assert.DoesNotContain("active", openAi.Classes);
        Assert.Contains("active", anthropic.Classes);
    }

    [Fact]
    public void LoadingCommandSuppressesItemActivationAndCommandExecution()
    {
        var executionCount = 0;
        var active = new CodexCommandItem { Content = "Current", IsActive = true };
        var target = new CodexCommandItem
        {
            Content = "Run",
            Command = new TestCommand(() => executionCount++)
        };
        var command = new CodexCommand
        {
            IsLoading = true,
            Content = new StackPanel
            {
                Children =
                {
                    active,
                    target
                }
            }
        };

        InvokeClick(target);

        Assert.Contains("loading", command.Classes);
        Assert.Equal(0, executionCount);
        Assert.True(active.IsActive);
        Assert.False(target.IsActive);

        command.IsLoading = false;
        InvokeClick(target);

        Assert.Equal(1, executionCount);
        Assert.False(active.IsActive);
        Assert.True(target.IsActive);
    }

    [Fact]
    public void CommandFiltersKeyboardNavigatesAndPublishesSelection()
    {
        var executionCount = 0;
        var selectedValues = new List<string?>();
        var provider = new CodexCommandItem
        {
            Content = "Switch provider",
            Value = "provider",
            Keywords = "model route",
            Command = new TestCommand(() => executionCount++)
        };
        var logs = new CodexCommandItem { Content = "Open logs", Value = "logs", Keywords = "diagnostics" };
        var preferences = new CodexCommandItem
        {
            Content = "Provider preferences",
            Value = "preferences",
            Keywords = "provider settings",
            Command = new TestCommand(() => executionCount++)
        };
        var separator = new CodexCommandSeparator();
        var command = new CodexCommand
        {
            SearchText = "provider",
            Content = new CodexCommandList
            {
                Items =
                {
                    new CodexCommandGroup
                    {
                        Header = "Results",
                        Items =
                        {
                            provider,
                            logs
                        }
                    },
                    separator,
                    new CodexCommandGroup
                    {
                        Header = "Settings",
                        Items =
                        {
                            preferences
                        }
                    }
                }
            }
        };
        command.ItemSelected += (_, args) => selectedValues.Add(args.Value);
        command.SearchText = "provider";

        Assert.Contains("searching", command.Classes);
        Assert.Contains("filtering", command.Classes);
        Assert.Contains("has-results", command.Classes);
        Assert.DoesNotContain("empty-results", command.Classes);
        Assert.DoesNotContain("filtered-out", provider.Classes);
        Assert.DoesNotContain("filtered-out", preferences.Classes);
        Assert.Contains("filtered-out", logs.Classes);
        Assert.Contains("filtered-out", separator.Classes);

        Assert.True(provider.IsActive);
        Assert.True(command.TryHandleNavigationKey(Key.Down));
        Assert.True(preferences.IsActive);
        Assert.True(command.TrySelectActiveItem());

        Assert.Equal(1, executionCount);
        Assert.Same(preferences, command.SelectedItem);
        Assert.Equal(["preferences"], selectedValues);

        command.SearchText = "missing";

        Assert.Contains("empty-results", command.Classes);
        Assert.Contains("filtered-out", provider.Classes);
        Assert.Contains("filtered-out", preferences.Classes);
        Assert.False(command.TryHandleNavigationKey(Key.Down));
    }

    [Fact]
    public void SkeletonMatchesShadcnPulseDefaults()
    {
        var skeleton = new CodexSkeleton();

        Assert.False(skeleton.Focusable);
        Assert.False(skeleton.IsHitTestVisible);
        Assert.Contains("animated", skeleton.Classes);
        Assert.DoesNotContain("static", skeleton.Classes);
        Assert.Equal(1, skeleton.PulseOpacity);
        Assert.Equal(0.5, skeleton.PulseLowOpacity);
        Assert.Equal(1, skeleton.PulseHighOpacity);
        Assert.Equal(TimeSpan.FromSeconds(2), skeleton.PulseDuration);
        Assert.Equal(0, skeleton.ShimmerOpacity);

        skeleton.IsAnimated = false;

        Assert.Contains("static", skeleton.Classes);
        Assert.DoesNotContain("animated", skeleton.Classes);
        Assert.Equal(1, skeleton.PulseOpacity);
        Assert.Equal(0, skeleton.ShimmerOpacity);
    }

    [Fact]
    public void EveryVariantValueSyncsExactlyOneVariantClass()
    {
        var classNames = Enum.GetValues<CodexControlVariant>().Select(VariantClass).ToArray();

        foreach (var variantCase in VariantCases)
        {
            var control = variantCase.Create();
            foreach (var variant in Enum.GetValues<CodexControlVariant>())
            {
                variantCase.SetVariant(control, variant);
                AssertExclusiveClass(control, VariantClass(variant), classNames, $"{variantCase.Component} {variant}");
            }
        }
    }

    [Fact]
    public void EverySizeValueSyncsExactlyOneSizeClass()
    {
        var classNames = Enum.GetValues<CodexControlSize>().Select(SizeClass).ToArray();

        foreach (var sizeCase in SizeCases)
        {
            var control = sizeCase.Create();
            foreach (var size in Enum.GetValues<CodexControlSize>())
            {
                sizeCase.SetSize(control, size);
                AssertExclusiveClass(control, SizeClass(size), classNames, $"{sizeCase.Component} {size}");
            }
        }
    }

    [Fact]
    public void EveryIntentValueSyncsExactlyOneIntentClass()
    {
        var classNames = Enum.GetValues<CodexControlIntent>().Select(IntentClass).ToArray();

        foreach (var intentCase in IntentCases)
        {
            var control = intentCase.Create();
            foreach (var intent in Enum.GetValues<CodexControlIntent>())
            {
                intentCase.SetIntent(control, intent);
                AssertExclusiveClass(control, IntentClass(intent), classNames, $"{intentCase.Component} {intent}");
            }
        }
    }

    [Fact]
    public void NonDefaultVariantClassesHaveMatchingStyleSelectors()
    {
        var root = FindRepositoryRoot();
        var failures = new List<string>();

        foreach (var variantCase in VariantCases)
        {
            var style = ReadStyle(root, variantCase.Component);
            foreach (var variant in Enum.GetValues<CodexControlVariant>().Where(value => value != CodexControlVariant.Default))
            {
                var selector = $"{variantCase.StyleSelector}.{VariantClass(variant)}";
                if (!style.Contains(selector, StringComparison.Ordinal))
                {
                    failures.Add($"{variantCase.Component}: missing selector '{selector}'.");
                }
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void NonDefaultSizeClassesHaveMatchingStyleSelectors()
    {
        var root = FindRepositoryRoot();
        var failures = new List<string>();

        foreach (var sizeCase in SizeCases)
        {
            var style = ReadStyle(root, sizeCase.Component);
            foreach (var size in Enum.GetValues<CodexControlSize>().Where(value => value != CodexControlSize.Medium))
            {
                var selector = $"{sizeCase.StyleSelector}.{SizeClass(size)}";
                if (!style.Contains(selector, StringComparison.Ordinal))
                {
                    failures.Add($"{sizeCase.Component}: missing selector '{selector}'.");
                }
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void NonDefaultIntentClassesHaveMatchingStyleSelectors()
    {
        var root = FindRepositoryRoot();
        var failures = new List<string>();

        foreach (var intentCase in IntentCases)
        {
            var style = ReadStyle(root, intentCase.Component);
            foreach (var intent in Enum.GetValues<CodexControlIntent>().Where(value => value != CodexControlIntent.Default))
            {
                var selector = $"{intentCase.StyleSelector}.{IntentClass(intent)}";
                if (!style.Contains(selector, StringComparison.Ordinal))
                {
                    failures.Add($"{intentCase.Component}: missing selector '{selector}'.");
                }
            }
        }

        AssertNoFailures(failures);
    }

    private static void AssertExclusiveClass(Control control, string expected, IEnumerable<string> allClasses, string context)
    {
        foreach (var className in allClasses)
        {
            Assert.True(
                control.Classes.Contains(className) == (className == expected),
                $"{context}: expected only '{expected}' among [{string.Join(", ", allClasses)}], but '{className}' presence was {control.Classes.Contains(className)}.");
        }
    }

    private static string VariantClass(CodexControlVariant variant)
    {
        return variant switch
        {
            CodexControlVariant.Default => "variant-default",
            CodexControlVariant.Secondary => "variant-secondary",
            CodexControlVariant.Destructive => "variant-destructive",
            CodexControlVariant.Outline => "variant-outline",
            CodexControlVariant.Ghost => "variant-ghost",
            CodexControlVariant.Link => "variant-link",
            CodexControlVariant.Success => "variant-success",
            CodexControlVariant.Warning => "variant-warning",
            _ => throw new ArgumentOutOfRangeException(nameof(variant), variant, null)
        };
    }

    private static string SizeClass(CodexControlSize size)
    {
        return size switch
        {
            CodexControlSize.Small => "size-sm",
            CodexControlSize.Medium => "size-md",
            CodexControlSize.Large => "size-lg",
            CodexControlSize.Icon => "size-icon",
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
        };
    }

    private static string IntentClass(CodexControlIntent intent)
    {
        return intent switch
        {
            CodexControlIntent.Default => "intent-default",
            CodexControlIntent.Error => "intent-error",
            CodexControlIntent.Success => "intent-success",
            CodexControlIntent.Warning => "intent-warning",
            _ => throw new ArgumentOutOfRangeException(nameof(intent), intent, null)
        };
    }

    private static string FindRepositoryRoot()
    {
        return TestRepository.FindRoot();
    }

    private static void InvokeClick(Button button)
    {
        var onClick = button.GetType().GetMethod("OnClick", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(onClick);
        onClick.Invoke(button, null);
    }

    private static string ReadStyle(string root, string component)
    {
        return File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml"));
    }

    private static void AssertNoFailures(IReadOnlyCollection<string> failures)
    {
        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    private sealed record VariantCase(string Component, string StyleSelector, Func<Control> Create, Action<Control, CodexControlVariant> SetVariant);

    private sealed record SizeCase(string Component, string StyleSelector, Func<Control> Create, Action<Control, CodexControlSize> SetSize);

    private sealed record IntentCase(string Component, string StyleSelector, Func<Control> Create, Action<Control, CodexControlIntent> SetIntent);

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
