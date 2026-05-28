using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public enum CodexAccordionType
{
    Single,
    Multiple
}

public enum CodexAccordionValueChangeSource
{
    Programmatic,
    Trigger,
    Keyboard,
    Normalization
}

public sealed class CodexAccordionValueChangedEventArgs : EventArgs
{
    public CodexAccordionValueChangedEventArgs(
        IReadOnlyList<string> oldValues,
        IReadOnlyList<string> newValues,
        CodexAccordionItem? changedItem = null,
        int changedIndex = -1,
        string? changedValue = null,
        CodexAccordionValueChangeSource source = CodexAccordionValueChangeSource.Programmatic)
    {
        OldValues = oldValues;
        NewValues = newValues;
        ChangedItem = changedItem;
        ChangedIndex = changedIndex;
        ChangedValue = changedValue;
        Source = source;
        IsOpen = !string.IsNullOrWhiteSpace(changedValue)
            && newValues.Any(value => string.Equals(value, changedValue, StringComparison.Ordinal));
    }

    public IReadOnlyList<string> OldValues { get; }

    public IReadOnlyList<string> NewValues { get; }

    public string? OldValue => OldValues.Count > 0 ? OldValues[0] : null;

    public string? NewValue => NewValues.Count > 0 ? NewValues[0] : null;

    public CodexAccordionItem? ChangedItem { get; }

    public int ChangedIndex { get; }

    public string? ChangedValue { get; }

    public CodexAccordionValueChangeSource Source { get; }

    public bool IsOpen { get; }
}

public class CodexAccordion : ItemsControl
{
    public static readonly StyledProperty<CodexAccordionType> TypeProperty =
        AvaloniaProperty.Register<CodexAccordion, CodexAccordionType>(nameof(Type), CodexAccordionType.Single);

    public static readonly StyledProperty<bool> IsCollapsibleProperty =
        AvaloniaProperty.Register<CodexAccordion, bool>(nameof(IsCollapsible));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexAccordion, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexAccordion, Orientation>(nameof(Orientation), Orientation.Vertical);

    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<CodexAccordion, TimeSpan>(nameof(AnimationDuration));

    public static readonly StyledProperty<IReadOnlyList<string>> OpenValuesProperty =
        AvaloniaProperty.Register<CodexAccordion, IReadOnlyList<string>>(nameof(OpenValues), Array.Empty<string>());

    private bool _isUpdatingItems;

    static CodexAccordion()
    {
        TypeProperty.Changed.AddClassHandler<CodexAccordion>((accordion, _) =>
        {
            accordion.SyncClasses();
            accordion.NormalizeOpenItems();
        });
        IsCollapsibleProperty.Changed.AddClassHandler<CodexAccordion>((accordion, _) => accordion.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexAccordion>((accordion, _) =>
        {
            accordion.SyncClasses();
            accordion.SyncItemStates();
        });
        OrientationProperty.Changed.AddClassHandler<CodexAccordion>((accordion, _) =>
        {
            accordion.SyncClasses();
            accordion.SyncItemStates();
        });
        AnimationDurationProperty.Changed.AddClassHandler<CodexAccordion>((accordion, _) => accordion.SyncItemStates());
    }

    public CodexAccordion()
    {
        Focusable = false;
        SyncClasses();
    }

    public event EventHandler<CodexAccordionValueChangedEventArgs>? ValueChanged;

    public CodexAccordionType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public bool IsCollapsible
    {
        get => GetValue(IsCollapsibleProperty);
        set => SetValue(IsCollapsibleProperty, value);
    }

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

    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    public IReadOnlyList<string> OpenValues => GetValue(OpenValuesProperty);

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CodexAccordionItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not CodexAccordionItem;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is not CodexAccordionItem accordionItem)
        {
            return;
        }

        if (item is not CodexAccordionItem)
        {
            accordionItem.Header ??= item;
        }

        SyncItemState(accordionItem, index, ItemsView.Count);
        NormalizeOpenItems();
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        if (element is CodexAccordionItem accordionItem)
        {
            accordionItem.Classes.Set("first", false);
            accordionItem.Classes.Set("last", false);
        }

