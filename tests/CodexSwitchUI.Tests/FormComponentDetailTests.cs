using CodexSwitchUI.Controls;
using Xunit;

namespace CodexSwitchUI.Tests;

public class FormComponentDetailTests
{
    private static readonly string[] FormStyleFiles =
    [
        "Button",
        "Input",
        "Select",
        "Checkbox",
        "Radio",
        "Switch",
        "Slider"
    ];

    [Fact]
    public void ButtonSyncsLoadingAndIconClasses()
    {
        var button = new CodexButton
        {
            IsLoading = true,
            LoadingContent = "Saving",
            LeadingIcon = "L",
            TrailingIcon = "R"
        };

        Assert.Contains("is-loading", button.Classes);
        Assert.Contains("has-loading-content", button.Classes);
        Assert.Contains("has-leading-icon", button.Classes);
        Assert.Contains("has-trailing-icon", button.Classes);
    }

    [Fact]
    public void FormControlsSyncIntentAndSizeClasses()
    {
        var controls = new Avalonia.Controls.Control[]
        {
            new CodexSelect { Intent = CodexControlIntent.Warning, Size = CodexControlSize.Large },
            new CodexCheckBox { Intent = CodexControlIntent.Error, Size = CodexControlSize.Small },
            new CodexRadio { Intent = CodexControlIntent.Success, Size = CodexControlSize.Large },
            new CodexSwitch { Intent = CodexControlIntent.Warning, Size = CodexControlSize.Small },
            new CodexSlider { Intent = CodexControlIntent.Error, Size = CodexControlSize.Large }
        };

        Assert.Contains("intent-warning", controls[0].Classes);
        Assert.Contains("size-lg", controls[0].Classes);
        Assert.Contains("intent-error", controls[1].Classes);
        Assert.Contains("size-sm", controls[1].Classes);
        Assert.Contains("intent-success", controls[2].Classes);
        Assert.Contains("size-lg", controls[2].Classes);
        Assert.Contains("intent-warning", controls[3].Classes);
        Assert.Contains("size-sm", controls[3].Classes);
        Assert.Contains("intent-error", controls[4].Classes);
        Assert.Contains("size-lg", controls[4].Classes);
    }

    [Fact]
    public void TextBoxSyncsReadOnlyClass()
    {
        var textBox = new CodexTextBox
        {
            IsReadOnly = true
        };

        Assert.Contains("is-read-only", textBox.Classes);
    }

    [Fact]
    public void FormStylesDeclareTemplatesAndMotion()
    {
        var root = FindRepositoryRoot();

        foreach (var component in FormStyleFiles)
        {
            var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml"));

            Assert.Contains("ControlTemplate", style);
            Assert.Contains("Transitions", style);
        }
    }

    [Fact]
    public void FormStylesContainCustomInteractionParts()
    {
        var root = FindRepositoryRoot();

        AssertStyleContains(root, "Button", "PART_LoadingIndicator", "PART_FocusRing", "has-leading-icon", "variant-ghost");
        AssertStyleContains(root, "Input", "PART_TextPresenter", "PART_FocusRing", "SelectionBrush", "CaretBrush", "is-read-only");
        AssertStyleContains(root, "Select", "PART_Popup", "PART_Chevron", "PART_FocusRing", "Placement=\"Bottom\"", "controls|CodexSelect ComboBoxItem:selected");
        AssertStyleContains(root, "Checkbox", "PART_Check", "PART_FocusRing", ":indeterminate", ":pressed", "TransformOperationsTransition");
        AssertStyleContains(root, "Radio", "PART_Dot", "PART_FocusRing", "TransformOperationsTransition", ":checked", ":pressed");
        AssertStyleContains(root, "Switch", "PART_Track", "PART_Thumb", "PART_FocusRing", "size-lg:checked", "TransformOperationsTransition");
        AssertStyleContains(root, "Slider", "PART_Track", "PART_Thumb", "RepeatButton.Template", "TransformOperationsTransition");
    }

