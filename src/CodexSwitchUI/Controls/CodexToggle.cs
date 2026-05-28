using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public enum CodexToggleGroupType
{
    Single,
    Multiple
}

public enum CodexTogglePressedChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public enum CodexToggleGroupValueChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public sealed class CodexTogglePressedChangedEventArgs(
    bool oldValue,
    bool newValue,
    CodexTogglePressedChangeSource source = CodexTogglePressedChangeSource.Programmatic)
    : EventArgs
{
    public bool OldValue { get; } = oldValue;

    public bool NewValue { get; } = newValue;

    public CodexTogglePressedChangeSource Source { get; } = source;
}

public sealed class CodexToggleGroupValueChangedEventArgs : EventArgs
{
    public CodexToggleGroupValueChangedEventArgs(
        string? oldValue,
        string? newValue,
        IReadOnlyList<string> oldValues,
        IReadOnlyList<string> newValues,
        CodexToggleGroupValueChangeSource source = CodexToggleGroupValueChangeSource.Programmatic)
    {
        OldValue = oldValue;
        NewValue = newValue;
        OldValues = oldValues;
        NewValues = newValues;
        Source = source;
    }

    public string? OldValue { get; }

    public string? NewValue { get; }

    public IReadOnlyList<string> OldValues { get; }

    public IReadOnlyList<string> NewValues { get; }

