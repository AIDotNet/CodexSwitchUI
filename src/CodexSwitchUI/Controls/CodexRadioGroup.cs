using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public sealed class CodexRadioGroupValueChangedEventArgs(
    string? oldValue,
    string? newValue,
    CodexRadioGroupItem? oldItem = null,
    CodexRadioGroupItem? newItem = null,
    int oldIndex = -1,
    int newIndex = -1)
    : EventArgs
{
    public string? OldValue { get; } = oldValue;

    public string? NewValue { get; } = newValue;

    public CodexRadioGroupItem? OldItem { get; } = oldItem;

    public CodexRadioGroupItem? NewItem { get; } = newItem;

    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;
}

public class CodexRadioGroup : ItemsControl
{
    private static int _nextGeneratedGroupId;

    private readonly string _generatedGroupName = $"codex-radio-group-{Interlocked.Increment(ref _nextGeneratedGroupId)}";
    private bool _isUpdatingItems;
    private bool _isApplyingSelection;

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexRadioGroup, Orientation>(nameof(Orientation), Orientation.Vertical);

    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexRadioGroup, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexRadioGroup, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<string?> SelectedValueProperty =
        AvaloniaProperty.Register<CodexRadioGroup, string?>(nameof(SelectedValue));

    public static readonly StyledProperty<string?> RadioGroupNameProperty =
        AvaloniaProperty.Register<CodexRadioGroup, string?>(nameof(RadioGroupName));

    public static readonly StyledProperty<bool> IsLoopProperty =
        AvaloniaProperty.Register<CodexRadioGroup, bool>(nameof(IsLoop), true);

    public static readonly StyledProperty<bool> IsRovingFocusProperty =
        AvaloniaProperty.Register<CodexRadioGroup, bool>(nameof(IsRovingFocus), true);

    public static readonly StyledProperty<bool> IsRequiredProperty =
        AvaloniaProperty.Register<CodexRadioGroup, bool>(nameof(IsRequired));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexRadioGroup, bool>(nameof(IsLoading));

    static CodexRadioGroup()
    {
        OrientationProperty.Changed.AddClassHandler<CodexRadioGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
        IntentProperty.Changed.AddClassHandler<CodexRadioGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
        SizeProperty.Changed.AddClassHandler<CodexRadioGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
        SelectedValueProperty.Changed.AddClassHandler<CodexRadioGroup>((group, _) => group.ApplyExternalSelection());
        RadioGroupNameProperty.Changed.AddClassHandler<CodexRadioGroup>((group, _) => group.SyncItemStates());
        IsLoopProperty.Changed.AddClassHandler<CodexRadioGroup>((group, _) => group.SyncClasses());
        IsRovingFocusProperty.Changed.AddClassHandler<CodexRadioGroup>((group, _) => group.SyncClasses());
        IsRequiredProperty.Changed.AddClassHandler<CodexRadioGroup>((group, _) => group.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexRadioGroup>((group, _) =>
        {
            group.SyncClasses();
            group.SyncItemStates();
        });
    }

    public CodexRadioGroup()
    {
        Focusable = false;
        SyncClasses();
    }

    public event EventHandler<CodexRadioGroupValueChangedEventArgs>? ValueChanged;

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

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

    public string? SelectedValue
    {
        get => GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    public string? RadioGroupName
    {
        get => GetValue(RadioGroupNameProperty);
        set => SetValue(RadioGroupNameProperty, value);
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

    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CodexRadioGroupItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not CodexRadioGroupItem;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is not CodexRadioGroupItem radioItem)
        {
            return;
        }

        if (item is not CodexRadioGroupItem)
        {
            radioItem.SetCurrentValue(ContentControl.ContentProperty, item);
        }

        SyncItemState(radioItem);
        ApplySelectionToItem(radioItem);
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        if (element is CodexRadioGroupItem radioItem)
        {
            radioItem.Classes.Set("horizontal", false);
            radioItem.Classes.Set("vertical", false);
            radioItem.Classes.Set("group-loading", false);
        }

        base.ClearContainerForItemOverride(element);
        UpdateSelectedValue();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SyncItemStates();
        ApplyExternalSelection();
    }

    internal void HandleItemCheckedChanged(CodexRadioGroupItem item)
    {
        item.SyncStateClasses();

        if (_isUpdatingItems)
        {
            return;
        }

        if (item.IsChecked == true)
        {
            _isUpdatingItems = true;
            try
            {
                foreach (var candidate in GetRadioItems())
                {
                    if (!ReferenceEquals(candidate, item))
                    {
                        candidate.IsChecked = false;
                    }
                }
            }
            finally
            {
                _isUpdatingItems = false;
            }
        }

        UpdateSelectedValue();
    }

    internal bool TryHandleItemNavigationKey(CodexRadioGroupItem item, Key key, bool moveFocus = true)
    {
        if (!IsRovingFocus || IsLoading || !IsEnabled)
        {
            return false;
        }

        var items = GetRadioItems().Where(candidate => candidate.IsEnabled).ToList();
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
            Key.Right or Key.Down => NextIndex(currentIndex, 1, items.Count),
            Key.Left or Key.Up => NextIndex(currentIndex, -1, items.Count),
            _ => -1
        };

        if (nextIndex < 0)
        {
            return false;
        }

        var nextItem = items[nextIndex];
        SelectItem(nextItem);
        if (moveFocus)
        {
            nextItem.Focus();
        }

        return true;
    }

