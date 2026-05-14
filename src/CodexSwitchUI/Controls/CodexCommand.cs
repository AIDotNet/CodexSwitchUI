using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

public class CodexCommand : CodexFrame
{
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<CodexCommand, string?>(nameof(Placeholder), "Type a command...");

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexCommand, bool>(nameof(IsLoading));

    static CodexCommand()
    {
        IsLoadingProperty.Changed.AddClassHandler<CodexCommand>((command, _) => command.SyncClasses());
    }

    public CodexCommand()
    {
        SyncClasses();
    }

    public string? Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("loading", IsLoading);
    }
}

public class CodexCommandInput : TextBox
{
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

public class CodexCommandItem : ContentControl
{
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexCommandItem, bool>(nameof(IsActive));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexCommandItem, object?>(nameof(Icon));

    public static readonly StyledProperty<string?> ShortcutProperty =
        AvaloniaProperty.Register<CodexCommandItem, string?>(nameof(Shortcut));

    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<CodexCommandItem, bool>(nameof(HasIcon));

    public static readonly StyledProperty<bool> HasShortcutProperty =
        AvaloniaProperty.Register<CodexCommandItem, bool>(nameof(HasShortcut));

    static CodexCommandItem()
    {
        IsActiveProperty.Changed.AddClassHandler<CodexCommandItem>((item, _) => item.SyncClasses());
        IconProperty.Changed.AddClassHandler<CodexCommandItem>((item, _) => item.SyncClasses());
        ShortcutProperty.Changed.AddClassHandler<CodexCommandItem>((item, _) => item.SyncClasses());
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

    public bool HasIcon => GetValue(HasIconProperty);

    public bool HasShortcut => GetValue(HasShortcutProperty);

    private void SyncClasses()
    {
        Classes.Set("active", IsActive);
        Classes.Set("has-icon", Icon is not null);
        Classes.Set("has-shortcut", !string.IsNullOrWhiteSpace(Shortcut));
        SetValue(HasIconProperty, Icon is not null);
        SetValue(HasShortcutProperty, !string.IsNullOrWhiteSpace(Shortcut));
    }
}

public class CodexCommandEmpty : CodexFrame
{
}

public class CodexCommandLoading : CodexFrame
{
}
