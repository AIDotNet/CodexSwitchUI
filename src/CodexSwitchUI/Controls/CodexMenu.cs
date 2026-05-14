using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

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
        SyncClasses();
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

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("loading", IsLoading);
    }
}

public class CodexMenuItem : MenuItem
{
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
        SyncClasses();
    }

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
