using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CodexSwitchUI.Controls;

internal static class CodexMenuActivation
{
    public static readonly TimeSpan PointerSubMenuOpenDelay = TimeSpan.FromMilliseconds(100);
    public static readonly TimeSpan PointerSubMenuCloseDelay = TimeSpan.FromMilliseconds(300);

    private static readonly ConditionalWeakTable<MenuItem, OwnerReference> Owners = new();
    private static readonly ConditionalWeakTable<MenuItem, PointerSubMenuState> PointerStates = new();
    private static readonly List<WeakReference<ItemsControl>> RegisteredOwners = [];

    public static bool CanActivate(MenuItem item)
    {
        return item.IsEnabled
               && !IsInsideLoadingMenu(item)
               && (item.Command?.CanExecute(item.CommandParameter) ?? true);
    }

    public static void TrackOwner(Control container, ItemsControl owner)
    {
        if (container is not MenuItem item)
        {
            return;
        }

        Owners.Remove(item);
        Owners.Add(item, new OwnerReference(owner));
    }

    public static void RegisterOwner(ItemsControl owner)
    {
        for (var index = RegisteredOwners.Count - 1; index >= 0; index--)
        {
            if (!RegisteredOwners[index].TryGetTarget(out var target))
            {
                RegisteredOwners.RemoveAt(index);
                continue;
            }

            if (ReferenceEquals(target, owner))
            {
                return;
            }
        }

        RegisteredOwners.Add(new WeakReference<ItemsControl>(owner));
    }

    public static void TrackItemOwners(ItemsControl owner)
    {
        foreach (var item in owner.ItemsView)
        {
            if (item is MenuItem menuItem)
            {
                TrackOwner(menuItem, owner);
            }
        }
    }

