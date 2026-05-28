using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace CodexSwitchUI.Controls;

public sealed class CodexMenubarActiveMenuChangedEventArgs(CodexMenubarItem? oldMenu, CodexMenubarItem? newMenu)
    : EventArgs
{
    public CodexMenubarItem? OldMenu { get; } = oldMenu;

    public CodexMenubarItem? NewMenu { get; } = newMenu;
}

public class CodexMenubar : Menu
{
    private static readonly ConditionalWeakTable<CodexMenubarItem, OwnerReference> Owners = new();

    private CodexMenubarItem? _activeMenu;

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexMenubar, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexMenubar, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexMenubar, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> LoopProperty =
        AvaloniaProperty.Register<CodexMenubar, bool>(nameof(Loop));

    static CodexMenubar()
    {
        SizeProperty.Changed.AddClassHandler<CodexMenubar>((menubar, _) => menubar.SyncClasses());
        OrientationProperty.Changed.AddClassHandler<CodexMenubar>((menubar, _) => menubar.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexMenubar>((menubar, _) => menubar.SyncClasses());
        LoopProperty.Changed.AddClassHandler<CodexMenubar>((menubar, _) => menubar.SyncClasses());
    }

    public CodexMenubar()
    {
        CodexMenuActivation.RegisterOwner(this);
        SyncClasses();
        ItemsView.CollectionChanged += OnItemsViewChanged;
    }

    public event EventHandler<CodexMenubarActiveMenuChangedEventArgs>? ActiveMenuChanged;

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

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool Loop
    {
        get => GetValue(LoopProperty);
        set => SetValue(LoopProperty, value);
    }

    public new bool IsOpen => _activeMenu is { IsSubMenuOpen: true };

    public CodexMenubarItem? ActiveMenu => _activeMenu;

    public bool OpenMenu(CodexMenubarItem item, bool focusFirstChild = false)
    {
        if (!ReferenceEquals(FindOwner(item), this) || !item.HasSubMenu || !CodexMenuActivation.CanActivate(item))
        {
            return false;
        }

        var oldMenu = _activeMenu;

        foreach (var menu in TopLevelMenus())
        {
            menu.IsSubMenuOpen = ReferenceEquals(menu, item);
        }

        SetActiveMenu(item, oldMenu);

        if (focusFirstChild)
        {
            FocusSubMenuItem(item, first: true);
        }

        return true;
    }

    public bool ToggleMenu(CodexMenubarItem item, bool focusFirstChild = false)
    {
        if (ReferenceEquals(_activeMenu, item) && item.IsSubMenuOpen)
        {
            return Dismiss();
        }

        return OpenMenu(item, focusFirstChild);
    }

    public bool Dismiss()
    {
        var oldMenu = _activeMenu;
        var closed = false;

        foreach (var menu in TopLevelMenus())
        {
            if (menu.IsSubMenuOpen)
            {
                menu.IsSubMenuOpen = false;
                closed = true;
            }
        }

        if (oldMenu is not null)
        {
            SetActiveMenu(null, oldMenu);
            closed = true;
        }

        return closed;
    }

    public bool TryHandleTopLevelNavigationKey(CodexMenubarItem item, Key key, bool moveFocus = true)
    {
        if (!ReferenceEquals(FindOwner(item), this))
        {
            return false;
        }

        if (IsLoading)
        {
            return true;
        }

        if (key == Key.Escape)
        {
            return Dismiss();
        }

        if (IsOpenKey(key))
        {
            return OpenMenu(item, focusFirstChild: true);
        }

        var step = NavigationStep(key);
        if (step is null)
        {
            return false;
        }

        return MoveActiveMenu(item, step.Value, moveFocus);
    }

    internal bool TryHandleTopLevelPointerEntered(CodexMenubarItem item)
    {
        if (!ReferenceEquals(FindOwner(item), this) || !IsOpen || ReferenceEquals(_activeMenu, item))
        {
            return false;
        }

        return OpenMenu(item);
    }

    internal bool TryHandleTopLevelPointerRelease(CodexMenubarItem item, PointerUpdateKind updateKind)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased
            || !ReferenceEquals(FindOwner(item), this)
            || !item.HasSubMenu)
        {
            return false;
        }