    public CodexToggleGroupValueChangeSource Source { get; }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexToggle : ToggleButton
{
    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexToggle, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexToggle, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    private CodexTogglePressedChangeSource? _pendingPressedChangeSource;

    static CodexToggle()
    {
        VariantProperty.Changed.AddClassHandler<CodexToggle>((toggle, _) => toggle.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexToggle>((toggle, _) => toggle.SyncClasses());
        IsCheckedProperty.Changed.AddClassHandler<CodexToggle>((toggle, args) => toggle.OnPressedChanged(args));
    }

    public CodexToggle()
    {
        SyncClasses();
        SyncPressedClasses();
    }

    public event EventHandler<CodexTogglePressedChangedEventArgs>? PressedChanged;

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public new bool IsPressed
    {
        get => IsChecked == true;
        set => IsChecked = value;
    }

    internal bool TryHandleActivationKey(Key key)
    {
        if (key is not (Key.Enter or Key.Space))
        {
            return false;
        }

        if (!IsEnabled)
        {
            return true;
        }

        _ = TogglePressed(CodexTogglePressedChangeSource.Keyboard);
        return true;
    }

    internal bool SetPressedState(bool isPressed, CodexTogglePressedChangeSource source)
    {
        if (!IsEnabled || IsPressed == isPressed)
        {
            return false;
        }

        RunWithPressedChangeSource(source, () => IsPressed = isPressed);
        return true;
    }

    protected bool TogglePressed(CodexTogglePressedChangeSource source)
    {
        return SetPressedState(!IsPressed, source);
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
        if (IsEnabled
            && e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            _pendingPressedChangeSource = CodexTogglePressedChangeSource.Pointer;
        }

        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        try
        {
            base.OnPointerReleased(e);
        }
        finally
        {
            _pendingPressedChangeSource = null;
        }
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        _pendingPressedChangeSource = null;
        base.OnPointerCaptureLost(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (TryHandleActivationKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    protected virtual void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        SyncPressedClasses();
    }

    protected void SyncPressedClasses()
    {
        var pressed = IsPressed;
        Classes.Set("pressed", pressed);
        Classes.Set("state-on", pressed);
        Classes.Set("state-off", !pressed);
    }

    private void OnPressedChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncPressedClasses();

        var oldValue = ToPressedValue(args.OldValue);
        var newValue = ToPressedValue(args.NewValue);
        if (oldValue == newValue)
        {
            return;
        }

        var source = _pendingPressedChangeSource ?? CodexTogglePressedChangeSource.Programmatic;
        PressedChanged?.Invoke(this, new CodexTogglePressedChangedEventArgs(oldValue, newValue, source));
    }

    private void RunWithPressedChangeSource(CodexTogglePressedChangeSource source, Action action)
    {
        var previousSource = _pendingPressedChangeSource;
        _pendingPressedChangeSource = source;
        try
        {
            action();
        }
        finally
        {
            _pendingPressedChangeSource = previousSource;
        }
    }

    private static bool ToPressedValue(object? value)
    {
        return value is bool pressed && pressed;
    }
}

public partial class CodexToggleGroup : ItemsControl
{
    public static readonly StyledProperty<CodexToggleGroupType> TypeProperty =
        AvaloniaProperty.Register<CodexToggleGroup, CodexToggleGroupType>(nameof(Type), CodexToggleGroupType.Single);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexToggleGroup, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexToggleGroup, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexToggleGroup, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<int> SpacingProperty =
        AvaloniaProperty.Register<CodexToggleGroup, int>(nameof(Spacing), 2);

    public static readonly StyledProperty<bool> IsLoopProperty =
        AvaloniaProperty.Register<CodexToggleGroup, bool>(nameof(IsLoop), true);

    public static readonly StyledProperty<bool> IsRovingFocusProperty =
        AvaloniaProperty.Register<CodexToggleGroup, bool>(nameof(IsRovingFocus), true);

    public static readonly StyledProperty<string?> SelectedValueProperty =
        AvaloniaProperty.Register<CodexToggleGroup, string?>(nameof(SelectedValue));

    public static readonly StyledProperty<IReadOnlyList<string>> SelectedValuesProperty =
        AvaloniaProperty.Register<CodexToggleGroup, IReadOnlyList<string>>(nameof(SelectedValues), Array.Empty<string>());

    private bool _isUpdatingItems;
    private bool _isApplyingSelectionProperties;
    private bool _hasExternalSelection;
    private CodexToggleGroupValueChangeSource? _pendingValueChangeSource;
    private bool _hasPendingOldSelection;
    private string? _pendingOldValue;
    private IReadOnlyList<string>? _pendingOldValues;

    static CodexToggleGroup()
    {
        TypeProperty.Changed.AddClassHandler<CodexToggleGroup>((group, _) =>
        {
            group.SyncClasses();
            group.NormalizeSelection();
        });
        OrientationProperty.Changed.AddClassHandler<CodexToggleGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
        VariantProperty.Changed.AddClassHandler<CodexToggleGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
        SizeProperty.Changed.AddClassHandler<CodexToggleGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
        SpacingProperty.Changed.AddClassHandler<CodexToggleGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
        IsLoopProperty.Changed.AddClassHandler<CodexToggleGroup>((group, _) => group.SyncClasses());
        IsRovingFocusProperty.Changed.AddClassHandler<CodexToggleGroup>((group, _) => group.SyncClasses());
        SelectedValueProperty.Changed.AddClassHandler<CodexToggleGroup>((group, args) => group.ApplyExternalSelection(args));
        SelectedValuesProperty.Changed.AddClassHandler<CodexToggleGroup>((group, args) => group.ApplyExternalSelection(args));
    }

    public CodexToggleGroup()
    {
        Focusable = false;
        SyncClasses();
    }

    public event EventHandler<CodexToggleGroupValueChangedEventArgs>? ValueChanged;

    public CodexToggleGroupType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public int Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, Math.Max(0, value));
    }

    public bool IsLoop
    {
        get => GetValue(IsLoopProperty);
        set => SetValue(IsLoopProperty, value);
    }

    public bool IsRovingFocus
    {
        get => GetValue(IsRovingFocusProperty);
        set => SetValue(IsRovingFocusProperty, value);
    }

    public string? SelectedValue
    {
        get => GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    public IReadOnlyList<string> SelectedValues
    {
        get => GetValue(SelectedValuesProperty);
        set => SetValue(SelectedValuesProperty, value ?? Array.Empty<string>());
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CodexToggleGroupItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not CodexToggleGroupItem;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is not CodexToggleGroupItem toggleItem)
        {
            return;
        }

        if (item is not CodexToggleGroupItem)
        {
            toggleItem.SetCurrentValue(ContentControl.ContentProperty, item);
        }

        SyncItemState(toggleItem);

        if (_hasExternalSelection)
        {
            ApplySelectionToItem(toggleItem);
        }
        else
        {
            NormalizeSelection();
        }
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        if (element is CodexToggleGroupItem toggleItem)
        {
            toggleItem.Classes.Set("horizontal", false);
            toggleItem.Classes.Set("vertical", false);
            toggleItem.Classes.Set("group-single", false);
            toggleItem.Classes.Set("group-first", false);
            toggleItem.Classes.Set("group-middle", false);
            toggleItem.Classes.Set("group-last", false);
        }

        base.ClearContainerForItemOverride(element);
        UpdateSelectedValues();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SyncItemStates();
        if (_hasExternalSelection)
        {
            RunWithValueChangeSource(
                CodexToggleGroupValueChangeSource.Programmatic,
                () => ApplyExternalSelection(SelectedValue, SelectedValues));
        }
        else
        {
            NormalizeSelection();
        }
    }

    internal void HandleItemCheckedChanged(CodexToggleGroupItem item)
    {
        if (_isUpdatingItems)
        {
            return;
        }

        if (Type == CodexToggleGroupType.Single && item.IsPressed)
        {
            _isUpdatingItems = true;
            try
            {
                foreach (var candidate in GetToggleItems())
                {
                    if (!ReferenceEquals(candidate, item))
                    {
                        candidate.IsPressed = false;
                    }
                }
            }
            finally
            {
                _isUpdatingItems = false;
            }
        }

        UpdateSelectedValues();
    }

    internal bool ToggleItem(CodexToggleGroupItem item, CodexToggleGroupValueChangeSource source)
    {
        if (!IsEnabled || !item.IsEnabled)
        {
            return false;
        }

        RunWithValueChangeSource(source, () => item.IsPressed = !item.IsPressed);
        return true;
    }

    internal bool TryHandleItemNavigationKey(CodexToggleGroupItem item, Key key, bool moveFocus = true)
    {
        if (!IsRovingFocus)
        {
            return false;
        }

        var items = GetToggleItems().Where(candidate => candidate.IsEnabled).ToList();
        if (items.Count == 0)
        {
            return false;
        }

        var currentIndex = items.IndexOf(item);
        if (currentIndex < 0)
        {
            return false;
        }

        var nextIndex = key switch
        {
            Key.Home => 0,
            Key.End => items.Count - 1,
            Key.Right when Orientation == Orientation.Horizontal => NextIndex(currentIndex, 1, items.Count),
            Key.Left when Orientation == Orientation.Horizontal => NextIndex(currentIndex, -1, items.Count),
            Key.Down when Orientation == Orientation.Vertical => NextIndex(currentIndex, 1, items.Count),
            Key.Up when Orientation == Orientation.Vertical => NextIndex(currentIndex, -1, items.Count),
            _ => -1
        };

        if (nextIndex < 0)
        {
            return false;
        }

        if (moveFocus)
        {
            items[nextIndex].Focus();
        }

        return true;
    }

    private int NextIndex(int currentIndex, int step, int count)
    {
        var nextIndex = currentIndex + step;
        if (nextIndex >= 0 && nextIndex < count)
        {
            return nextIndex;
        }

        return IsLoop ? (nextIndex + count) % count : -1;
    }

    private void ApplyExternalSelection(AvaloniaPropertyChangedEventArgs args)
    {
        if (_isApplyingSelectionProperties)
        {
            return;
        }

        _hasExternalSelection = true;
        var oldValue = args.Property == SelectedValueProperty
            ? args.OldValue as string
            : SelectedValue;
        var oldValues = args.Property == SelectedValuesProperty && args.OldValue is IReadOnlyList<string> values
            ? values
            : SelectedValues;

        RunWithValueChangeSource(
            CodexToggleGroupValueChangeSource.Programmatic,
            () => ApplyExternalSelection(oldValue, oldValues));
    }

    private void ApplyExternalSelection(string? oldValue, IReadOnlyList<string> oldValues)
    {
        var items = GetToggleItems().ToList();
        if (items.Count == 0)
        {
            SyncClasses();
            return;
        }

        _isUpdatingItems = true;
        try
        {
            foreach (var item in items)
            {
                ApplySelectionToItem(item);
            }
        }
        finally
        {
            _isUpdatingItems = false;
        }

        var previousOldValue = _pendingOldValue;
        var previousOldValues = _pendingOldValues;
        var previousHasPendingOldSelection = _hasPendingOldSelection;
        _hasPendingOldSelection = true;
        _pendingOldValue = oldValue;
        _pendingOldValues = oldValues.ToArray();
        try
        {
            NormalizeSelection();
        }
        finally
        {
            _hasPendingOldSelection = previousHasPendingOldSelection;
            _pendingOldValue = previousOldValue;
            _pendingOldValues = previousOldValues;
        }

        SyncClasses();
    }

    private void ApplySelectionToItem(CodexToggleGroupItem item)
    {
        var value = ResolveItemValue(item);
        var shouldPress = Type == CodexToggleGroupType.Single
            ? string.Equals(value, SelectedValue, StringComparison.Ordinal)
            : SelectedValues.Contains(value, StringComparer.Ordinal);
        item.IsPressed = shouldPress;
    }

    private void NormalizeSelection()
    {
        if (Type == CodexToggleGroupType.Single)
        {
            CodexToggleGroupItem? firstPressed = null;

            _isUpdatingItems = true;
            try
            {
                foreach (var item in GetToggleItems())
                {
                    if (!item.IsPressed)
                    {
                        continue;
                    }

                    if (firstPressed is null)
                    {
                        firstPressed = item;
                        continue;
                    }

                    item.IsPressed = false;
                }
            }
            finally
            {
                _isUpdatingItems = false;
            }
        }

        UpdateSelectedValues();
    }

    private void UpdateSelectedValues()
    {
        var nextValues = GetToggleItems()
            .Where(item => item.IsPressed)
            .Select(ResolveItemValue)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToArray();

        var nextValue = nextValues.FirstOrDefault();
        var oldValue = _hasPendingOldSelection ? _pendingOldValue : SelectedValue;
        var oldValues = _hasPendingOldSelection && _pendingOldValues is not null ? _pendingOldValues : SelectedValues;

        if (ValuesEqual(oldValues, nextValues) && string.Equals(oldValue, nextValue, StringComparison.Ordinal))
        {
            SyncClasses();
            return;
        }

        _isApplyingSelectionProperties = true;
        try
        {
            SetValue(SelectedValueProperty, nextValue);
            SetValue(SelectedValuesProperty, nextValues);
        }
        finally
        {
            _isApplyingSelectionProperties = false;
        }

        SyncClasses();
        var source = _pendingValueChangeSource ?? CodexToggleGroupValueChangeSource.Programmatic;
        ValueChanged?.Invoke(this, new CodexToggleGroupValueChangedEventArgs(oldValue, nextValue, oldValues, nextValues, source));
    }

    private void RunWithValueChangeSource(CodexToggleGroupValueChangeSource source, Action action)
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

    private void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("type-single", Type == CodexToggleGroupType.Single);
        Classes.Set("type-multiple", Type == CodexToggleGroupType.Multiple);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("roving", IsRovingFocus);
        Classes.Set("no-roving", !IsRovingFocus);
        Classes.Set("loop", IsLoop);
        Classes.Set("no-loop", !IsLoop);
        Classes.Set("has-value", SelectedValues.Count > 0);
        SyncSpacingClasses();
    }

