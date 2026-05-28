using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;

namespace CodexSwitchUI.Controls;

public enum CodexSelectValueChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public enum CodexSelectOpenChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public sealed class CodexSelectValueChangedEventArgs(
    object? oldItem,
    object? newItem,
    int oldIndex,
    int newIndex,
    string? oldValue,
    string? newValue,
    CodexSelectValueChangeSource source = CodexSelectValueChangeSource.Programmatic)
    : EventArgs
{
    public object? OldItem { get; } = oldItem;

    public object? NewItem { get; } = newItem;

    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public string? OldValue { get; } = oldValue;

    public string? NewValue { get; } = newValue;

    public CodexSelectValueChangeSource Source { get; } = source;
}

public sealed class CodexSelectOpenChangedEventArgs(
    bool isOpen,
    CodexSelectOpenChangeSource source = CodexSelectOpenChangeSource.Programmatic)
    : EventArgs
{
    public bool IsOpen { get; } = isOpen;

    public CodexSelectOpenChangeSource Source { get; } = source;
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexSelect : ComboBox
{
    private object? _lastSelectedItem;
    private int _lastSelectedIndex = -1;
    private string? _lastSelectedValue;
    private CodexSelectValueChangeSource? _pendingValueChangeSource;
    private CodexSelectValueChangeSource? _nextInteractionSource;
    private CodexSelectOpenChangeSource? _pendingOpenChangeSource;
    private CodexSelectOpenChangeSource? _nextOpenChangeSource;

    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexSelect, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexSelect, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexSelect()
    {
        IntentProperty.Changed.AddClassHandler<CodexSelect>((select, _) => select.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexSelect>((select, _) => select.SyncClasses());
        SelectedItemProperty.Changed.AddClassHandler<CodexSelect>((select, _) => select.SyncSelectionClasses());
        SelectedIndexProperty.Changed.AddClassHandler<CodexSelect>((select, _) => select.SyncSelectionClasses());
        IsDropDownOpenProperty.Changed.AddClassHandler<CodexSelect>((select, args) => select.OnOpenChanged(args));
        SelectingItemsControl.SelectionChangedEvent.AddClassHandler<CodexSelect>((select, args) => select.OnSelectionChanged(args));
    }

    public CodexSelect()
    {
        SyncClasses();
        RememberSelection();
    }

    public event EventHandler<CodexSelectValueChangedEventArgs>? ValueChanged;

    public event EventHandler<CodexSelectOpenChangedEventArgs>? OpenChanged;

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
        _nextInteractionSource = CodexSelectValueChangeSource.Pointer;
        _nextOpenChangeSource = CodexSelectOpenChangeSource.Pointer;
        base.OnPointerPressed(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (IsSelectionKey(e.Key))
        {
            _nextInteractionSource = CodexSelectValueChangeSource.Keyboard;
            _nextOpenChangeSource = CodexSelectOpenChangeSource.Keyboard;
        }

        base.OnKeyDown(e);
    }

    internal bool SetDropDownOpen(
        bool isOpen,
        CodexSelectOpenChangeSource source = CodexSelectOpenChangeSource.Programmatic)
    {
        if (IsDropDownOpen == isOpen)
        {
            return false;
        }

        RunWithOpenChangeSource(source, () => IsDropDownOpen = isOpen);
        return true;
    }

    internal bool SelectIndex(int index, CodexSelectValueChangeSource source = CodexSelectValueChangeSource.Programmatic)
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

        var source = _pendingOpenChangeSource ?? _nextOpenChangeSource ?? CodexSelectOpenChangeSource.Programmatic;
        _nextOpenChangeSource = null;
        OpenChanged?.Invoke(this, new CodexSelectOpenChangedEventArgs(IsDropDownOpen, source));
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

        var source = _pendingValueChangeSource ?? _nextInteractionSource ?? CodexSelectValueChangeSource.Programmatic;
        _nextInteractionSource = null;

        ValueChanged?.Invoke(
            this,
            new CodexSelectValueChangedEventArgs(
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
        Classes.Set("select", true);
        SyncSelectionClasses();
    }

    private void SyncSelectionClasses()
    {
        var hasSelection = SelectedIndex >= 0 && SelectedItem is not null;
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

    private void RunWithValueChangeSource(CodexSelectValueChangeSource source, Action action)
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

    private void RunWithOpenChangeSource(CodexSelectOpenChangeSource source, Action action)
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
            ComboBoxItem comboBoxItem => comboBoxItem.Content?.ToString(),
            null => null,
            _ => item.ToString()
        };
    }
}
