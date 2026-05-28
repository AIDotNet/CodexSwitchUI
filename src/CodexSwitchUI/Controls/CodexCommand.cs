using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public sealed class CodexCommandItemSelectedEventArgs(
    CodexCommandItem item,
    string? value,
    CodexCommandItemSelectSource source = CodexCommandItemSelectSource.Programmatic) : EventArgs
{
    public CodexCommandItem Item { get; } = item;

    public string? Value { get; } = value;

    public CodexCommandItemSelectSource Source { get; } = source;
}

public enum CodexCommandItemSelectSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public class CodexCommand : CodexFrame
{
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<CodexCommand, string?>(nameof(Placeholder), "Type a command...");

    public static readonly StyledProperty<string?> SearchTextProperty =
        AvaloniaProperty.Register<CodexCommand, string?>(nameof(SearchText));

    public static readonly StyledProperty<bool> ShouldFilterProperty =
        AvaloniaProperty.Register<CodexCommand, bool>(nameof(ShouldFilter), true);

    public static readonly StyledProperty<bool> LoopNavigationProperty =
        AvaloniaProperty.Register<CodexCommand, bool>(nameof(LoopNavigation));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexCommand, bool>(nameof(IsLoading));

    public static readonly StyledProperty<CodexCommandItem?> SelectedItemProperty =
        AvaloniaProperty.Register<CodexCommand, CodexCommandItem?>(nameof(SelectedItem));

    static CodexCommand()
    {
        SearchTextProperty.Changed.AddClassHandler<CodexCommand>((command, _) => command.SyncFilter());
        ShouldFilterProperty.Changed.AddClassHandler<CodexCommand>((command, _) => command.SyncFilter());
        LoopNavigationProperty.Changed.AddClassHandler<CodexCommand>((command, _) => command.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexCommand>((command, _) =>
        {
            command.SyncClasses();
            command.SyncItemStates();
        });
    }

    public CodexCommand()
    {
        AddHandler(InputElement.KeyDownEvent, OnDescendantKeyDown, RoutingStrategies.Bubble, handledEventsToo: true);
        SyncClasses();
        SyncFilter();
    }

    public event EventHandler<CodexCommandItemSelectedEventArgs>? ItemSelected;

    public string? Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public string? SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public bool ShouldFilter
    {
        get => GetValue(ShouldFilterProperty);
        set => SetValue(ShouldFilterProperty, value);
    }

    public bool LoopNavigation
    {
        get => GetValue(LoopNavigationProperty);
        set => SetValue(LoopNavigationProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public CodexCommandItem? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        private set => SetValue(SelectedItemProperty, value);
    }

    public bool TryHandleNavigationKey(Key key, bool moveFocus = false)
    {
        if (key is not (Key.Up or Key.Down or Key.Home or Key.End))
        {
            return false;
        }

        if (IsLoading)
        {
            return true;
        }

        var items = VisibleItems().Where(item => item.CanSelect()).ToList();
        if (items.Count == 0)
        {
            return false;
        }

        var currentIndex = items.FindIndex(item => item.IsActive);
        var nextIndex = key switch
        {
            Key.Home => 0,
            Key.End => items.Count - 1,
            Key.Down => NextIndex(items.Count, currentIndex, 1),
            Key.Up => NextIndex(items.Count, currentIndex, -1),
            _ => -1
        };

        if (nextIndex < 0)
        {
            return true;
        }

        SetActiveItem(items[nextIndex]);
        if (moveFocus)
        {
            FocusCommandItem(items[nextIndex]);
        }

        return true;
    }

    public bool TrySelectActiveItem()
    {
        if (IsLoading)
        {
            return true;
        }

        var active = VisibleItems().FirstOrDefault(item => item.IsActive && item.CanSelect())
                     ?? VisibleItems().FirstOrDefault(item => item.CanSelect());

        return active?.TrySelect(CodexCommandItemSelectSource.Keyboard) == true;
    }

    private void SyncClasses()
    {
        Classes.Set("loading", IsLoading);
        Classes.Set("loop", LoopNavigation);
    }

    private void SyncItemStates()
    {
        foreach (var item in this.GetLogicalDescendants().OfType<CodexCommandItem>())
        {
            item.SyncOwnerState();
        }
    }

    private void OnDescendantKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Source is CodexCommandItem && e.Handled)
        {
            return;
        }

        if (TryHandleNavigationKey(e.Key, moveFocus: e.Source is CodexCommandItem))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter && e.Source is not CodexCommandItem && TrySelectActiveItem())
        {
            e.Handled = true;
        }
    }

    internal void NotifyItemSelected(
        CodexCommandItem item,
        CodexCommandItemSelectSource source = CodexCommandItemSelectSource.Programmatic)
    {
        if (!ReferenceEquals(FindOwner(item), this))
        {
            return;
        }

        SelectedItem = item;
        ItemSelected?.Invoke(this, new CodexCommandItemSelectedEventArgs(item, item.ResolveValue(), source));
    }

    private void SyncFilter()
    {
        var search = SearchText?.Trim();
        var isSearching = !string.IsNullOrEmpty(search);
        var shouldFilter = ShouldFilter && isSearching;

        var visibleCount = 0;
        var itemCount = 0;
        var visibleItems = new List<CodexCommandItem>();
        foreach (var item in this.GetLogicalDescendants().OfType<CodexCommandItem>())
        {
            itemCount++;
            var matches = !shouldFilter || item.MatchesSearch(search);
            item.Classes.Set("filtered-out", !matches);
            if (matches)
            {
                visibleCount++;
                visibleItems.Add(item);
            }
        }

        foreach (var separator in this.GetLogicalDescendants().OfType<CodexCommandSeparator>())
        {
            separator.Classes.Set("filtered-out", shouldFilter && !separator.AlwaysRender);
        }

        foreach (var group in this.GetLogicalDescendants().OfType<CodexCommandGroup>())
        {
            var groupItems = group.GetLogicalDescendants().OfType<CodexCommandItem>().ToList();
            var hideGroup = shouldFilter
                            && groupItems.Count > 0
                            && groupItems.All(item => item.Classes.Contains("filtered-out"));
            group.Classes.Set("filtered-out", hideGroup);
        }

        Classes.Set("searching", isSearching);
        Classes.Set("filtering", shouldFilter);
        Classes.Set("has-results", itemCount > 0 && visibleCount > 0);
        Classes.Set("empty-results", shouldFilter && visibleCount == 0);

        var selectableItems = visibleItems.Where(item => item.CanSelect()).ToList();
        if (shouldFilter && selectableItems.Count > 0 && selectableItems.All(item => !item.IsActive))
        {
            SetActiveItem(selectableItems[0]);
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SyncFilter();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            SyncFilter();
        }
    }

    private IEnumerable<CodexCommandItem> VisibleItems()
    {
        return this.GetLogicalDescendants()
            .OfType<CodexCommandItem>()
            .Where(item => !item.Classes.Contains("filtered-out"));
    }

    private int NextIndex(int count, int currentIndex, int direction)
    {
        if (count == 0)
        {
            return -1;
        }

        if (currentIndex < 0)
        {
            return direction > 0 ? 0 : count - 1;
        }

        var next = currentIndex + direction;
        if (next >= 0 && next < count)
        {
            return next;
        }

        return LoopNavigation
            ? (next < 0 ? count - 1 : 0)
            : -1;
    }

    internal void SetActiveItem(CodexCommandItem activeItem)
    {
        foreach (var item in VisibleItems())
        {
            item.IsActive = ReferenceEquals(item, activeItem);
        }
    }

    private static void FocusCommandItem(CodexCommandItem item)
    {
        if (!item.Focus(NavigationMethod.Directional, KeyModifiers.None))
        {
            item.Focus(NavigationMethod.Tab, KeyModifiers.None);
        }
    }

    internal static CodexCommand? FindOwner(CodexCommandItem item)
    {
        for (var parent = item.GetLogicalParent(); parent is not null; parent = parent.GetLogicalParent())
        {
            if (parent is CodexCommand command)
            {
                return command;
            }
        }

        return null;
    }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexCommandInput : TextBox
{
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
}

