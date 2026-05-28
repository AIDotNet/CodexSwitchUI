using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Collections.Specialized;

namespace CodexSwitchUI.Controls;

public enum CodexMenuItemSelectSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public sealed class CodexMenuItemSelectedEventArgs(
    MenuItem item,
    CodexMenuItemSelectSource source,
    bool didCloseOnSelect)
    : EventArgs
{
    public MenuItem Item { get; } = item;

    public object? Header => Item.Header;

    public object? CommandParameter => Item.CommandParameter;

    public MenuItemToggleType ToggleType => Item.ToggleType;

    public bool IsChecked => Item.IsChecked;

    public bool HasSubMenu => Item.HasSubMenu;

    public CodexMenuItemSelectSource Source { get; } = source;

    public bool DidCloseOnSelect { get; } = didCloseOnSelect;
}

public class CodexMenu : Menu
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexMenu, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexMenu, bool>(nameof(IsLoading));

    static CodexMenu()
    {
        SizeProperty.Changed.AddClassHandler<CodexMenu>((menu, _) => menu.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexMenu>((menu, _) => menu.SyncClasses());
    }

    public CodexMenu()
    {
        CodexMenuActivation.RegisterOwner(this);
        SyncClasses();
        ItemsView.CollectionChanged += OnItemsViewChanged;
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CodexMenuItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<CodexMenuItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        CodexMenuActivation.TrackOwner(container, this);
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        CodexMenuActivation.ClearOwner(element);
        base.ClearContainerForItemOverride(element);
    }

    private void OnItemsViewChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CodexMenuActivation.ClearItemOwners(e.OldItems);
        CodexMenuActivation.TrackItemOwners(this);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("loading", IsLoading);
    }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexMenuItem : MenuItem
{
    private CodexMenuItemSelectSource? _pendingSelectSource;

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexMenuItem, bool>(nameof(IsActive));

    public static readonly StyledProperty<string?> ShortcutProperty =
        AvaloniaProperty.Register<CodexMenuItem, string?>(nameof(Shortcut));

    public static readonly StyledProperty<bool> HasShortcutProperty =
        AvaloniaProperty.Register<CodexMenuItem, bool>(nameof(HasShortcut));

    static CodexMenuItem()
    {
        IsActiveProperty.Changed.AddClassHandler<CodexMenuItem>((item, _) => item.SyncClasses());
        ShortcutProperty.Changed.AddClassHandler<CodexMenuItem>((item, _) => item.SyncClasses());
        IsCheckedProperty.Changed.AddClassHandler<CodexMenuItem>((item, _) => item.SyncClasses());
        ToggleTypeProperty.Changed.AddClassHandler<CodexMenuItem>((item, _) => item.SyncClasses());
    }

    public CodexMenuItem()
    {
        CodexMenuActivation.RegisterOwner(this);
        SyncClasses();
        ItemsView.CollectionChanged += OnItemsViewChanged;
    }

    public event EventHandler<CodexMenuItemSelectedEventArgs>? ItemSelected;

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public string? Shortcut
    {
        get => GetValue(ShortcutProperty);
        set => SetValue(ShortcutProperty, value);
    }

    public bool HasShortcut => GetValue(HasShortcutProperty);

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

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        CodexMenuActivation.RequestPointerSubMenuOpen(this);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        CodexMenuActivation.RequestPointerSubMenuClose(this);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        if (!TryHandlePointerSelection(updateKind))
        {
            if (!CodexMenuActivation.CanActivate(this))
            {
                e.Handled = true;
            }

            return;
        }

        try
        {
            base.OnPointerReleased(e);
        }
        finally
        {
            ClearPendingSelectSourceLater(CodexMenuItemSelectSource.Pointer);
        }
    }

    internal bool TryHandlePointerSelection(PointerUpdateKind updateKind)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased
            || !CodexMenuActivation.CanActivate(this))
        {
            return false;
        }

        _pendingSelectSource = CodexMenuItemSelectSource.Pointer;
        return true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (TryHandleSubMenuKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        if (TryHandleSiblingNavigationKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        if (CodexMenuActivation.IsActivationKey(e.Key) && !CodexMenuActivation.CanActivate(this))
        {
            e.Handled = true;
            return;
        }

        var tracksKeyboardSelect = CodexMenuActivation.IsActivationKey(e.Key);
        if (tracksKeyboardSelect)
        {
            _pendingSelectSource = CodexMenuItemSelectSource.Keyboard;
        }

        try
        {
            base.OnKeyDown(e);
        }
        finally
        {
            if (tracksKeyboardSelect)
            {
                ClearPendingSelectSourceLater(CodexMenuItemSelectSource.Keyboard);
            }
        }
    }

    internal bool TryHandleSubMenuKey(Key key)
    {
        return CodexMenuActivation.TryHandleSubMenuKey(this, key, openOnDown: true);
    }

    internal bool TryHandleSiblingNavigationKey(Key key)
    {
        return CodexMenuActivation.TryHandleSiblingNavigationKey(this, key);
    }

    internal bool TryCloseOnSelect()
    {
        return CodexMenuActivation.TryCloseOnSelect(this);
    }

    protected override void OnClick(RoutedEventArgs e)
    {
        if (!CodexMenuActivation.CanActivate(this))
        {
            e.Handled = true;
            return;
        }

        var source = _pendingSelectSource ?? CodexMenuItemSelectSource.Programmatic;
        _pendingSelectSource = null;
        var shouldCloseOnSelect = CodexMenuActivation.ShouldCloseOnSelect(this);
        base.OnClick(e);
        var didCloseOnSelect = false;

        if (shouldCloseOnSelect)
        {
            didCloseOnSelect = CodexMenuActivation.TryCloseOnSelect(this);
        }

        if (!HasSubMenu)
        {
            ItemSelected?.Invoke(this, new CodexMenuItemSelectedEventArgs(this, source, didCloseOnSelect));
        }
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        CodexMenuActivation.TrackOwner(container, this);
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        CodexMenuActivation.ClearOwner(element);
        base.ClearContainerForItemOverride(element);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        CodexMenuActivation.CancelPointerSubMenuRequests(this);
        base.OnDetachedFromVisualTree(e);
    }

    private void OnItemsViewChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CodexMenuActivation.ClearItemOwners(e.OldItems);
        CodexMenuActivation.TrackItemOwners(this);
    }

    private void ClearPendingSelectSourceLater(CodexMenuItemSelectSource source)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_pendingSelectSource == source)
            {
                _pendingSelectSource = null;
            }
        });
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property.Name is nameof(HasSubMenu) or nameof(IsSubMenuOpen))
        {
            SyncClasses();
        }
    }

    private void SyncClasses()
    {
        Classes.Set("active", IsActive);
        Classes.Set("has-submenu", HasSubMenu);
        Classes.Set("is-checked", IsChecked);
        Classes.Set("is-check", ToggleType == MenuItemToggleType.CheckBox);
        Classes.Set("is-radio", ToggleType == MenuItemToggleType.Radio);
        SetValue(HasShortcutProperty, !string.IsNullOrWhiteSpace(Shortcut));
    }
}

public class CodexMenuSeparator : Separator
{
}

public class CodexMenuGroup : MenuItem
{
    public CodexMenuGroup()
    {
        Focusable = false;
    }
}

public class CodexMenuEmpty : CodexFrame
{
}

public class CodexMenuLoading : CodexFrame
{
}
