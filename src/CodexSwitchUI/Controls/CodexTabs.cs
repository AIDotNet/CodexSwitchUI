using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using System.Globalization;

namespace CodexSwitchUI.Controls;

public enum CodexTabsActivationMode
{
    Automatic,
    Manual
}

public enum CodexTabsValueChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public sealed class CodexTabsValueChangedEventArgs(
    object? oldItem,
    object? newItem,
    int oldIndex,
    int newIndex,
    string? oldValue,
    string? newValue,
    CodexTabsValueChangeSource source)
    : EventArgs
{
    public object? OldItem { get; } = oldItem;

    public object? NewItem { get; } = newItem;

    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public string? OldValue { get; } = oldValue;

    public string? NewValue { get; } = newValue;

    public CodexTabsValueChangeSource Source { get; } = source;
}

public class CodexTabs : TabControl
{
    private static readonly AttachedProperty<bool> MirrorsPlainTabItemProperty =
        AvaloniaProperty.RegisterAttached<CodexTabs, CodexTabItem, bool>("MirrorsPlainTabItem");

    private object? _lastSelectedItem;
    private int _lastSelectedIndex = -1;
    private string? _lastSelectedValue;
    private bool _hasExplicitSelectedValue;
    private bool _isSyncingSelectedValue;
    private CodexTabsValueChangeSource? _pendingValueChangeSource;

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexTabs, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexTabs, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<CodexTabsVariant> VariantProperty =
        AvaloniaProperty.Register<CodexTabs, CodexTabsVariant>(nameof(Variant), CodexTabsVariant.Default);

    public new static readonly StyledProperty<string?> SelectedValueProperty =
        AvaloniaProperty.Register<CodexTabs, string?>(nameof(SelectedValue));

    public static readonly StyledProperty<CodexTabsActivationMode> ActivationModeProperty =
        AvaloniaProperty.Register<CodexTabs, CodexTabsActivationMode>(nameof(ActivationMode), CodexTabsActivationMode.Automatic);

    public static readonly StyledProperty<bool> IsLoopProperty =
        AvaloniaProperty.Register<CodexTabs, bool>(nameof(IsLoop), true);

    static CodexTabs()
    {
        SizeProperty.Changed.AddClassHandler<CodexTabs>((tabs, _) => tabs.SyncClasses());
        OrientationProperty.Changed.AddClassHandler<CodexTabs>((tabs, _) => tabs.SyncClasses());
        VariantProperty.Changed.AddClassHandler<CodexTabs>((tabs, _) => tabs.SyncClasses());
        SelectedValueProperty.Changed.AddClassHandler<CodexTabs>((tabs, _) => tabs.OnSelectedValueChanged());
        ActivationModeProperty.Changed.AddClassHandler<CodexTabs>((tabs, _) => tabs.SyncClasses());
        IsLoopProperty.Changed.AddClassHandler<CodexTabs>((tabs, _) => tabs.SyncClasses());
        SelectedIndexProperty.Changed.AddClassHandler<CodexTabs>((tabs, _) => tabs.OnSelectedIndexChanged());
    }

    public CodexTabs()
    {
        SyncClasses();
        RememberSelection();
    }