        base.ClearContainerForItemOverride(element);
        UpdateOpenValues(source: CodexAccordionValueChangeSource.Programmatic);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SyncItemStates();
        NormalizeOpenItems();
    }

    internal bool ToggleItem(CodexAccordionItem item, CodexAccordionValueChangeSource source = CodexAccordionValueChangeSource.Trigger)
    {
        if (!IsEnabled || !item.IsEnabled)
        {
            return true;
        }

        if (Type == CodexAccordionType.Multiple)
        {
            _isUpdatingItems = true;
            try
            {
                SetItemOpen(item, !item.IsOpen, source);
            }
            finally
            {
                _isUpdatingItems = false;
            }

            UpdateOpenValues(item, source);
            return true;
        }

        if (item.IsOpen)
        {
            if (IsCollapsible)
            {
                _isUpdatingItems = true;
                try
                {
                    SetItemOpen(item, false, source);
                }
                finally
                {
                    _isUpdatingItems = false;
                }

                UpdateOpenValues(item, source);
            }

            return true;
        }

        _isUpdatingItems = true;
        try
        {
            foreach (var candidate in GetAccordionItems())
            {
                SetItemOpen(candidate, ReferenceEquals(candidate, item), source);
            }
        }
        finally
        {
            _isUpdatingItems = false;
        }

        UpdateOpenValues(item, source);
        return true;
    }

    internal void HandleItemOpenChanged(CodexAccordionItem item)
    {
        if (_isUpdatingItems)
        {
            return;
        }

        if (Type == CodexAccordionType.Single && item.IsOpen)
        {
            _isUpdatingItems = true;
            try
            {
                foreach (var candidate in GetAccordionItems())
                {
                    if (!ReferenceEquals(candidate, item))
                    {
                        SetItemOpen(candidate, false, CodexAccordionValueChangeSource.Programmatic);
                    }
                }
            }
            finally
            {
                _isUpdatingItems = false;
            }
        }

        UpdateOpenValues(item, CodexAccordionValueChangeSource.Programmatic);
    }

    internal bool TryHandleItemNavigationKey(CodexAccordionItem item, Key key, bool moveFocus = true)
    {
        var items = GetAccordionItems().Where(candidate => candidate.IsEnabled).ToList();
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
            Key.Down when Orientation == Orientation.Vertical => (currentIndex + 1) % items.Count,
            Key.Up when Orientation == Orientation.Vertical => (currentIndex - 1 + items.Count) % items.Count,
            Key.Right when Orientation == Orientation.Horizontal => (currentIndex + 1) % items.Count,
            Key.Left when Orientation == Orientation.Horizontal => (currentIndex - 1 + items.Count) % items.Count,
            _ => -1
        };

        if (nextIndex < 0)
        {
            return false;
        }

        if (moveFocus)
        {
            items[nextIndex].FocusTrigger();
        }

        return true;
    }

    private void NormalizeOpenItems()
    {
        if (Type == CodexAccordionType.Single)
        {
            CodexAccordionItem? firstOpen = null;

            _isUpdatingItems = true;
            try
            {
                foreach (var item in GetAccordionItems())
                {
                    if (!item.IsOpen)
                    {
                        continue;
                    }

                    if (firstOpen is null)
                    {
                        firstOpen = item;
                        continue;
                    }

                        SetItemOpen(item, false, CodexAccordionValueChangeSource.Normalization);
                }
            }
            finally
            {
                _isUpdatingItems = false;
            }
        }

        SyncItemStates();
        UpdateOpenValues(source: CodexAccordionValueChangeSource.Normalization);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("type-single", Type == CodexAccordionType.Single);
        Classes.Set("type-multiple", Type == CodexAccordionType.Multiple);
        Classes.Set("collapsible", IsCollapsible);
        Classes.Set("non-collapsible", !IsCollapsible);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
    }

    private void SyncItemStates()
    {
        var items = GetAccordionItems().ToList();
        for (var index = 0; index < items.Count; index++)
        {
            SyncItemState(items[index], index, items.Count);
        }
    }

    private void SyncItemState(CodexAccordionItem item, int index, int count)
    {
        item.Size = Size;
        item.AnimationDuration = AnimationDuration;
        item.Classes.Set("first", index == 0);
        item.Classes.Set("last", index == count - 1);
        item.Classes.Set("vertical", Orientation == Orientation.Vertical);
        item.Classes.Set("horizontal", Orientation == Orientation.Horizontal);
    }

    private void SetItemOpen(
        CodexAccordionItem item,
        bool isOpen,
        CodexAccordionValueChangeSource source = CodexAccordionValueChangeSource.Programmatic)
    {
        if (item.IsOpen != isOpen)
        {
            item.SetOpen(isOpen, ToCollapsibleOpenChangeSource(source));
        }
    }

    private static CodexCollapsibleOpenChangeSource ToCollapsibleOpenChangeSource(CodexAccordionValueChangeSource source)
    {
        return source switch
        {
            CodexAccordionValueChangeSource.Keyboard => CodexCollapsibleOpenChangeSource.Keyboard,
            CodexAccordionValueChangeSource.Trigger => CodexCollapsibleOpenChangeSource.Pointer,
            _ => CodexCollapsibleOpenChangeSource.Programmatic
        };
    }

    private void UpdateOpenValues(
        CodexAccordionItem? changedItem = null,
        CodexAccordionValueChangeSource source = CodexAccordionValueChangeSource.Programmatic)
    {
        var nextValues = GetAccordionItems()
            .Where(item => item.IsOpen)
            .Select(ResolveItemValue)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToArray();

        if (ValuesEqual(OpenValues, nextValues))
        {
            return;
        }

        var oldValues = OpenValues;
        var changedIndex = changedItem is null ? -1 : IndexOfItem(changedItem);
        var changedValue = changedItem is null ? null : ResolveItemValue(changedItem);
        SetValue(OpenValuesProperty, nextValues);
        ValueChanged?.Invoke(
            this,
            new CodexAccordionValueChangedEventArgs(
                oldValues,
                nextValues,
                changedItem,
                changedIndex,
                changedValue,
                source));
    }

    private string ResolveItemValue(CodexAccordionItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.Value))
        {
            return item.Value;
        }

        var index = IndexOfItem(item);
        return index >= 0 ? $"item-{index + 1}" : string.Empty;
    }

    private int IndexOfItem(CodexAccordionItem item)
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (ReferenceEquals(GetAccordionItemAt(index), item))
            {
                return index;
            }
        }

        return -1;
    }

    private IEnumerable<CodexAccordionItem> GetAccordionItems()
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (GetAccordionItemAt(index) is { } item)
            {
                yield return item;
            }
        }
    }

    private CodexAccordionItem? GetAccordionItemAt(int index)
    {
        return ContainerFromIndex(index) as CodexAccordionItem
            ?? ItemsView[index] as CodexAccordionItem;
    }

    private static bool ValuesEqual(IReadOnlyList<string> left, IReadOnlyList<string> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        for (var index = 0; index < left.Count; index++)
        {
            if (!string.Equals(left[index], right[index], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexAccordionItem : CodexCollapsible
{
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CodexAccordionItem, string?>(nameof(Value));

    static CodexAccordionItem()
    {
        IsOpenProperty.Changed.AddClassHandler<CodexAccordionItem>((item, _) => item.FindAccordion()?.HandleItemOpenChanged(item));
        ValueProperty.Changed.AddClassHandler<CodexAccordionItem>((item, _) => item.FindAccordion()?.HandleItemOpenChanged(item));
    }

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public override void Toggle()
    {
        var accordion = FindAccordion();
        if (accordion is not null)
        {
            accordion.ToggleItem(this);
            return;
        }

        base.Toggle();
    }

    internal override bool TryHandleTriggerKey(Key key)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var accordion = FindAccordion();
        if (accordion?.IsEnabled == false)
        {
            return false;
        }

        if (key is Key.Enter or Key.Space)
        {
            if (accordion is not null)
            {
                accordion.ToggleItem(this, CodexAccordionValueChangeSource.Keyboard);
            }
            else
            {
                base.Toggle(CodexCollapsibleOpenChangeSource.Keyboard);
            }

            return true;
        }

        return accordion?.TryHandleItemNavigationKey(this, key) == true
            || base.TryHandleTriggerKey(key);
    }

    internal override bool TryHandleTriggerPointerRelease(PointerUpdateKind updateKind)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var accordion = FindAccordion();
        if (accordion?.IsEnabled == false)
        {
            return false;
        }

        if (updateKind != PointerUpdateKind.LeftButtonReleased)
        {
            return false;
        }

        if (accordion is not null)
        {
            accordion.ToggleItem(this);
        }
        else
        {
            base.Toggle(CodexCollapsibleOpenChangeSource.Pointer);
        }

        return true;
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

    private CodexAccordion? FindAccordion()
    {
        return ItemsControl.ItemsControlFromItemContainer(this) as CodexAccordion
            ?? this.GetLogicalAncestors().OfType<CodexAccordion>().FirstOrDefault()
            ?? this.GetVisualAncestors().OfType<CodexAccordion>().FirstOrDefault();
    }
}
