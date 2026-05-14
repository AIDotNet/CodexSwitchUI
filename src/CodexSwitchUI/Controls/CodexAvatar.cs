using Avalonia;
using Avalonia.Media;

namespace CodexSwitchUI.Controls;

public class CodexAvatar : CodexFrame
{
    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexAvatar, CodexControlVariant>(nameof(Variant), CodexControlVariant.Secondary);

    public static readonly StyledProperty<string?> FallbackProperty =
        AvaloniaProperty.Register<CodexAvatar, string?>(nameof(Fallback));

    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<CodexAvatar, IImage?>(nameof(Source));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexAvatar, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<CodexControlVariant> StatusVariantProperty =
        AvaloniaProperty.Register<CodexAvatar, CodexControlVariant>(nameof(StatusVariant), CodexControlVariant.Success);

    public static readonly StyledProperty<bool> IsStatusVisibleProperty =
        AvaloniaProperty.Register<CodexAvatar, bool>(nameof(IsStatusVisible));

    public static readonly StyledProperty<bool> HasImageProperty =
        AvaloniaProperty.Register<CodexAvatar, bool>(nameof(HasImage));

    public static readonly StyledProperty<bool> HasFallbackProperty =
        AvaloniaProperty.Register<CodexAvatar, bool>(nameof(HasFallback));

    static CodexAvatar()
    {
        VariantProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.SyncClasses());
        FallbackProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.SyncClasses());
        SourceProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.SyncClasses());
        StatusVariantProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.SyncClasses());
        IsStatusVisibleProperty.Changed.AddClassHandler<CodexAvatar>((avatar, _) => avatar.SyncClasses());
    }

    public CodexAvatar()
    {
        SyncClasses();
    }

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public string? Fallback
    {
        get => GetValue(FallbackProperty);
        set => SetValue(FallbackProperty, value);
    }

    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public CodexControlVariant StatusVariant
    {
        get => GetValue(StatusVariantProperty);
        set => SetValue(StatusVariantProperty, value);
    }

    public bool IsStatusVisible
    {
        get => GetValue(IsStatusVisibleProperty);
        set => SetValue(IsStatusVisibleProperty, value);
    }

    public bool HasImage => GetValue(HasImageProperty);

    public bool HasFallback => GetValue(HasFallbackProperty);

    private void SyncClasses()
    {
        var hasImage = Source is not null;
        var hasFallback = !string.IsNullOrWhiteSpace(Fallback);

        SetValue(HasImageProperty, hasImage);
        SetValue(HasFallbackProperty, hasFallback);
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        SetStatusVariantClasses();
        Classes.Set("has-image", hasImage);
        Classes.Set("has-fallback", hasFallback);
        Classes.Set("status-visible", IsStatusVisible);
    }

    private void SetStatusVariantClasses()
    {
        Classes.Set("status-default", StatusVariant == CodexControlVariant.Default);
        Classes.Set("status-secondary", StatusVariant == CodexControlVariant.Secondary);
        Classes.Set("status-destructive", StatusVariant == CodexControlVariant.Destructive);
        Classes.Set("status-outline", StatusVariant == CodexControlVariant.Outline);
        Classes.Set("status-ghost", StatusVariant == CodexControlVariant.Ghost);
        Classes.Set("status-link", StatusVariant == CodexControlVariant.Link);
        Classes.Set("status-success", StatusVariant == CodexControlVariant.Success);
        Classes.Set("status-warning", StatusVariant == CodexControlVariant.Warning);
    }
}
