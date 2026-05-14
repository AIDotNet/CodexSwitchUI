using CodexSwitchUI.Tokens;
using Xunit;

namespace CodexSwitchUI.Tests;

public class ComponentStructureTests
{
    private static readonly string[] ExpectedTopLevelControls =
    [
        "CodexButton",
        "CodexTextBox",
        "CodexSelect",
        "CodexCheckBox",
        "CodexRadio",
        "CodexSwitch",
        "CodexSlider",
        "CodexTabs",
        "CodexNavigationMenu",
        "CodexCard",
        "CodexPopover",
        "CodexDialog",
        "CodexToast",
        "CodexSonner",
        "CodexAlert",
        "CodexBadge",
        "CodexAvatar",
        "CodexSpinner",
        "CodexProgress",
        "CodexTable",
        "CodexMenu",
        "CodexContextMenu",
        "CodexCommand",
        "CodexCollapsible",
        "CodexSeparator",
        "CodexSkeleton"
    ];

    private static readonly string[] Components =
    [
        "Button",
        "Input",
        "Select",
        "Checkbox",
        "Radio",
        "Switch",
        "Slider",
        "Tabs",
        "NavigationMenu",
        "Card",
        "Popover",
        "Dialog",
        "Toast",
        "Sonner",
        "Alert",
        "Badge",
        "Avatar",
        "Spinner",
        "Progress",
        "Table",
        "Menu",
        "ContextMenu",
        "Command",
        "Collapsible",
        "Separator",
        "Skeleton"
    ];

