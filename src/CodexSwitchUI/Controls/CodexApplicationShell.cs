using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

public class CodexSidebar : CodexFrame
{
}

public class CodexSidebarHeader : CodexFrame
{
}

public class CodexSidebarContent : CodexFrame
{
}

public class CodexSidebarFooter : CodexFrame
{
}

public class CodexSidebarGroup : CodexFrame
{
}

public class CodexSidebarGroupLabel : ContentControl
{
}

public class CodexSidebarGroupContent : CodexFrame
{
}

public class CodexSidebarGroupAction : Button
{
}

public class CodexSidebarMenu : ItemsControl
{
}

public class CodexSidebarMenuItem : ContentControl
{
}

public class CodexSidebarMenuButton : Button
{
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexSidebarMenuButton, object?>(nameof(Icon));

    public static readonly StyledProperty<object?> BadgeProperty =
        AvaloniaProperty.Register<CodexSidebarMenuButton, object?>(nameof(Badge));

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexSidebarMenuButton, bool>(nameof(IsActive));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexSidebarMenuButton, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<CodexSidebarMenuButton, bool>(nameof(HasIcon));

    public static readonly StyledProperty<bool> HasBadgeProperty =
        AvaloniaProperty.Register<CodexSidebarMenuButton, bool>(nameof(HasBadge));

    static CodexSidebarMenuButton()
    {
        IconProperty.Changed.AddClassHandler<CodexSidebarMenuButton>((button, _) => button.SyncClasses());
        BadgeProperty.Changed.AddClassHandler<CodexSidebarMenuButton>((button, _) => button.SyncClasses());
        IsActiveProperty.Changed.AddClassHandler<CodexSidebarMenuButton>((button, _) => button.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexSidebarMenuButton>((button, _) => button.SyncClasses());
    }

    public CodexSidebarMenuButton()
    {
        SyncClasses();
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public object? Badge
    {
        get => GetValue(BadgeProperty);
        set => SetValue(BadgeProperty, value);
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool HasIcon => GetValue(HasIconProperty);

    public bool HasBadge => GetValue(HasBadgeProperty);

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("active", IsActive);
        Classes.Set("has-icon", Icon is not null);
        Classes.Set("has-badge", Badge is not null);
        SetValue(HasIconProperty, Icon is not null);
        SetValue(HasBadgeProperty, Badge is not null);
    }
}

public class CodexSidebarMenuAction : Button
{
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexSidebarMenuAction, bool>(nameof(IsActive));

    public static readonly StyledProperty<bool> IsShowOnHoverProperty =
        AvaloniaProperty.Register<CodexSidebarMenuAction, bool>(nameof(IsShowOnHover));

    static CodexSidebarMenuAction()
    {
        IsActiveProperty.Changed.AddClassHandler<CodexSidebarMenuAction>((action, _) => action.SyncClasses());
        IsShowOnHoverProperty.Changed.AddClassHandler<CodexSidebarMenuAction>((action, _) => action.SyncClasses());
    }

    public CodexSidebarMenuAction()
    {
        SyncClasses();
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public bool IsShowOnHover
    {
        get => GetValue(IsShowOnHoverProperty);
        set => SetValue(IsShowOnHoverProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("active", IsActive);
        Classes.Set("show-on-hover", IsShowOnHover);
    }
}

public class CodexSidebarMenuBadge : ContentControl
{
}

public class CodexSidebarMenuSub : ItemsControl
{
}

public class CodexSidebarMenuSubItem : ContentControl
{
}

public class CodexSidebarMenuSubButton : Button
{
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexSidebarMenuSubButton, bool>(nameof(IsActive));

    static CodexSidebarMenuSubButton()
    {
        IsActiveProperty.Changed.AddClassHandler<CodexSidebarMenuSubButton>((button, _) => button.SyncClasses());
    }

    public CodexSidebarMenuSubButton()
    {
        SyncClasses();
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("active", IsActive);
    }
}

public class CodexSection : CodexFrame
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CodexSection, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexSection, string?>(nameof(Description));

    public static readonly StyledProperty<object?> ActionsProperty =
        AvaloniaProperty.Register<CodexSection, object?>(nameof(Actions));

    public static readonly StyledProperty<bool> HasTitleProperty =
        AvaloniaProperty.Register<CodexSection, bool>(nameof(HasTitle));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexSection, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasActionsProperty =
        AvaloniaProperty.Register<CodexSection, bool>(nameof(HasActions));

    static CodexSection()
    {
        TitleProperty.Changed.AddClassHandler<CodexSection>((section, _) => section.SyncSlots());
        DescriptionProperty.Changed.AddClassHandler<CodexSection>((section, _) => section.SyncSlots());
        ActionsProperty.Changed.AddClassHandler<CodexSection>((section, _) => section.SyncSlots());
    }

    public CodexSection()
    {
        SyncSlots();
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

    public object? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    public bool HasTitle => GetValue(HasTitleProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasActions => GetValue(HasActionsProperty);

    private void SyncSlots()
    {
        SetValue(HasTitleProperty, HasText(Title));
        SetValue(HasDescriptionProperty, HasText(Description));
        SetValue(HasActionsProperty, HasValue(Actions));
    }

    private static bool HasText(string? value) => !string.IsNullOrWhiteSpace(value);

    private static bool HasValue(object? value) => value is not null;
}

public class CodexField : CodexFrame
{
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<CodexField, string?>(nameof(Label));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexField, string?>(nameof(Description));

    public static readonly StyledProperty<bool> HasLabelProperty =
        AvaloniaProperty.Register<CodexField, bool>(nameof(HasLabel));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexField, bool>(nameof(HasDescription));

    static CodexField()
    {
        LabelProperty.Changed.AddClassHandler<CodexField>((field, _) => field.SyncSlots());
        DescriptionProperty.Changed.AddClassHandler<CodexField>((field, _) => field.SyncSlots());
    }

    public CodexField()
    {
        SyncSlots();
    }

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public bool HasLabel => GetValue(HasLabelProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    private void SyncSlots()
    {
        SetValue(HasLabelProperty, !string.IsNullOrWhiteSpace(Label));
        SetValue(HasDescriptionProperty, !string.IsNullOrWhiteSpace(Description));
    }
}

public class CodexKbd : ContentControl
{
}
