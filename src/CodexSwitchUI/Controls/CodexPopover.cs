using Avalonia;
using Avalonia.Controls;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public class CodexPopover : CodexFrame
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CodexPopover, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexPopover, string?>(nameof(Description));

    public static readonly StyledProperty<object?> ActionProperty =
        AvaloniaProperty.Register<CodexPopover, object?>(nameof(Action));

    public static readonly StyledProperty<object?> CloseContentProperty =
        AvaloniaProperty.Register<CodexPopover, object?>(nameof(CloseContent));

    public static readonly StyledProperty<ICommand?> CloseCommandProperty =
        AvaloniaProperty.Register<CodexPopover, ICommand?>(nameof(CloseCommand));

    public static readonly StyledProperty<bool> IsCloseVisibleProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(IsCloseVisible), true);

    public static readonly StyledProperty<bool> HasTitleProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(HasTitle));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasHeaderProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(HasHeader));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(HasContent));

    public static readonly StyledProperty<bool> HasActionProperty =
        AvaloniaProperty.Register<CodexPopover, bool>(nameof(HasAction));

    static CodexPopover()
    {
        TitleProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
        DescriptionProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
        ContentControl.ContentProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
        ActionProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
        CloseContentProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
        IsCloseVisibleProperty.Changed.AddClassHandler<CodexPopover>((popover, _) => popover.SyncSlotStates());
    }

    public CodexPopover()
    {
        SyncSlotStates();
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public object? Action
    {
        get => GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }

    public object? CloseContent
    {
        get => GetValue(CloseContentProperty);
        set => SetValue(CloseContentProperty, value);
    }

    public ICommand? CloseCommand
    {
        get => GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    public bool IsCloseVisible
    {
        get => GetValue(IsCloseVisibleProperty);
        set => SetValue(IsCloseVisibleProperty, value);
    }

    public bool HasTitle => GetValue(HasTitleProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasHeader => GetValue(HasHeaderProperty);

    public bool HasContent => GetValue(HasContentProperty);

    public bool HasAction => GetValue(HasActionProperty);

    private void SyncSlotStates()
    {
        var hasTitle = HasValue(Title);
        var hasDescription = HasValue(Description);

        SetValue(HasTitleProperty, hasTitle);
        SetValue(HasDescriptionProperty, hasDescription);
        SetValue(HasHeaderProperty, hasTitle || hasDescription);
        SetValue(HasContentProperty, HasValue(Content));
        SetValue(HasActionProperty, HasValue(Action));
        Classes.Set("has-title", hasTitle);
        Classes.Set("has-description", hasDescription);
        Classes.Set("has-header", HasHeader);
        Classes.Set("has-content", HasContent);
        Classes.Set("has-action", HasAction);
        Classes.Set("has-close", IsCloseVisible);
        Classes.Set("has-close-content", HasValue(CloseContent));
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