    internal bool SelectItem(CodexRadioGroupItem item)
    {
        if (IsLoading || !IsEnabled || !item.IsEnabled)
        {
            return false;
        }

        item.IsChecked = true;
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

    private void ApplyExternalSelection()
    {
        if (_isApplyingSelection)
        {
            return;
        }

        var items = GetRadioItems().ToList();
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

        UpdateSelectedValue();
    }

    private void ApplySelectionToItem(CodexRadioGroupItem item)
    {
        var shouldCheck = string.Equals(ResolveItemValue(item), SelectedValue, StringComparison.Ordinal);
        item.IsChecked = shouldCheck;
        item.SyncStateClasses();
    }

    private void UpdateSelectedValue()
    {
        var items = GetRadioItems().ToList();
        var newItem = items.FirstOrDefault(item => item.IsChecked == true && !string.IsNullOrWhiteSpace(ResolveItemValue(item)));
        var nextValue = newItem is null ? null : ResolveItemValue(newItem);
        var oldValue = SelectedValue;
        var oldItem = FindItemByValue(items, oldValue);
        var oldIndex = IndexOfItem(oldItem);
        var newIndex = IndexOfItem(newItem);

        if (string.Equals(oldValue, nextValue, StringComparison.Ordinal))
        {
            SyncClasses();
            return;
        }

        _isApplyingSelection = true;
        try
        {
            SetValue(SelectedValueProperty, nextValue);
        }
        finally
        {
            _isApplyingSelection = false;
        }

        SyncClasses();
        ValueChanged?.Invoke(
            this,
            new CodexRadioGroupValueChangedEventArgs(oldValue, nextValue, oldItem, newItem, oldIndex, newIndex));
    }

    private void SyncClasses()
    {
        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("radio-group", true);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("roving", IsRovingFocus);
        Classes.Set("no-roving", !IsRovingFocus);
        Classes.Set("loop", IsLoop);
        Classes.Set("no-loop", !IsLoop);
        Classes.Set("required", IsRequired);
        Classes.Set("loading", IsLoading);
        Classes.Set("has-value", !string.IsNullOrWhiteSpace(SelectedValue));
    }

    private void SyncItemStates()
    {
        foreach (var item in GetRadioItems())
        {
            SyncItemState(item);
        }
    }

    private void SyncItemState(CodexRadioGroupItem item)
    {
        item.SetCurrentValue(CodexRadio.IntentProperty, Intent);
        item.SetCurrentValue(CodexRadio.SizeProperty, Size);
        item.SetCurrentValue(RadioButton.GroupNameProperty, EffectiveGroupName());
        item.SetCurrentValue(CodexRadioGroupItem.IsRequiredProperty, IsRequired);
        item.Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        item.Classes.Set("vertical", Orientation == Orientation.Vertical);
        item.Classes.Set("group-item", true);
        item.Classes.Set("group-loading", IsLoading);
        item.SyncStateClasses();
    }

    private string EffectiveGroupName()
    {
        return string.IsNullOrWhiteSpace(RadioGroupName) ? _generatedGroupName : RadioGroupName!;
    }

    private string ResolveItemValue(CodexRadioGroupItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.Value))
        {
            return item.Value;
        }