    private void SyncItemStates()
    {
        var items = GetToggleItems().ToList();
        for (var index = 0; index < items.Count; index++)
        {
            SyncItemState(items[index], index, items.Count);
        }
    }

    private void SyncItemState(CodexToggleGroupItem item, int index = 0, int count = 1)
    {
        item.SetCurrentValue(CodexToggle.VariantProperty, Variant);
        item.SetCurrentValue(CodexToggle.SizeProperty, Size);
        item.Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        item.Classes.Set("vertical", Orientation == Orientation.Vertical);
        item.Classes.Set("group-item", true);
        item.Classes.Set("group-single", count == 1);
        item.Classes.Set("group-first", count > 1 && index == 0);
        item.Classes.Set("group-middle", count > 2 && index > 0 && index < count - 1);
        item.Classes.Set("group-last", count > 1 && index == count - 1);
        item.Classes.Set("connected", Spacing == 0);
    }

    private void SyncSpacingClasses()
    {
        var spacing = Math.Clamp(Spacing, 0, 4);
        Classes.Set("spacing-0", spacing == 0);
        Classes.Set("spacing-1", spacing == 1);
        Classes.Set("spacing-2", spacing == 2);
        Classes.Set("spacing-3", spacing == 3);
        Classes.Set("spacing-4", spacing >= 4);
        Classes.Set("connected", spacing == 0);
        Classes.Set("spaced", spacing > 0);
    }

