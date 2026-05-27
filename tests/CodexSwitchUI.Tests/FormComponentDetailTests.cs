using Avalonia.Layout;
using Avalonia.Threading;
using CodexSwitchUI.Controls;
using Xunit;

namespace CodexSwitchUI.Tests;

public class FormComponentDetailTests
{
    private static readonly string[] FormStyleFiles =
    [
        "Button",
        "ButtonGroup",
        "InputGroup",
        "InputOtp",
        "Label",
        "Field",
        "Input",
        "Textarea",
        "Select",
        "Combobox",
        "NativeSelect",
        "Calendar",
        "DatePicker",
        "Checkbox",
        "Radio",
        "RadioGroup",
        "Switch",
        "Toggle",
        "Slider"
    ];

    private static readonly string[] InteractiveFormStyleFiles =
    [
        "Button",
        "Input",
        "Textarea",
        "InputOtp",
        "Select",
        "Combobox",
        "NativeSelect",
        "Calendar",
        "DatePicker",
        "Checkbox",
        "Radio",
        "Switch",
        "Toggle",
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
            new CodexNativeSelect { Intent = CodexControlIntent.Error, Size = CodexControlSize.Small },
            new CodexInputOtp { Intent = CodexControlIntent.Warning, Size = CodexControlSize.Large },
            new CodexCalendar { Intent = CodexControlIntent.Success, Size = CodexControlSize.Small },
            new CodexDatePicker { Intent = CodexControlIntent.Warning, Size = CodexControlSize.Large },
            new CodexCheckBox { Intent = CodexControlIntent.Error, Size = CodexControlSize.Small },
            new CodexRadio { Intent = CodexControlIntent.Success, Size = CodexControlSize.Large },
            new CodexRadioGroup { Intent = CodexControlIntent.Warning, Size = CodexControlSize.Small },
            new CodexSwitch { Intent = CodexControlIntent.Warning, Size = CodexControlSize.Small },
            new CodexToggle { Variant = CodexControlVariant.Outline, Size = CodexControlSize.Large, IsPressed = true },
            new CodexSlider { Intent = CodexControlIntent.Error, Size = CodexControlSize.Large }
        };