public class CodexCommandList : ItemsControl
{
}

public class CodexCommandGroup : ItemsControl
{
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<CodexCommandGroup, object?>(nameof(Header));

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexCommandItem : Button
{
    private ICommand? _subscribedCommand;
    private bool _hasPrimaryPointerPress;
    private PointerUpdateKind? _pendingPointerReleaseKind;
    private bool _selectionHandledByPointerRelease;

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexCommandItem, bool>(nameof(IsActive));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexCommandItem, object?>(nameof(Icon));

    public static readonly StyledProperty<string?> ShortcutProperty =
        AvaloniaProperty.Register<CodexCommandItem, string?>(nameof(Shortcut));

    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CodexCommandItem, string?>(nameof(Value));

    public static readonly StyledProperty<string?> KeywordsProperty =
        AvaloniaProperty.Register<CodexCommandItem, string?>(nameof(Keywords));

    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<CodexCommandItem, bool>(nameof(HasIcon));

    public static readonly StyledProperty<bool> HasShortcutProperty =
        AvaloniaProperty.Register<CodexCommandItem, bool>(nameof(HasShortcut));

    static CodexCommandItem()
    {
        IsActiveProperty.Changed.AddClassHandler<CodexCommandItem>((item, _) => item.SyncClasses());
        IconProperty.Changed.AddClassHandler<CodexCommandItem>((item, _) => item.SyncClasses());
        ShortcutProperty.Changed.AddClassHandler<CodexCommandItem>((item, _) => item.SyncClasses());
        CommandProperty.Changed.AddClassHandler<CodexCommandItem>((item, args) => item.OnCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        CommandParameterProperty.Changed.AddClassHandler<CodexCommandItem>((item, _) => item.SyncClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexCommandItem>((item, _) => item.SyncClasses());
    }

    public CodexCommandItem()
    {
        SyncClasses();
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string? Shortcut
    {
        get => GetValue(ShortcutProperty);
        set => SetValue(ShortcutProperty, value);
    }

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string? Keywords
    {
        get => GetValue(KeywordsProperty);
        set => SetValue(KeywordsProperty, value);
    }

    public bool HasIcon => GetValue(HasIconProperty);

    public bool HasShortcut => GetValue(HasShortcutProperty);

    public bool CanSelect()
    {
        return IsEnabled
               && !IsInsideLoadingCommand()
               && CanExecuteCommand();
    }

    public bool TrySelect()
    {
        return TrySelect(CodexCommandItemSelectSource.Programmatic);
    }

    internal bool TrySelect(CodexCommandItemSelectSource source)
    {
        if (!CanSelect())
        {
            return false;
        }

        SelectSiblingItems();
        base.OnClick();
        CodexCommand.FindOwner(this)?.NotifyItemSelected(this, source);
        CodexCommandDialog.FindOwner(this)?.NotifyItemSelected(this, source);
        return true;
    }

    internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased)
        {
            return false;
        }

        return TrySelect(CodexCommandItemSelectSource.Pointer);
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
        _hasPrimaryPointerPress = e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed;
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        var canActivateFromPointer = _hasPrimaryPointerPress && IsPointerOver;
        _hasPrimaryPointerPress = false;

        if (canActivateFromPointer)
        {
            _pendingPointerReleaseKind = updateKind;
            try
            {
                base.OnPointerReleased(e);
            }
            finally
            {
                _pendingPointerReleaseKind = null;
            }

            if (_selectionHandledByPointerRelease)
            {
                _selectionHandledByPointerRelease = false;
                e.Handled = true;
            }

            return;
        }

        base.OnPointerReleased(e);
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);

        if (CanSelect() && CodexCommand.FindOwner(this) is { } owner)
        {
            owner.SetActiveItem(this);
        }
    }

    protected override void OnClick()
    {
        if (_pendingPointerReleaseKind is { } updateKind)
        {
            _selectionHandledByPointerRelease = TryHandlePointerActivation(updateKind);
            return;
        }

        TrySelect(CodexCommandItemSelectSource.Keyboard);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
            _subscribedCommand = null;
        }

        base.OnDetachedFromVisualTree(e);
    }