    private string ResolveItemValue(CodexToggleGroupItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.Value))
        {
            return item.Value;
        }

        var index = IndexOfItem(item);
        return index >= 0 ? $"item-{index + 1}" : string.Empty;
    }

    private int IndexOfItem(CodexToggleGroupItem item)
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (ReferenceEquals(GetToggleItemAt(index), item))
            {
                return index;
            }
        }

        return -1;
    }

    private CodexToggleGroupItem? GetToggleItemAt(int index)
    {
        if (index < 0 || index >= ItemsView.Count)
        {
            return null;
        }

        if (ItemsView[index] is CodexToggleGroupItem item)
        {
            return item;
        }

        return ContainerFromIndex(index) as CodexToggleGroupItem;
    }

    private IEnumerable<CodexToggleGroupItem> GetToggleItems()
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (GetToggleItemAt(index) is { } item)
            {
                yield return item;
            }
        }
    }

    private static bool ValuesEqual(IReadOnlyList<string> left, IReadOnlyList<string> right)
    {
        return left.Count == right.Count && left.SequenceEqual(right, StringComparer.Ordinal);
    }
}

public class CodexToggleGroupItem : CodexToggle
{
    private bool _hasPrimaryPointerPress;

    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CodexToggleGroupItem, string?>(nameof(Value));