        Assert.Contains("intent-warning", controls[0].Classes);
        Assert.Contains("size-lg", controls[0].Classes);
        Assert.Contains("intent-error", controls[1].Classes);
        Assert.Contains("size-sm", controls[1].Classes);
        Assert.Contains("intent-warning", controls[2].Classes);
        Assert.Contains("size-lg", controls[2].Classes);
        Assert.Contains("intent-success", controls[3].Classes);
        Assert.Contains("size-sm", controls[3].Classes);
        Assert.Contains("intent-warning", controls[4].Classes);
        Assert.Contains("size-lg", controls[4].Classes);
        Assert.Contains("intent-error", controls[5].Classes);
        Assert.Contains("size-sm", controls[5].Classes);
        Assert.Contains("intent-success", controls[6].Classes);
        Assert.Contains("size-lg", controls[6].Classes);
        Assert.Contains("intent-warning", controls[7].Classes);
        Assert.Contains("size-sm", controls[7].Classes);
        Assert.Contains("intent-warning", controls[8].Classes);
        Assert.Contains("size-sm", controls[8].Classes);
        Assert.Contains("variant-outline", controls[9].Classes);
        Assert.Contains("size-lg", controls[9].Classes);
        Assert.Contains("state-on", controls[9].Classes);
        Assert.Contains("pressed", controls[9].Classes);
        Assert.Contains("intent-error", controls[10].Classes);
        Assert.Contains("size-lg", controls[10].Classes);
    }

    [Fact]
    public void CheckboxMirrorsWebCheckedStateChangeContract()
    {
        var checkBox = new CodexCheckBox();
        var changes = new List<(bool? OldValue, bool? NewValue)>();
        checkBox.CheckedStateChanged += (_, args) => changes.Add((args.OldValue, args.NewValue));

        Assert.Contains("state-unchecked", checkBox.Classes);

        checkBox.IsChecked = true;
        Assert.Contains("state-checked", checkBox.Classes);
        Assert.DoesNotContain("state-unchecked", checkBox.Classes);
        Assert.DoesNotContain("state-indeterminate", checkBox.Classes);

        checkBox.IsThreeState = true;
        checkBox.IsChecked = null;
        Assert.Contains("state-indeterminate", checkBox.Classes);
        Assert.DoesNotContain("state-checked", checkBox.Classes);

        checkBox.IsChecked = false;
        Assert.Contains("state-unchecked", checkBox.Classes);
        Assert.Equal(
            [
                (false, true),
                (true, null),
                (null, false)
            ],
            changes);
    }

    [Fact]
    public void ToggleAndToggleGroupMirrorWebPressedAndSelectionState()
    {
        var toggle = new CodexToggle { Content = "Bookmark" };
        var pressedChanges = new List<(bool OldValue, bool NewValue)>();
        toggle.PressedChanged += (_, args) => pressedChanges.Add((args.OldValue, args.NewValue));

        Assert.Contains("state-off", toggle.Classes);
        Assert.True(toggle.TryHandleActivationKey(Avalonia.Input.Key.Space));
        Assert.True(toggle.IsPressed);
        Assert.Contains("state-on", toggle.Classes);
        Assert.True(toggle.TryHandleActivationKey(Avalonia.Input.Key.Enter));
        Assert.False(toggle.IsPressed);
        toggle.IsPressed = false;
        toggle.IsPressed = true;
        Assert.Equal(
            [
                (false, true),
                (true, false),
                (false, true)
            ],
            pressedChanges);

        var left = new CodexToggleGroupItem { Content = "Left", Value = "left" };
        var disabled = new CodexToggleGroupItem { Content = "Disabled", Value = "disabled", IsEnabled = false };
        var right = new CodexToggleGroupItem { Content = "Right", Value = "right" };
        var group = new CodexToggleGroup
        {
            Items =
            {
                left,
                disabled,
                right
            }
        };
        left.IsPressed = true;

        Assert.Equal(2, group.Spacing);
        Assert.Equal("left", group.SelectedValue);
        Assert.Contains("type-single", group.Classes);
        Assert.Contains("has-value", group.Classes);
        Assert.Contains("spacing-2", group.Classes);
        Assert.Contains("spaced", group.Classes);

        Assert.True(right.TryHandleActivationKey(Avalonia.Input.Key.Enter));
        Assert.False(left.IsPressed);
        Assert.True(right.IsPressed);
        Assert.Equal("right", group.SelectedValue);

        Assert.True(right.TryHandleActivationKey(Avalonia.Input.Key.Space));
        Assert.False(right.IsPressed);
        Assert.Null(group.SelectedValue);
        Assert.Empty(group.SelectedValues);

        Assert.True(group.TryHandleItemNavigationKey(left, Avalonia.Input.Key.Right, moveFocus: false));
        Assert.True(group.TryHandleItemNavigationKey(right, Avalonia.Input.Key.Left, moveFocus: false));
        Assert.True(group.TryHandleItemNavigationKey(left, Avalonia.Input.Key.End, moveFocus: false));
        Assert.False(group.TryHandleItemNavigationKey(left, Avalonia.Input.Key.Down, moveFocus: false));

        group.IsLoop = false;
        Assert.False(group.TryHandleItemNavigationKey(right, Avalonia.Input.Key.Right, moveFocus: false));
        Assert.Contains("no-loop", group.Classes);

        group.Spacing = 0;
        Assert.Equal(0, group.Spacing);
        Assert.Contains("spacing-0", group.Classes);
        Assert.Contains("connected", group.Classes);
        Assert.DoesNotContain("spaced", group.Classes);
        Assert.Contains("group-first", left.Classes);
        Assert.Contains("group-middle", disabled.Classes);
        Assert.Contains("group-last", right.Classes);
        Assert.Contains("connected", left.Classes);
        Assert.Contains("connected", disabled.Classes);
        Assert.Contains("connected", right.Classes);

        var bold = new CodexToggleGroupItem { Content = "Bold", Value = "bold" };
        var italic = new CodexToggleGroupItem { Content = "Italic", Value = "italic" };
        var multiple = new CodexToggleGroup
        {
            Type = CodexToggleGroupType.Multiple,
            Items =
            {
                bold,
                italic
            }
        };

        Assert.True(bold.TryHandleActivationKey(Avalonia.Input.Key.Enter));
        Assert.True(italic.TryHandleActivationKey(Avalonia.Input.Key.Enter));
        Assert.Equal(["bold", "italic"], multiple.SelectedValues);
        Assert.Contains("type-multiple", multiple.Classes);
        multiple.Spacing = -8;
        Assert.Equal(0, multiple.Spacing);
        Assert.Contains("spacing-0", multiple.Classes);
    }

    [Fact]
    public void RadioGroupMirrorsWebValueOrientationAndRovingSelectionState()
    {
        var comfortable = new CodexRadioGroupItem { Content = "Comfortable", Value = "comfortable" };
        var disabled = new CodexRadioGroupItem { Content = "Disabled", Value = "disabled", IsEnabled = false };
        var compact = new CodexRadioGroupItem { Content = "Compact", Value = "compact" };
        var group = new CodexRadioGroup
        {
            Items =
            {
                comfortable,
                disabled,
                compact
            }
        };
        var changes = new List<CodexRadioGroupValueChangedEventArgs>();
        group.ValueChanged += (_, args) => changes.Add(args);
        group.SelectedValue = "comfortable";
        group.ApplyTemplate();

        Assert.True(comfortable.IsChecked);
        Assert.False(disabled.IsChecked);
        Assert.Equal("comfortable", group.SelectedValue);
        Assert.Contains("has-value", group.Classes);
        Assert.Contains("state-checked", comfortable.Classes);
        Assert.Contains("state-unchecked", compact.Classes);

        Assert.True(group.TryHandleItemNavigationKey(comfortable, Avalonia.Input.Key.Right, moveFocus: false));
        Assert.False(comfortable.IsChecked);
        Assert.False(disabled.IsChecked);
        Assert.True(compact.IsChecked);
        Assert.Equal("compact", group.SelectedValue);
        Assert.Single(changes);
        Assert.Equal("comfortable", changes[0].OldValue);
        Assert.Equal("compact", changes[0].NewValue);
        Assert.Same(comfortable, changes[0].OldItem);
        Assert.Same(compact, changes[0].NewItem);
        Assert.Equal(0, changes[0].OldIndex);
        Assert.Equal(2, changes[0].NewIndex);

        Assert.True(comfortable.TryHandleActivationKey(Avalonia.Input.Key.Space));
        Assert.True(comfortable.IsChecked);
        Assert.False(compact.IsChecked);
        Assert.Equal("comfortable", group.SelectedValue);
        Assert.Equal(2, changes.Count);
        Assert.Equal("compact", changes[1].OldValue);
        Assert.Equal("comfortable", changes[1].NewValue);
        Assert.Same(compact, changes[1].OldItem);
        Assert.Same(comfortable, changes[1].NewItem);
        Assert.Equal(2, changes[1].OldIndex);
        Assert.Equal(0, changes[1].NewIndex);

        group.Orientation = Orientation.Horizontal;
        Assert.Contains("horizontal", group.Classes);
        Assert.Contains("horizontal", comfortable.Classes);

        group.IsLoop = false;
        Assert.False(group.TryHandleItemNavigationKey(compact, Avalonia.Input.Key.Right, moveFocus: false));

        group.IsLoading = true;
        Assert.True(compact.TryHandleActivationKey(Avalonia.Input.Key.Enter));
        Assert.Equal("comfortable", group.SelectedValue);
        Assert.Contains("loading", group.Classes);
        Assert.Contains("group-loading", compact.Classes);
    }

    [Fact]
    public void ButtonGroupSyncsWebActionCompositionClasses()
    {
        var label = new CodexButtonGroupText { Content = "Mode" };
        var preview = new CodexButton { Content = "Preview" };
        var separator = new CodexButtonGroupSeparator();
        var code = new CodexButton { Content = "Code" };
        var group = new CodexButtonGroup
        {
            Variant = CodexControlVariant.Outline,
            Size = CodexControlSize.Small,
            Items =
            {
                label,
                preview,
                separator,
                code
            }
        };

        group.SyncItemStates();

        Assert.Contains("button-group", group.Classes);
        Assert.Contains("horizontal", group.Classes);
        Assert.Contains("variant-outline", group.Classes);
        Assert.Contains("size-sm", group.Classes);
        Assert.Contains("has-items", group.Classes);
        Assert.Contains("button-group-text", label.Classes);
        Assert.Contains("group-first", label.Classes);
        Assert.Contains("group-middle", preview.Classes);
        Assert.Contains("group-middle", separator.Classes);
        Assert.Contains("group-last", code.Classes);
        Assert.Contains("button-group-item", preview.Classes);
        Assert.Contains("variant-outline", preview.Classes);
        Assert.Contains("size-sm", preview.Classes);
        Assert.Contains("button-group-separator", separator.Classes);
        Assert.Equal(Orientation.Vertical, separator.Orientation);

        group.Orientation = Orientation.Vertical;
        group.SyncItemStates();

        Assert.Contains("vertical", group.Classes);
        Assert.Contains("vertical", preview.Classes);
        Assert.Equal(Orientation.Horizontal, separator.Orientation);
    }

    [Fact]
    public void InputGroupSyncsWebAddonControlAndFocusWithinClasses()
    {
        var prefix = new CodexInputGroupAddon { Content = "https://" };
        var input = new CodexInputGroupInput { Text = "api.openai.com" };
        var action = new CodexInputGroupButton { Content = "Copy", IsLoading = true };
        var suffix = new CodexInputGroupAddon
        {
            Align = CodexInputGroupAddonAlign.InlineEnd,
            Content = action
        };
        var group = new CodexInputGroup
        {
            Intent = CodexControlIntent.Warning,
            Size = CodexControlSize.Small,
            Items =
            {
                prefix,
                input,
                suffix
            }
        };

        group.SyncItemStates();

        Assert.Contains("input-group", group.Classes);
        Assert.Contains("inline", group.Classes);
        Assert.Contains("intent-warning", group.Classes);
        Assert.Contains("size-sm", group.Classes);
        Assert.Contains("input-group-addon", prefix.Classes);
        Assert.Contains("align-inline-start", prefix.Classes);
        Assert.Contains("group-first", prefix.Classes);
        Assert.Contains("input-group-control", input.Classes);
        Assert.Contains("group-middle", input.Classes);
        Assert.Contains("intent-warning", input.Classes);
        Assert.Contains("size-sm", input.Classes);
        Assert.Contains("align-inline-end", suffix.Classes);
        Assert.Contains("group-last", suffix.Classes);
        Assert.Contains("input-group-button", action.Classes);
        Assert.Contains("is-loading", action.Classes);

        suffix.Align = CodexInputGroupAddonAlign.BlockEnd;
        group.SyncItemStates();

        Assert.Contains("block", group.Classes);
        Assert.Contains("align-block-end", suffix.Classes);
    }

    [Fact]
    public void InputOtpSyncsSlotsPatternPasteAndKeyboardClasses()
    {
        var slot0 = new CodexInputOtpSlot { Index = 0 };
        var slot1 = new CodexInputOtpSlot { Index = 1 };
        var slot2 = new CodexInputOtpSlot { Index = 2 };
        var slot3 = new CodexInputOtpSlot { Index = 3 };
        var group = new CodexInputOtpGroup
        {
            Items =
            {
                slot0,
                slot1,
                slot2,
                slot3
            }
        };
        var input = new CodexInputOtp
        {
            MaxLength = 4,
            Pattern = CodexInputOtp.DigitsPattern,
            Intent = CodexControlIntent.Warning,
            Size = CodexControlSize.Small,
            Items =
            {
                group
            }
        };

        Assert.True(input.TryInsertText("12AB34"));
        Assert.Equal("1234", input.Text);
        Assert.Contains("complete", input.Classes);
        Assert.Contains("intent-warning", input.Classes);
        Assert.Contains("size-sm", input.Classes);

        input.FocusSlot(2);
        Assert.Equal(2, input.ActiveIndex);
        Assert.Equal("1", slot0.Character);
        Assert.Equal("2", slot1.Character);
        Assert.Equal("3", slot2.Character);
        Assert.Equal("4", slot3.Character);
        Assert.Contains("input-otp-slot", slot2.Classes);
        Assert.Contains("has-character", slot2.Classes);
        Assert.Contains("input-otp-group", group.Classes);
        Assert.Contains("group-first", slot0.Classes);
        Assert.Contains("group-last", slot3.Classes);

        input.IsInvalid = true;
        Assert.Contains("invalid", input.Classes);
        Assert.Contains("invalid", slot0.Classes);

        input.ActiveIndex = 4;
        Assert.False(input.TryInsertText("9"));
        input.Clear();
        Assert.Equal(string.Empty, input.Text);
        Assert.DoesNotContain("complete", input.Classes);
    }

    [Fact]
    public void LabelSyncsTargetRequiredIntentAndSizeClasses()
    {
        var target = new CodexTextBox { IsEnabled = false };
        var label = new CodexLabel
        {
            Target = target,
            Content = "_Provider",
            IsRequired = true,
            Intent = CodexControlIntent.Error,
            Size = CodexControlSize.Large
        };

        Assert.Contains("label", label.Classes);
        Assert.Contains("has-target", label.Classes);
        Assert.Contains("target-disabled", label.Classes);
        Assert.Contains("required", label.Classes);
        Assert.Contains("intent-error", label.Classes);
        Assert.Contains("size-lg", label.Classes);
    }

    [Fact]
    public void SelectRaisesWebValueAndOpenChangeEvents()
    {
        var valueChanges = new List<CodexSelectValueChangedEventArgs>();
        var openChanges = new List<bool>();
        var select = new CodexSelect
        {
            ItemsSource = new[] { "OpenAI", "Claude", "Responses" },
            PlaceholderText = "Select provider"
        };
        select.ValueChanged += (_, args) => valueChanges.Add(args);
        select.OpenChanged += (_, args) => openChanges.Add(args.IsOpen);

        Assert.Contains("select", select.Classes);
        Assert.Contains("placeholder-visible", select.Classes);
        Assert.DoesNotContain("has-selection", select.Classes);

        select.SelectedIndex = 1;

        Assert.Equal("Claude", select.SelectedItem);
        Assert.Contains("has-selection", select.Classes);
        Assert.DoesNotContain("placeholder-visible", select.Classes);
        Assert.Single(valueChanges);
        Assert.Equal(-1, valueChanges[0].OldIndex);
        Assert.Equal(1, valueChanges[0].NewIndex);
        Assert.Null(valueChanges[0].OldValue);
        Assert.Equal("Claude", valueChanges[0].NewValue);

        select.SelectedIndex = 2;

        Assert.Equal(1, valueChanges[^1].OldIndex);
        Assert.Equal("Claude", valueChanges[^1].OldValue);
        Assert.Equal("Responses", valueChanges[^1].NewValue);

        select.IsDropDownOpen = true;
        Dispatcher.UIThread.RunJobs();

        Assert.Contains(true, openChanges);
        Assert.Contains("popup-open", select.Classes);

        select.IsDropDownOpen = false;

        Assert.Contains(false, openChanges);
        Assert.DoesNotContain("popup-open", select.Classes);
    }

    [Fact]
    public void NativeSelectRaisesWebValueAndOpenChangeEvents()
    {
        var placeholder = new CodexNativeSelectOption { Value = "", Content = "Select status" };
        var todo = new CodexNativeSelectOption { Value = "todo", Content = "Todo" };
        var done = new CodexNativeSelectOption { Value = "done", Content = "Done" };
        var valueChanges = new List<CodexNativeSelectValueChangedEventArgs>();
        var openChanges = new List<bool>();
        var select = new CodexNativeSelect
        {
            Items =
            {
                placeholder,
                todo,
                done
            }
        };
        select.ValueChanged += (_, args) => valueChanges.Add(args);
        select.OpenChanged += (_, args) => openChanges.Add(args.IsOpen);

        Assert.Contains("native-select", select.Classes);
        Assert.Contains("placeholder-visible", select.Classes);
        Assert.DoesNotContain("has-selection", select.Classes);

        select.SelectedItem = placeholder;

        Assert.Single(valueChanges);
        Assert.Equal(-1, valueChanges[0].OldIndex);
        Assert.Equal(0, valueChanges[0].NewIndex);
        Assert.Null(valueChanges[0].OldValue);
        Assert.Equal("", valueChanges[0].NewValue);
        Assert.Contains("placeholder-visible", select.Classes);
        Assert.DoesNotContain("has-selection", select.Classes);

        select.SelectedItem = todo;

        Assert.Equal(2, valueChanges.Count);
        Assert.Equal(0, valueChanges[^1].OldIndex);
        Assert.Equal(1, valueChanges[^1].NewIndex);
        Assert.Equal("", valueChanges[^1].OldValue);
        Assert.Equal("todo", valueChanges[^1].NewValue);
        Assert.Same(placeholder, valueChanges[^1].OldItem);
        Assert.Same(todo, valueChanges[^1].NewItem);
        Assert.Contains("has-selection", select.Classes);
        Assert.DoesNotContain("placeholder-visible", select.Classes);

        select.IsDropDownOpen = true;
        Dispatcher.UIThread.RunJobs();

        Assert.Contains(true, openChanges);
        Assert.Contains("popup-open", select.Classes);

        select.IsDropDownOpen = false;

        Assert.Contains(false, openChanges);
        Assert.DoesNotContain("popup-open", select.Classes);
    }

    [Fact]
    public void NativeSelectSyncsSelectionInvalidOptionAndOptGroupClasses()
    {
        var placeholder = new CodexNativeSelectOption { Value = "", Content = "Select status" };
        var todo = new CodexNativeSelectOption { Value = "todo", Content = "Todo" };
        var disabled = new CodexNativeSelectOption { Value = "done", Content = "Done", IsEnabled = false };
        var group = new CodexNativeSelectOptGroup { Label = "Engineering" };
        var select = new CodexNativeSelect
        {
            IsInvalid = true,
            Items =
            {
                placeholder,
                group,
                todo,
                disabled
            }
        };

        select.SelectedItem = placeholder;

        Assert.Contains("native-select", select.Classes);
        Assert.Contains("invalid", select.Classes);
        Assert.Contains("placeholder-visible", select.Classes);
        Assert.DoesNotContain("has-selection", select.Classes);
        Assert.Contains("native-select-option", todo.Classes);
        Assert.Contains("has-value", todo.Classes);
        Assert.Contains("option-disabled", disabled.Classes);
        Assert.Contains("native-select-optgroup", group.Classes);
        Assert.Contains("has-label", group.Classes);
        Assert.False(group.IsEnabled);

        select.SelectedItem = todo;

        Assert.Contains("has-selection", select.Classes);
        Assert.DoesNotContain("placeholder-visible", select.Classes);
    }

    [Fact]
    public void ComboboxFiltersHighlightsSelectsClearsAndDismissesLikeWeb()
    {
        var changes = new List<CodexComboboxSelectionChangedEventArgs>();
        var inputChanges = new List<(string? OldText, string? NewText)>();
        var openChanges = new List<bool>();
        var combobox = new CodexCombobox
        {
            ItemsSource = new[] { "Next.js", "SvelteKit", "Nuxt.js", "Remix", "Astro" },
            PlaceholderText = "Select a framework",
            EmptyContent = "No frameworks found.",
            AutoHighlight = true
        };
        combobox.SelectionChanged += (_, args) => changes.Add(args);
        combobox.InputValueChanged += (_, args) => inputChanges.Add((args.OldText, args.NewText));
        combobox.OpenChanged += (_, args) => openChanges.Add(args.IsOpen);

        Assert.Contains("closed", combobox.Classes);
        Assert.Contains("auto-highlight", combobox.Classes);
        Assert.Contains("highlight-on-hover", combobox.Classes);
        Assert.Contains("close-on-select", combobox.Classes);
        Assert.Contains("open-on-input", combobox.Classes);
        Assert.True(combobox.HasFilteredItems);

        combobox.Text = "n";

        Assert.True(combobox.IsOpen);
        Assert.Contains("open", combobox.Classes);
        Assert.Contains("has-text", combobox.Classes);
        Assert.Equal("Next.js", combobox.HighlightedItem);
        Assert.Equal(["Next.js", "Nuxt.js"], combobox.FilteredItems.OfType<CodexComboboxItem>().Select(item => item.Value));
        Assert.Contains(true, openChanges);
        Assert.Contains(inputChanges, change => change.NewText == "n");

        Assert.True(combobox.TryHandleInputKey(Avalonia.Input.Key.Down));
        Assert.Equal("Nuxt.js", combobox.HighlightedItem);
        Assert.True(combobox.TryHandleInputKey(Avalonia.Input.Key.Enter));
        Assert.Equal("Nuxt.js", combobox.SelectedItem);
        Assert.Equal("Nuxt.js", combobox.Text);
        Assert.False(combobox.IsOpen);
        Assert.Contains("has-selection", combobox.Classes);
        Assert.Contains("has-clear", combobox.Classes);
        Assert.Equal("Nuxt.js", changes[^1].NewItem);
        Assert.Equal("Nuxt.js", changes[^1].NewValue);
        Assert.Null(changes[^1].OldItem);
        Assert.Equal(-1, changes[^1].OldIndex);
        Assert.Equal(2, changes[^1].NewIndex);
        Assert.Equal(CodexComboboxSelectionChangeSource.Keyboard, changes[^1].Source);

        Assert.True(combobox.ClearSelection());
        Assert.Null(combobox.SelectedItem);
        Assert.Equal(string.Empty, combobox.Text);
        Assert.DoesNotContain("has-selection", combobox.Classes);
        Assert.DoesNotContain("has-clear", combobox.Classes);
        Assert.Equal("Nuxt.js", changes[^1].OldValue);
        Assert.Null(changes[^1].NewValue);
        Assert.Equal(2, changes[^1].OldIndex);
        Assert.Equal(-1, changes[^1].NewIndex);
        Assert.Equal(CodexComboboxSelectionChangeSource.Clear, changes[^1].Source);

        combobox.Text = "zz";

        Assert.False(combobox.HasFilteredItems);
        Assert.Contains("empty", combobox.Classes);
        Assert.False(combobox.TryHandleInputKey(Avalonia.Input.Key.Enter));

        combobox.Text = "re";
        combobox.CloseOnSelect = false;
        Assert.True(combobox.TryHandleInputKey(Avalonia.Input.Key.Enter));
        Assert.Equal("Remix", combobox.SelectedItem);
        Assert.True(combobox.IsOpen);
        Assert.DoesNotContain("close-on-select", combobox.Classes);
        Assert.Equal(CodexComboboxSelectionChangeSource.Keyboard, changes[^1].Source);
        Assert.Equal(3, changes[^1].NewIndex);

        combobox.CloseOnEscape = false;
        Assert.False(combobox.TryHandleInputKey(Avalonia.Input.Key.Escape));
        Assert.True(combobox.IsOpen);
        combobox.CloseOnEscape = true;
        Assert.True(combobox.TryHandleInputKey(Avalonia.Input.Key.Escape));
        Assert.False(combobox.IsOpen);

        var openChangeCount = openChanges.Count;
        combobox.IsOpen = true;
        combobox.IsOpen = false;

        Assert.Equal([true, false], openChanges.Skip(openChangeCount));

        Assert.True(combobox.SelectItem("Astro"));
        Assert.Equal("Astro", combobox.SelectedItem);
        Assert.Equal("Astro", combobox.Text);
        Assert.Equal(4, changes[^1].NewIndex);
        Assert.Equal(CodexComboboxSelectionChangeSource.Programmatic, changes[^1].Source);

        Assert.True(combobox.SelectItem("SvelteKit", CodexComboboxSelectionChangeSource.Item));
        Assert.Equal("SvelteKit", combobox.SelectedItem);
        Assert.Equal("SvelteKit", combobox.Text);
        Assert.Equal(1, changes[^1].NewIndex);
        Assert.Equal(CodexComboboxSelectionChangeSource.Item, changes[^1].Source);

        combobox.IsLoading = true;
        Assert.False(combobox.Open());
        Assert.False(combobox.SelectItem("Astro"));
        Assert.Contains("loading", combobox.Classes);
    }

    [Fact]
    public void SliderRaisesWebValueChangeAndCommitEvents()
    {
        var changing = new List<CodexSliderValueChangingEventArgs>();
        var committed = new List<CodexSliderValueCommittedEventArgs>();
        var slider = new CodexSlider
        {
            Minimum = 0,
            Maximum = 100,
            Value = 24,
            SmallChange = 1,
            LargeChange = 10
        };
        slider.ValueChanging += (_, args) => changing.Add(args);
        slider.ValueCommitted += (_, args) => committed.Add(args);

        Assert.Contains("slider", slider.Classes);
        Assert.Contains("has-value", slider.Classes);
        Assert.DoesNotContain("at-min", slider.Classes);

        slider.Value = 42;

        Assert.Single(changing);
        Assert.Equal(24, changing[0].OldValue);
        Assert.Equal(42, changing[0].NewValue);
        Assert.Equal([24], changing[0].OldValues);
        Assert.Equal([42], changing[0].NewValues);

        Assert.True(slider.CommitValue());

        Assert.Single(committed);
        Assert.Equal(24, committed[0].OldValue);
        Assert.Equal(42, committed[0].NewValue);
        Assert.Equal("programmatic", committed[0].Source);
        Assert.Equal([24], committed[0].OldValues);
        Assert.Equal([42], committed[0].NewValues);

        Assert.False(slider.CommitValue());

        slider.Value = 0;

        Assert.Contains("at-min", slider.Classes);
        Assert.DoesNotContain("has-value", slider.Classes);
        Assert.True(slider.CommitValue());
        Assert.Equal("programmatic", committed[^1].Source);
        Assert.Equal(42, committed[^1].OldValue);
        Assert.Equal(0, committed[^1].NewValue);
    }

    [Fact]
    public void SliderSourceOwnsWebEventAndStateClasses()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexSlider.cs"));

        Assert.Contains("public event EventHandler<CodexSliderValueChangingEventArgs>? ValueChanging;", source);
        Assert.Contains("public event EventHandler<CodexSliderValueCommittedEventArgs>? ValueCommitted;", source);
        Assert.Contains("public bool CommitValue()", source);
        Assert.Contains("Classes.Set(\"slider\", true);", source);
        Assert.Contains("Classes.Set(\"has-value\"", source);
        Assert.Contains("Classes.Set(\"at-min\"", source);
        Assert.Contains("Classes.Set(\"at-max\"", source);
        Assert.Contains("Classes.Set(\"dragging\"", source);
        Assert.Contains("CommitValue(\"pointer\")", source);
        Assert.Contains("CommitValue(\"keyboard\")", source);
        Assert.Contains("CommitValue(\"focus\")", source);
        Assert.Contains("CommitValue(\"programmatic\")", source);
    }

    [Fact]
    public void ComboboxCanDeclareStandaloneItemsForDocsSamples()
    {
        var combobox = new CodexCombobox
        {
            AutoHighlight = true
        };
        combobox.Items.Add("Next.js");
        combobox.Items.Add("SvelteKit");
        combobox.Items.Add("Nuxt.js");

        combobox.Text = "n";

        Assert.True(combobox.IsOpen);
        Assert.Equal("Next.js", combobox.HighlightedItem);
        Assert.Equal(["Next.js", "Nuxt.js"], combobox.FilteredItems.OfType<CodexComboboxItem>().Select(item => item.Value));

        combobox.SelectedItem = "SvelteKit";

        Assert.Equal("SvelteKit", combobox.Text);
        Assert.Contains("has-selection", combobox.Classes);
    }

    [Fact]
    public void CalendarSyncsSelectionRangeBookedBoundsAndClasses()
    {
        var calendar = new CodexCalendar
        {
            DisplayDate = new DateTime(2026, 1, 1),
            SelectedDate = new DateTime(2026, 1, 16),
            ActiveDate = new DateTime(2026, 1, 16),
            MinDate = new DateTime(2026, 1, 5),
            MaxDate = new DateTime(2026, 1, 25),
            BookedDates = [new DateTime(2026, 1, 12)],
            Intent = CodexControlIntent.Warning,
            Size = CodexControlSize.Small
        };

        Assert.Contains("calendar", calendar.Classes);
        Assert.Contains("mode-single", calendar.Classes);
        Assert.Contains("show-outside-days", calendar.Classes);
        Assert.Contains("has-selected-date", calendar.Classes);
        Assert.Contains("intent-warning", calendar.Classes);
        Assert.Contains("size-sm", calendar.Classes);

        var selected = calendar.Items.OfType<CodexCalendarDayButton>().Single(button => button.Date == new DateTime(2026, 1, 16));
        var booked = calendar.Items.OfType<CodexCalendarDayButton>().Single(button => button.Date == new DateTime(2026, 1, 12));
        var beforeMin = calendar.Items.OfType<CodexCalendarDayButton>().Single(button => button.Date == new DateTime(2026, 1, 4));
        var selectedChanges = new List<CodexCalendarSelectedDateChangedEventArgs>();
        var rangeChanges = new List<CodexCalendarRangeChangedEventArgs>();
        var displayChanges = new List<CodexCalendarDisplayDateChangedEventArgs>();
        var activeChanges = new List<CodexCalendarActiveDateChangedEventArgs>();

        calendar.SelectedDateChanged += (_, args) => selectedChanges.Add(args);
        calendar.RangeChanged += (_, args) => rangeChanges.Add(args);
        calendar.DisplayDateChanged += (_, args) => displayChanges.Add(args);
        calendar.ActiveDateChanged += (_, args) => activeChanges.Add(args);

        Assert.True(selected.IsSelected);
        Assert.True(selected.IsActive);
        Assert.True(booked.IsBooked);
        Assert.True(booked.IsUnavailable);
        Assert.False(booked.IsEnabled);
        Assert.True(beforeMin.IsUnavailable);

        calendar.SelectDate(booked.Date);
        Assert.Equal(new DateTime(2026, 1, 16), calendar.SelectedDate);
        Assert.Empty(selectedChanges);

        calendar.SelectDate(new DateTime(2026, 1, 18));
        Assert.Equal(new DateTime(2026, 1, 18), calendar.SelectedDate);
        Assert.Equal(new DateTime(2026, 1, 16), selectedChanges[0].OldDate);
        Assert.Equal(new DateTime(2026, 1, 18), selectedChanges[0].NewDate);
        Assert.Equal(new DateTime(2026, 1, 16), activeChanges[0].OldDate);
        Assert.Equal(new DateTime(2026, 1, 18), activeChanges[0].NewDate);

        calendar.SelectionMode = CodexCalendarSelectionMode.Range;
        calendar.SelectDate(new DateTime(2026, 1, 14));
        calendar.SelectDate(new DateTime(2026, 1, 20));

        Assert.Equal(new DateTime(2026, 1, 14), calendar.RangeStart);
        Assert.Equal(new DateTime(2026, 1, 20), calendar.RangeEnd);
        Assert.Contains("mode-range", calendar.Classes);
        Assert.Contains("has-range", calendar.Classes);
        Assert.Contains("range-complete", calendar.Classes);
        Assert.True(rangeChanges.Last().IsComplete);
        Assert.Equal(new DateTime(2026, 1, 14), rangeChanges.Last().NewStart);
        Assert.Equal(new DateTime(2026, 1, 20), rangeChanges.Last().NewEnd);

        var rangeMiddle = calendar.Items.OfType<CodexCalendarDayButton>().Single(button => button.Date == new DateTime(2026, 1, 16));
        Assert.True(rangeMiddle.IsRangeMiddle);

        calendar.ShowOutsideDays = false;
        Assert.Contains("hide-outside-days", calendar.Classes);
        Assert.Contains(calendar.Items.OfType<CodexCalendarDayButton>(), button => button.IsBlank);

        calendar.ShowWeekNumbers = true;
        Assert.Contains("week-numbers", calendar.Classes);
        Assert.NotEmpty(calendar.Items.OfType<CodexCalendarWeekNumber>());

        calendar.MaxDate = new DateTime(2026, 2, 28);
        calendar.NavigateNextMonth();
        Assert.Equal(new DateTime(2026, 2, 1), calendar.DisplayDate);
        Assert.Equal(calendar.DisplayDate.ToString("MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture), calendar.MonthTitle);
        Assert.Equal(new DateTime(2026, 1, 1), displayChanges.Last().OldDisplayDate);
        Assert.Equal(new DateTime(2026, 2, 1), displayChanges.Last().NewDisplayDate);
        Assert.Equal(1, displayChanges.Last().MonthDelta);
        Assert.Contains("can-previous", calendar.Classes);
    }

    [Fact]
    public void DatePickerSyncsSelectionRangeOpenClearAndGuards()
    {
        var picker = new CodexDatePicker
        {
            DisplayDate = new DateTime(2026, 5, 16),
            DateFormat = "yyyy-MM-dd",
            MinDate = new DateTime(2026, 5, 10),
            MaxDate = new DateTime(2026, 5, 25),
            Intent = CodexControlIntent.Warning,
            Size = CodexControlSize.Small
        };
        var openChanges = new List<bool>();
        var selectedDates = new List<DateTime?>();
        var ranges = new List<(DateTime? Start, DateTime? End)>();
        picker.OpenChanged += (_, args) => openChanges.Add(args.IsOpen);
        picker.SelectedDateChanged += (_, args) => selectedDates.Add(args.NewDate);
        picker.RangeChanged += (_, args) => ranges.Add((args.Start, args.End));

        Assert.Contains("date-picker", picker.Classes);
        Assert.Contains("single", picker.Classes);
        Assert.Contains("placeholder-visible", picker.Classes);
        Assert.Contains("intent-warning", picker.Classes);
        Assert.Contains("size-sm", picker.Classes);
        Assert.Equal(new DateTime(2026, 5, 1), picker.DisplayDate);
        Assert.Null(picker.DisplayText);

        Assert.True(picker.Open());
        Assert.True(picker.SelectDate(new DateTime(2026, 5, 20, 17, 30, 0)));

        Assert.False(picker.IsOpen);
        Assert.Equal(new DateTime(2026, 5, 20), picker.SelectedDate);
        Assert.Equal("2026-05-20", picker.DisplayText);
        Assert.True(picker.HasSelection);
        Assert.True(picker.HasClearButton);
        Assert.Contains("has-selection", picker.Classes);
        Assert.Contains("has-clear", picker.Classes);
        Assert.Equal([true, false], openChanges);
        Assert.Contains(new DateTime(2026, 5, 20), selectedDates);

        Assert.True(picker.TryHandleInputKey(Avalonia.Input.Key.Delete));
        Assert.False(picker.HasSelection);
        Assert.Null(picker.DisplayText);

        picker.SelectionMode = CodexCalendarSelectionMode.Range;
        Assert.Contains("range", picker.Classes);
        picker.IsOpen = true;
        Assert.True(picker.SelectDate(new DateTime(2026, 5, 23)));
        Assert.True(picker.IsOpen);
        Assert.Equal(new DateTime(2026, 5, 23), picker.RangeStart);
        Assert.Null(picker.RangeEnd);

        Assert.True(picker.SelectDate(new DateTime(2026, 5, 18)));
        Assert.False(picker.IsOpen);
        Assert.Equal(new DateTime(2026, 5, 18), picker.RangeStart);
        Assert.Equal(new DateTime(2026, 5, 23), picker.RangeEnd);
        Assert.Equal("2026-05-18 - 2026-05-23", picker.DisplayText);
        Assert.Contains("range-complete", picker.Classes);
        Assert.Contains(ranges, range => range.Start == new DateTime(2026, 5, 18) && range.End == new DateTime(2026, 5, 23));

        picker.IsOpen = true;
        Assert.True(picker.TryHandleInputKey(Avalonia.Input.Key.Escape));
        Assert.False(picker.IsOpen);

        Assert.False(picker.SelectDate(new DateTime(2026, 5, 30)));
        Assert.Equal(new DateTime(2026, 5, 18), picker.RangeStart);
        picker.IsLoading = true;
        Assert.False(picker.Open());
        Assert.False(picker.SelectDate(new DateTime(2026, 5, 21)));
        Assert.Contains("loading", picker.Classes);
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
    public void SwitchSyncsOptionalLabelContentState()
    {
        var toggle = new CodexSwitch();
        var checkedChanges = new List<(bool OldValue, bool NewValue)>();
        toggle.CheckedChanged += (_, args) => checkedChanges.Add((args.OldValue, args.NewValue));

        Assert.False(toggle.HasContent);

        toggle.Content = "Enable streaming";

        Assert.True(toggle.HasContent);

        toggle.Content = "";

        Assert.False(toggle.HasContent);

        toggle.IsChecked = true;
        toggle.IsChecked = true;
        toggle.IsChecked = false;
        toggle.IsChecked = null;

        Assert.Equal(
            [
                (false, true),
                (true, false)
            ],
            checkedChanges);
    }

    [Fact]
    public void TextareaMatchesWebTextareaDefaults()
    {
        var textarea = new CodexTextarea
        {
            IsReadOnly = true,
            MinLines = 5,
            Intent = CodexControlIntent.Warning,
            Size = CodexControlSize.Large
        };

        Assert.True(textarea.AcceptsReturn);
        Assert.Contains("textarea", textarea.Classes);
        Assert.Contains("textarea-tall", textarea.Classes);
        Assert.Contains("is-read-only", textarea.Classes);
        Assert.Contains("intent-warning", textarea.Classes);
        Assert.Contains("size-lg", textarea.Classes);
    }

    [Fact]
    public void FieldSyncsSlotsIntentSizeAndRequiredClasses()
    {
        var field = new CodexField
        {
            Label = "API key",
            Description = "Stored locally.",
            Message = "Required before saving.",
            Intent = CodexControlIntent.Error,
            Size = CodexControlSize.Large,
            Orientation = CodexFieldOrientation.Horizontal,
            IsRequired = true,
            IsInvalid = true,
            Content = new CodexTextBox()
        };

        Assert.True(field.HasLabel);
        Assert.True(field.HasDescription);
        Assert.True(field.HasMessage);
        Assert.Contains("has-label", field.Classes);
        Assert.Contains("has-description", field.Classes);
        Assert.Contains("has-message", field.Classes);
        Assert.Contains("required", field.Classes);
        Assert.Contains("invalid", field.Classes);
        Assert.Contains("orientation-horizontal", field.Classes);
        Assert.Contains("intent-error", field.Classes);
        Assert.Contains("size-lg", field.Classes);

        field.Message = "";
        field.IsRequired = false;
        field.IsInvalid = false;

        Assert.False(field.HasMessage);
        Assert.DoesNotContain("has-message", field.Classes);
        Assert.DoesNotContain("required", field.Classes);
        Assert.DoesNotContain("invalid", field.Classes);
    }

    [Fact]
    public void FieldCompositionPrimitivesSyncWebClasses()
    {
        var firstField = new CodexField { Label = "Name", Content = new CodexTextBox() };
        var secondField = new CodexField { Label = "Email", Content = new CodexTextBox() };
        var group = new CodexFieldGroup
        {
            Orientation = CodexFieldOrientation.Responsive,
            Size = CodexControlSize.Small,
            Items =
            {
                firstField,
                secondField
            }
        };

        var set = new CodexFieldSet
        {
            Legend = "Notifications",
            Description = "Choose delivery channels.",
            Orientation = CodexFieldOrientation.Horizontal,
            Size = CodexControlSize.Large,
            Items =
            {
                new CodexField { Label = "Email", Content = new CodexCheckBox() },
                new CodexField { Label = "Push", Content = new CodexSwitch() }
            }
        };

        var legend = new CodexFieldLegend
        {
            Content = "Delivery",
            Variant = CodexFieldLegendVariant.Label,
            Size = CodexControlSize.Small
        };
        var content = new CodexFieldContent { Content = new CodexFieldTitle { Content = "Touch ID" } };
        var description = new CodexFieldDescription { Content = "Unlock faster." };
        var separator = new CodexFieldSeparator { Content = "Or continue with", Size = CodexControlSize.Small };
        var error = new CodexFieldError { Message = "Enter a valid email address." };
        error.Items.Add("Use a company domain.");

        Assert.Contains("field-group", group.Classes);
        Assert.Contains("orientation-responsive", group.Classes);
        Assert.Contains("size-sm", group.Classes);
        Assert.Contains("has-items", group.Classes);
        Assert.Contains("field-group-item", firstField.Classes);
        Assert.Contains("size-sm", firstField.Classes);

        Assert.Contains("field-set", set.Classes);
        Assert.True(set.HasLegend);
        Assert.True(set.HasDescription);
        Assert.True(set.HasItems);
        Assert.Contains("orientation-horizontal", set.Classes);
        Assert.Contains("size-lg", set.Classes);

        Assert.Contains("field-legend", legend.Classes);
        Assert.Contains("variant-label", legend.Classes);
        Assert.Contains("has-content", legend.Classes);
        Assert.Contains("field-content", content.Classes);
        Assert.Contains("field-title", ((CodexFieldTitle)content.Content!).Classes);
        Assert.Contains("field-description", description.Classes);
        Assert.Contains("field-separator", separator.Classes);
        Assert.Contains("has-content", separator.Classes);
        Assert.Contains("field-error", error.Classes);
        Assert.Contains("has-message", error.Classes);
        Assert.Contains("has-items", error.Classes);
    }

    [Fact]
    public void FormStylesDeclareTemplatesAndMotion()
    {
        var root = FindRepositoryRoot();

        foreach (var component in InteractiveFormStyleFiles)
        {
            var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml"));

            Assert.Contains("ControlTemplate", style);
            Assert.Contains("Transitions", style);
        }
    }

    [Fact]
    public void FormStylesUseTokenizedWebMotion()
    {
        var root = FindRepositoryRoot();

        foreach (var component in InteractiveFormStyleFiles)
        {
            var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml"));

            Assert.DoesNotContain("Duration=\"0:0:0.", style);
            Assert.Contains("CodexSwitch.MotionDuration", style);
            Assert.Contains("CodexSwitch.MotionEaseOut", style);
        }
    }

    [Fact]
    public void InteractiveFormControlsUseFocusVisibleContract()
    {
        var root = FindRepositoryRoot();
        var controlSources = new[]
        {
            "CodexButton",
            "CodexTextBox",
            "CodexInputOtp",
            "CodexSelect",
            "CodexCombobox",
            "CodexNativeSelect",
            "CodexCalendar",
            "CodexDatePicker",
            "CodexCheckBox",
            "CodexRadio",
            "CodexSwitch",
            "CodexToggle",
            "CodexSlider"
        };

        foreach (var control in controlSources)
        {
            var source = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", $"{control}.cs"));

            Assert.Contains("[PseudoClasses(CodexFocusVisible.PseudoClass)]", source);
            Assert.Contains("OnGotFocus(FocusChangedEventArgs e)", source);
            Assert.Contains("CodexFocusVisible.FromFocusChange(e)", source);
            Assert.Contains("OnPointerPressed(PointerPressedEventArgs e)", source);
            Assert.Contains("PseudoClasses.Set(CodexFocusVisible.PseudoClass, false)", source);
        }

        foreach (var component in InteractiveFormStyleFiles)
        {
            var style = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", $"{component}.axaml"));

            Assert.Contains(":focus-visible", style);
            Assert.DoesNotContain(":focus /template", style);
        }
    }

    [Fact]
    public void FormStylesContainCustomInteractionParts()
    {
        var root = FindRepositoryRoot();

        AssertStyleContains(root, "Button", "PART_LoadingIndicator", "PART_FocusRing", "has-leading-icon", "variant-ghost");
        AssertStyleContains(root, "ButtonGroup", "PART_ItemsPresenter", "PART_Text", "CodexButtonGroupSeparator", "button-group-item", "group-first", "group-last", "controls|CodexTextBox.group-item", "controls|CodexSelect.group-item", "controls|CodexCombobox.group-item", "controls|CodexNativeSelect.group-item");
        AssertStyleContains(root, "InputGroup", "PART_ItemsPresenter", "PART_FocusRing", "PART_Addon", "PART_AddonContent", "PART_Text", "has-focus-within", "input-group-control", "align-block-end", "controls|CodexTextarea.input-group-control", "controls|CodexCombobox.input-group-control", "controls|CodexNativeSelect.input-group-control");
        AssertStyleContains(root, "InputOtp", "PART_ItemsPresenter", "PART_GroupItemsPresenter", "PART_SlotRoot", "PART_Character", "PART_FocusRing", "PART_Separator", "input-otp-slot", "input-otp-separator", "active", "complete", "has-character", "TransformOperationsTransition");
        AssertStyleContains(root, "Label", "PART_Content", "PART_Required", "RecognizesAccessKey=\"True\"", "has-target", "target-disabled", "required");
        AssertStyleContains(root, "Field", "PART_Label", "controls:CodexLabel", "PART_Control", "PART_Description", "PART_Message", "has-message", "required");
        AssertStyleContains(root, "Input", "PART_TextPresenter", "PART_FocusRing", "SelectionBrush", "CaretBrush", "is-read-only");
        AssertStyleContains(root, "Textarea", "PART_TextPresenter", "PART_FocusRing", "SelectionBrush", "CaretBrush", "AcceptsReturn", "textarea-tall");
        AssertStyleContains(root, "Select", "PART_Popup", "PART_Chevron", "PART_FocusRing", "Placement=\"Bottom\"", "controls|CodexSelect ComboBoxItem:selected");
        AssertStyleContains(root, "Combobox", "PART_InputGroup", "PART_Input", "PART_Clear", "PART_Trigger", "PART_Chevron", "PART_Popup", "PART_PopupBorder", "PART_Loading", "PART_Empty", "PART_List", "CodexComboboxItem", "controls|CodexComboboxItem.highlighted", "controls|CodexComboboxItem.selected");
        AssertStyleContains(root, "NativeSelect", "PART_Popup", "PART_Chevron", "PART_FocusRing", "Placement=\"Bottom\"", "CodexNativeSelectOption", "CodexNativeSelectOptGroup", "placeholder-visible", "invalid", "controls|CodexNativeSelect ComboBoxItem:selected");
        AssertStyleContains(root, "Calendar", "PART_Header", "PART_PreviousButton", "PART_NextButton", "PART_MonthTitle", "PART_DayRange", "PART_DayRoot", "PART_DayContent", "PART_DayFocusRing", "calendar-day", "range-start", "range-end", "range-middle", "booked", "unavailable", "week-numbers", "TransformOperationsTransition");
        AssertStyleContains(root, "DatePicker", "PART_InputGroup", "PART_FocusRing", "PART_Trigger", "PART_CalendarIcon", "PART_Clear", "PART_Chevron", "PART_Popup", "PART_PopupBorder", "PART_Calendar", "PART_Loading", "controls:CodexCalendar", "placeholder-visible", "range-complete", "close-on-select", "close-on-escape", "TransformOperationsTransition");
        AssertStyleContains(root, "Checkbox", "PART_Check", "PART_FocusRing", "state-checked", "state-indeterminate", ":indeterminate", ":pressed", "TransformOperationsTransition");
        AssertStyleContains(root, "Radio", "PART_Dot", "PART_FocusRing", "TransformOperationsTransition", ":checked", ":pressed");
        AssertStyleContains(root, "RadioGroup", "PART_ItemsPresenter", "CodexRadioGroupItem", "state-checked", "state-unchecked", "horizontal", "vertical", "roving", "loop", "loading");
        AssertStyleContains(root, "Switch", "PART_Track", "PART_Thumb", "PART_FocusRing", "PART_Content", "HasContent", "size-lg:checked", "TransformOperationsTransition");
        AssertStyleContains(root, "Toggle", "PART_Root", "PART_Content", "PART_FocusRing", "controls|CodexToggleGroup", "CodexToggleGroupItem", "state-on", "type-single", "type-multiple", "roving", "loop", "spacing-0", "spacing-2", "group-first", "group-last", "connected", "TransformOperationsTransition");
        AssertStyleContains(root, "Slider", "PART_Track", "PART_Thumb", "RepeatButton.Template", "dragging", "TransformOperationsTransition");
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
        Assert.Contains("SelectingItemsControl.SelectionChangedEvent.AddClassHandler<CodexSelect>", selectCode);
        Assert.Contains("public event EventHandler<CodexSelectValueChangedEventArgs>? ValueChanged;", selectCode);
        Assert.Contains("public event EventHandler<CodexSelectOpenChangedEventArgs>? OpenChanged;", selectCode);
        Assert.Contains("Classes.Set(\"has-selection\", hasSelection)", selectCode);
        Assert.Contains("Classes.Set(\"placeholder-visible\", !hasSelection)", selectCode);
        Assert.Contains("Classes.Set(\"popup-open\", false)", selectCode);
        Assert.Contains("Dispatcher.UIThread.Post", selectCode);
        Assert.Contains("Classes.Set(\"popup-open\", true)", selectCode);
    }

    [Fact]
    public void NativeSelectStyleOwnsOptionsGroupsInvalidAndOpeningMotion()
    {
        var root = FindRepositoryRoot();
        var selectCode = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexNativeSelect.cs"));

        AssertStyleContains(
            root,
            "NativeSelect",
            "ItemContainerTheme",
            "<ControlTheme TargetType=\"ComboBoxItem\"",
            "<ControlTemplate TargetType=\"controls:CodexNativeSelectOption\"",
            "<ControlTemplate TargetType=\"controls:CodexNativeSelectOptGroup\"",
            "native-select-option",
            "native-select-optgroup",
            "placeholder-visible",
            "has-selection",
            "invalid",
            "Opacity=\"0\"",
            "RenderTransform=\"translate(0px, -2px) scale(0.985)\"",
            "controls|CodexNativeSelect.popup-open /template/ Border#PART_PopupBorder",
            "<Setter Property=\"Opacity\" Value=\"1\"",
            "<Setter Property=\"RenderTransform\" Value=\"translate(0px, 0px) scale(1)\"",
            "TransformOperationsTransition");

        Assert.Contains("IsDropDownOpenProperty.Changed.AddClassHandler<CodexNativeSelect>", selectCode);
        Assert.Contains("SelectingItemsControl.SelectionChangedEvent.AddClassHandler<CodexNativeSelect>", selectCode);
        Assert.Contains("SelectedItemProperty.Changed.AddClassHandler<CodexNativeSelect>", selectCode);
        Assert.Contains("public event EventHandler<CodexNativeSelectValueChangedEventArgs>? ValueChanged;", selectCode);
        Assert.Contains("public event EventHandler<CodexNativeSelectOpenChangedEventArgs>? OpenChanged;", selectCode);
        Assert.Contains("Classes.Set(\"popup-open\", false)", selectCode);
        Assert.Contains("Dispatcher.UIThread.Post", selectCode);
        Assert.Contains("Classes.Set(\"popup-open\", true)", selectCode);
        Assert.Contains("Classes.Set(\"placeholder-visible\", !hasSelection)", selectCode);
    }

    [Fact]
    public void ComboboxStyleOwnsSearchPopupItemStatesAndOpeningMotion()
    {
        var root = FindRepositoryRoot();
        var comboboxCode = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Controls", "CodexCombobox.cs"));

        AssertStyleContains(
            root,
            "Combobox",
            "<ControlTemplate TargetType=\"controls:CodexCombobox\"",
            "<ControlTemplate TargetType=\"controls:CodexComboboxItem\"",
            "PART_InputGroup",
            "PART_Input",
            "PART_Clear",
            "PART_Trigger",
            "PART_Chevron",
            "PART_Popup",
            "PART_PopupBorder",
            "PART_Loading",
            "PART_Empty",
            "PART_List",
            "PART_ItemRoot",
            "PART_Check",
            "Opacity=\"0\"",
            "RenderTransform=\"scale(0.98)\"",
            "controls|CodexCombobox.open /template/ Border#PART_PopupBorder",
            "<Setter Property=\"Opacity\" Value=\"1\"",
            "<Setter Property=\"RenderTransform\" Value=\"scale(1)\"",
            "controls|CodexComboboxItem.highlighted",
            "controls|CodexComboboxItem.selected /template/ PathIcon#PART_Check",
            "TransformOperationsTransition");

        Assert.Contains("ItemsSourceProperty.Changed.AddClassHandler<CodexCombobox>", comboboxCode);
        Assert.Contains("SelectedItemProperty.Changed.AddClassHandler<CodexCombobox>", comboboxCode);
        Assert.Contains("TextProperty.Changed.AddClassHandler<CodexCombobox>", comboboxCode);
        Assert.Contains("[Content]", comboboxCode);
        Assert.Contains("TryHandleInputKey(Key key)", comboboxCode);
        Assert.Contains("OpenOnInput", comboboxCode);
        Assert.Contains("AutoHighlight", comboboxCode);
        Assert.Contains("HighlightItemOnHover", comboboxCode);
        Assert.Contains("CloseOnSelect", comboboxCode);
    }

    [Fact]
    public void FormStylesCoverStateSizeIntentAndVariantMatrix()
    {
        var root = FindRepositoryRoot();

        foreach (var component in InteractiveFormStyleFiles)
        {
            AssertStyleContains(root, component, "pointerover", ":focus-visible", ":disabled");
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

        foreach (var component in InteractiveFormStyleFiles.Except(["Button", "Toggle"]))
        {
            AssertStyleContains(root, component, "intent-error", "intent-success", "intent-warning", "size-sm", "size-lg");
        }

        AssertStyleContains(root, "Field", "intent-error", "intent-success", "intent-warning", "size-sm", "size-lg", ":disabled");
        AssertStyleContains(root, "InputGroup", "intent-error", "intent-success", "intent-warning", "size-sm", "size-lg", ":disabled", "has-focus-within");
        AssertStyleContains(root, "Label", "intent-error", "intent-success", "intent-warning", "size-sm", "size-lg", ":disabled");
        AssertStyleContains(root, "Select", ":pressed", "dropdownopen");
        AssertStyleContains(root, "Combobox", "open", "empty", "loading", "highlighted", "selected", "has-clear");
        AssertStyleContains(root, "NativeSelect", ":pressed", "dropdownopen", "placeholder-visible", "invalid");
        AssertStyleContains(root, "Checkbox", ":pressed", ":checked");
        AssertStyleContains(root, "Radio", ":pressed", ":checked");
        AssertStyleContains(root, "RadioGroup", "intent-error", "intent-success", "intent-warning", "size-sm", "size-lg", "state-checked", "loading");
        AssertStyleContains(root, "Switch", ":pressed", ":checked");
        AssertStyleContains(root, "Toggle", ":pressed", ":checked", "variant-outline", "variant-ghost", "state-on", "state-off", "size-sm", "size-lg", "size-icon");
        AssertStyleContains(root, "Slider", ":pressed", "dragging", "PART_DecreaseButton", "PART_IncreaseButton");
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
