using Avalonia.Input;
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
    public void SplitButtonMenuTriggerKeysOpenLikeWebMenuTrigger()
    {
        var changes = new List<(bool IsOpen, CodexSplitButtonOpenChangeSource Source)>();
        var splitButton = new CodexSplitButton
        {
            Content = "Run sync",
            DropDownContent = new CodexButton { Content = "Run once" },
            IsArrowVisible = true
        };
        splitButton.OpenChanged += (_, args) => changes.Add((args.IsOpen, args.Source));

        Assert.True(splitButton.HasDropDownContent);
        Assert.True(splitButton.CanOpenDropDown);
        Assert.Contains("closed", splitButton.Classes);
        Assert.True(splitButton.TryHandleMenuTriggerKey(Key.Enter));
        Assert.True(splitButton.IsOpen);
        Assert.Equal([(true, CodexSplitButtonOpenChangeSource.Keyboard)], changes);

        Assert.True(splitButton.TryHandleDismissKey(Key.Escape));
        Assert.False(splitButton.IsOpen);
        Assert.Equal(
            [
                (true, CodexSplitButtonOpenChangeSource.Keyboard),
                (false, CodexSplitButtonOpenChangeSource.Keyboard)
            ],
            changes);

        Assert.True(splitButton.TryHandleMenuTriggerKey(Key.Space));
        Assert.True(splitButton.IsOpen);
        Assert.Equal(CodexSplitButtonOpenChangeSource.Keyboard, changes[^1].Source);

        Assert.True(splitButton.TryHandleDismissKey(Key.Escape));
        Assert.True(splitButton.TryHandleMenuTriggerKey(Key.Down));
        Assert.True(splitButton.IsOpen);
        Assert.Equal(
            [
                (true, CodexSplitButtonOpenChangeSource.Keyboard),
                (false, CodexSplitButtonOpenChangeSource.Keyboard),
                (true, CodexSplitButtonOpenChangeSource.Keyboard),
                (false, CodexSplitButtonOpenChangeSource.Keyboard),
                (true, CodexSplitButtonOpenChangeSource.Keyboard)
            ],
            changes);

        Assert.False(splitButton.TryHandleMenuTriggerKey(Key.Up));
        Assert.True(splitButton.IsOpen);

        Assert.True(splitButton.TryHandleDismissKey(Key.Escape));
        Assert.False(splitButton.TryHandleMenuTriggerPointerRelease(PointerUpdateKind.RightButtonReleased));
        Assert.False(splitButton.TryHandleMenuTriggerPointerRelease(PointerUpdateKind.MiddleButtonReleased));
        Assert.False(splitButton.IsOpen);

        Assert.True(splitButton.TryHandleMenuTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(splitButton.IsOpen);
        Assert.Equal((true, CodexSplitButtonOpenChangeSource.Pointer), changes[^1]);

        splitButton.IsLoading = true;
        splitButton.IsOpen = false;
        Assert.Equal((false, CodexSplitButtonOpenChangeSource.Programmatic), changes[^1]);
        Assert.False(splitButton.TryHandleMenuTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.False(splitButton.TryHandleMenuTriggerKey(Key.Enter));
        Assert.False(splitButton.IsOpen);
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
        var changes = new List<CodexCheckBoxCheckedStateChangedEventArgs>();
        checkBox.CheckedStateChanged += (_, args) => changes.Add(args);

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
                (false, true, CodexCheckBoxCheckedStateChangeSource.Programmatic),
                (true, null, CodexCheckBoxCheckedStateChangeSource.Programmatic),
                (null, false, CodexCheckBoxCheckedStateChangeSource.Programmatic)
            ],
            changes.Select(change => (change.OldValue, change.NewValue, change.Source)));

        changes.Clear();
        Assert.True(checkBox.TryHandleActivationKey(Key.Space));
        Assert.Equal(
            (false, true, CodexCheckBoxCheckedStateChangeSource.Keyboard),
            changes.Select(change => (change.OldValue, change.NewValue, change.Source)).Single());

        changes.Clear();
        Assert.True(checkBox.SetCheckedState(false, CodexCheckBoxCheckedStateChangeSource.Pointer));
        Assert.Equal(
            (true, false, CodexCheckBoxCheckedStateChangeSource.Pointer),
            changes.Select(change => (change.OldValue, change.NewValue, change.Source)).Single());
        Assert.False(checkBox.TryHandleActivationKey(Key.Enter));
    }

    [Fact]
    public void ToggleAndToggleGroupMirrorWebPressedAndSelectionState()
    {
        var toggle = new CodexToggle { Content = "Bookmark" };
        var pressedChanges = new List<CodexTogglePressedChangedEventArgs>();
        toggle.PressedChanged += (_, args) => pressedChanges.Add(args);

        Assert.Contains("state-off", toggle.Classes);
        Assert.True(toggle.TryHandleActivationKey(Avalonia.Input.Key.Space));
        Assert.True(toggle.IsPressed);
        Assert.Contains("state-on", toggle.Classes);
        Assert.True(toggle.TryHandleActivationKey(Avalonia.Input.Key.Enter));
        Assert.False(toggle.IsPressed);
        toggle.IsPressed = false;
        toggle.IsPressed = true;
        Assert.True(toggle.SetPressedState(false, CodexTogglePressedChangeSource.Pointer));
        Assert.Equal(
            [
                (false, true, CodexTogglePressedChangeSource.Keyboard),
                (true, false, CodexTogglePressedChangeSource.Keyboard),
                (false, true, CodexTogglePressedChangeSource.Programmatic),
                (true, false, CodexTogglePressedChangeSource.Pointer)
            ],
            pressedChanges.Select(change => (change.OldValue, change.NewValue, change.Source)));

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
    public void ToggleGroupValueChangedPublishesSourceMetadataAndPrimaryPointerRelease()
    {
        var left = new CodexToggleGroupItem { Content = "Left", Value = "left" };
        var code = new CodexToggleGroupItem { Content = "Code", Value = "code" };
        var disabled = new CodexToggleGroupItem { Content = "Disabled", Value = "disabled", IsEnabled = false };
        var group = new CodexToggleGroup
        {
            Items =
            {
                left,
                code,
                disabled
            }
        };
        left.IsPressed = true;

        var changes = new List<CodexToggleGroupValueChangedEventArgs>();
        group.ValueChanged += (_, args) => changes.Add(args);

        Assert.False(code.TryHandlePointerActivation(PointerUpdateKind.RightButtonReleased));
        Assert.False(code.TryHandlePointerActivation(PointerUpdateKind.MiddleButtonReleased));
        Assert.Equal("left", group.SelectedValue);
        Assert.Empty(changes);

        Assert.True(code.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));

        var pointerChange = Assert.Single(changes);
        Assert.Equal("left", pointerChange.OldValue);
        Assert.Equal("code", pointerChange.NewValue);
        Assert.Equal(["left"], pointerChange.OldValues);
        Assert.Equal(["code"], pointerChange.NewValues);
        Assert.Equal(CodexToggleGroupValueChangeSource.Pointer, pointerChange.Source);

        changes.Clear();
        Assert.True(left.TryHandleActivationKey(Key.Enter));

        var keyboardChange = Assert.Single(changes);
        Assert.Equal("code", keyboardChange.OldValue);
        Assert.Equal("left", keyboardChange.NewValue);
        Assert.Equal(CodexToggleGroupValueChangeSource.Keyboard, keyboardChange.Source);

        changes.Clear();
        group.SelectedValue = "code";

        var programmaticChange = Assert.Single(changes);
        Assert.Equal("left", programmaticChange.OldValue);
        Assert.Equal("code", programmaticChange.NewValue);
        Assert.Equal(CodexToggleGroupValueChangeSource.Programmatic, programmaticChange.Source);

        Assert.False(disabled.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.Equal("code", group.SelectedValue);
        Assert.Single(changes);

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
        bold.IsPressed = true;
        var multipleChanges = new List<CodexToggleGroupValueChangedEventArgs>();
        multiple.ValueChanged += (_, args) => multipleChanges.Add(args);

        Assert.True(italic.TryHandleActivationKey(Key.Space));

        var multipleKeyboardChange = Assert.Single(multipleChanges);
        Assert.Equal(["bold"], multipleKeyboardChange.OldValues);
        Assert.Equal(["bold", "italic"], multipleKeyboardChange.NewValues);
        Assert.Equal(CodexToggleGroupValueChangeSource.Keyboard, multipleKeyboardChange.Source);

        multipleChanges.Clear();
        multiple.SelectedValues = ["italic"];

        var multipleProgrammaticChange = Assert.Single(multipleChanges);
        Assert.Equal(["bold", "italic"], multipleProgrammaticChange.OldValues);
        Assert.Equal(["italic"], multipleProgrammaticChange.NewValues);
        Assert.Equal(CodexToggleGroupValueChangeSource.Programmatic, multipleProgrammaticChange.Source);
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
        Assert.Equal(CodexRadioGroupValueChangeSource.KeyboardNavigation, changes[0].Source);
        Assert.Equal(CodexRadioGroupValueChangeSource.Keyboard, changes[1].Source);

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
    public void RadioGroupItemPointerActivationUsesPrimaryReleaseOnly()
    {
        var balanced = new CodexRadioGroupItem { Content = "Balanced", Value = "balanced" };
        var reasoning = new CodexRadioGroupItem { Content = "Reasoning", Value = "reasoning" };
        var disabled = new CodexRadioGroupItem { Content = "Disabled", Value = "disabled", IsEnabled = false };
        var group = new CodexRadioGroup
        {
            SelectedValue = "balanced",
            Items =
            {
                balanced,
                reasoning,
                disabled
            }
        };
        var changes = new List<CodexRadioGroupValueChangedEventArgs>();
        group.ValueChanged += (_, args) => changes.Add(args);
        group.ApplyTemplate();
        Assert.True(group.SelectItem(balanced));

        Assert.False(reasoning.TryHandlePointerActivation(PointerUpdateKind.RightButtonReleased));
        Assert.False(reasoning.TryHandlePointerActivation(PointerUpdateKind.MiddleButtonReleased));
        Assert.True(balanced.IsChecked);
        Assert.False(reasoning.IsChecked);
        Assert.Empty(changes);

        Assert.True(reasoning.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.False(balanced.IsChecked);
        Assert.True(reasoning.IsChecked);
        Assert.Equal("reasoning", group.SelectedValue);
        var change = Assert.Single(changes);
        Assert.Equal("balanced", change.OldValue);
        Assert.Equal("reasoning", change.NewValue);
        Assert.Equal(CodexRadioGroupValueChangeSource.Pointer, change.Source);

        Assert.False(disabled.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.True(reasoning.IsChecked);
        Assert.Equal("reasoning", group.SelectedValue);

        group.IsLoading = true;
        Assert.False(balanced.TryHandlePointerActivation(PointerUpdateKind.LeftButtonReleased));
        Assert.True(reasoning.IsChecked);
        Assert.Equal("reasoning", group.SelectedValue);
        Assert.Single(changes);
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
        var openChanges = new List<(bool IsOpen, CodexSelectOpenChangeSource Source)>();
        var select = new CodexSelect
        {
            ItemsSource = new[] { "OpenAI", "Claude", "Responses" },
            PlaceholderText = "Select provider"
        };
        select.ValueChanged += (_, args) => valueChanges.Add(args);
        select.OpenChanged += (_, args) => openChanges.Add((args.IsOpen, args.Source));

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
        Assert.Equal(CodexSelectValueChangeSource.Programmatic, valueChanges[0].Source);

        Assert.True(select.SelectIndex(2, CodexSelectValueChangeSource.Keyboard));

        Assert.Equal(1, valueChanges[^1].OldIndex);
        Assert.Equal("Claude", valueChanges[^1].OldValue);
        Assert.Equal("Responses", valueChanges[^1].NewValue);
        Assert.Equal(CodexSelectValueChangeSource.Keyboard, valueChanges[^1].Source);

        Assert.True(select.SelectIndex(0, CodexSelectValueChangeSource.Pointer));

        Assert.Equal("Responses", valueChanges[^1].OldValue);
        Assert.Equal("OpenAI", valueChanges[^1].NewValue);
        Assert.Equal(CodexSelectValueChangeSource.Pointer, valueChanges[^1].Source);

        Assert.True(select.SetDropDownOpen(true, CodexSelectOpenChangeSource.Keyboard));
        Dispatcher.UIThread.RunJobs();

        Assert.Equal((true, CodexSelectOpenChangeSource.Keyboard), openChanges[^1]);
        Assert.Contains("popup-open", select.Classes);

        Assert.True(select.SetDropDownOpen(false, CodexSelectOpenChangeSource.Pointer));

        Assert.Equal((false, CodexSelectOpenChangeSource.Pointer), openChanges[^1]);
        Assert.DoesNotContain("popup-open", select.Classes);

        var openChangeCount = openChanges.Count;
        select.IsDropDownOpen = true;
        select.IsDropDownOpen = false;

        Assert.Equal(
            [
                (true, CodexSelectOpenChangeSource.Programmatic),
                (false, CodexSelectOpenChangeSource.Programmatic)
            ],
            openChanges.Skip(openChangeCount));
    }

    [Fact]
    public void NativeSelectRaisesWebValueAndOpenChangeEvents()
    {
        var placeholder = new CodexNativeSelectOption { Value = "", Content = "Select status" };
        var todo = new CodexNativeSelectOption { Value = "todo", Content = "Todo" };
        var done = new CodexNativeSelectOption { Value = "done", Content = "Done" };
        var valueChanges = new List<CodexNativeSelectValueChangedEventArgs>();
        var openChanges = new List<(bool IsOpen, CodexNativeSelectOpenChangeSource Source)>();
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
        select.OpenChanged += (_, args) => openChanges.Add((args.IsOpen, args.Source));

        Assert.Contains("native-select", select.Classes);
        Assert.Contains("placeholder-visible", select.Classes);
        Assert.DoesNotContain("has-selection", select.Classes);

        select.SelectedItem = placeholder;

        Assert.Single(valueChanges);
        Assert.Equal(-1, valueChanges[0].OldIndex);
        Assert.Equal(0, valueChanges[0].NewIndex);
        Assert.Null(valueChanges[0].OldValue);
        Assert.Equal("", valueChanges[0].NewValue);
        Assert.Equal(CodexNativeSelectValueChangeSource.Programmatic, valueChanges[0].Source);
        Assert.Contains("placeholder-visible", select.Classes);
        Assert.DoesNotContain("has-selection", select.Classes);

        Assert.True(select.SelectIndex(1, CodexNativeSelectValueChangeSource.Pointer));

        Assert.Equal(2, valueChanges.Count);
        Assert.Equal(0, valueChanges[^1].OldIndex);
        Assert.Equal(1, valueChanges[^1].NewIndex);
        Assert.Equal("", valueChanges[^1].OldValue);
        Assert.Equal("todo", valueChanges[^1].NewValue);
        Assert.Equal(CodexNativeSelectValueChangeSource.Pointer, valueChanges[^1].Source);
        Assert.Same(placeholder, valueChanges[^1].OldItem);
        Assert.Same(todo, valueChanges[^1].NewItem);
        Assert.Contains("has-selection", select.Classes);
        Assert.DoesNotContain("placeholder-visible", select.Classes);

        Assert.True(select.SelectIndex(2, CodexNativeSelectValueChangeSource.Keyboard));

        Assert.Equal("todo", valueChanges[^1].OldValue);
        Assert.Equal("done", valueChanges[^1].NewValue);
        Assert.Equal(CodexNativeSelectValueChangeSource.Keyboard, valueChanges[^1].Source);

        Assert.True(select.SetDropDownOpen(true, CodexNativeSelectOpenChangeSource.Keyboard));
        Dispatcher.UIThread.RunJobs();

        Assert.Equal((true, CodexNativeSelectOpenChangeSource.Keyboard), openChanges[^1]);
        Assert.Contains("popup-open", select.Classes);

        Assert.True(select.SetDropDownOpen(false, CodexNativeSelectOpenChangeSource.Pointer));

        Assert.Equal((false, CodexNativeSelectOpenChangeSource.Pointer), openChanges[^1]);
        Assert.DoesNotContain("popup-open", select.Classes);

        var openChangeCount = openChanges.Count;
        select.IsDropDownOpen = true;
        select.IsDropDownOpen = false;

        Assert.Equal(
            [
                (true, CodexNativeSelectOpenChangeSource.Programmatic),
                (false, CodexNativeSelectOpenChangeSource.Programmatic)
            ],
            openChanges.Skip(openChangeCount));
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
        var openChanges = new List<(bool IsOpen, CodexComboboxOpenChangeSource Source)>();
        var combobox = new CodexCombobox
        {
            ItemsSource = new[] { "Next.js", "SvelteKit", "Nuxt.js", "Remix", "Astro" },
            PlaceholderText = "Select a framework",
            EmptyContent = "No frameworks found.",
            AutoHighlight = true
        };
        combobox.SelectionChanged += (_, args) => changes.Add(args);
        combobox.InputValueChanged += (_, args) => inputChanges.Add((args.OldText, args.NewText));
        combobox.OpenChanged += (_, args) => openChanges.Add((args.IsOpen, args.Source));

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
        Assert.Equal((true, CodexComboboxOpenChangeSource.Input), openChanges[^1]);
        Assert.Contains(inputChanges, change => change.NewText == "n");

        Assert.True(combobox.TryHandleInputKey(Avalonia.Input.Key.Down));
        Assert.Equal("Nuxt.js", combobox.HighlightedItem);
        Assert.True(combobox.TryHandleInputKey(Avalonia.Input.Key.Enter));
        Assert.Equal("Nuxt.js", combobox.SelectedItem);
        Assert.Equal("Nuxt.js", combobox.Text);
        Assert.False(combobox.IsOpen);
        Assert.Equal((false, CodexComboboxOpenChangeSource.Keyboard), openChanges[^1]);
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
        Assert.True(combobox.IsOpen);
        Assert.Equal((true, CodexComboboxOpenChangeSource.Clear), openChanges[^1]);
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
        Assert.Equal((false, CodexComboboxOpenChangeSource.Keyboard), openChanges[^1]);

        var openChangeCount = openChanges.Count;
        combobox.IsOpen = true;
        combobox.IsOpen = false;

        Assert.Equal(
            [
                (true, CodexComboboxOpenChangeSource.Programmatic),
                (false, CodexComboboxOpenChangeSource.Programmatic)
            ],
            openChanges.Skip(openChangeCount));

        Assert.True(combobox.Open(CodexComboboxOpenChangeSource.Clear));
        Assert.Equal((true, CodexComboboxOpenChangeSource.Clear), openChanges[^1]);
        Assert.True(combobox.Close());

        Assert.True(combobox.SelectItem("Astro"));
        Assert.Equal("Astro", combobox.SelectedItem);
        Assert.Equal("Astro", combobox.Text);
        Assert.Equal(4, changes[^1].NewIndex);
        Assert.Equal(CodexComboboxSelectionChangeSource.Programmatic, changes[^1].Source);

        Assert.True(combobox.Open());
        combobox.CloseOnSelect = true;
        Assert.True(combobox.SelectItem("SvelteKit", CodexComboboxSelectionChangeSource.Item));
        Assert.Equal("SvelteKit", combobox.SelectedItem);
        Assert.Equal("SvelteKit", combobox.Text);
        Assert.Equal(1, changes[^1].NewIndex);
        Assert.Equal(CodexComboboxSelectionChangeSource.Item, changes[^1].Source);
        Assert.Equal((false, CodexComboboxOpenChangeSource.Item), openChanges[^1]);

        combobox.IsLoading = true;
        Assert.False(combobox.Open());
        Assert.False(combobox.SelectItem("Astro"));
        Assert.Contains("loading", combobox.Classes);
    }

    [Fact]
    public void ComboboxTriggerPointerReleaseUsesPrimaryButtonOnly()
    {
        var openChanges = new List<(bool IsOpen, CodexComboboxOpenChangeSource Source)>();
        var combobox = new CodexCombobox
        {
            ItemsSource = new[] { "Next.js", "SvelteKit", "Nuxt.js" }
        };
        combobox.OpenChanged += (_, args) => openChanges.Add((args.IsOpen, args.Source));

        Assert.False(combobox.TryHandleTriggerPointerRelease(PointerUpdateKind.RightButtonReleased));
        Assert.False(combobox.TryHandleTriggerPointerRelease(PointerUpdateKind.MiddleButtonReleased));
        Assert.False(combobox.IsOpen);
        Assert.Empty(openChanges);

        Assert.True(combobox.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(combobox.IsOpen);
        Assert.Equal([(true, CodexComboboxOpenChangeSource.Pointer)], openChanges);

        Assert.True(combobox.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.False(combobox.IsOpen);
        Assert.Equal(
            [
                (true, CodexComboboxOpenChangeSource.Pointer),
                (false, CodexComboboxOpenChangeSource.Pointer)
            ],
            openChanges);

        combobox.IsLoading = true;
        Assert.False(combobox.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.False(combobox.IsOpen);
        Assert.Equal(
            [
                (true, CodexComboboxOpenChangeSource.Pointer),
                (false, CodexComboboxOpenChangeSource.Pointer)
            ],
            openChanges);
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
    public void SliderPointerDragStartsAndCommitsOnlyForPrimaryPointer()
    {
        var committed = new List<CodexSliderValueCommittedEventArgs>();
        var slider = new CodexSlider
        {
            Minimum = 0,
            Maximum = 100,
            Value = 24
        };
        slider.ValueCommitted += (_, args) => committed.Add(args);

        Assert.False(slider.TryBeginPointerChange(PointerUpdateKind.RightButtonPressed));
        Assert.DoesNotContain("dragging", slider.Classes);
        slider.Value = 32;
        Assert.False(slider.TryCommitPointerValue(PointerUpdateKind.RightButtonReleased));
        Assert.Empty(committed);

        Assert.False(slider.TryBeginPointerChange(PointerUpdateKind.MiddleButtonPressed));
        Assert.DoesNotContain("dragging", slider.Classes);
        slider.Value = 40;
        Assert.False(slider.TryCommitPointerValue(PointerUpdateKind.MiddleButtonReleased));
        Assert.Empty(committed);

        Assert.True(slider.TryBeginPointerChange(PointerUpdateKind.LeftButtonPressed));
        Assert.Contains("dragging", slider.Classes);
        slider.Value = 58;
        Assert.False(slider.TryCommitPointerValue(PointerUpdateKind.RightButtonReleased));
        Assert.Contains("dragging", slider.Classes);
        Assert.Empty(committed);

        Assert.True(slider.TryCommitPointerValue(PointerUpdateKind.LeftButtonReleased));
        Assert.DoesNotContain("dragging", slider.Classes);
        Assert.Single(committed);
        Assert.Equal(24, committed[0].OldValue);
        Assert.Equal(58, committed[0].NewValue);
        Assert.Equal("pointer", committed[0].Source);

        slider.IsEnabled = false;
        Assert.False(slider.TryBeginPointerChange(PointerUpdateKind.LeftButtonPressed));
        Assert.DoesNotContain("dragging", slider.Classes);
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
        Assert.Contains("TryBeginPointerChange(PointerUpdateKind updateKind)", source);
        Assert.Contains("TryCommitPointerValue(PointerUpdateKind updateKind)", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonPressed", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("Properties.PointerUpdateKind", source);
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
        Assert.Equal(CodexCalendarChangeSource.Programmatic, selectedChanges[0].Source);
        Assert.Equal(new DateTime(2026, 1, 16), activeChanges[0].OldDate);
        Assert.Equal(new DateTime(2026, 1, 18), activeChanges[0].NewDate);
        Assert.Equal(CodexCalendarChangeSource.Programmatic, activeChanges[0].Source);

        calendar.SelectDate(new DateTime(2026, 1, 19), CodexCalendarChangeSource.Keyboard);
        Assert.Equal(new DateTime(2026, 1, 19), calendar.SelectedDate);
        Assert.Equal(CodexCalendarChangeSource.Keyboard, selectedChanges[^1].Source);

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
        Assert.Equal(CodexCalendarChangeSource.Programmatic, rangeChanges.Last().Source);

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
        Assert.Equal(CodexCalendarChangeSource.Programmatic, displayChanges.Last().Source);
        Assert.Contains("can-previous", calendar.Classes);

        calendar.NavigatePreviousMonth(CodexCalendarChangeSource.Keyboard);
        Assert.Equal(new DateTime(2026, 1, 1), calendar.DisplayDate);
        Assert.Equal(CodexCalendarChangeSource.Keyboard, displayChanges.Last().Source);
    }

    [Fact]
    public void CalendarDayButtonCommandBlocksSelectionBeforeActivation()
    {
        var commandExecutions = 0;
        var command = new TestCommand(() => commandExecutions++)
        {
            CanExecuteValue = false
        };
        var calendar = new CodexCalendar
        {
            DisplayDate = new DateTime(2026, 1, 1),
            SelectedDate = new DateTime(2026, 1, 16),
            ActiveDate = new DateTime(2026, 1, 16)
        };
        var day = calendar.Items.OfType<CodexCalendarDayButton>().Single(button => button.Date == new DateTime(2026, 1, 18));

        day.Command = command;

        Assert.False(day.CanActivate);
        Assert.Contains("command-blocked", day.Classes);
        Assert.DoesNotContain("can-activate", day.Classes);

        InvokeButtonClick(day);

        Assert.Equal(0, commandExecutions);
        Assert.Equal(new DateTime(2026, 1, 16), calendar.SelectedDate);

        command.CanExecuteValue = true;
        command.RaiseCanExecuteChanged();

        Assert.True(day.CanActivate);
        Assert.Contains("can-activate", day.Classes);
        Assert.DoesNotContain("command-blocked", day.Classes);

        InvokeButtonClick(day);

        Assert.Equal(1, commandExecutions);
        Assert.Equal(new DateTime(2026, 1, 18), calendar.SelectedDate);
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
        var openChanges = new List<CodexDatePickerOpenChangedEventArgs>();
        var selectedDates = new List<CodexDatePickerSelectedDateChangedEventArgs>();
        var ranges = new List<CodexDatePickerRangeChangedEventArgs>();
        picker.OpenChanged += (_, args) => openChanges.Add(args);
        picker.SelectedDateChanged += (_, args) => selectedDates.Add(args);
        picker.RangeChanged += (_, args) => ranges.Add(args);

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
        Assert.Equal([true, false], openChanges.Select(change => change.IsOpen));
        Assert.All(openChanges, change => Assert.Equal(CodexDatePickerChangeSource.Programmatic, change.Source));
        Assert.Contains(selectedDates, change =>
            change.NewDate == new DateTime(2026, 5, 20)
            && change.Source == CodexDatePickerChangeSource.Programmatic);

        Assert.True(picker.TryHandleInputKey(Avalonia.Input.Key.Delete));
        Assert.False(picker.HasSelection);
        Assert.Null(picker.DisplayText);
        Assert.Equal(CodexDatePickerChangeSource.Keyboard, selectedDates[^1].Source);

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
        Assert.Contains(ranges, range =>
            range.Start == new DateTime(2026, 5, 18)
            && range.End == new DateTime(2026, 5, 23)
            && range.Source == CodexDatePickerChangeSource.Programmatic);

        picker.IsOpen = true;
        Assert.True(picker.TryHandleInputKey(Avalonia.Input.Key.Escape));
        Assert.False(picker.IsOpen);
        Assert.Equal(CodexDatePickerChangeSource.Keyboard, openChanges[^1].Source);

        Assert.False(picker.SelectDate(new DateTime(2026, 5, 30)));
        Assert.Equal(new DateTime(2026, 5, 18), picker.RangeStart);
        picker.IsLoading = true;
        Assert.False(picker.Open());
        Assert.False(picker.SelectDate(new DateTime(2026, 5, 21)));
        Assert.Contains("loading", picker.Classes);
    }

    [Fact]
    public void DatePickerTriggerPointerReleaseUsesPrimaryButtonOnly()
    {
        var openChanges = new List<CodexDatePickerOpenChangedEventArgs>();
        var picker = new CodexDatePicker();
        picker.OpenChanged += (_, args) => openChanges.Add(args);

        Assert.False(picker.TryHandleTriggerPointerRelease(PointerUpdateKind.RightButtonReleased));
        Assert.False(picker.TryHandleTriggerPointerRelease(PointerUpdateKind.MiddleButtonReleased));
        Assert.False(picker.IsOpen);
        Assert.Empty(openChanges);

        Assert.True(picker.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.True(picker.IsOpen);
        Assert.Equal([true], openChanges.Select(change => change.IsOpen));
        Assert.Equal(CodexDatePickerChangeSource.Pointer, openChanges[^1].Source);

        Assert.True(picker.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.False(picker.IsOpen);
        Assert.Equal([true, false], openChanges.Select(change => change.IsOpen));
        Assert.Equal(CodexDatePickerChangeSource.Pointer, openChanges[^1].Source);

        picker.IsLoading = true;
        Assert.False(picker.TryHandleTriggerPointerRelease(PointerUpdateKind.LeftButtonReleased));
        Assert.False(picker.IsOpen);
        Assert.Equal([true, false], openChanges.Select(change => change.IsOpen));
    }

    [Fact]
    public void DatePickerCalendarPointerReleaseSyncsOnlyPrimarySelection()
    {
        var picker = new CodexDatePicker
        {
            DisplayDate = new DateTime(2026, 5, 1),
            DateFormat = "yyyy-MM-dd"
        };
        var selectedDates = new List<CodexDatePickerSelectedDateChangedEventArgs>();
        var openChanges = new List<CodexDatePickerOpenChangedEventArgs>();
        picker.SelectedDateChanged += (_, args) => selectedDates.Add(args);
        picker.OpenChanged += (_, args) => openChanges.Add(args);
        var calendar = new CodexCalendar
        {
            DisplayDate = new DateTime(2026, 5, 1),
            SelectedDate = new DateTime(2026, 5, 20)
        };

        picker.IsOpen = true;
        Assert.False(picker.TryHandleCalendarPointerRelease(PointerUpdateKind.RightButtonReleased, calendar));
        Assert.True(picker.IsOpen);
        Assert.Null(picker.SelectedDate);

        Assert.False(picker.TryHandleCalendarPointerRelease(PointerUpdateKind.MiddleButtonReleased, calendar));
        Assert.True(picker.IsOpen);
        Assert.Null(picker.SelectedDate);

        Assert.True(picker.TryHandleCalendarPointerRelease(PointerUpdateKind.LeftButtonReleased, calendar));
        Assert.False(picker.IsOpen);
        Assert.Equal(new DateTime(2026, 5, 20), picker.SelectedDate);
        Assert.Equal("2026-05-20", picker.DisplayText);
        Assert.Equal(CodexDatePickerChangeSource.Pointer, selectedDates.Single().Source);
        Assert.Equal(CodexDatePickerChangeSource.Pointer, openChanges[^1].Source);

        calendar.SelectedDate = new DateTime(2026, 5, 21);
        picker.IsOpen = true;
        picker.IsLoading = true;
        Assert.False(picker.TryHandleCalendarPointerRelease(PointerUpdateKind.LeftButtonReleased, calendar));
        Assert.True(picker.IsOpen);
        Assert.Equal(new DateTime(2026, 5, 20), picker.SelectedDate);
    }

    [Fact]
    public void DatePickerRangeCalendarPointerReleaseClosesOnlyWhenPrimarySelectionCompletes()
    {
        var picker = new CodexDatePicker
        {
            SelectionMode = CodexCalendarSelectionMode.Range,
            DisplayDate = new DateTime(2026, 5, 1)
        };
        var ranges = new List<CodexDatePickerRangeChangedEventArgs>();
        picker.RangeChanged += (_, args) => ranges.Add(args);
        var calendar = new CodexCalendar
        {
            SelectionMode = CodexCalendarSelectionMode.Range,
            DisplayDate = new DateTime(2026, 5, 1),
            RangeStart = new DateTime(2026, 5, 18)
        };

        picker.IsOpen = true;
        Assert.False(picker.TryHandleCalendarPointerRelease(PointerUpdateKind.RightButtonReleased, calendar));
        Assert.True(picker.IsOpen);
        Assert.Null(picker.RangeStart);
        Assert.Null(picker.RangeEnd);

        Assert.True(picker.TryHandleCalendarPointerRelease(PointerUpdateKind.LeftButtonReleased, calendar));
        Assert.True(picker.IsOpen);
        Assert.Equal(new DateTime(2026, 5, 18), picker.RangeStart);
        Assert.Null(picker.RangeEnd);
        Assert.Equal(CodexDatePickerChangeSource.Pointer, ranges[^1].Source);

        calendar.RangeEnd = new DateTime(2026, 5, 23);
        Assert.True(picker.TryHandleCalendarPointerRelease(PointerUpdateKind.LeftButtonReleased, calendar));
        Assert.False(picker.IsOpen);
        Assert.Equal(new DateTime(2026, 5, 18), picker.RangeStart);
        Assert.Equal(new DateTime(2026, 5, 23), picker.RangeEnd);
        Assert.Contains("range-complete", picker.Classes);
        Assert.Equal(CodexDatePickerChangeSource.Pointer, ranges[^1].Source);
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
        var checkedChanges = new List<CodexSwitchCheckedChangedEventArgs>();
        toggle.CheckedChanged += (_, args) => checkedChanges.Add(args);

        Assert.False(toggle.HasContent);

        toggle.Content = "Enable streaming";

        Assert.True(toggle.HasContent);

        toggle.Content = "";

        Assert.False(toggle.HasContent);

        toggle.IsChecked = true;
        toggle.IsChecked = true;
        toggle.IsChecked = false;
        toggle.IsChecked = null;
        Assert.True(toggle.TryHandleActivationKey(Key.Space));
        Assert.True(toggle.SetChecked(false, CodexSwitchCheckedChangeSource.Pointer));

        Assert.Equal(
            [
                (false, true, CodexSwitchCheckedChangeSource.Programmatic),
                (true, false, CodexSwitchCheckedChangeSource.Programmatic),
                (false, true, CodexSwitchCheckedChangeSource.Keyboard),
                (true, false, CodexSwitchCheckedChangeSource.Pointer)
            ],
            checkedChanges.Select(change => (change.OldValue, change.NewValue, change.Source)));
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
    public void DatePickerPointerReleaseContractsUsePrimaryButtonOnly()
    {
        var source = File.ReadAllText(Path.Combine(
            FindRepositoryRoot(),
            "src",
            "CodexSwitchUI",
            "Controls",
            "CodexDatePicker.cs"));

        Assert.Contains("TryHandleTriggerPointerRelease(PointerUpdateKind updateKind)", source);
        Assert.Contains("OnTriggerPointerReleased", source);
        Assert.Contains("TryHandleCalendarPointerRelease(PointerUpdateKind updateKind", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("IsLoading || !IsEnabled", source);
        Assert.Contains("Properties.PointerUpdateKind", source);
        Assert.Contains("TryHandleTriggerPointerRelease(updateKind)", source);
        Assert.Contains("TryHandleCalendarPointerRelease(updateKind)", source);
        Assert.Contains("InputElement.PointerReleasedEvent", source);
        Assert.Contains("public enum CodexDatePickerChangeSource", source);
        Assert.Contains("public CodexDatePickerChangeSource Source { get; }", source);
        Assert.Contains("TogglePopup(CodexDatePickerChangeSource.Pointer)", source);
        Assert.Contains("Open(CodexDatePickerChangeSource.Keyboard)", source);
        Assert.Contains("Close(CodexDatePickerChangeSource.Keyboard)", source);
        Assert.Contains("ClearSelection(CodexDatePickerChangeSource.Keyboard)", source);
        Assert.Contains("SyncFromCalendar(calendar, CodexDatePickerChangeSource.Pointer)", source);
        Assert.Contains("RunWithChangeSource(CodexDatePickerChangeSource source", source);
    }

    [Fact]
    public void CalendarSelectionEventsExposeWebSources()
    {
        var source = File.ReadAllText(Path.Combine(
            FindRepositoryRoot(),
            "src",
            "CodexSwitchUI",
            "Controls",
            "CodexCalendar.cs"));

        Assert.Contains("public enum CodexCalendarChangeSource", source);
        Assert.Contains("public CodexCalendarChangeSource Source { get; }", source);
        Assert.Contains("SelectDate(activeDate, CodexCalendarChangeSource.Keyboard)", source);
        Assert.Contains("MoveActiveDate(activeDate.AddDays(1), CodexCalendarChangeSource.Keyboard)", source);
        Assert.Contains("NavigateNextMonth(CodexCalendarChangeSource.Keyboard)", source);
        Assert.Contains("owner.SelectDate(Date, owner.CurrentChangeSource);", source);
        Assert.Contains("CodexCalendarChangeSource.Pointer", source);
        Assert.Contains("RunWithChangeSource(CodexCalendarChangeSource source", source);
    }

    [Fact]
    public void RadioGroupItemPointerAndKeyboardContractsExposeWebSources()
    {
        var source = File.ReadAllText(Path.Combine(
            FindRepositoryRoot(),
            "src",
            "CodexSwitchUI",
            "Controls",
            "CodexRadioGroup.cs"));

        Assert.Contains("public enum CodexRadioGroupValueChangeSource", source);
        Assert.Contains("Pointer", source);
        Assert.Contains("KeyboardNavigation", source);
        Assert.Contains("public CodexRadioGroupValueChangeSource Source", source);
        Assert.Contains("SelectItem(nextItem, CodexRadioGroupValueChangeSource.KeyboardNavigation)", source);
        Assert.Contains("group.SelectItem(this, CodexRadioGroupValueChangeSource.Keyboard)", source);
        Assert.Contains("group.SelectItem(this, CodexRadioGroupValueChangeSource.Pointer)", source);
        Assert.Contains("TryHandlePointerActivation(PointerUpdateKind updateKind)", source);
        Assert.Contains("updateKind != PointerUpdateKind.LeftButtonReleased", source);
        Assert.Contains("protected override void OnPointerPressed(PointerPressedEventArgs e)", source);
        Assert.Contains("protected override void OnPointerReleased(PointerReleasedEventArgs e)", source);
        Assert.Contains("Properties.PointerUpdateKind", source);
        Assert.Contains("_hasPrimaryPointerPress && IsPointerOver", source);
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
        AssertStyleContains(root, "Calendar", "PART_Header", "PART_PreviousButton", "PART_NextButton", "PART_MonthTitle", "PART_DayRange", "PART_DayRoot", "PART_DayContent", "PART_DayFocusRing", "calendar-day", "range-start", "range-end", "range-middle", "booked", "unavailable", "can-activate", "command-blocked", "week-numbers", "TransformOperationsTransition");
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
        var selectStyle = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Select.axaml"));
        var selectItemTheme = ExtractBlock(selectStyle, "<ControlTheme TargetType=\"ComboBoxItem\">", "</ControlTheme>");
        var selectItemStyle = ExtractStyleBlock(selectStyle, "controls|CodexSelect ComboBoxItem");
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

        Assert.Contains("Margin=\"0,4,0,0\"", selectStyle);
        Assert.Contains("<Setter Property=\"Padding\" Value=\"8,6\" />", selectItemTheme);
        Assert.Contains("<Setter Property=\"Padding\" Value=\"8,6\" />", selectItemStyle);
        Assert.DoesNotContain("Margin=\"0,6,0,0\"", selectStyle);
        Assert.DoesNotContain("<Setter Property=\"Padding\" Value=\"10,7\" />", selectItemTheme);
        Assert.DoesNotContain("<Setter Property=\"Padding\" Value=\"10,7\" />", selectItemStyle);

        Assert.Contains("IsDropDownOpenProperty.Changed.AddClassHandler<CodexSelect>", selectCode);
        Assert.Contains("SelectingItemsControl.SelectionChangedEvent.AddClassHandler<CodexSelect>", selectCode);
        Assert.Contains("public enum CodexSelectValueChangeSource", selectCode);
        Assert.Contains("public CodexSelectValueChangeSource Source { get; } = source;", selectCode);
        Assert.Contains("private CodexSelectValueChangeSource? _pendingValueChangeSource;", selectCode);
        Assert.Contains("private CodexSelectValueChangeSource? _nextInteractionSource;", selectCode);
        Assert.Contains("internal bool SelectIndex(int index, CodexSelectValueChangeSource source = CodexSelectValueChangeSource.Programmatic)", selectCode);
        Assert.Contains("new CodexSelectValueChangedEventArgs(", selectCode);
        Assert.Contains("source));", selectCode);
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
            "PART_Trigger",
            "PART_FocusRing",
            "PART_Placeholder",
            "PART_SelectedContentHost",
            "PART_Chevron",
            "PART_Popup",
            "PART_PopupBorder",
            "PART_ItemsPresenter",
            "PART_ItemRoot",
            "PART_OptionContent",
            "PART_OptGroup",
            "PART_OptGroupLabel",
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
        Assert.Contains("public enum CodexNativeSelectValueChangeSource", selectCode);
        Assert.Contains("public CodexNativeSelectValueChangeSource Source { get; } = source;", selectCode);
        Assert.Contains("private CodexNativeSelectValueChangeSource? _pendingValueChangeSource;", selectCode);
        Assert.Contains("private CodexNativeSelectValueChangeSource? _nextInteractionSource;", selectCode);
        Assert.Contains("internal bool SelectIndex(int index, CodexNativeSelectValueChangeSource source = CodexNativeSelectValueChangeSource.Programmatic)", selectCode);
        Assert.Contains("new CodexNativeSelectValueChangedEventArgs(", selectCode);
        Assert.Contains("source));", selectCode);
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
        var comboboxStyle = File.ReadAllText(Path.Combine(root, "src", "CodexSwitchUI", "Themes", "Controls", "Combobox.axaml"));
        var comboboxItemStyle = ExtractStyleBlock(comboboxStyle, "controls|CodexComboboxItem");
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

        Assert.Contains("Margin=\"0,4,0,0\"", comboboxStyle);
        Assert.Contains("<Setter Property=\"MinHeight\" Value=\"32\" />", comboboxItemStyle);
        Assert.Contains("<Setter Property=\"Padding\" Value=\"8,6\" />", comboboxItemStyle);
        Assert.DoesNotContain("Margin=\"0,6,0,0\"", comboboxStyle);
        Assert.DoesNotContain("<Setter Property=\"MinHeight\" Value=\"34\" />", comboboxItemStyle);
        Assert.DoesNotContain("<Setter Property=\"Padding\" Value=\"8,7\" />", comboboxItemStyle);

        Assert.Contains("ItemsSourceProperty.Changed.AddClassHandler<CodexCombobox>", comboboxCode);
        Assert.Contains("SelectedItemProperty.Changed.AddClassHandler<CodexCombobox>", comboboxCode);
        Assert.Contains("TextProperty.Changed.AddClassHandler<CodexCombobox>", comboboxCode);
        Assert.Contains("[Content]", comboboxCode);
        Assert.Contains("TryHandleInputKey(Key key)", comboboxCode);
        Assert.Contains("TryHandleTriggerPointerRelease(PointerUpdateKind updateKind)", comboboxCode);
        Assert.Contains("OnTriggerPointerReleased", comboboxCode);
        Assert.Contains("InputElement.PointerReleasedEvent", comboboxCode);
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

    private static string ExtractStyleBlock(string style, string selector)
    {
        return ExtractBlock(style, $"<Style Selector=\"{selector}\"", "</Style>");
    }

    private static string ExtractBlock(string style, string open, string close)
    {
        var start = style.IndexOf(open, StringComparison.Ordinal);

        Assert.True(start >= 0, $"Missing block opener '{open}'.");

        var end = style.IndexOf(close, start, StringComparison.Ordinal);

        Assert.True(end >= 0, $"Block opener '{open}' is not closed by '{close}'.");

        return style[start..(end + close.Length)];
    }

    private static string FindRepositoryRoot()
    {
        return TestRepository.FindRoot();
    }

    private static void InvokeButtonClick(Avalonia.Controls.Button button)
    {
        var method = button.GetType().GetMethod("OnClick", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(method);
        method.Invoke(button, null);
    }

    private sealed class TestCommand(Action execute) : System.Windows.Input.ICommand
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
