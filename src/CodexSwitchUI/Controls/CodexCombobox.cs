using System.Collections;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;

namespace CodexSwitchUI.Controls;

public enum CodexComboboxSelectionChangeSource
{
    Programmatic,
    Item,
    Keyboard,
    Clear
}

public enum CodexComboboxOpenChangeSource
{
    Programmatic,
    Pointer,
    Keyboard,
    Input,
    Focus,
    Clear,
    Item
}

public sealed class CodexComboboxSelectionChangedEventArgs(
    object? oldItem,
    object? newItem,
    int oldIndex = -1,
    int newIndex = -1,
    string? oldValue = null,
    string? newValue = null,
    CodexComboboxSelectionChangeSource source = CodexComboboxSelectionChangeSource.Programmatic)
    : EventArgs
{
    public object? OldItem { get; } = oldItem;

    public object? NewItem { get; } = newItem;

    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public string? OldValue { get; } = oldValue;

    public string? NewValue { get; } = newValue;

    public CodexComboboxSelectionChangeSource Source { get; } = source;
}

public sealed class CodexComboboxInputChangedEventArgs(string? oldText, string? newText)
    : EventArgs
{
    public string? OldText { get; } = oldText;

    public string? NewText { get; } = newText;
}

public sealed class CodexComboboxOpenChangedEventArgs(
    bool isOpen,
    CodexComboboxOpenChangeSource source = CodexComboboxOpenChangeSource.Programmatic)
    : EventArgs
{
    public bool IsOpen { get; } = isOpen;

    public CodexComboboxOpenChangeSource Source { get; } = source;
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexCombobox : TemplatedControl
{
    private readonly AvaloniaList<CodexComboboxItem> _filteredItems = [];
    private CodexTextBox? _input;
    private Button? _trigger;
    private Button? _clear;
    private bool _syncingText;
    private bool _suppressOpenOnTextChange;
    private CodexComboboxSelectionChangeSource? _pendingSelectionSource;
    private CodexComboboxOpenChangeSource? _pendingOpenChangeSource;

    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<CodexCombobox, IEnumerable?>(nameof(ItemsSource));

    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<CodexCombobox, object?>(nameof(SelectedItem));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<CodexCombobox, string?>(nameof(Text));

    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<CodexCombobox, string?>(nameof(PlaceholderText), "Select an option...");

    public static readonly StyledProperty<string?> DisplayMemberPathProperty =
        AvaloniaProperty.Register<CodexCombobox, string?>(nameof(DisplayMemberPath));

    public static readonly StyledProperty<object?> EmptyContentProperty =
        AvaloniaProperty.Register<CodexCombobox, object?>(nameof(EmptyContent), "No items found.");

    public static readonly StyledProperty<object?> LoadingContentProperty =
        AvaloniaProperty.Register<CodexCombobox, object?>(nameof(LoadingContent), "Loading...");

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexCombobox, bool>(nameof(IsOpen));

    public static readonly StyledProperty<bool> AutoHighlightProperty =
        AvaloniaProperty.Register<CodexCombobox, bool>(nameof(AutoHighlight));

    public static readonly StyledProperty<bool> HighlightItemOnHoverProperty =
        AvaloniaProperty.Register<CodexCombobox, bool>(nameof(HighlightItemOnHover), true);

    public static readonly StyledProperty<bool> IsClearVisibleProperty =
        AvaloniaProperty.Register<CodexCombobox, bool>(nameof(IsClearVisible), true);

    public static readonly StyledProperty<bool> CloseOnSelectProperty =
        AvaloniaProperty.Register<CodexCombobox, bool>(nameof(CloseOnSelect), true);

    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<CodexCombobox, bool>(nameof(CloseOnEscape), true);

    public static readonly StyledProperty<bool> OpenOnInputProperty =
        AvaloniaProperty.Register<CodexCombobox, bool>(nameof(OpenOnInput), true);

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexCombobox, bool>(nameof(IsLoading));

    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexCombobox, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexCombobox, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<double> MaxPopupHeightProperty =
        AvaloniaProperty.Register<CodexCombobox, double>(nameof(MaxPopupHeight), 320);

    public static readonly StyledProperty<IEnumerable> FilteredItemsProperty =
        AvaloniaProperty.Register<CodexCombobox, IEnumerable>(nameof(FilteredItems));

    public static readonly StyledProperty<int> HighlightedIndexProperty =
        AvaloniaProperty.Register<CodexCombobox, int>(nameof(HighlightedIndex), -1);

    public static readonly StyledProperty<bool> HasSelectionProperty =
        AvaloniaProperty.Register<CodexCombobox, bool>(nameof(HasSelection));

    public static readonly StyledProperty<bool> HasTextProperty =
        AvaloniaProperty.Register<CodexCombobox, bool>(nameof(HasText));

    public static readonly StyledProperty<bool> HasFilteredItemsProperty =
        AvaloniaProperty.Register<CodexCombobox, bool>(nameof(HasFilteredItems));

    public static readonly StyledProperty<bool> HasClearButtonProperty =
        AvaloniaProperty.Register<CodexCombobox, bool>(nameof(HasClearButton));

    static CodexCombobox()
    {
        ItemsSourceProperty.Changed.AddClassHandler<CodexCombobox>((combobox, _) => combobox.RefreshFilteredItems());
        SelectedItemProperty.Changed.AddClassHandler<CodexCombobox>((combobox, args) => combobox.OnSelectedItemChanged(args));
        TextProperty.Changed.AddClassHandler<CodexCombobox>((combobox, args) => combobox.OnTextChanged(args));
        DisplayMemberPathProperty.Changed.AddClassHandler<CodexCombobox>((combobox, _) => combobox.RefreshFilteredItems());
        IsOpenProperty.Changed.AddClassHandler<CodexCombobox>((combobox, args) => combobox.OnOpenChanged(args));
        AutoHighlightProperty.Changed.AddClassHandler<CodexCombobox>((combobox, _) => combobox.RefreshFilteredItems());
        HighlightItemOnHoverProperty.Changed.AddClassHandler<CodexCombobox>((combobox, _) => combobox.SyncClasses());
        IsClearVisibleProperty.Changed.AddClassHandler<CodexCombobox>((combobox, _) => combobox.SyncClasses());
        CloseOnSelectProperty.Changed.AddClassHandler<CodexCombobox>((combobox, _) => combobox.SyncClasses());
        CloseOnEscapeProperty.Changed.AddClassHandler<CodexCombobox>((combobox, _) => combobox.SyncClasses());
        OpenOnInputProperty.Changed.AddClassHandler<CodexCombobox>((combobox, _) => combobox.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexCombobox>((combobox, _) => combobox.SyncClasses());
        IntentProperty.Changed.AddClassHandler<CodexCombobox>((combobox, _) => combobox.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexCombobox>((combobox, _) => combobox.SyncClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexCombobox>((combobox, _) => combobox.SyncClasses());
    }

    public CodexCombobox()
    {
        Items.CollectionChanged += (_, _) =>
        {
            if (ItemsSource is null)
            {
                RefreshFilteredItems();
            }
        };
        SetValue(FilteredItemsProperty, _filteredItems);
        RefreshFilteredItems();
        SyncClasses();
    }

    public event EventHandler<CodexComboboxSelectionChangedEventArgs>? SelectionChanged;

    public event EventHandler<CodexComboboxInputChangedEventArgs>? InputValueChanged;

    public event EventHandler<CodexComboboxOpenChangedEventArgs>? OpenChanged;

    [Content]
    public AvaloniaList<object?> Items { get; } = [];

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public string? DisplayMemberPath
    {
        get => GetValue(DisplayMemberPathProperty);
        set => SetValue(DisplayMemberPathProperty, value);
    }

    public object? EmptyContent
    {
        get => GetValue(EmptyContentProperty);
        set => SetValue(EmptyContentProperty, value);
    }

    public object? LoadingContent
    {
        get => GetValue(LoadingContentProperty);
        set => SetValue(LoadingContentProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool AutoHighlight
    {
        get => GetValue(AutoHighlightProperty);
        set => SetValue(AutoHighlightProperty, value);
    }

    public bool HighlightItemOnHover
    {
        get => GetValue(HighlightItemOnHoverProperty);
        set => SetValue(HighlightItemOnHoverProperty, value);
    }

    public bool IsClearVisible
    {
        get => GetValue(IsClearVisibleProperty);
        set => SetValue(IsClearVisibleProperty, value);
    }

    public bool CloseOnSelect
    {
        get => GetValue(CloseOnSelectProperty);
        set => SetValue(CloseOnSelectProperty, value);
    }

    public bool CloseOnEscape
    {
        get => GetValue(CloseOnEscapeProperty);
        set => SetValue(CloseOnEscapeProperty, value);
    }

    public bool OpenOnInput
    {
        get => GetValue(OpenOnInputProperty);
        set => SetValue(OpenOnInputProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
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

    public double MaxPopupHeight
    {
        get => GetValue(MaxPopupHeightProperty);
        set => SetValue(MaxPopupHeightProperty, value);
    }

    public IEnumerable FilteredItems => GetValue(FilteredItemsProperty);

    public int HighlightedIndex
    {
        get => GetValue(HighlightedIndexProperty);
        private set => SetValue(HighlightedIndexProperty, value);
    }

    public bool HasSelection => GetValue(HasSelectionProperty);

    public bool HasText => GetValue(HasTextProperty);

    public bool HasFilteredItems => GetValue(HasFilteredItemsProperty);

    public bool HasClearButton => GetValue(HasClearButtonProperty);

    public object? HighlightedItem => HighlightedIndex >= 0 && HighlightedIndex < _filteredItems.Count
        ? _filteredItems[HighlightedIndex].Value
        : null;

    public bool Open()
    {
        return Open(CodexComboboxOpenChangeSource.Programmatic);
    }

    internal bool Open(CodexComboboxOpenChangeSource source)
    {
        if (!IsEnabled || IsLoading || IsOpen)
        {
            return false;
        }

        RunWithOpenChangeSource(source, () => IsOpen = true);
        return true;
    }

    public bool Close()
    {
        return Close(CodexComboboxOpenChangeSource.Programmatic);
    }

    internal bool Close(CodexComboboxOpenChangeSource source)
    {
        if (!IsOpen)
        {
            return false;
        }

        RunWithOpenChangeSource(source, () => IsOpen = false);
        return true;
    }

    public bool TogglePopup()
    {
        return TogglePopup(CodexComboboxOpenChangeSource.Programmatic);
    }

    internal bool TogglePopup(CodexComboboxOpenChangeSource source)
    {
        return IsOpen ? Close(source) : Open(source);
    }

    public bool ClearSelection()
    {
        if (!HasSelection && string.IsNullOrEmpty(Text))
        {
            return false;
        }

        if (SelectedItem is not null)
        {
            _pendingSelectionSource = CodexComboboxSelectionChangeSource.Clear;
        }

        try
        {
            SelectedItem = null;
        }
        finally
        {
            _pendingSelectionSource = null;
        }

        _suppressOpenOnTextChange = true;
        try
        {
            Text = string.Empty;
        }
        finally
        {
            _suppressOpenOnTextChange = false;
        }

        RefreshFilteredItems();
        Open(CodexComboboxOpenChangeSource.Clear);
        return true;
    }

    public bool SelectItem(object? item)
    {
        return SelectItem(item, CodexComboboxSelectionChangeSource.Programmatic);
    }

    internal bool SelectItem(object? item, CodexComboboxSelectionChangeSource source)
    {
        if (!IsEnabled || IsLoading || item is null)
        {
            return false;
        }

        _pendingSelectionSource = source;
        try
        {
            SelectedItem = item;
        }
        finally
        {
            _pendingSelectionSource = null;
        }

        _syncingText = true;
        Text = FormatItem(item);
        _syncingText = false;
        RefreshFilteredItems();

        if (CloseOnSelect)
        {
            Close(ToOpenChangeSource(source));
        }

        return true;
    }

    public bool HighlightItem(object? item)
    {
        var index = IndexOfFilteredItem(item);
        if (index < 0)
        {
            return false;
        }

        SetHighlightedIndex(index);
        return true;
    }

    public bool TryHandleInputKey(Key key)
    {
        if (!IsEnabled)
        {
            return false;
        }

        switch (key)
        {
            case Key.Down:
                if (!IsOpen)
                {
                    Open(CodexComboboxOpenChangeSource.Keyboard);
                    SetHighlightedIndex(HighlightedIndex < 0 ? 0 : HighlightedIndex);
                    return true;
                }

                return MoveHighlight(1);

            case Key.Up:
                if (!IsOpen)
                {
                    Open(CodexComboboxOpenChangeSource.Keyboard);
                    SetHighlightedIndex(_filteredItems.Count - 1);
                    return true;
                }

                return MoveHighlight(-1);

            case Key.Home when IsOpen:
                SetHighlightedIndex(0);
                return _filteredItems.Count > 0;

            case Key.End when IsOpen:
                SetHighlightedIndex(_filteredItems.Count - 1);
                return _filteredItems.Count > 0;

            case Key.Enter:
                if (!IsOpen)
                {
                    return Open(CodexComboboxOpenChangeSource.Keyboard);
                }

                return SelectItem(HighlightedItem, CodexComboboxSelectionChangeSource.Keyboard);

            case Key.Escape:
                return CloseOnEscape && Close(CodexComboboxOpenChangeSource.Keyboard);

            default:
                return false;
        }
    }

    internal bool TryHandleTriggerPointerRelease(PointerUpdateKind updateKind)
    {
        return updateKind == PointerUpdateKind.LeftButtonReleased && TogglePopup(CodexComboboxOpenChangeSource.Pointer);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_input is not null)
        {
            _input.KeyDown -= OnInputKeyDown;
            _input.GotFocus -= OnInputGotFocus;
        }

        if (_trigger is not null)
        {
            _trigger.RemoveHandler(InputElement.PointerReleasedEvent, OnTriggerPointerReleased);
        }

        if (_clear is not null)
        {
            _clear.Click -= OnClearClick;
        }

        base.OnApplyTemplate(e);

        _input = e.NameScope.Find<CodexTextBox>("PART_Input");
        _trigger = e.NameScope.Find<Button>("PART_Trigger");
        _clear = e.NameScope.Find<Button>("PART_Clear");

        if (_input is not null)
        {
            _input.KeyDown += OnInputKeyDown;
            _input.GotFocus += OnInputGotFocus;
        }

        if (_trigger is not null)
        {
            _trigger.AddHandler(
                InputElement.PointerReleasedEvent,
                OnTriggerPointerReleased,
                RoutingStrategies.Bubble,
                handledEventsToo: true);
        }

        if (_clear is not null)
        {
            _clear.Click += OnClearClick;
        }
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

    private void OnInputKeyDown(object? sender, KeyEventArgs e)
    {
        if (TryHandleInputKey(e.Key))
        {
            e.Handled = true;
        }
    }

    private void OnInputGotFocus(object? sender, RoutedEventArgs e)
    {
        if (OpenOnInput && !IsOpen)
        {
            Open(CodexComboboxOpenChangeSource.Focus);
        }
    }

    private void OnTriggerPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var updateKind = e.GetCurrentPoint((Control?)_trigger ?? this).Properties.PointerUpdateKind;
        if (TryHandleTriggerPointerRelease(updateKind))
        {
            e.Handled = true;
        }
    }

    private void OnClearClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!ClearSelection())
        {
            Open(CodexComboboxOpenChangeSource.Clear);
        }
    }

    private void OnOpenChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncClasses();

        if (args.NewValue is true && HighlightedIndex < 0)
        {
            HighlightInitialItem();
        }

        OpenChanged?.Invoke(this, new CodexComboboxOpenChangedEventArgs(IsOpen, CurrentOpenChangeSource));
    }

    private void OnSelectedItemChanged(AvaloniaPropertyChangedEventArgs args)
    {
        var oldItem = args.OldValue;
        var newItem = SelectedItem;
        var source = _pendingSelectionSource ?? CodexComboboxSelectionChangeSource.Programmatic;

        if (!_syncingText && SelectedItem is not null)
        {
            _syncingText = true;
            Text = FormatItem(SelectedItem);
            _syncingText = false;
        }

        SetValue(HasSelectionProperty, SelectedItem is not null);
        RefreshFilteredItems();
        SelectionChanged?.Invoke(
            this,
            new CodexComboboxSelectionChangedEventArgs(
                oldItem,
                newItem,
                IndexOfSourceItem(oldItem),
                IndexOfSourceItem(newItem),
                FormatItemValue(oldItem),
                FormatItemValue(newItem),
                source));
        SyncClasses();
    }

    private void OnTextChanged(AvaloniaPropertyChangedEventArgs args)
    {
        var oldText = args.OldValue as string;
        SetValue(HasTextProperty, !string.IsNullOrEmpty(Text));
        InputValueChanged?.Invoke(this, new CodexComboboxInputChangedEventArgs(oldText, Text));

        if (!_syncingText)
        {
            if (!_suppressOpenOnTextChange && OpenOnInput && IsEnabled && !IsLoading)
            {
                Open(CodexComboboxOpenChangeSource.Input);
            }

            RefreshFilteredItems();
        }

        SyncClasses();
    }

    private void RefreshFilteredItems()
    {
        var previousHighlight = HighlightedItem;
        _filteredItems.Clear();

        foreach (var item in EnumerateItems())
        {
            if (!Matches(item, Text))
            {
                continue;
            }

            var comboboxItem = new CodexComboboxItem
            {
                Owner = this,
                Value = item,
                Content = FormatItem(item),
                IsSelected = AreEqual(item, SelectedItem)
            };
            _filteredItems.Add(comboboxItem);
        }

        SetValue(HasFilteredItemsProperty, _filteredItems.Count > 0);
        HighlightInitialItem(previousHighlight);
        SyncClasses();
    }

    private IEnumerable<object?> EnumerateItems()
    {
        var source = ItemsSource ?? Items;
        if (source is null)
        {
            yield break;
        }

        foreach (var item in source)
        {
            yield return item;
        }
    }

    private bool Matches(object? item, string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        return FormatItem(item).Contains(text, StringComparison.CurrentCultureIgnoreCase);
    }

    private string FormatItem(object? item)
    {
        if (item is null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
        {
            var property = item.GetType().GetProperty(DisplayMemberPath);
            if (property?.GetValue(item) is { } value)
            {
                return value.ToString() ?? string.Empty;
            }
        }

        return item.ToString() ?? string.Empty;
    }

    private string? FormatItemValue(object? item)
    {
        return item is null ? null : FormatItem(item);
    }

    private void HighlightInitialItem(object? preferredItem = null)
    {
        var preferredIndex = IndexOfFilteredItem(preferredItem);
        if (preferredIndex >= 0)
        {
            SetHighlightedIndex(preferredIndex);
            return;
        }

        var selectedIndex = IndexOfFilteredItem(SelectedItem);
        if (selectedIndex >= 0)
        {
            SetHighlightedIndex(selectedIndex);
            return;
        }

        SetHighlightedIndex(AutoHighlight && _filteredItems.Count > 0 ? 0 : -1);
    }

    private int IndexOfFilteredItem(object? item)
    {
        if (item is null)
        {
            return -1;
        }

        for (var i = 0; i < _filteredItems.Count; i++)
        {
            if (AreEqual(_filteredItems[i].Value, item))
            {
                return i;
            }
        }

        return -1;
    }

    private int IndexOfSourceItem(object? item)
    {
        if (item is null)
        {
            return -1;
        }

        var index = 0;
        foreach (var candidate in EnumerateItems())
        {
            if (AreEqual(candidate, item))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    private bool MoveHighlight(int delta)
    {
        if (_filteredItems.Count == 0)
        {
            SetHighlightedIndex(-1);
            return false;
        }

        var next = HighlightedIndex < 0
            ? (delta > 0 ? 0 : _filteredItems.Count - 1)
            : Math.Clamp(HighlightedIndex + delta, 0, _filteredItems.Count - 1);
        SetHighlightedIndex(next);
        return true;
    }

    private void SetHighlightedIndex(int index)
    {
        var safeIndex = _filteredItems.Count == 0 ? -1 : Math.Clamp(index, -1, _filteredItems.Count - 1);
        HighlightedIndex = safeIndex;

        for (var i = 0; i < _filteredItems.Count; i++)
        {
            _filteredItems[i].IsHighlighted = i == safeIndex;
        }
    }

    private void SyncClasses()
    {
        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("has-selection", HasSelection);
        Classes.Set("has-text", HasText);
        Classes.Set("has-filtered-items", HasFilteredItems);
        Classes.Set("empty", !HasFilteredItems && !IsLoading);
        Classes.Set("loading", IsLoading);
        Classes.Set("auto-highlight", AutoHighlight);
        Classes.Set("highlight-on-hover", HighlightItemOnHover);
        Classes.Set("close-on-select", CloseOnSelect);
        Classes.Set("close-on-escape", CloseOnEscape);
        Classes.Set("open-on-input", OpenOnInput);
        SetValue(HasClearButtonProperty, IsClearVisible && (HasSelection || HasText));
        Classes.Set("has-clear", HasClearButton);
    }

    private static bool AreEqual(object? left, object? right)
    {
        return Equals(left, right);
    }

    private CodexComboboxOpenChangeSource CurrentOpenChangeSource =>
        _pendingOpenChangeSource ?? CodexComboboxOpenChangeSource.Programmatic;

    private void RunWithOpenChangeSource(CodexComboboxOpenChangeSource source, Action action)
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

    private static CodexComboboxOpenChangeSource ToOpenChangeSource(CodexComboboxSelectionChangeSource source)
    {
        return source switch
        {
            CodexComboboxSelectionChangeSource.Item => CodexComboboxOpenChangeSource.Item,
            CodexComboboxSelectionChangeSource.Keyboard => CodexComboboxOpenChangeSource.Keyboard,
            CodexComboboxSelectionChangeSource.Clear => CodexComboboxOpenChangeSource.Clear,
            _ => CodexComboboxOpenChangeSource.Programmatic
        };
    }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexComboboxItem : Button
{
    internal CodexCombobox? Owner { get; set; }

    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<CodexComboboxItem, object?>(nameof(Value));

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<CodexComboboxItem, bool>(nameof(IsSelected));

    public static readonly StyledProperty<bool> IsHighlightedProperty =
        AvaloniaProperty.Register<CodexComboboxItem, bool>(nameof(IsHighlighted));

    static CodexComboboxItem()
    {
        IsSelectedProperty.Changed.AddClassHandler<CodexComboboxItem>((item, _) => item.SyncClasses());
        IsHighlightedProperty.Changed.AddClassHandler<CodexComboboxItem>((item, _) => item.SyncClasses());
    }

    public CodexComboboxItem()
    {
        SyncClasses();
    }

    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public bool IsHighlighted
    {
        get => GetValue(IsHighlightedProperty);
        set => SetValue(IsHighlightedProperty, value);
    }

    protected override void OnClick()
    {
        if (Owner?.SelectItem(Value, CodexComboboxSelectionChangeSource.Item) == true)
        {
            return;
        }

        base.OnClick();
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);

        if (Owner?.HighlightItemOnHover == true)
        {
            Owner.HighlightItem(Value);
        }
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

    private void SyncClasses()
    {
        Classes.Set("selected", IsSelected);
        Classes.Set("highlighted", IsHighlighted);
    }
}
