using Avalonia.Controls;
using CodexSwitchUI.Controls;
using Avalonia.Layout;
using Xunit;

namespace CodexSwitchUI.Tests;

public class ControlStateTests
{
    private static readonly VariantCase[] VariantCases =
    [
        new("Button", "controls|CodexButton", () => new CodexButton(), (control, variant) => ((CodexButton)control).Variant = variant),
        new("Alert", "controls|CodexAlert", () => new CodexAlert(), (control, variant) => ((CodexAlert)control).Variant = variant),
        new("Badge", "controls|CodexBadge", () => new CodexBadge(), (control, variant) => ((CodexBadge)control).Variant = variant),
        new("Toast", "controls|CodexToast", () => new CodexToast(), (control, variant) => ((CodexToast)control).Variant = variant)
    ];

    private static readonly SizeCase[] SizeCases =
    [
        new("Button", "controls|CodexButton", () => new CodexButton(), (control, size) => ((CodexButton)control).Size = size),
        new("Input", "controls|CodexTextBox", () => new CodexTextBox(), (control, size) => ((CodexTextBox)control).Size = size),
        new("Select", "controls|CodexSelect", () => new CodexSelect(), (control, size) => ((CodexSelect)control).Size = size),
        new("Checkbox", "controls|CodexCheckBox", () => new CodexCheckBox(), (control, size) => ((CodexCheckBox)control).Size = size),
        new("Radio", "controls|CodexRadio", () => new CodexRadio(), (control, size) => ((CodexRadio)control).Size = size),
        new("Switch", "controls|CodexSwitch", () => new CodexSwitch(), (control, size) => ((CodexSwitch)control).Size = size),
        new("Slider", "controls|CodexSlider", () => new CodexSlider(), (control, size) => ((CodexSlider)control).Size = size),
        new("Tabs", "controls|CodexTabs", () => new CodexTabs(), (control, size) => ((CodexTabs)control).Size = size),
        new("Menu", "controls|CodexMenu", () => new CodexMenu(), (control, size) => ((CodexMenu)control).Size = size),
        new("ContextMenu", "controls|CodexContextMenu", () => new CodexContextMenu(), (control, size) => ((CodexContextMenu)control).Size = size),
        new("Collapsible", "controls|CodexCollapsible", () => new CodexCollapsible(), (control, size) => ((CodexCollapsible)control).Size = size),
        new("Avatar", "controls|CodexAvatar", () => new CodexAvatar(), (control, size) => ((CodexAvatar)control).Size = size),
        new("Separator", "controls|CodexSeparator", () => new CodexSeparator(), (control, size) => ((CodexSeparator)control).Size = size)
    ];

    private static readonly IntentCase[] IntentCases =
    [
        new("Input", "controls|CodexTextBox", () => new CodexTextBox(), (control, intent) => ((CodexTextBox)control).Intent = intent),
        new("Select", "controls|CodexSelect", () => new CodexSelect(), (control, intent) => ((CodexSelect)control).Intent = intent),
        new("Checkbox", "controls|CodexCheckBox", () => new CodexCheckBox(), (control, intent) => ((CodexCheckBox)control).Intent = intent),
        new("Radio", "controls|CodexRadio", () => new CodexRadio(), (control, intent) => ((CodexRadio)control).Intent = intent),
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

    private sealed record VariantCase(string Component, string StyleSelector, Func<Control> Create, Action<Control, CodexControlVariant> SetVariant);

    private sealed record SizeCase(string Component, string StyleSelector, Func<Control> Create, Action<Control, CodexControlSize> SetSize);

    private sealed record IntentCase(string Component, string StyleSelector, Func<Control> Create, Action<Control, CodexControlIntent> SetIntent);
}
