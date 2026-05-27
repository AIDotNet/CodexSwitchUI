using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Collections.Specialized;

namespace CodexSwitchUI.Controls;

public class CodexContextMenu : ContextMenu
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexContextMenu, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexContextMenu, bool>(nameof(IsLoading));

    static CodexContextMenu()
    {
        SizeProperty.Changed.AddClassHandler<CodexContextMenu>((menu, _) => menu.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexContextMenu>((menu, _) => menu.SyncClasses());
        IsOpenProperty.Changed.AddClassHandler<CodexContextMenu>((menu, _) => menu.SyncOpenClasses());
        PlacementProperty.Changed.AddClassHandler<CodexContextMenu>((menu, _) => menu.SyncPlacementClasses());
    }

    public CodexContextMenu()
    {
        CodexMenuActivation.RegisterOwner(this);
        SyncClasses();
        SyncPlacementClasses();
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
        return new CodexContextMenuItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<CodexContextMenuItem>(item, out recycleKey);
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

    private void SyncOpenClasses()
    {
        Classes.Set("context-menu-open", false);

        if (!IsOpen)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (IsOpen)
            {
                Classes.Set("context-menu-open", true);
            }
        });
    }

    private void SyncPlacementClasses()
    {
        Classes.Set("side-top", Placement is PlacementMode.Top or PlacementMode.TopEdgeAlignedLeft or PlacementMode.TopEdgeAlignedRight);
        Classes.Set("side-left", Placement is PlacementMode.Left or PlacementMode.LeftEdgeAlignedTop or PlacementMode.LeftEdgeAlignedBottom);
        Classes.Set("side-right", Placement is PlacementMode.Right or PlacementMode.RightEdgeAlignedTop or PlacementMode.RightEdgeAlignedBottom);
        Classes.Set("side-bottom", Placement is not (PlacementMode.Top or PlacementMode.TopEdgeAlignedLeft or PlacementMode.TopEdgeAlignedRight
            or PlacementMode.Left or PlacementMode.LeftEdgeAlignedTop or PlacementMode.LeftEdgeAlignedBottom
            or PlacementMode.Right or PlacementMode.RightEdgeAlignedTop or PlacementMode.RightEdgeAlignedBottom));
    }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexContextMenuItem : MenuItem
{
    private CodexMenuItemSelectSource? _pendingSelectSource;

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexContextMenuItem, bool>(nameof(IsActive));

    public static readonly StyledProperty<string?> ShortcutProperty =
        AvaloniaProperty.Register<CodexContextMenuItem, string?>(nameof(Shortcut));

    public static readonly StyledProperty<bool> HasShortcutProperty =
        AvaloniaProperty.Register<CodexContextMenuItem, bool>(nameof(HasShortcut));

    public static readonly StyledProperty<bool> IsInsetProperty =
        AvaloniaProperty.Register<CodexContextMenuItem, bool>(nameof(IsInset));

    public static readonly StyledProperty<PlacementMode> SubMenuPlacementProperty =
        AvaloniaProperty.Register<CodexContextMenuItem, PlacementMode>(nameof(SubMenuPlacement), PlacementMode.RightEdgeAlignedTop);

    static CodexContextMenuItem()
    {
        IsActiveProperty.Changed.AddClassHandler<CodexContextMenuItem>((item, _) => item.SyncClasses());
        ShortcutProperty.Changed.AddClassHandler<CodexContextMenuItem>((item, _) => item.SyncClasses());
        IsInsetProperty.Changed.AddClassHandler<CodexContextMenuItem>((item, _) => item.SyncClasses());
        IsCheckedProperty.Changed.AddClassHandler<CodexContextMenuItem>((item, _) => item.SyncClasses());
        ToggleTypeProperty.Changed.AddClassHandler<CodexContextMenuItem>((item, _) => item.SyncClasses());
        IsSubMenuOpenProperty.Changed.AddClassHandler<CodexContextMenuItem>((item, _) => item.SyncSubMenuClasses());
        SubMenuPlacementProperty.Changed.AddClassHandler<CodexContextMenuItem>((item, _) => item.SyncSubMenuPlacementClasses());
    }

    public CodexContextMenuItem()
    {
        CodexMenuActivation.RegisterOwner(this);
        SyncClasses();
        SyncSubMenuPlacementClasses();
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

    public bool IsInset
    {
        get => GetValue(IsInsetProperty);
        set => SetValue(IsInsetProperty, value);
    }

    public PlacementMode SubMenuPlacement
    {
        get => GetValue(SubMenuPlacementProperty);
        set => SetValue(SubMenuPlacementProperty, value);
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
        if (!CodexMenuActivation.CanActivate(this))
        {
            e.Handled = true;
            return;
        }

        _pendingSelectSource = CodexMenuItemSelectSource.Pointer;
        try
        {
            base.OnPointerReleased(e);
        }
        finally
        {
            ClearPendingSelectSourceLater(CodexMenuItemSelectSource.Pointer);
        }
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
        return CodexMenuActivation.TryHandleSubMenuKey(this, key, openOnDown: false);
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

        if (change.Property.Name is nameof(HasSubMenu))
        {
            SyncClasses();
        }
    }

    private void SyncClasses()
    {
        Classes.Set("active", IsActive);
        Classes.Set("inset", IsInset);
        Classes.Set("has-submenu", HasSubMenu);
        Classes.Set("is-checked", IsChecked);
        Classes.Set("is-check", ToggleType == MenuItemToggleType.CheckBox);
        Classes.Set("is-radio", ToggleType == MenuItemToggleType.Radio);
        SetValue(HasShortcutProperty, !string.IsNullOrWhiteSpace(Shortcut));
    }

    private void SyncSubMenuClasses()
    {
        SyncClasses();
        Classes.Set("submenu-open", false);

        if (!IsSubMenuOpen)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (IsSubMenuOpen)
            {
                Classes.Set("submenu-open", true);
            }
        });
    }

    private void SyncSubMenuPlacementClasses()
    {
        Classes.Set("submenu-side-top", SubMenuPlacement is PlacementMode.Top or PlacementMode.TopEdgeAlignedLeft or PlacementMode.TopEdgeAlignedRight);
        Classes.Set("submenu-side-left", SubMenuPlacement is PlacementMode.Left or PlacementMode.LeftEdgeAlignedTop or PlacementMode.LeftEdgeAlignedBottom);
        Classes.Set("submenu-side-right", SubMenuPlacement is PlacementMode.Right or PlacementMode.RightEdgeAlignedTop or PlacementMode.RightEdgeAlignedBottom);
        Classes.Set("submenu-side-bottom", SubMenuPlacement is PlacementMode.Bottom or PlacementMode.BottomEdgeAlignedLeft or PlacementMode.BottomEdgeAlignedRight);
    }
}