        var index = IndexOfItem(item);
        return index >= 0 ? $"item-{index + 1}" : string.Empty;
    }

    private int IndexOfItem(CodexRadioGroupItem? item)
    {
        if (item is null)
        {
            return -1;
        }

        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (ReferenceEquals(GetRadioItemAt(index), item))
            {
                return index;
            }
        }

        return -1;
    }

    private CodexRadioGroupItem? FindItemByValue(IReadOnlyList<CodexRadioGroupItem> items, string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : items.FirstOrDefault(item => string.Equals(ResolveItemValue(item), value, StringComparison.Ordinal));
    }

    private CodexRadioGroupItem? GetRadioItemAt(int index)
    {
        if (index < 0 || index >= ItemsView.Count)
        {
            return null;
        }

        if (ItemsView[index] is CodexRadioGroupItem item)
        {
            return item;
        }

        return ContainerFromIndex(index) as CodexRadioGroupItem;
    }

    private IEnumerable<CodexRadioGroupItem> GetRadioItems()
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (GetRadioItemAt(index) is { } item)
            {
                yield return item;
            }
        }
    }
}

public class CodexRadioGroupItem : CodexRadio
{
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CodexRadioGroupItem, string?>(nameof(Value));

    public static readonly StyledProperty<bool> IsRequiredProperty =
        AvaloniaProperty.Register<CodexRadioGroupItem, bool>(nameof(IsRequired));

    static CodexRadioGroupItem()
    {
        IsCheckedProperty.Changed.AddClassHandler<CodexRadioGroupItem>((item, _) =>
        {
            item.SyncStateClasses();
            item.FindOwningGroup()?.HandleItemCheckedChanged(item);
        });
        ValueProperty.Changed.AddClassHandler<CodexRadioGroupItem>((item, _) => item.FindOwningGroup()?.HandleItemCheckedChanged(item));
        IsRequiredProperty.Changed.AddClassHandler<CodexRadioGroupItem>((item, _) => item.SyncStateClasses());
    }

    public CodexRadioGroupItem()
    {
        Classes.Set("radio-group-item", true);
        SyncStateClasses();
    }

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    internal bool TryHandleActivationKey(Key key)
    {
        if (key is not (Key.Enter or Key.Space))
        {
            return false;
        }

        var group = FindOwningGroup();
        if (group is not null)
        {
            _ = group.SelectItem(this);
            return true;
        }

        IsChecked = true;
        return true;
    }

    internal void SyncStateClasses()
    {
        Classes.Set("radio-group-item", true);
        Classes.Set("state-checked", IsChecked == true);
        Classes.Set("state-unchecked", IsChecked != true);
        Classes.Set("required", IsRequired);
    }

    protected override void OnClick()
    {
        var group = FindOwningGroup();
        if (group is not null)
        {
            _ = group.SelectItem(this);
            return;
        }

        base.OnClick();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        var group = FindOwningGroup();
        if (group?.TryHandleItemNavigationKey(this, e.Key) == true
            || TryHandleActivationKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    private CodexRadioGroup? FindOwningGroup()
    {
        return ItemsControl.ItemsControlFromItemContainer(this) as CodexRadioGroup
            ?? this.GetLogicalAncestors().OfType<CodexRadioGroup>().FirstOrDefault()
            ?? this.GetVisualAncestors().OfType<CodexRadioGroup>().FirstOrDefault();
    }
}