    static CodexToggleGroupItem()
    {
        IsCheckedProperty.Changed.AddClassHandler<CodexToggleGroupItem>((item, _) =>
        {
            item.SyncPressedClasses();
            item.FindOwningGroup()?.HandleItemCheckedChanged(item);
        });
        ValueProperty.Changed.AddClassHandler<CodexToggleGroupItem>((item, _) => item.FindOwningGroup()?.HandleItemCheckedChanged(item));
    }

    public CodexToggleGroupItem()
    {
        Classes.Set("group-item", true);
    }

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    internal new bool TryHandleActivationKey(Key key)
    {
        if (key is not (Key.Enter or Key.Space))
        {
            return false;
        }

        var group = FindOwningGroup();
        if (!IsEnabled || group is not null && !group.IsEnabled)
        {
            return true;
        }

        if (group is not null)
        {
            return group.ToggleItem(this, CodexToggleGroupValueChangeSource.Keyboard);
        }

        return TogglePressed(CodexTogglePressedChangeSource.Keyboard);
    }

    internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased)
        {
            return false;
        }

        var group = FindOwningGroup();
        if (group is not null)
        {
            return group.ToggleItem(this, CodexToggleGroupValueChangeSource.Pointer);
        }

        if (!IsEnabled)
        {
            return false;
        }

        return TogglePressed(CodexTogglePressedChangeSource.Pointer);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        FindOwningGroup()?.HandleItemCheckedChanged(this);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        var group = FindOwningGroup();
        if (TryHandleActivationKey(e.Key)
            || group?.TryHandleItemNavigationKey(this, e.Key) == true)
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
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

    private CodexToggleGroup? FindOwningGroup()
    {
        return ItemsControl.ItemsControlFromItemContainer(this) as CodexToggleGroup
            ?? this.GetLogicalAncestors().OfType<CodexToggleGroup>().FirstOrDefault()
            ?? this.GetVisualAncestors().OfType<CodexToggleGroup>().FirstOrDefault();
    }
}