public class CodexContextMenuLabel : ContentControl
{
    public static readonly StyledProperty<bool> IsInsetProperty =
        AvaloniaProperty.Register<CodexContextMenuLabel, bool>(nameof(IsInset));

    static CodexContextMenuLabel()
    {
        IsInsetProperty.Changed.AddClassHandler<CodexContextMenuLabel>((label, _) => label.SyncClasses());
    }

    public CodexContextMenuLabel()
    {
        SyncClasses();
    }

    public bool IsInset
    {
        get => GetValue(IsInsetProperty);
        set => SetValue(IsInsetProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("inset", IsInset);
    }
}

public class CodexContextMenuShortcut : ContentControl
{
}

public class CodexContextMenuSeparator : Separator
{
}

public class CodexContextMenuGroup : ItemsControl
{
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<CodexContextMenuGroup, object?>(nameof(Header));

    public static readonly StyledProperty<bool> HasHeaderProperty =
        AvaloniaProperty.Register<CodexContextMenuGroup, bool>(nameof(HasHeader));

    static CodexContextMenuGroup()
    {
        HeaderProperty.Changed.AddClassHandler<CodexContextMenuGroup>((group, _) => group.SyncClasses());
    }

    public CodexContextMenuGroup()
    {
        SyncClasses();
    }

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public bool HasHeader => GetValue(HasHeaderProperty);

    private void SyncClasses()
    {
        var hasHeader = Header is string text ? !string.IsNullOrWhiteSpace(text) : Header is not null;
        SetValue(HasHeaderProperty, hasHeader);
        Classes.Set("has-header", hasHeader);
    }
}