    public static void ClearItemOwners(IEnumerable? items)
    {
        if (items is null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item is MenuItem menuItem)
            {
                Owners.Remove(menuItem);
            }
        }
    }

    public static void ClearOwner(Control container)
    {
        if (container is MenuItem item)
        {
            Owners.Remove(item);
        }
    }

    public static bool IsActivationKey(Key key)
    {
        return key is Key.Enter or Key.Space;
    }

    public static bool TryHandleSubMenuKey(MenuItem item, Key key, bool openOnDown)
    {
        if (key is Key.Left or Key.Escape)
        {
            if (item.IsSubMenuOpen)
            {
                item.IsSubMenuOpen = false;
                FocusMenuItem(item);
                return true;
            }

            return TryCloseOwnerSubMenu(item);
        }

        if (!item.HasSubMenu)
        {
            return false;
        }

        if (!IsSubMenuOpenKey(key, openOnDown))
        {
            return false;
        }

        if (!CanActivate(item))
        {
            return true;
        }

        OpenSubMenu(item, focusFirstChild: true);
        return true;
    }

    public static bool RequestPointerSubMenuOpen(MenuItem item, TimeSpan? delay = null)
    {
        CancelOwnerCloseRequest(item);

        if (!item.HasSubMenu || !CanActivate(item))
        {
            return false;
        }

        var state = PointerStates.GetValue(item, static _ => new PointerSubMenuState());
        state.StopCloseTimer();

        if (item.IsSubMenuOpen)
        {
            return false;
        }

        var openDelay = delay ?? PointerSubMenuOpenDelay;
        if (openDelay <= TimeSpan.Zero)
        {
            OpenSubMenu(item, focusFirstChild: false);
            return true;
        }

        state.StartOpenTimer(openDelay, () => OpenSubMenu(item, focusFirstChild: false));
        return true;
    }

    public static bool RequestPointerSubMenuClose(MenuItem item, TimeSpan? delay = null)
    {
        if (!item.HasSubMenu)
        {
            return false;
        }

        var state = PointerStates.GetValue(item, static _ => new PointerSubMenuState());
        state.StopOpenTimer();

        if (!item.IsSubMenuOpen)
        {
            return false;
        }

        var closeDelay = delay ?? PointerSubMenuCloseDelay;
        if (closeDelay <= TimeSpan.Zero)
        {
            item.IsSubMenuOpen = false;
            return true;
        }

        state.StartCloseTimer(closeDelay, () => item.IsSubMenuOpen = false);
        return true;
    }

    public static void CancelPointerSubMenuRequests(MenuItem item)
    {
        if (PointerStates.TryGetValue(item, out var state))
        {
            state.StopTimers();
        }
    }

    public static bool ShouldCloseOnSelect(MenuItem item)
    {
        return !item.HasSubMenu && CanActivate(item);
    }

    public static bool TryCloseOnSelect(MenuItem item)
    {
        if (!ShouldCloseOnSelect(item))
        {
            return false;
        }

        var closed = false;
        CancelPointerSubMenuRequests(item);

        for (var current = item; FindItemsOwner(current) is { } owner;)
        {
            if (owner is MenuItem ownerItem)
            {
                CancelPointerSubMenuRequests(ownerItem);
                if (ownerItem.IsSubMenuOpen)
                {
                    ownerItem.IsSubMenuOpen = false;
                    closed = true;
                }

                if (ReferenceEquals(ownerItem, current))
                {
                    break;
                }

                current = ownerItem;
                continue;
            }

            if (owner is ContextMenu contextMenu)
            {
                if (contextMenu.IsOpen)
                {
                    contextMenu.Close();
                    closed = true;
                }

                break;
            }

            break;
        }

        return closed;
    }

    public static bool TryHandleSiblingNavigationKey(MenuItem item, Key key)
    {
        if (!IsSiblingNavigationKey(key))
        {
            return false;
        }

        if (IsInsideLoadingMenu(item))
        {
            return true;
        }

        var owner = FindItemsOwner(item);
        if (owner is null || owner.ItemsView.Count == 0)
        {
            return false;
        }

        var currentIndex = IndexOf(owner, item);
        var nextIndex = key switch
        {
            Key.Home => FirstNavigableIndex(owner),
            Key.End => LastNavigableIndex(owner),
            Key.Down => NextNavigableIndex(owner, currentIndex, 1),
            Key.Up => NextNavigableIndex(owner, currentIndex, -1),
            _ => -1
        };

        if (nextIndex < 0 || MenuItemAt(owner, nextIndex) is not { } nextItem)
        {
            return true;
        }

        FocusMenuItem(nextItem);
        return true;
    }

    private static bool IsSubMenuOpenKey(Key key, bool openOnDown)
    {
        return key is Key.Right or Key.Enter or Key.Space
               || (openOnDown && key == Key.Down);
    }

    private static bool IsSiblingNavigationKey(Key key)
    {
        return key is Key.Up or Key.Down or Key.Home or Key.End;
    }

    private static bool TryCloseOwnerSubMenu(MenuItem item)
    {
        if (FindItemsOwner(item) is not MenuItem ownerItem || !ownerItem.IsSubMenuOpen)
        {
            return false;
        }

        ownerItem.IsSubMenuOpen = false;
        FocusMenuItem(ownerItem);
        return true;
    }

    private static void CancelOwnerCloseRequest(MenuItem item)
    {
        if (FindItemsOwner(item) is MenuItem ownerItem
            && PointerStates.TryGetValue(ownerItem, out var state))
        {
            state.StopCloseTimer();
        }
    }

    private static void OpenSubMenu(MenuItem item, bool focusFirstChild)
    {
        if (!item.HasSubMenu || !CanActivate(item))
        {
            return;
        }

        item.IsSubMenuOpen = true;

        if (focusFirstChild)
        {
            FocusFirstSubMenuItem(item);
            Dispatcher.UIThread.Post(() =>
            {
                if (item.IsSubMenuOpen)
                {
                    FocusFirstSubMenuItem(item);
                }
            });
        }
    }

    private static void FocusFirstSubMenuItem(MenuItem item)
    {
        var firstIndex = FirstNavigableIndex(item);
        if (firstIndex < 0 || MenuItemAt(item, firstIndex) is not { } child)
        {
            return;
        }

        FocusMenuItem(child);
    }

    private static void FocusMenuItem(MenuItem item)
    {
        if (!item.Focus(NavigationMethod.Directional, KeyModifiers.None))
        {
            item.Focus(NavigationMethod.Tab, KeyModifiers.None);
        }
    }

    private static ItemsControl? FindItemsOwner(MenuItem item)
    {
        if (Owners.TryGetValue(item, out var ownerReference))
        {
            return ownerReference.Owner;
        }

        if (FindRegisteredOwner(item) is { } registeredOwner)
        {
            return registeredOwner;
        }

        if (ItemsControl.ItemsControlFromItemContainer(item) is { } owner)
        {
            return owner;
        }

        for (var parent = item.GetLogicalParent(); parent is not null; parent = parent.GetLogicalParent())
        {
            if (parent is ItemsControl itemsControl)
            {
                return itemsControl;
            }
        }

        return item.GetVisualAncestors().OfType<ItemsControl>().FirstOrDefault();
    }

    private static ItemsControl? FindRegisteredOwner(MenuItem item)
    {
        for (var index = RegisteredOwners.Count - 1; index >= 0; index--)
        {
            if (!RegisteredOwners[index].TryGetTarget(out var owner))
            {
                RegisteredOwners.RemoveAt(index);
                continue;
            }

            if (IndexOf(owner, item) >= 0)
            {
                return owner;
            }
        }

        return null;
    }

    private static int IndexOf(ItemsControl owner, MenuItem item)
    {
        var itemsIndex = 0;
        foreach (var ownerItem in owner.Items)
        {
            if (ReferenceEquals(ownerItem, item))
            {
                return itemsIndex;
            }

            itemsIndex++;
        }

        for (var index = 0; index < owner.ItemsView.Count; index++)
        {
            if (ReferenceEquals(owner.ItemsView[index], item)
                || ReferenceEquals(owner.ContainerFromIndex(index), item))
            {
                return index;
            }
        }

        return -1;
    }

    private static int FirstNavigableIndex(ItemsControl owner)
    {
        for (var index = 0; index < owner.ItemsView.Count; index++)
        {
            if (IsNavigable(MenuItemAt(owner, index)))
            {
                return index;
            }
        }

        return -1;
    }

    private static int LastNavigableIndex(ItemsControl owner)
    {
        for (var index = owner.ItemsView.Count - 1; index >= 0; index--)
        {
            if (IsNavigable(MenuItemAt(owner, index)))
            {
                return index;
            }
        }

        return -1;
    }

    private static int NextNavigableIndex(ItemsControl owner, int currentIndex, int step)
    {
        if (owner.ItemsView.Count == 0)
        {
            return -1;
        }

        if (currentIndex < 0)
        {
            return step > 0 ? FirstNavigableIndex(owner) : LastNavigableIndex(owner);
        }

        for (var offset = 1; offset <= owner.ItemsView.Count; offset++)
        {
            var index = (currentIndex + (offset * step) + owner.ItemsView.Count) % owner.ItemsView.Count;
            if (IsNavigable(MenuItemAt(owner, index)))
            {
                return index;
            }
        }

        return -1;
    }

    private static MenuItem? MenuItemAt(ItemsControl owner, int index)
    {
        if (index < 0 || index >= owner.ItemsView.Count)
        {
            return null;
        }

        var itemsIndex = 0;
        foreach (var ownerItem in owner.Items)
        {
            if (itemsIndex == index)
            {
                return ownerItem as MenuItem;
            }

            itemsIndex++;
        }

        return owner.ItemsView[index] as MenuItem
               ?? owner.ContainerFromIndex(index) as MenuItem;
    }

    private static bool IsNavigable(MenuItem? item)
    {
        return item is { Focusable: true } && CanActivate(item);
    }

    private static bool IsInsideLoadingMenu(MenuItem item)
    {
        for (var parent = item.GetLogicalParent(); parent is not null; parent = parent.GetLogicalParent())
        {
            if (IsLoadingMenu(parent))
            {
                return true;
            }
        }

        if (ItemsControl.ItemsControlFromItemContainer(item) is { } itemOwner && IsLoadingMenu(itemOwner))
        {
            return true;
        }

        for (var current = item; FindItemsOwner(current) is { } owner;)
        {
            if (IsLoadingMenu(owner))
            {
                return true;
            }

            if (owner is not MenuItem ownerItem || ReferenceEquals(ownerItem, current))
            {
                break;
            }

            current = ownerItem;
        }

        return false;
    }

    private static bool IsLoadingMenu(object owner)
    {
        return owner is CodexMenu { IsLoading: true }
               || owner is CodexContextMenu { IsLoading: true }
               || owner is CodexMenubar { IsLoading: true };
    }

    private sealed class OwnerReference(ItemsControl owner)
    {
        public ItemsControl Owner { get; } = owner;
    }

    private sealed class PointerSubMenuState
    {
        private DispatcherTimer? _openTimer;
        private DispatcherTimer? _closeTimer;

        public void StartOpenTimer(TimeSpan delay, Action action)
        {
            _openTimer = RestartTimer(_openTimer, delay, () =>
            {
                _openTimer = null;
                action();
            });
        }

        public void StartCloseTimer(TimeSpan delay, Action action)
        {
            _closeTimer = RestartTimer(_closeTimer, delay, () =>
            {
                _closeTimer = null;
                action();
            });
        }

        public void StopOpenTimer()
        {
            StopTimer(ref _openTimer);
        }

        public void StopCloseTimer()
        {
            StopTimer(ref _closeTimer);
        }

        public void StopTimers()
        {
            StopOpenTimer();
            StopCloseTimer();
        }

        private static DispatcherTimer RestartTimer(DispatcherTimer? current, TimeSpan delay, Action action)
        {
            current?.Stop();

            var timer = new DispatcherTimer
            {
                Interval = delay
            };

            timer.Tick += OnTick;
            timer.Start();
            return timer;

            void OnTick(object? sender, EventArgs e)
            {
                timer.Tick -= OnTick;
                timer.Stop();
                action();
            }
        }

        private static void StopTimer(ref DispatcherTimer? timer)
        {
            timer?.Stop();
            timer = null;
        }
    }
}
