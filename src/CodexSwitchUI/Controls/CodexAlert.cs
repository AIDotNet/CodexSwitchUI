using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

public class CodexAlert : CodexFrame
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CodexAlert, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexAlert, string?>(nameof(Description));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexAlert, object?>(nameof(Icon));

    public static readonly StyledProperty<object?> ActionProperty =
        AvaloniaProperty.Register<CodexAlert, object?>(nameof(Action));

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexAlert, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<bool> HasTitleProperty =
        AvaloniaProperty.Register<CodexAlert, bool>(nameof(HasTitle));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexAlert, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<CodexAlert, bool>(nameof(HasIcon));

    public static readonly StyledProperty<bool> HasActionProperty =
        AvaloniaProperty.Register<CodexAlert, bool>(nameof(HasAction));

    static CodexAlert()
    {
        VariantProperty.Changed.AddClassHandler<CodexAlert>((alert, _) => alert.SyncClasses());
        TitleProperty.Changed.AddClassHandler<CodexAlert>((alert, _) => alert.SyncSlotStates());
        DescriptionProperty.Changed.AddClassHandler<CodexAlert>((alert, _) => alert.SyncSlotStates());
        IconProperty.Changed.AddClassHandler<CodexAlert>((alert, _) => alert.SyncSlotStates());
        ContentControl.ContentProperty.Changed.AddClassHandler<CodexAlert>((alert, _) => alert.SyncSlotStates());
        ActionProperty.Changed.AddClassHandler<CodexAlert>((alert, _) => alert.SyncSlotStates());
    }

    public CodexAlert()
    {
        SyncClasses();
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

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public object? Action
    {
        get => GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public bool HasTitle => GetValue(HasTitleProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasIcon => GetValue(HasIconProperty);

    public bool HasAction => GetValue(HasActionProperty);

    private void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
    }

    private void SyncSlotStates()
    {
        var hasTitle = HasValue(Title);
        var hasDescription = HasValue(Description);
        var hasIcon = HasValue(Icon);
        var hasAction = HasValue(Action);

        SetValue(HasTitleProperty, hasTitle);
        SetValue(HasDescriptionProperty, hasDescription);
        SetValue(HasIconProperty, hasIcon);
        SetValue(HasActionProperty, hasAction);
        Classes.Set("has-title", hasTitle);
        Classes.Set("has-description", hasDescription);
        Classes.Set("has-icon", hasIcon);
        Classes.Set("has-action", hasAction);
        Classes.Set("has-content", HasValue(Content));
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