    internal void SyncOwnerState()
    {
        SyncClasses();
    }

    private bool IsInsideLoadingCommand()
    {
        for (var parent = this.GetLogicalParent(); parent is not null; parent = parent.GetLogicalParent())
        {
            if (parent is CodexCommand command)
            {
                return command.IsLoading;
            }
        }

        return false;
    }

    private void SelectSiblingItems()
    {
        if (CodexCommand.FindOwner(this) is { } owner)
        {
            owner.SetActiveItem(this);
            return;
        }

        var parent = this.GetLogicalParent();
        if (parent is null)
        {
            IsActive = true;
            return;
        }

        foreach (var child in parent.GetLogicalChildren())
        {
            if (child is CodexCommandItem item)
            {
                item.IsActive = ReferenceEquals(item, this);
            }
        }
    }

    private void SyncClasses()
    {
        Classes.Set("active", IsActive);
        Classes.Set("has-icon", Icon is not null);
        Classes.Set("has-shortcut", !string.IsNullOrWhiteSpace(Shortcut));
        Classes.Set("can-select", CanSelect());
        Classes.Set("command-blocked", Command is not null && IsEnabled && !IsInsideLoadingCommand() && !CanExecuteCommand());
        SetValue(HasIconProperty, Icon is not null);
        SetValue(HasShortcutProperty, !string.IsNullOrWhiteSpace(Shortcut));
    }

    private bool CanExecuteCommand()
    {
        return Command?.CanExecute(CommandParameter) ?? true;
    }

    private void OnCommandChanged(ICommand? oldCommand, ICommand? newCommand)
    {
        if (ReferenceEquals(oldCommand, newCommand))
        {
            return;
        }

        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
        }

        _subscribedCommand = newCommand;

        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged += OnCommandCanExecuteChanged;
        }

        SyncClasses();
    }

    private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
    {
        SyncClasses();
    }

    internal bool MatchesSearch(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return true;
        }

        return ContainsSearch(ResolveValue(), search)
               || ContainsSearch(Content?.ToString(), search)
               || ContainsSearch(Keywords, search);
    }

    internal string? ResolveValue()
    {
        var value = string.IsNullOrWhiteSpace(Value)
            ? Content?.ToString()
            : Value;

        return value?.Trim();
    }

    private static bool ContainsSearch(string? value, string search)
    {
        return value?.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) >= 0;
    }
}

public class CodexCommandShortcut : ContentControl
{
}

public class CodexCommandSeparator : Separator
{
    public static readonly StyledProperty<bool> AlwaysRenderProperty =
        AvaloniaProperty.Register<CodexCommandSeparator, bool>(nameof(AlwaysRender));

    public bool AlwaysRender
    {
        get => GetValue(AlwaysRenderProperty);
        set => SetValue(AlwaysRenderProperty, value);
    }
}

public class CodexCommandEmpty : CodexFrame
{
}

public class CodexCommandLoading : CodexFrame
{
}