        return ToggleMenu(item);
    }

    internal void NotifyTopLevelOpenStateChanged(CodexMenubarItem item)
    {
        if (!ReferenceEquals(FindOwner(item), this))
        {
            return;
        }

        if (item.IsSubMenuOpen)
        {
            SetActiveMenu(item, _activeMenu);
        }
        else if (ReferenceEquals(_activeMenu, item))
        {
            SetActiveMenu(null, item);
        }
        else
        {
            SyncOpenClasses();
        }
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CodexMenubarMenu();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<CodexMenubarItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        CodexMenuActivation.TrackOwner(container, this);

        if (container is CodexMenubarItem menuItem)
        {
            TrackOwner(menuItem, this);
            menuItem.SetTopLevelOwner(true);
        }
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        CodexMenuActivation.ClearOwner(element);

        if (element is CodexMenubarItem menuItem)
        {
            ClearOwner(menuItem);
            menuItem.SetTopLevelOwner(false);
        }

        base.ClearContainerForItemOverride(element);
    }

    private bool MoveActiveMenu(CodexMenubarItem current, int step, bool moveFocus)
    {
        var menus = TopLevelMenus()
            .Where(CodexMenuActivation.CanActivate)
            .ToArray();
        if (menus.Length == 0)
        {
            return true;
        }

        var currentIndex = Array.IndexOf(menus, current);
        if (currentIndex < 0)
        {
            currentIndex = step > 0 ? -1 : menus.Length;
        }

        var nextIndex = step switch
        {
            int.MinValue => 0,
            int.MaxValue => menus.Length - 1,
            > 0 => NextIndex(currentIndex, menus.Length, 1),
            < 0 => NextIndex(currentIndex, menus.Length, -1),
            _ => currentIndex
        };

        if (nextIndex < 0 || nextIndex >= menus.Length)
        {
            return true;
        }

        var next = menus[nextIndex];
        if (moveFocus)
        {
            FocusMenu(next);
        }

        if (IsOpen)
        {
            OpenMenu(next);
        }
        else
        {
            SetActiveMenu(next, _activeMenu);
        }

        return true;
    }

    private int? NavigationStep(Key key)
    {
        if (key == Key.Home)
        {
            return int.MinValue;
        }

        if (key == Key.End)
        {
            return int.MaxValue;
        }

        return Orientation == Orientation.Horizontal
            ? key switch
            {
                Key.Right => 1,
                Key.Left => -1,
                _ => null
            }
            : key switch
            {
                Key.Down => 1,
                Key.Up => -1,
                _ => null
            };
    }

    private bool IsOpenKey(Key key)
    {
        return key is Key.Enter or Key.Space
               || (Orientation == Orientation.Horizontal && key is Key.Down or Key.Up)
               || (Orientation == Orientation.Vertical && key == Key.Right);
    }

    private int NextIndex(int currentIndex, int length, int step)
    {
        var next = currentIndex + step;
        if (next >= 0 && next < length)
        {
            return next;
        }

        return Loop ? (next + length) % length : -1;
    }

    private IReadOnlyList<CodexMenubarItem> TopLevelMenus()
    {
        var menus = new List<CodexMenubarItem>();

        foreach (var item in Items)
        {
            if (item is CodexMenubarItem menu)
            {
                menus.Add(menu);
            }
        }

        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (ItemsView[index] is CodexMenubarItem menu && !menus.Contains(menu))
            {
                menus.Add(menu);
            }
            else if (ContainerFromIndex(index) is CodexMenubarItem container && !menus.Contains(container))
            {
                menus.Add(container);
            }
        }

        return menus;
    }

    private void SetActiveMenu(CodexMenubarItem? newMenu, CodexMenubarItem? oldMenu)
    {
        if (ReferenceEquals(newMenu, oldMenu))
        {
            SyncOpenClasses();
            newMenu?.SyncMenubarClasses();
            return;
        }

        _activeMenu = newMenu;
        oldMenu?.SyncMenubarClasses();
        newMenu?.SyncMenubarClasses();
        SyncOpenClasses();
        ActiveMenuChanged?.Invoke(this, new CodexMenubarActiveMenuChangedEventArgs(oldMenu, newMenu));
    }

    private void SyncClasses()
    {
        Classes.Set("menubar", true);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("loading", IsLoading);
        Classes.Set("loop", Loop);
        SyncOpenClasses();
    }

    private void SyncOpenClasses()
    {
        var isOpen = _activeMenu is { IsSubMenuOpen: true };
        Classes.Set("open", isOpen);
        Classes.Set("closed", !isOpen);
        Classes.Set("has-open-menu", isOpen);
    }

    private void OnItemsViewChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is CodexMenubarItem menuItem)
                {
                    ClearOwner(menuItem);
                    menuItem.SetTopLevelOwner(false);
                }
            }
        }

        CodexMenuActivation.ClearItemOwners(e.OldItems);
        CodexMenuActivation.TrackItemOwners(this);

        foreach (var item in ItemsView.OfType<CodexMenubarItem>())
        {
            TrackOwner(item, this);
            item.SetTopLevelOwner(true);
        }
    }

    private static void FocusSubMenuItem(CodexMenubarItem item, bool first)
    {
        var candidates = item.ItemsView
            .OfType<MenuItem>()
            .Where(CodexMenuActivation.CanActivate)
            .ToArray();

        if (candidates.Length == 0)
        {
            return;
        }

        FocusMenu(first ? candidates[0] : candidates[^1]);
    }

    private static void FocusMenu(MenuItem item)
    {
        if (!item.Focus(NavigationMethod.Directional, KeyModifiers.None))
        {
            item.Focus(NavigationMethod.Tab, KeyModifiers.None);
        }
    }

    private static void TrackOwner(CodexMenubarItem item, CodexMenubar owner)
    {
        Owners.Remove(item);
        Owners.Add(item, new OwnerReference(owner));
    }

    private static void ClearOwner(CodexMenubarItem item)
    {
        Owners.Remove(item);
    }

    internal static CodexMenubar? FindOwner(CodexMenubarItem item)
    {
        if (Owners.TryGetValue(item, out var reference))
        {
            return reference.Owner;
        }

        if (ItemsControl.ItemsControlFromItemContainer(item) is CodexMenubar owner)
        {
            return owner;
        }

        for (var parent = item.GetLogicalParent(); parent is not null; parent = parent.GetLogicalParent())
        {
            if (parent is CodexMenubar menubar)
            {
                return menubar;
            }
        }

        return item.GetVisualAncestors().OfType<CodexMenubar>().FirstOrDefault();
    }

    private sealed class OwnerReference(CodexMenubar owner)
    {
        public CodexMenubar Owner { get; } = owner;
    }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexMenubarItem : CodexMenuItem
{
    private bool _isTopLevel;

    public CodexMenubarItem()
    {
        Classes.Set("menubar-item", true);
        SyncMenubarClasses();
        ItemsView.CollectionChanged += (_, _) => SyncMenubarClasses();
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CodexMenubarItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<CodexMenubarItem>(item, out recycleKey);
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        if (_isTopLevel)
        {
            CodexMenubar.FindOwner(this)?.TryHandleTopLevelPointerEntered(this);
            return;
        }

        base.OnPointerEntered(e);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        if (_isTopLevel)
        {
            return;
        }

        base.OnPointerExited(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (_isTopLevel && HasSubMenu)
        {
            var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
            if (CodexMenubar.FindOwner(this)?.TryHandleTopLevelPointerRelease(this, updateKind) == true)
            {
                e.Handled = true;
            }

            return;
        }

        base.OnPointerReleased(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (_isTopLevel && CodexMenubar.FindOwner(this)?.TryHandleTopLevelNavigationKey(this, e.Key) == true)
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property.Name is nameof(HasSubMenu) or nameof(IsChecked) or nameof(ToggleType))
        {
            SyncMenubarClasses();
        }
        else if (change.Property.Name is nameof(IsSubMenuOpen))
        {
            SyncMenubarClasses();
            CodexMenubar.FindOwner(this)?.NotifyTopLevelOpenStateChanged(this);
        }
    }

    internal void SetTopLevelOwner(bool isTopLevel)
    {
        _isTopLevel = isTopLevel;
        SyncMenubarClasses();
    }

    internal void SyncMenubarClasses()
    {
        Classes.Set("top-level", _isTopLevel);
        Classes.Set("menu-content-item", !_isTopLevel);
        Classes.Set("open", IsSubMenuOpen);
        Classes.Set("closed", !IsSubMenuOpen);
        Classes.Set("has-items", HasSubMenu);
        Classes.Set("is-checked", IsChecked);
        Classes.Set("is-check", ToggleType == MenuItemToggleType.CheckBox);
        Classes.Set("is-radio", ToggleType == MenuItemToggleType.Radio);
        Classes.Set("active-menu", ReferenceEquals(CodexMenubar.FindOwner(this)?.ActiveMenu, this));
    }
}

public class CodexMenubarMenu : CodexMenubarItem
{
    public CodexMenubarMenu()
    {
        Classes.Set("menubar-menu", true);
    }
}

public class CodexMenubarCheckboxItem : CodexMenubarItem
{
    public CodexMenubarCheckboxItem()
    {
        ToggleType = MenuItemToggleType.CheckBox;
        Classes.Set("menubar-checkbox-item", true);
    }
}

public class CodexMenubarRadioItem : CodexMenubarItem
{
    public CodexMenubarRadioItem()
    {
        ToggleType = MenuItemToggleType.Radio;
        Classes.Set("menubar-radio-item", true);
    }
}

public class CodexMenubarSeparator : Separator
{
    public CodexMenubarSeparator()
    {
        Classes.Set("menubar-separator", true);
    }
}

public class CodexMenubarGroup : CodexMenuGroup
{
    public CodexMenubarGroup()
    {
        Classes.Set("menubar-group", true);
    }
}

public class CodexMenubarLabel : ContentControl
{
    public CodexMenubarLabel()
    {
        Classes.Set("menubar-label", true);
    }
}