    public event EventHandler<CodexTabsValueChangedEventArgs>? ValueChanged;

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public CodexTabsVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public new string? SelectedValue
    {
        get => GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    public CodexTabsActivationMode ActivationMode
    {
        get => GetValue(ActivationModeProperty);
        set => SetValue(ActivationModeProperty, value);
    }

    public bool IsLoop
    {
        get => GetValue(IsLoopProperty);
        set => SetValue(IsLoopProperty, value);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CodexTabItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<CodexTabItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is not CodexTabItem tabContainer)
        {
            return;
        }

        if (item is TabItem source && !ReferenceEquals(container, source))
        {
            tabContainer.SetValue(MirrorsPlainTabItemProperty, true);
            tabContainer.SetCurrentValue(ContentControl.ContentProperty, source.Content);
            tabContainer.SetCurrentValue(ContentControl.ContentTemplateProperty, source.ContentTemplate);
            tabContainer.SetCurrentValue(HeaderedContentControl.HeaderProperty, source.Header);
            tabContainer.SetCurrentValue(HeaderedContentControl.HeaderTemplateProperty, source.HeaderTemplate);
            tabContainer.SetCurrentValue(InputElement.IsEnabledProperty, source.IsEnabled);
            tabContainer.SetCurrentValue(TabItem.IconProperty, source.Icon);
            tabContainer.SetCurrentValue(TabItem.IconTemplateProperty, source.IconTemplate);
            if (source is CodexTabItem codexSource)
            {
                tabContainer.SetCurrentValue(CodexTabItem.ValueProperty, codexSource.Value);
            }

            ApplySelectedValue();
            return;
        }

        tabContainer.ClearValue(MirrorsPlainTabItemProperty);
        ApplySelectedValue();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ApplySelectedValue();
        RememberSelection();
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        if (element is CodexTabItem tabContainer && tabContainer.GetValue(MirrorsPlainTabItemProperty))
        {
            tabContainer.ClearValue(InputElement.IsEnabledProperty);
            tabContainer.ClearValue(TabItem.IconProperty);
            tabContainer.ClearValue(TabItem.IconTemplateProperty);
            tabContainer.ClearValue(CodexTabItem.ValueProperty);
            tabContainer.ClearValue(MirrorsPlainTabItemProperty);
        }

        base.ClearContainerForItemOverride(element);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if ((e.Source is CodexTabs or TabItem) && TryHandleSelectionKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    internal bool TryHandleSelectionKey(Key key)
    {
        if (ItemsView.Count == 0)
        {
            return false;
        }

        var currentIndex = SelectedIndex;
        if (currentIndex < 0 || currentIndex >= ItemsView.Count || !IsSelectable(currentIndex))
        {
            currentIndex = FirstSelectableIndex();
        }

        var nextIndex = key switch
        {
            Key.Home => FirstSelectableIndex(),
            Key.End => LastSelectableIndex(),
            Key.Right when Orientation == Orientation.Horizontal => NextSelectableIndex(currentIndex, 1),
            Key.Left when Orientation == Orientation.Horizontal => NextSelectableIndex(currentIndex, -1),
            Key.Down when Orientation == Orientation.Vertical => NextSelectableIndex(currentIndex, 1),
            Key.Up when Orientation == Orientation.Vertical => NextSelectableIndex(currentIndex, -1),
            _ => -1
        };

        if (nextIndex < 0)
        {
            return false;
        }

        EnsureSelectionSnapshot();

        if (ActivationMode == CodexTabsActivationMode.Manual)
        {
            if (ContainerFromIndex(nextIndex) is Control container)
            {
                container.Focus();
            }

            return true;
        }

        SelectIndex(nextIndex, CodexTabsValueChangeSource.Keyboard);
        return true;
    }

    internal bool SelectItem(CodexTabItem item, CodexTabsValueChangeSource source = CodexTabsValueChangeSource.Programmatic)
    {
        if (!item.IsEnabled)
        {
            return false;
        }

        var index = IndexOfItem(item);
        return index >= 0 && SelectIndex(index, source);
    }

    internal bool SelectIndex(int index, CodexTabsValueChangeSource source = CodexTabsValueChangeSource.Programmatic)
    {
        if (index < 0 || index >= ItemsView.Count || !IsSelectable(index))
        {
            return false;
        }

        RunWithValueChangeSource(source, () => SelectedIndex = index);
        return true;
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("tabs", true);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("variant-default", Variant == CodexTabsVariant.Default);
        Classes.Set("variant-line", Variant == CodexTabsVariant.Line);
        Classes.Set("activation-automatic", ActivationMode == CodexTabsActivationMode.Automatic);
        Classes.Set("activation-manual", ActivationMode == CodexTabsActivationMode.Manual);
        Classes.Set("loop", IsLoop);
        SyncSelectionClasses();
    }

    private void SyncSelectionClasses()
    {
        Classes.Set("has-selection", SelectedIndex >= 0);
    }

    private int FirstSelectableIndex()
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (IsSelectable(index))
            {
                return index;
            }
        }

        return -1;
    }

    private int LastSelectableIndex()
    {
        for (var index = ItemsView.Count - 1; index >= 0; index--)
        {
            if (IsSelectable(index))
            {
                return index;
            }
        }

        return -1;
    }

    private int NextSelectableIndex(int currentIndex, int step)
    {
        var count = ItemsView.Count;
        for (var offset = 1; offset <= count; offset++)
        {
            var candidate = currentIndex + (offset * step);
            if (!IsLoop && (candidate < 0 || candidate >= count))
            {
                return -1;
            }

            var index = (candidate + count) % count;
            if (IsSelectable(index))
            {
                return index;
            }
        }

        return -1;
    }

    private bool IsSelectable(int index)
    {
        if (index < 0 || index >= ItemsView.Count)
        {
            return false;
        }

        if (ItemsView[index] is Control itemControl && !itemControl.IsEnabled)
        {
            return false;
        }

        return ContainerFromIndex(index) is not Control container || container.IsEnabled;
    }

    private void OnSelectedIndexChanged()
    {
        SyncSelectionClasses();

        var oldItem = _lastSelectedItem;
        var oldIndex = _lastSelectedIndex;
        var oldValue = _lastSelectedValue;
        var newIndex = SelectedIndex;
        var newItem = GetItemAt(newIndex);
        var newValue = GetItemValue(newItem, newIndex);

        if (_hasExplicitSelectedValue
            && _lastSelectedIndex < 0
            && !string.IsNullOrWhiteSpace(SelectedValue)
            && !string.Equals(SelectedValue, newValue, StringComparison.Ordinal))
        {
            ApplySelectedValue();
            return;
        }

        SyncSelectedValue(newValue);
        RememberSelection();

        if (oldIndex == newIndex && Equals(oldItem, newItem) && string.Equals(oldValue, newValue, StringComparison.Ordinal))
        {
            return;
        }

        var source = _pendingValueChangeSource ?? CodexTabsValueChangeSource.Programmatic;
        ValueChanged?.Invoke(this, new CodexTabsValueChangedEventArgs(oldItem, newItem, oldIndex, newIndex, oldValue, newValue, source));
    }

