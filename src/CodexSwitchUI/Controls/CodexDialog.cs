using Avalonia;
using Avalonia.Controls;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public class CodexDialog : CodexFrame
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CodexDialog, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexDialog, string?>(nameof(Description));

    public static readonly StyledProperty<object?> ActionProperty =
        AvaloniaProperty.Register<CodexDialog, object?>(nameof(Action));

    public static readonly StyledProperty<object?> CloseContentProperty =
        AvaloniaProperty.Register<CodexDialog, object?>(nameof(CloseContent));

    public static readonly StyledProperty<ICommand?> CloseCommandProperty =
        AvaloniaProperty.Register<CodexDialog, ICommand?>(nameof(CloseCommand));

    public static readonly StyledProperty<bool> IsCloseVisibleProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(IsCloseVisible), true);

    public static readonly StyledProperty<bool> HasTitleProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(HasTitle));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasHeaderProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(HasHeader));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(HasContent));

    public static readonly StyledProperty<bool> HasActionProperty =
        AvaloniaProperty.Register<CodexDialog, bool>(nameof(HasAction));

    static CodexDialog()
    {
        TitleProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
        DescriptionProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
        ContentControl.ContentProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
        ActionProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
        CloseContentProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
        IsCloseVisibleProperty.Changed.AddClassHandler<CodexDialog>((dialog, _) => dialog.SyncSlotStates());
    }

    public CodexDialog()
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
