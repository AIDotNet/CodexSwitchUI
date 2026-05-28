using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;

namespace CodexSwitchUI.Controls;

public enum CodexNativeSelectValueChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public enum CodexNativeSelectOpenChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public sealed class CodexNativeSelectValueChangedEventArgs(
    object? oldItem,
    object? newItem,
    int oldIndex,
    int newIndex,
    string? oldValue,
    string? newValue,
    CodexNativeSelectValueChangeSource source = CodexNativeSelectValueChangeSource.Programmatic)
    : EventArgs
{
    public object? OldItem { get; } = oldItem;

    public object? NewItem { get; } = newItem;

    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public string? OldValue { get; } = oldValue;

    public string? NewValue { get; } = newValue;

    public CodexNativeSelectValueChangeSource Source { get; } = source;
}

public sealed class CodexNativeSelectOpenChangedEventArgs(
    bool isOpen,
    CodexNativeSelectOpenChangeSource source = CodexNativeSelectOpenChangeSource.Programmatic)
    : EventArgs
{
    public bool IsOpen { get; } = isOpen;

    public CodexNativeSelectOpenChangeSource Source { get; } = source;
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexNativeSelect : ComboBox
{
    private object? _lastSelectedItem;
    private int _lastSelectedIndex = -1;
    private string? _lastSelectedValue;
    private CodexNativeSelectValueChangeSource? _pendingValueChangeSource;
    private CodexNativeSelectValueChangeSource? _nextInteractionSource;
    private CodexNativeSelectOpenChangeSource? _pendingOpenChangeSource;
    private CodexNativeSelectOpenChangeSource? _nextOpenChangeSource;

    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexNativeSelect, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexNativeSelect, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsInvalidProperty =
        AvaloniaProperty.Register<CodexNativeSelect, bool>(nameof(IsInvalid));

    static CodexNativeSelect()
    {
        IntentProperty.Changed.AddClassHandler<CodexNativeSelect>((select, _) => select.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexNativeSelect>((select, _) => select.SyncClasses());
        IsInvalidProperty.Changed.AddClassHandler<CodexNativeSelect>((select, _) => select.SyncClasses());
        SelectedItemProperty.Changed.AddClassHandler<CodexNativeSelect>((select, _) => select.SyncSelectionClasses());
        SelectedIndexProperty.Changed.AddClassHandler<CodexNativeSelect>((select, _) => select.SyncSelectionClasses());
        IsDropDownOpenProperty.Changed.AddClassHandler<CodexNativeSelect>((select, args) => select.OnOpenChanged(args));
        SelectingItemsControl.SelectionChangedEvent.AddClassHandler<CodexNativeSelect>((select, args) => select.OnSelectionChanged(args));
    }

    public CodexNativeSelect()
    {
        SyncClasses();
        RememberSelection();
    }

    public event EventHandler<CodexNativeSelectValueChangedEventArgs>? ValueChanged;

    public event EventHandler<CodexNativeSelectOpenChangedEventArgs>? OpenChanged;

    public CodexControlIntent Intent
    {
        get => GetValue(IntentProperty);
        set => SetValue(IntentProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool IsInvalid
    {
        get => GetValue(IsInvalidProperty);
        set => SetValue(IsInvalidProperty, value);
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, CodexFocusVisible.FromFocusChange(e));
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        _nextInteractionSource = CodexNativeSelectValueChangeSource.Pointer;
        _nextOpenChangeSource = CodexNativeSelectOpenChangeSource.Pointer;
        base.OnPointerPressed(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (IsSelectionKey(e.Key))
        {
            _nextInteractionSource = CodexNativeSelectValueChangeSource.Keyboard;
            _nextOpenChangeSource = CodexNativeSelectOpenChangeSource.Keyboard;
        }

        base.OnKeyDown(e);
    }

    internal bool SetDropDownOpen(
        bool isOpen,
        CodexNativeSelectOpenChangeSource source = CodexNativeSelectOpenChangeSource.Programmatic)
    {
        if (IsDropDownOpen == isOpen)
        {
            return false;
        }

        RunWithOpenChangeSource(source, () => IsDropDownOpen = isOpen);
        return true;
    }

    internal bool SelectIndex(int index, CodexNativeSelectValueChangeSource source = CodexNativeSelectValueChangeSource.Programmatic)
    {
        if (index < -1 || index >= ItemsView.Count)
        {
            return false;
        }

        RunWithValueChangeSource(source, () => SelectedIndex = index);
        return true;
    }

    private void OnOpenChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncPopupClasses();

        if (args.OldValue is bool oldValue && oldValue == IsDropDownOpen)
        {
            return;
        }

        var source = _pendingOpenChangeSource ?? _nextOpenChangeSource ?? CodexNativeSelectOpenChangeSource.Programmatic;
        _nextOpenChangeSource = null;
        OpenChanged?.Invoke(this, new CodexNativeSelectOpenChangedEventArgs(IsDropDownOpen, source));
    }

    private void OnSelectionChanged(SelectionChangedEventArgs args)
    {
        var oldItem = args.RemovedItems.Count > 0 ? args.RemovedItems[0] : _lastSelectedItem;
        var newItem = args.AddedItems.Count > 0 ? args.AddedItems[0] : SelectedItem;
        var oldIndex = _lastSelectedIndex;
        var newIndex = SelectedIndex;
        var oldValue = _lastSelectedValue;
        var newValue = GetItemValue(newItem);

        SyncSelectionClasses();
        RememberSelection();

        if (oldIndex == newIndex && Equals(oldItem, newItem) && string.Equals(oldValue, newValue, StringComparison.Ordinal))
        {
            return;
        }

        var source = _pendingValueChangeSource ?? _nextInteractionSource ?? CodexNativeSelectValueChangeSource.Programmatic;
        _nextInteractionSource = null;

        ValueChanged?.Invoke(
            this,
            new CodexNativeSelectValueChangedEventArgs(
                oldItem,
                newItem,
                oldIndex,
                newIndex,
                oldValue,
                newValue,
                source));
    }

    private void SyncClasses()
    {
        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("native-select", true);
        Classes.Set("invalid", IsInvalid);
        SyncSelectionClasses();
    }

    private void SyncSelectionClasses()
    {
        var hasSelection = !string.IsNullOrEmpty(GetItemValue(SelectedItem));
        Classes.Set("has-selection", hasSelection);
        Classes.Set("placeholder-visible", !hasSelection);
    }

    private void SyncPopupClasses()
    {
        Classes.Set("popup-open", false);

        if (!IsDropDownOpen)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (IsDropDownOpen)
            {
                Classes.Set("popup-open", true);
            }
        });
    }

    private void RememberSelection()
    {
        _lastSelectedItem = SelectedItem;
        _lastSelectedIndex = SelectedIndex;
        _lastSelectedValue = GetItemValue(SelectedItem);
    }

    private void RunWithValueChangeSource(CodexNativeSelectValueChangeSource source, Action action)
    {
        var previousSource = _pendingValueChangeSource;
        _pendingValueChangeSource = source;
        try
        {
            action();
        }
        finally
        {
            _pendingValueChangeSource = previousSource;
        }
    }

    private void RunWithOpenChangeSource(CodexNativeSelectOpenChangeSource source, Action action)
    {
        var previousSource = _pendingOpenChangeSource;
        _pendingOpenChangeSource = source;
        try
        {
            action();
        }
        finally
        {
            _pendingOpenChangeSource = previousSource;
        }
    }

    private static bool IsSelectionKey(Key key)
    {
        return key is Key.Enter
            or Key.Space
            or Key.Up
            or Key.Down
            or Key.PageUp
            or Key.PageDown
            or Key.Home
            or Key.End;
    }

    private static string? GetItemValue(object? item)
    {
        return item switch
        {
            CodexNativeSelectOption option => option.Value ?? option.Content?.ToString(),
            ComboBoxItem comboBoxItem => comboBoxItem.Content?.ToString(),
            null => null,
            _ => item.ToString()
        };
    }
}

public class CodexNativeSelectOption : ComboBoxItem
{
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CodexNativeSelectOption, string?>(nameof(Value));

    static CodexNativeSelectOption()
    {
        ValueProperty.Changed.AddClassHandler<CodexNativeSelectOption>((option, _) => option.SyncClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexNativeSelectOption>((option, _) => option.SyncClasses());
    }

    public CodexNativeSelectOption()
    {
        SyncClasses();
    }

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            SyncClasses();
        }
    }

    private void SyncClasses()
    {
        Classes.Set("native-select-option", true);
        Classes.Set("has-value", !string.IsNullOrEmpty(Value));
        Classes.Set("option-disabled", !IsEnabled);
    }
}

public class CodexNativeSelectOptGroup : ComboBoxItem
{
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<CodexNativeSelectOptGroup, string?>(nameof(Label));

    static CodexNativeSelectOptGroup()
    {
        LabelProperty.Changed.AddClassHandler<CodexNativeSelectOptGroup>((group, _) => group.SyncClasses());
    }

    public CodexNativeSelectOptGroup()
    {
        Focusable = false;
        IsEnabled = false;
        SyncClasses();
    }

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == LabelProperty || change.Property == ContentProperty || change.Property == IsEnabledProperty)
        {
            SyncClasses();
        }
    }

    private void SyncClasses()
    {
        if (Content is null && !string.IsNullOrEmpty(Label))
        {
            SetCurrentValue(ContentProperty, Label);
        }

        Classes.Set("native-select-optgroup", true);
        Classes.Set("has-label", !string.IsNullOrEmpty(Label) || Content is not null);
    }
}
