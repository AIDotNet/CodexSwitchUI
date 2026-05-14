using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace CodexSwitchUI.Controls;

public class CodexProviderCard : Button
{
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(IsActive));

    public static readonly StyledProperty<bool> IsDraggingProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(IsDragging));

    public static readonly StyledProperty<object?> LeadingProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Leading));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Icon));

    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Header));

    public static readonly StyledProperty<object?> MetaProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Meta));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexProviderCard, string?>(nameof(Description));

    public static readonly StyledProperty<object?> StatusProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Status));

    public static readonly StyledProperty<object?> UsageProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Usage));

    public static readonly StyledProperty<object?> ActionsProperty =
        AvaloniaProperty.Register<CodexProviderCard, object?>(nameof(Actions));

    public static readonly StyledProperty<bool> HasLeadingProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasLeading));

    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasIcon));

    public static readonly StyledProperty<bool> HasMetaProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasMeta));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasStatusProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasStatus));

    public static readonly StyledProperty<bool> HasUsageProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasUsage));

    public static readonly StyledProperty<bool> HasActionsProperty =
        AvaloniaProperty.Register<CodexProviderCard, bool>(nameof(HasActions));

    static CodexProviderCard()
    {
        IsActiveProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncClasses());
        IsDraggingProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncClasses());
        LeadingProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        IconProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        MetaProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        DescriptionProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        StatusProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        UsageProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
        ActionsProperty.Changed.AddClassHandler<CodexProviderCard>((card, _) => card.SyncSlots());
    }

    public CodexProviderCard()
    {
        SyncClasses();
        SyncSlots();
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public bool IsDragging
    {
        get => GetValue(IsDraggingProperty);
        set => SetValue(IsDraggingProperty, value);
    }

    public object? Leading
    {
        get => GetValue(LeadingProperty);
        set => SetValue(LeadingProperty, value);
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object? Meta
    {
        get => GetValue(MetaProperty);
        set => SetValue(MetaProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public object? Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public object? Usage
    {
        get => GetValue(UsageProperty);
        set => SetValue(UsageProperty, value);
    }

    public object? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    public bool HasLeading => GetValue(HasLeadingProperty);

    public bool HasIcon => GetValue(HasIconProperty);

    public bool HasMeta => GetValue(HasMetaProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasStatus => GetValue(HasStatusProperty);

    public bool HasUsage => GetValue(HasUsageProperty);

    public bool HasActions => GetValue(HasActionsProperty);

    protected override void OnClick()
    {
        base.OnClick();
        SelectSiblingCards();
    }

    private void SelectSiblingCards()
    {
        var parent = this.GetLogicalParent();
        if (parent is null)
        {
            IsActive = true;
            return;
        }

        foreach (var child in parent.GetLogicalChildren())
        {
            if (child is CodexProviderCard card)
            {
                card.IsActive = ReferenceEquals(card, this);
            }
        }
    }

    private void SyncClasses()
    {
        Classes.Set("active", IsActive);
        Classes.Set("dragging", IsDragging);
    }

    private void SyncSlots()
    {
        SetValue(HasLeadingProperty, Leading is not null);
        SetValue(HasIconProperty, Icon is not null);
        SetValue(HasMetaProperty, Meta is not null);
        SetValue(HasDescriptionProperty, !string.IsNullOrWhiteSpace(Description));
        SetValue(HasStatusProperty, Status is not null);
        SetValue(HasUsageProperty, Usage is not null);
        SetValue(HasActionsProperty, Actions is not null);
    }
}
