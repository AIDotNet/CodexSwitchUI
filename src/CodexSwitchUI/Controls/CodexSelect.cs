using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;

namespace CodexSwitchUI.Controls;

public sealed class CodexSelectValueChangedEventArgs(
    object? oldItem,
    object? newItem,
    int oldIndex,
    int newIndex,
    string? oldValue,
    string? newValue)
    : EventArgs
{
    public object? OldItem { get; } = oldItem;

    public object? NewItem { get; } = newItem;

    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public string? OldValue { get; } = oldValue;

    public string? NewValue { get; } = newValue;
}

public sealed class CodexSelectOpenChangedEventArgs(bool isOpen)
    : EventArgs
{
    public bool IsOpen { get; } = isOpen;
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexSelect : ComboBox
{
    private object? _lastSelectedItem;
    private int _lastSelectedIndex = -1;
    private string? _lastSelectedValue;

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
        base.OnPointerPressed(e);
    }

    private void OnOpenChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncPopupClasses();

        if (args.OldValue is bool oldValue && oldValue == IsDropDownOpen)
        {
            return;
        }

        OpenChanged?.Invoke(this, new CodexSelectOpenChangedEventArgs(IsDropDownOpen));
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

        ValueChanged?.Invoke(
            this,
            new CodexSelectValueChangedEventArgs(
                oldItem,
                newItem,
                oldIndex,
                newIndex,
                oldValue,
                newValue));
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