    [Fact]
    public void SelectStyleOwnsPopupItemsAndOpeningMotion()
    {
        var root = FindRepositoryRoot();
        var selectCode = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexSelect.cs"));

        AssertStyleContains(
            root,
            "Select",
            "ItemContainerTheme",
            "<ControlTheme TargetType=\"ComboBoxItem\"",
            "Selector=\"^:pointerover\"",
            "Selector=\"^:selected\"",
            "Opacity=\"0\"",
            "RenderTransform=\"scale(0.98)\"",
            "controls|CodexSelect.popup-open /template/ Border#PART_PopupBorder",
            "<Setter Property=\"Opacity\" Value=\"1\"",
            "<Setter Property=\"RenderTransform\" Value=\"scale(1)\"",
            "TransformOperationsTransition");

        Assert.Contains("IsDropDownOpenProperty.Changed.AddClassHandler<CodexSelect>", selectCode);
        Assert.Contains("Classes.Set(\"popup-open\", false)", selectCode);
        Assert.Contains("Dispatcher.UIThread.Post", selectCode);
        Assert.Contains("Classes.Set(\"popup-open\", true)", selectCode);
    }

    [Fact]
    public void FormStylesCoverStateSizeIntentAndVariantMatrix()
    {
        var root = FindRepositoryRoot();

        foreach (var component in FormStyleFiles)
        {
            AssertStyleContains(root, component, "pointerover", ":focus", ":disabled");
        }

        AssertStyleContains(
            root,
            "Button",
            ":pressed",
            "variant-secondary",
            "variant-destructive",
            "variant-outline",
            "variant-ghost",
            "variant-link",
            "variant-success",
            "variant-warning",
            "size-sm",
            "size-lg",
            "size-icon");

        foreach (var component in FormStyleFiles.Except(["Button"]))
        {
            AssertStyleContains(root, component, "intent-error", "intent-success", "intent-warning", "size-sm", "size-lg");
        }

        AssertStyleContains(root, "Select", ":pressed", "dropdownopen");
        AssertStyleContains(root, "Checkbox", ":pressed", ":checked");
        AssertStyleContains(root, "Radio", ":pressed", ":checked");
        AssertStyleContains(root, "Switch", ":pressed", ":checked");
        AssertStyleContains(root, "Slider", ":pressed", "PART_DecreaseButton", "PART_IncreaseButton");
    }

    [Fact]
    public void FocusStatesDoNotChangeExistingBorderThickness()
    {
        var root = FindRepositoryRoot();

        foreach (var component in FormStyleFiles)
        {
            var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml"));

            foreach (var block in ExtractStyleBlocks(style).Where(block => block.Contains(":focus", StringComparison.Ordinal)))
            {
                Assert.DoesNotContain("Property=\"BorderThickness\"", block);
                Assert.DoesNotContain("CodexSwitch.FocusThickness", block);
            }
        }
    }

    [Fact]
    public void FormStylesDoNotLeakIntoAvaloniaDefaultControlSelectors()
    {
        var root = FindRepositoryRoot();
        var forbiddenSelectors = new[]
        {
            "<Style Selector=\"Button",
            "<Style Selector=\"TextBox",
            "<Style Selector=\"ComboBoxItem",
            "<Style Selector=\"CheckBox",
            "<Style Selector=\"RadioButton",
            "<Style Selector=\"ToggleButton",
            "<Style Selector=\"Slider",
            "<Style Selector=\"RepeatButton",
            "<Style Selector=\"Thumb"
        };

        foreach (var component in FormStyleFiles)
        {
            var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml"));

            foreach (var selector in forbiddenSelectors)
            {
                Assert.DoesNotContain(selector, style);
            }
        }
    }

    private static void AssertStyleContains(string root, string component, params string[] snippets)
    {
        var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml"));

        foreach (var snippet in snippets)
        {
            Assert.Contains(snippet, style);
        }
    }

    private static IEnumerable<string> ExtractStyleBlocks(string style)
    {
        const string open = "<Style Selector=";
        const string close = "</Style>";
        var start = 0;

        while ((start = style.IndexOf(open, start, StringComparison.Ordinal)) >= 0)
        {
            var end = style.IndexOf(close, start, StringComparison.Ordinal);
            if (end < 0)
            {
                yield break;
            }

            end += close.Length;
            yield return style[start..end];
            start = end;
        }
    }

    private static string FindRepositoryRoot()
    {
        return TestRepository.FindRoot();
    }
}