    private void ApplySelectedValue()
    {
        if (_isSyncingSelectedValue || string.IsNullOrWhiteSpace(SelectedValue))
        {
            return;
        }

        var nextIndex = IndexOfValue(SelectedValue);
        if (nextIndex >= 0 && nextIndex != SelectedIndex)
        {
            SelectIndex(nextIndex, CodexTabsValueChangeSource.Programmatic);
        }
    }

    private void OnSelectedValueChanged()
    {
        if (!_isSyncingSelectedValue)
        {
            _hasExplicitSelectedValue = !string.IsNullOrWhiteSpace(SelectedValue);
        }

        ApplySelectedValue();
    }

    private void SyncSelectedValue(string? value)
    {
        _isSyncingSelectedValue = true;
        try
        {
            SetCurrentValue(SelectedValueProperty, value);
        }
        finally
        {
            _isSyncingSelectedValue = false;
        }
    }

    private void RememberSelection()
    {
        _lastSelectedIndex = SelectedIndex;
        _lastSelectedItem = GetItemAt(SelectedIndex);
        _lastSelectedValue = GetItemValue(_lastSelectedItem, SelectedIndex);
    }

    private void EnsureSelectionSnapshot()
    {
        var currentItem = GetItemAt(SelectedIndex);
        var currentValue = GetItemValue(currentItem, SelectedIndex);

        if (_lastSelectedIndex == SelectedIndex
            && Equals(_lastSelectedItem, currentItem)
            && string.Equals(_lastSelectedValue, currentValue, StringComparison.Ordinal))
        {
            return;
        }

        RememberSelection();
        SyncSelectedValue(currentValue);
    }

    private int IndexOfValue(string? value)
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (string.Equals(GetItemValue(GetItemAt(index), index), value, StringComparison.Ordinal))
            {
                return index;
            }
        }

        return -1;
    }

    private int IndexOfItem(CodexTabItem item)
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (ReferenceEquals(ItemsView[index], item) || ReferenceEquals(ContainerFromIndex(index), item))
            {
                return index;
            }
        }

        return -1;
    }

    private void RunWithValueChangeSource(CodexTabsValueChangeSource source, Action action)
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

    private object? GetItemAt(int index)
    {
        return index >= 0 && index < ItemsView.Count ? ItemsView[index] : null;
    }

    private string? GetItemValue(object? item, int index)
    {
        if (ContainerFromIndex(index) is CodexTabItem container && !string.IsNullOrWhiteSpace(container.Value))
        {
            return container.Value;
        }

        return item switch
        {
            CodexTabItem tabItem when !string.IsNullOrWhiteSpace(tabItem.Value) => tabItem.Value,
            CodexTabItem tabItem => tabItem.Header?.ToString(),
            TabItem tabItem => tabItem.Header?.ToString(),
            null => null,
            _ => item.ToString() ?? index.ToString(CultureInfo.InvariantCulture)
        };
    }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexTabItem : TabItem
{
    private bool _hasPrimaryPointerPress;

    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CodexTabItem, string?>(nameof(Value));

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
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
        _hasPrimaryPointerPress = e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed;
        if (_hasPrimaryPointerPress)
        {
            Focus();
            e.Handled = true;
            return;
        }

        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        var canActivateFromPointer = _hasPrimaryPointerPress && IsPointerOver;
        _hasPrimaryPointerPress = false;

        if (canActivateFromPointer && TryHandlePointerActivation(updateKind))
        {
            e.Handled = true;
            return;
        }

        base.OnPointerReleased(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        var tabs = ItemsControl.ItemsControlFromItemContainer(this) as CodexTabs
            ?? this.GetVisualAncestors().OfType<CodexTabs>().FirstOrDefault();

        if (TryHandleActivationKey(e.Key)
            || tabs?.TryHandleSelectionKey(e.Key) == true)
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    internal bool TryHandleActivationKey(Key key)
    {
        if (!IsEnabled || key is not (Key.Enter or Key.Space))
        {
            return false;
        }

        return TrySelect(CodexTabsValueChangeSource.Keyboard);
    }

    internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)
    {
        return updateKind == PointerUpdateKind.LeftButtonReleased
            && TrySelect(CodexTabsValueChangeSource.Pointer);
    }

    internal bool TrySelect(CodexTabsValueChangeSource source = CodexTabsValueChangeSource.Programmatic)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var tabs = ItemsControl.ItemsControlFromItemContainer(this) as CodexTabs
            ?? this.GetVisualAncestors().OfType<CodexTabs>().FirstOrDefault()
            ?? this.GetLogicalAncestors().OfType<CodexTabs>().FirstOrDefault();

        if (tabs is not null)
        {
            return tabs.SelectItem(this, source);
        }

        IsSelected = true;
        return true;
    }
}