    private static readonly StyleGuard[] HighRiskStyleGuards =
    [
        new("Button", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexButton\"",
            "PART_Root",
            "PART_ContentPresenter",
            "PART_LoadingIndicator",
            "PART_LeadingIcon",
            "PART_TrailingIcon",
            ":pointerover",
            ":pressed",
            ":focus",
            ":disabled"
        ]),
        new("Input", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexTextBox\"",
            "PART_BorderElement",
            "PART_ScrollViewer",
            "PART_Placeholder",
            "PART_TextPresenter",
            "SelectionBrush",
            ":pointerover",
            ":focus",
            ":disabled",
            "is-read-only"
        ]),
        new("Select", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexSelect\"",
            "PART_Trigger",
            "PART_SelectedContentHost",
            "PART_Chevron",
            "PART_Popup",
            "PART_PopupBorder",
            "PART_ItemsPresenter",
            "<ControlTemplate TargetType=\"ComboBoxItem\"",
            "ComboBoxItem:selected",
            "ComboBoxItem:pointerover",
            ":dropdownopen",
            ":disabled"
        ]),
        new("Checkbox", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexCheckBox\"",
            "PART_Box",
            "PART_Check",
            "PART_Indeterminate",
            ":pointerover",
            ":checked",
            ":indeterminate",
            ":focus",
            ":disabled"
        ]),
        new("Radio", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexRadio\"",
            "PART_Ring",
            "PART_Dot",
            ":pointerover",
            ":checked",
            ":focus",
            ":disabled"
        ]),
        new("Switch", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexSwitch\"",
            "PART_Track",
            "PART_Thumb",
            ":pointerover",
            ":pressed",
            ":checked",
            ":focus",
            ":disabled"
        ]),
        new("Slider", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexSlider\"",
            "PART_SliderRoot",
            "PART_Track",
            "PART_DecreaseButton",
            "PART_IncreaseButton",
            "PART_Thumb",
            ":pointerover",
            ":pressed",
            ":focus",
            ":disabled"
        ]),
        new("Tabs", true,
        [
            "PART_List",
            "PART_ContentTransitionHost",
            "PART_TriggerRoot",
            "PART_Indicator",
            "PART_VerticalIndicator",
            "PART_FocusRing",
            "TransitioningContentControl",
            "CrossFade",
            "BoxShadowsTransition",
            "variant-line",
            "TabItem:pointerover",
            "TabItem:focus",
            "TabItem:selected",
            "TabItem:disabled"
        ]),
        new("NavigationMenu", true,
        [
            "PART_List",
            "PART_ViewportPositioner",
            "PART_Viewport",
            "PART_Indicator",
            "PART_ContentTransitionHost",
            "TransitioningContentControl",
            "CompositePageTransition",
            "PageSlide",
            "CrossFade",
            "IsTransitionReversed",
            "motion-from-start",
            "motion-from-end",
            "controls|CodexNavigationMenuItem:pointerover",
            "controls|CodexNavigationMenuItem:focus",
            "controls|CodexNavigationMenuItem.open",
            "controls|CodexNavigationMenuItem:disabled"
        ]),
        new("Menu", true,
        [
            "PART_Surface",
            "PART_ItemRoot",
            "MenuItem:pointerover",
            "MenuItem:focus",
            "MenuItem:selected",
            "MenuItem:disabled"
        ]),
        new("ContextMenu", true,
        [
            "PART_Surface",
            "PART_ItemsPresenter",
            "PART_ItemRoot",
            "PART_SubMenuSurface",
            "PART_SubMenuItemsPresenter",
            "context-menu-open",
            "submenu-open",
            "side-bottom",
            "side-left",
            "side-right",
            "side-top",
            "TransformOperationsTransition",
            "RenderTransformOrigin",
            "MenuItem:pointerover",
            "MenuItem:focus",
            "MenuItem:selected",
            "MenuItem:disabled"
        ]),
        new("Command", true,
        [
            "PART_Surface",
            "PART_InputShell",
            "PART_Input",
            "PART_ItemRoot",
            "controls|CodexCommandInput",
            "<ControlTemplate TargetType=\"controls:CodexCommandInput\"",
            "controls|CodexCommandItem:pointerover",
            "controls|CodexCommandItem:focus",
            "controls|CodexCommandItem.active",
            "controls|CodexCommandItem:disabled"
        ]),
        new("Collapsible", true,
        [
            "<ControlTemplate TargetType=\"controls:CodexCollapsible\"",
            "PART_Trigger",
            "PART_Chevron",
            "PART_ContentClip",
            "PART_ContentMeasure",
            "PART_ContentPresenter",
            "open",
            "closed",
            "TransformOperationsTransition",
            ":pointerover",
            ":focus",
            ":disabled"
        ]),
        new("Progress", false,
        [
            "ControlTemplate",
            "PART_Track",
            "PART_Indicator",
            "PART_Text",
            ":disabled"
        ]),
        new("Table", false,
        [
            "ControlTemplate",
            "PART_TableSurface",
            "PART_RowRoot",
            "controls|CodexTableHeader",
            "controls|CodexTableBody",
            "controls|CodexTableFooter",
            "controls|CodexTableRow:pointerover",
            "controls|CodexTableRow.selected"
        ])
    ];

    [Fact]
    public void EveryComponentHasOwnStyleFileAndThemeInclude()
    {
        var root = FindRepositoryRoot();
        var theme = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "ComponentStyles.axaml"));

        foreach (var component in Components)
        {
            var stylePath = Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml");
            Assert.True(File.Exists(stylePath), $"Missing style file for {component}: {stylePath}");
            Assert.Contains($"Themes/Controls/{component}.axaml", theme);
        }
    }

    [Fact]
    public void EveryComponentStyleDeclaresMotionTransitions()
    {
        var root = FindRepositoryRoot();

        foreach (var component in Components)
        {
            var stylePath = Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml");
            var style = File.ReadAllText(stylePath);
            Assert.Contains("Transitions", style);
        }
    }

    [Fact]
    public void ComponentStylesDoNotReferenceFluentOrBasedOnDefaults()
    {
        var root = FindRepositoryRoot();
        var failures = new List<string>();

        foreach (var component in Components)
        {
            var style = ReadStyle(root, component);

            if (style.Contains("BasedOn=", StringComparison.Ordinal)
                || style.Contains("Avalonia.Themes.Fluent", StringComparison.Ordinal)
                || style.Contains("FluentTheme", StringComparison.Ordinal))
            {
                failures.Add($"{component}: references a Fluent/BasedOn default style path.");
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void TextInputStylesUseOnePlaceholderForegroundAlias()
    {
        var root = FindRepositoryRoot();
        var textInputStyles = new[] { "Input", "Select", "Command" };

        foreach (var component in textInputStyles)
        {
            var style = ReadStyle(root, component);

            Assert.Contains("PlaceholderForeground", style);
            Assert.DoesNotContain("WatermarkForeground", style);
        }
    }

    [Fact]
    public void DisabledStatesUseSemanticOpacityToken()
    {
        var root = FindRepositoryRoot();
        var failures = new List<string>();

        foreach (var component in Components)
        {
            var style = ReadStyle(root, component);

            if (style.Contains(":disabled", StringComparison.Ordinal)
                && !style.Contains("CodexSwitch.DisabledOpacity", StringComparison.Ordinal))
            {
                failures.Add($"{component}: disabled state does not use CodexSwitch.DisabledOpacity.");
            }

            if (style.Contains("Property=\"Opacity\" Value=\"0.5\"", StringComparison.Ordinal))
            {
                failures.Add($"{component}: disabled opacity is hard-coded instead of tokenized.");
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void ThemeTokenFilesDeclareMotionResourceKeys()
    {
        var root = FindRepositoryRoot();
        var baseTokens = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Tokens", "BaseTokens.axaml"));
        var lightTheme = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Light.axaml"));
        var darkTheme = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Dark.axaml"));

        var baseTokenKeys = new[]
        {
            CodexSwitchResourceKeys.MotionDurationFast,
            CodexSwitchResourceKeys.MotionDurationDefault,
            CodexSwitchResourceKeys.MotionDurationSlow,
            CodexSwitchResourceKeys.MotionEaseOut,
            CodexSwitchResourceKeys.MotionEaseInOut,
            CodexSwitchResourceKeys.DisabledOpacity,
            CodexSwitchResourceKeys.RingOffset,
            CodexSwitchResourceKeys.OverlayOpacity,
            CodexSwitchResourceKeys.PopoverEnterOffset,
            CodexSwitchResourceKeys.DialogEnterOffset,
            CodexSwitchResourceKeys.ToastEnterOffset,
            CodexSwitchResourceKeys.SkeletonShimmerDuration,
            CodexSwitchResourceKeys.SkeletonShimmerOpacity,
            CodexSwitchResourceKeys.ReducedMotion
        };

        foreach (var key in baseTokenKeys)
        {
            Assert.Contains($"x:Key=\"{key}\"", baseTokens);
        }

        Assert.Contains($"x:Key=\"{CodexSwitchResourceKeys.SkeletonShimmerBrush}\"", lightTheme);
        Assert.Contains($"x:Key=\"{CodexSwitchResourceKeys.SkeletonShimmerBrush}\"", darkTheme);
    }

    [Fact]
    public void EveryComponentHasOwnClassFile()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var classNames = Components.Select(component => component == "Input" ? "TextBox" : component);

        foreach (var className in classNames)
        {
            var filePath = Path.Combine(controls, $"Codex{className}.cs");
            Assert.True(File.Exists(filePath), $"Missing component class file: {filePath}");
        }
    }

    [Fact]
    public void EveryExpectedTopLevelControlHasOwnClassFile()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");

        foreach (var className in ExpectedTopLevelControls)
        {
            var filePath = Path.Combine(controls, $"{className}.cs");
            Assert.True(File.Exists(filePath), $"Missing top-level control class file: {filePath}");
        }
    }

    [Fact]
    public void EveryPublicCodexControlClassHasAStyleSelector()
    {
        var root = FindRepositoryRoot();
        var controls = Path.Combine(root, "src", "CodexSwitchUI", "Controls");
        var styles = string.Join(
            Environment.NewLine,
            Directory.EnumerateFiles(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls"), "*.axaml")
                .Select(File.ReadAllText));

        var controlClasses = Directory.EnumerateFiles(controls, "Codex*.cs")
            .SelectMany(path => File.ReadLines(path))
            .Select(line => line.Trim())
            .Where(line => line.StartsWith("public class Codex", StringComparison.Ordinal)
                           || line.StartsWith("public abstract class Codex", StringComparison.Ordinal))
            .Select(line => line.Split([' ', ':', '('], StringSplitOptions.RemoveEmptyEntries)
                .First(token => token.StartsWith("Codex", StringComparison.Ordinal)))
            .Where(className => className is not "CodexFrame")
            .Where(className => !className.StartsWith("CodexControl", StringComparison.Ordinal))
            .Distinct()
            .OrderBy(className => className)
            .ToArray();

        var missing = controlClasses
            .Where(className => !styles.Contains($"controls|{className}", StringComparison.Ordinal))
            .ToArray();

        Assert.Empty(missing);
    }

    [Fact]
    public void HighRiskComponentsOwnTemplatesFocusAdornersAndCriticalParts()
    {
        var root = FindRepositoryRoot();
        var failures = new List<string>();

        foreach (var guard in HighRiskStyleGuards)
        {
            var style = ReadStyle(root, guard.Component);

            if (!style.Contains("ControlTemplate", StringComparison.Ordinal))
            {
                failures.Add($"{guard.Component}: missing ControlTemplate.");
            }

            if (!style.Contains("Transitions", StringComparison.Ordinal))
            {
                failures.Add($"{guard.Component}: missing Transitions.");
            }

            if (guard.RequiresFocusAdorner && !style.Contains("FocusAdorner\" Value=\"{x:Null}", StringComparison.Ordinal))
            {
                failures.Add($"{guard.Component}: missing FocusAdorner null guard; Fluent focus chrome may leak.");
            }

            foreach (var fragment in guard.RequiredFragments)
            {
                if (!style.Contains(fragment, StringComparison.Ordinal))
                {
                    failures.Add($"{guard.Component}: missing '{fragment}'.");
                }
            }

            if (style.Contains("BasedOn=", StringComparison.Ordinal)
                || style.Contains("Avalonia.Themes.Fluent", StringComparison.Ordinal)
                || style.Contains("FluentTheme", StringComparison.Ordinal))
            {
                failures.Add($"{guard.Component}: references a Fluent/BasedOn default style path.");
            }
        }

        AssertNoFailures(failures);
    }

    [Fact]
    public void HighRiskNativeItemSelectorsAreScopedAndTemplated()
    {
        var root = FindRepositoryRoot();
        var select = ReadStyle(root, "Select");
        var menu = ReadStyle(root, "Menu");
        var contextMenu = ReadStyle(root, "ContextMenu");
        var tabs = ReadStyle(root, "Tabs");

        Assert.Contains("<ControlTemplate TargetType=\"ComboBoxItem\"", select);
        Assert.Contains("ComboBoxItem:selected", select);

        Assert.DoesNotContain("<Style Selector=\"MenuItem\"", menu);
        Assert.Contains("controls|CodexMenu MenuItem", menu);
        Assert.Contains("PART_ItemRoot", menu);

        Assert.DoesNotContain("<Style Selector=\"MenuItem\"", contextMenu);
        Assert.Contains("controls|CodexContextMenu MenuItem", contextMenu);
        Assert.Contains("PART_ItemRoot", contextMenu);

        Assert.DoesNotContain("<Style Selector=\"TabItem\"", tabs);
        Assert.Contains("controls|CodexTabs TabItem", tabs);
        Assert.Contains("PART_TriggerRoot", tabs);
    }

    [Fact]
    public void HighRiskNestedNativePartsDoNotDependOnDefaultTemplates()
    {
        var root = FindRepositoryRoot();
        var failures = new List<string>();
        var select = ReadStyle(root, "Select");

        if (select.Contains("<TextBox x:Name=\"PART_EditableTextBox\"", StringComparison.Ordinal))
        {
            failures.Add("Select: PART_EditableTextBox is a raw TextBox without its own template; editable mode may leak Fluent textbox chrome.");
        }

        AssertNoFailures(failures);
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "CodexSwitchUI.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root from test output directory.");
    }

    private static string ReadStyle(string root, string component)
    {
        return File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml"));
    }

    private static void AssertNoFailures(IReadOnlyCollection<string> failures)
    {
        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    private sealed record StyleGuard(string Component, bool RequiresFocusAdorner, string[] RequiredFragments);
}
