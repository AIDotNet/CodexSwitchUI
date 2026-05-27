using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public class CodexToast : CodexFrame
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CodexToast, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexToast, string?>(nameof(Description));

    public static readonly StyledProperty<object?> ActionProperty =
        AvaloniaProperty.Register<CodexToast, object?>(nameof(Action));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexToast, object?>(nameof(Icon));

    public static readonly StyledProperty<object?> CloseContentProperty =
        AvaloniaProperty.Register<CodexToast, object?>(nameof(CloseContent));

    public static readonly StyledProperty<ICommand?> CloseCommandProperty =
        AvaloniaProperty.Register<CodexToast, ICommand?>(nameof(CloseCommand));

    public static readonly StyledProperty<ICommand?> DismissCommandProperty =
        AvaloniaProperty.Register<CodexToast, ICommand?>(nameof(DismissCommand));

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexToast, bool>(nameof(IsOpen), true);

    public static readonly StyledProperty<bool> IsCloseVisibleProperty =
        AvaloniaProperty.Register<CodexToast, bool>(nameof(IsCloseVisible), true);

    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<CodexToast, bool>(nameof(CloseOnEscape), true);

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexToast, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<bool> HasTitleProperty =
        AvaloniaProperty.Register<CodexToast, bool>(nameof(HasTitle));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexToast, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasHeaderProperty =
        AvaloniaProperty.Register<CodexToast, bool>(nameof(HasHeader));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexToast, bool>(nameof(HasContent));

    public static readonly StyledProperty<bool> HasActionProperty =
        AvaloniaProperty.Register<CodexToast, bool>(nameof(HasAction));

    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<CodexToast, bool>(nameof(HasIcon));

    static CodexToast()
    {
        VariantProperty.Changed.AddClassHandler<CodexToast>((toast, _) => toast.SyncClasses());
        TitleProperty.Changed.AddClassHandler<CodexToast>((toast, _) => toast.SyncSlotStates());
        DescriptionProperty.Changed.AddClassHandler<CodexToast>((toast, _) => toast.SyncSlotStates());
        ContentControl.ContentProperty.Changed.AddClassHandler<CodexToast>((toast, _) => toast.SyncSlotStates());
        ActionProperty.Changed.AddClassHandler<CodexToast>((toast, _) => toast.SyncSlotStates());
        IconProperty.Changed.AddClassHandler<CodexToast>((toast, _) => toast.SyncSlotStates());
        CloseContentProperty.Changed.AddClassHandler<CodexToast>((toast, _) => toast.SyncSlotStates());
        IsOpenProperty.Changed.AddClassHandler<CodexToast>((toast, _) => toast.SyncOpenState());
        IsCloseVisibleProperty.Changed.AddClassHandler<CodexToast>((toast, _) => toast.SyncSlotStates());
    }

    public CodexToast()
    {
        DismissCommand = new CodexDismissCommand(Dismiss);
        SyncClasses();
        SyncOpenState();
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

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
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

    public ICommand? DismissCommand
    {
        get => GetValue(DismissCommandProperty);
        private set => SetValue(DismissCommandProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool IsCloseVisible
    {
        get => GetValue(IsCloseVisibleProperty);
        set => SetValue(IsCloseVisibleProperty, value);
    }

    public bool CloseOnEscape
    {
        get => GetValue(CloseOnEscapeProperty);
        set => SetValue(CloseOnEscapeProperty, value);
    }

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public bool HasTitle => GetValue(HasTitleProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasHeader => GetValue(HasHeaderProperty);

    public bool HasContent => GetValue(HasContentProperty);

    public bool HasAction => GetValue(HasActionProperty);

    public bool HasIcon => GetValue(HasIconProperty);

    public bool Dismiss()
    {
        if (!IsOpen)
        {
            return false;
        }

        IsOpen = false;

        if (CloseCommand?.CanExecute(null) == true)
        {
            CloseCommand.Execute(null);
        }

        return true;
    }

    internal bool TryHandleDismissKey(Key key)
    {
        return key == Key.Escape && CloseOnEscape && Dismiss();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (TryHandleDismissKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    private void SyncSlotStates()
    {
        var hasTitle = HasValue(Title);
        var hasDescription = HasValue(Description);
        var hasIcon = HasValue(Icon);

        SetValue(HasTitleProperty, hasTitle);
        SetValue(HasDescriptionProperty, hasDescription);
        SetValue(HasHeaderProperty, hasTitle || hasDescription);
        SetValue(HasContentProperty, HasValue(Content));
        SetValue(HasActionProperty, HasValue(Action));
        SetValue(HasIconProperty, hasIcon);
        Classes.Set("has-title", hasTitle);
        Classes.Set("has-description", hasDescription);
        Classes.Set("has-header", HasHeader);
        Classes.Set("has-content", HasContent);
        Classes.Set("has-action", HasAction);
        Classes.Set("has-icon", hasIcon);
        Classes.Set("has-close", IsCloseVisible);
        Classes.Set("has-close-content", HasValue(CloseContent));
    }

    private void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
    }

    private void SyncOpenState()
    {
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
