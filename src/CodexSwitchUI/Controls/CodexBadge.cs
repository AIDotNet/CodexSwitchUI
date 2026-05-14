using Avalonia;

namespace CodexSwitchUI.Controls;

public class CodexBadge : CodexFrame
{
    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexBadge, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexBadge, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<CodexControlVariant> StatusVariantProperty =
        AvaloniaProperty.Register<CodexBadge, CodexControlVariant>(nameof(StatusVariant), CodexControlVariant.Success);

    public static readonly StyledProperty<bool> IsStatusVisibleProperty =
        AvaloniaProperty.Register<CodexBadge, bool>(nameof(IsStatusVisible));

    static CodexBadge()
    {
        VariantProperty.Changed.AddClassHandler<CodexBadge>((badge, _) => badge.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexBadge>((badge, _) => badge.SyncClasses());
        StatusVariantProperty.Changed.AddClassHandler<CodexBadge>((badge, _) => badge.SyncClasses());
        IsStatusVisibleProperty.Changed.AddClassHandler<CodexBadge>((badge, _) => badge.SyncClasses());
    }

    public CodexBadge()
    {
        SyncClasses();
    }

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
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

    private void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        SetStatusVariantClasses();
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
