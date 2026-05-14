using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

public class CodexCard : CodexFrame
{
    public static readonly StyledProperty<bool> IsInteractiveProperty =
        AvaloniaProperty.Register<CodexCard, bool>(nameof(IsInteractive));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CodexCard, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexCard, string?>(nameof(Description));

    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<CodexCard, object?>(nameof(Footer));

    public static readonly StyledProperty<bool> HasTitleProperty =
        AvaloniaProperty.Register<CodexCard, bool>(nameof(HasTitle));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexCard, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasHeaderProperty =
        AvaloniaProperty.Register<CodexCard, bool>(nameof(HasHeader));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexCard, bool>(nameof(HasContent));

    public static readonly StyledProperty<bool> HasFooterProperty =
        AvaloniaProperty.Register<CodexCard, bool>(nameof(HasFooter));

    static CodexCard()
    {
        IsInteractiveProperty.Changed.AddClassHandler<CodexCard>((card, _) => card.SyncClasses());
        TitleProperty.Changed.AddClassHandler<CodexCard>((card, _) => card.SyncSlotStates());
        DescriptionProperty.Changed.AddClassHandler<CodexCard>((card, _) => card.SyncSlotStates());
        ContentControl.ContentProperty.Changed.AddClassHandler<CodexCard>((card, _) => card.SyncSlotStates());
        FooterProperty.Changed.AddClassHandler<CodexCard>((card, _) => card.SyncSlotStates());
    }

    public CodexCard()
    {
        SyncClasses();
        SyncSlotStates();
    }

    public bool IsInteractive
    {
        get => GetValue(IsInteractiveProperty);
        set => SetValue(IsInteractiveProperty, value);
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

    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public bool HasTitle => GetValue(HasTitleProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasHeader => GetValue(HasHeaderProperty);

    public bool HasContent => GetValue(HasContentProperty);

    public bool HasFooter => GetValue(HasFooterProperty);

    private void SyncClasses()
    {
        Classes.Set("interactive", IsInteractive);
    }

    private void SyncSlotStates()
    {
        var hasTitle = HasValue(Title);
        var hasDescription = HasValue(Description);

        SetValue(HasTitleProperty, hasTitle);
        SetValue(HasDescriptionProperty, hasDescription);
        SetValue(HasHeaderProperty, hasTitle || hasDescription);
        SetValue(HasContentProperty, HasValue(Content));
        SetValue(HasFooterProperty, HasValue(Footer));
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
