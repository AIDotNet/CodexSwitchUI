using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public enum CodexSidebarSide
{
    Left,
    Right
}

public enum CodexSidebarVariant
{
    Sidebar,
    Floating,
    Inset
}

public enum CodexSidebarCollapsible
{
    Offcanvas,
    Icon,
    None
}

public sealed class CodexSidebarOpenChangedEventArgs(bool isOpen) : EventArgs
{
    public bool IsOpen { get; } = isOpen;

    public bool IsCollapsed => !IsOpen;
}

public class CodexSidebarProvider : CodexFrame
{
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexSidebarProvider, bool>(nameof(IsOpen), true);

    public static readonly StyledProperty<bool> IsMobileOpenProperty =
        AvaloniaProperty.Register<CodexSidebarProvider, bool>(nameof(IsMobileOpen));

    public static readonly StyledProperty<Key> KeyboardShortcutProperty =
        AvaloniaProperty.Register<CodexSidebarProvider, Key>(nameof(KeyboardShortcut), Key.B);

    public static readonly StyledProperty<KeyModifiers> ShortcutModifiersProperty =
        AvaloniaProperty.Register<CodexSidebarProvider, KeyModifiers>(nameof(ShortcutModifiers), KeyModifiers.Control);

    static CodexSidebarProvider()
    {
        IsOpenProperty.Changed.AddClassHandler<CodexSidebarProvider>((provider, args) => provider.OnOpenChanged(args));
        IsMobileOpenProperty.Changed.AddClassHandler<CodexSidebarProvider>((provider, _) => provider.SyncClasses());
        KeyboardShortcutProperty.Changed.AddClassHandler<CodexSidebarProvider>((provider, _) => provider.SyncClasses());
        ShortcutModifiersProperty.Changed.AddClassHandler<CodexSidebarProvider>((provider, _) => provider.SyncClasses());
    }

    public CodexSidebarProvider()
    {
        Focusable = true;
        SyncClasses();
    }

    public event EventHandler<CodexSidebarOpenChangedEventArgs>? OpenChanged;

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool IsMobileOpen
    {
        get => GetValue(IsMobileOpenProperty);
        set => SetValue(IsMobileOpenProperty, value);
    }

    public Key KeyboardShortcut
    {
        get => GetValue(KeyboardShortcutProperty);
        set => SetValue(KeyboardShortcutProperty, value);
    }

    public KeyModifiers ShortcutModifiers
    {
        get => GetValue(ShortcutModifiersProperty);
        set => SetValue(ShortcutModifiersProperty, value);
    }

    public void Open() => IsOpen = true;

    public void Close() => IsOpen = false;

    public void ToggleOpen() => IsOpen = !IsOpen;

    public bool TryHandleShortcut(Key key, KeyModifiers modifiers)
    {
        if (key != KeyboardShortcut || !MatchesShortcutModifiers(modifiers))
        {
            return false;
        }

        ToggleOpen();
        return true;
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SyncDescendantState();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (!e.Handled && TryHandleShortcut(e.Key, e.KeyModifiers))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    private void OnOpenChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncClasses();
        SyncDescendantState();

        if (args.OldValue is bool oldValue && oldValue != IsOpen)
        {
            OpenChanged?.Invoke(this, new CodexSidebarOpenChangedEventArgs(IsOpen));
        }
    }

    private void SyncClasses()
    {
        Classes.Set("sidebar-provider", true);
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("state-expanded", IsOpen);
        Classes.Set("state-collapsed", !IsOpen);
        Classes.Set("mobile-open", IsMobileOpen);
        Classes.Set("mobile-closed", !IsMobileOpen);
        Classes.Set("has-shortcut", KeyboardShortcut != Key.None);
    }

    private bool MatchesShortcutModifiers(KeyModifiers modifiers)
    {
        if (ShortcutModifiers == KeyModifiers.Control)
        {
            return (modifiers & KeyModifiers.Control) == KeyModifiers.Control
                   || (modifiers & KeyModifiers.Meta) == KeyModifiers.Meta;
        }

        return (modifiers & ShortcutModifiers) == ShortcutModifiers;
    }

    internal void SyncDescendantState()
    {
        foreach (var sidebar in this.GetLogicalDescendants().OfType<CodexSidebar>())
        {
            sidebar.ApplyProviderState(this);
        }

        var state = ResolveState();
        foreach (var trigger in this.GetLogicalDescendants().OfType<CodexSidebarTrigger>())
        {
            trigger.SyncSidebarState(state);
        }

        foreach (var rail in this.GetLogicalDescendants().OfType<CodexSidebarRail>())
        {
            rail.SyncSidebarState(state);
        }

        foreach (var inset in this.GetLogicalDescendants().OfType<CodexSidebarInset>())
        {
            inset.SyncSidebarState(state);
        }
    }

    internal CodexSidebarState ResolveState()
    {
        var sidebar = this.GetLogicalDescendants().OfType<CodexSidebar>().FirstOrDefault();
        return sidebar is null
            ? new CodexSidebarState(IsOpen, CodexSidebarCollapsible.Offcanvas, CodexSidebarVariant.Sidebar, CodexSidebarSide.Left)
            : CodexSidebarState.FromSidebar(sidebar);
    }
}

public class CodexSidebar : CodexFrame
{
    public static readonly StyledProperty<CodexSidebarSide> SideProperty =
        AvaloniaProperty.Register<CodexSidebar, CodexSidebarSide>(nameof(Side), CodexSidebarSide.Left);

    public static readonly StyledProperty<CodexSidebarVariant> VariantProperty =
        AvaloniaProperty.Register<CodexSidebar, CodexSidebarVariant>(nameof(Variant), CodexSidebarVariant.Sidebar);

    public static readonly StyledProperty<CodexSidebarCollapsible> CollapsibleProperty =
        AvaloniaProperty.Register<CodexSidebar, CodexSidebarCollapsible>(nameof(Collapsible), CodexSidebarCollapsible.Offcanvas);

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexSidebar, bool>(nameof(IsOpen), true);

    static CodexSidebar()
    {
        SideProperty.Changed.AddClassHandler<CodexSidebar>((sidebar, _) => sidebar.SyncClasses());
        VariantProperty.Changed.AddClassHandler<CodexSidebar>((sidebar, _) => sidebar.SyncClasses());
        CollapsibleProperty.Changed.AddClassHandler<CodexSidebar>((sidebar, _) => sidebar.SyncClasses());
        IsOpenProperty.Changed.AddClassHandler<CodexSidebar>((sidebar, args) => sidebar.OnOpenChanged(args));
    }

    public CodexSidebar()
    {
        SyncClasses();
    }

    public event EventHandler<CodexSidebarOpenChangedEventArgs>? OpenChanged;

    public CodexSidebarSide Side
    {
        get => GetValue(SideProperty);
        set => SetValue(SideProperty, value);
    }

    public CodexSidebarVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public CodexSidebarCollapsible Collapsible
    {
        get => GetValue(CollapsibleProperty);
        set => SetValue(CollapsibleProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool IsCollapsed => Collapsible != CodexSidebarCollapsible.None && !IsOpen;

    public void Open() => IsOpen = true;

    public void Close() => IsOpen = false;

    public void ToggleOpen() => IsOpen = !IsOpen;

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);

        var provider = this.GetLogicalAncestors().OfType<CodexSidebarProvider>().FirstOrDefault();
        provider?.SyncDescendantState();
    }

    internal void ApplyProviderState(CodexSidebarProvider provider)
    {
        if (IsOpen != provider.IsOpen)
        {
            SetCurrentValue(IsOpenProperty, provider.IsOpen);
        }
        else
        {
            SyncClasses();
        }
    }

    private void OnOpenChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncClasses();

        if (args.OldValue is bool oldValue && oldValue != IsOpen)
        {
            OpenChanged?.Invoke(this, new CodexSidebarOpenChangedEventArgs(IsOpen));
        }

        this.GetLogicalAncestors().OfType<CodexSidebarProvider>().FirstOrDefault()?.SyncDescendantState();
    }

    private void SyncClasses()
    {
        var open = Collapsible == CodexSidebarCollapsible.None || IsOpen;
        var collapsed = !open;

        Classes.Set("sidebar", true);
        Classes.Set("open", open);
        Classes.Set("closed", collapsed);
        Classes.Set("state-expanded", open);
        Classes.Set("state-collapsed", collapsed);
        Classes.Set("expanded", open);
        Classes.Set("collapsed", collapsed);
        Classes.Set("side-left", Side == CodexSidebarSide.Left);
        Classes.Set("side-right", Side == CodexSidebarSide.Right);
        Classes.Set("variant-sidebar", Variant == CodexSidebarVariant.Sidebar);
        Classes.Set("variant-floating", Variant == CodexSidebarVariant.Floating);
        Classes.Set("variant-inset", Variant == CodexSidebarVariant.Inset);
        Classes.Set("collapsible-offcanvas", Collapsible == CodexSidebarCollapsible.Offcanvas);
        Classes.Set("collapsible-icon", Collapsible == CodexSidebarCollapsible.Icon);
        Classes.Set("collapsible-none", Collapsible == CodexSidebarCollapsible.None);
        Classes.Set("offcanvas", collapsed && Collapsible == CodexSidebarCollapsible.Offcanvas);
        Classes.Set("icon", collapsed && Collapsible == CodexSidebarCollapsible.Icon);
        Classes.Set("non-collapsible", Collapsible == CodexSidebarCollapsible.None);
    }
}

public class CodexSidebarTrigger : CodexButton
{
    private ICommand? _subscribedCommand;

    public static readonly StyledProperty<CodexSidebarProvider?> TargetProviderProperty =
        AvaloniaProperty.Register<CodexSidebarTrigger, CodexSidebarProvider?>(nameof(TargetProvider));

    public static readonly StyledProperty<CodexSidebar?> TargetSidebarProperty =
        AvaloniaProperty.Register<CodexSidebarTrigger, CodexSidebar?>(nameof(TargetSidebar));

    static CodexSidebarTrigger()
    {
        TargetProviderProperty.Changed.AddClassHandler<CodexSidebarTrigger>((trigger, _) => trigger.SyncResolvedState());
        TargetSidebarProperty.Changed.AddClassHandler<CodexSidebarTrigger>((trigger, _) => trigger.SyncResolvedState());
        CommandProperty.Changed.AddClassHandler<CodexSidebarTrigger>((trigger, args) => trigger.OnCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        CommandParameterProperty.Changed.AddClassHandler<CodexSidebarTrigger>((trigger, _) => trigger.SyncToggleClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexSidebarTrigger>((trigger, _) => trigger.SyncToggleClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexSidebarTrigger>((trigger, _) => trigger.SyncToggleClasses());
    }

    public CodexSidebarTrigger()
    {
        Size = CodexControlSize.Small;
        Variant = CodexControlVariant.Ghost;
        SyncResolvedState();
    }

    public CodexSidebarProvider? TargetProvider
    {
        get => GetValue(TargetProviderProperty);
        set => SetValue(TargetProviderProperty, value);
    }

    public CodexSidebar? TargetSidebar
    {
        get => GetValue(TargetSidebarProperty);
        set => SetValue(TargetSidebarProperty, value);
    }

    internal bool CanToggle => IsEnabled
                               && !IsLoading
                               && (Command?.CanExecute(CommandParameter) ?? true);

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SyncResolvedState();
    }

    protected override void OnClick()
    {
        if (!CanToggle)
        {
            return;
        }

        if (ResolveProvider() is { } provider)
        {
            provider.ToggleOpen();
            SyncSidebarState(provider.ResolveState());
        }
        else if (ResolveSidebar() is { } sidebar)
        {
            sidebar.ToggleOpen();
            SyncSidebarState(CodexSidebarState.FromSidebar(sidebar));
        }

        base.OnClick();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
            _subscribedCommand = null;
        }

        base.OnDetachedFromVisualTree(e);
    }

    internal void SyncSidebarState(CodexSidebarState state)
    {
        CodexSidebarClassSync.Apply(Classes, state);
        SyncToggleClasses();
    }

    private void SyncResolvedState()
    {
        if (ResolveProvider() is { } provider)
        {
            SyncSidebarState(provider.ResolveState());
            return;
        }

        if (ResolveSidebar() is { } sidebar)
        {
            SyncSidebarState(CodexSidebarState.FromSidebar(sidebar));
            return;
        }

        SyncSidebarState(new CodexSidebarState(true, CodexSidebarCollapsible.Offcanvas, CodexSidebarVariant.Sidebar, CodexSidebarSide.Left));
    }

    private CodexSidebarProvider? ResolveProvider()
    {
        return TargetProvider ?? this.GetLogicalAncestors().OfType<CodexSidebarProvider>().FirstOrDefault();
    }

    private CodexSidebar? ResolveSidebar()
    {
        return TargetSidebar
            ?? this.GetLogicalAncestors().OfType<CodexSidebar>().FirstOrDefault()
            ?? ResolveProvider()?.GetLogicalDescendants().OfType<CodexSidebar>().FirstOrDefault();
    }

    private void SyncToggleClasses()
    {
        Classes.Set("can-toggle", CanToggle);
        Classes.Set("command-blocked", Command is not null && IsEnabled && !IsLoading && !CanToggle);
    }

    private void OnCommandChanged(ICommand? oldCommand, ICommand? newCommand)
    {
        if (ReferenceEquals(oldCommand, newCommand))
        {
            return;
        }

        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
        }

        _subscribedCommand = newCommand;

        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged += OnCommandCanExecuteChanged;
        }

        SyncToggleClasses();
    }

    private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
    {
        SyncToggleClasses();
    }
}

public class CodexSidebarRail : Button
{
    private ICommand? _subscribedCommand;

    public static readonly StyledProperty<CodexSidebarProvider?> TargetProviderProperty =
        AvaloniaProperty.Register<CodexSidebarRail, CodexSidebarProvider?>(nameof(TargetProvider));

    public static readonly StyledProperty<CodexSidebar?> TargetSidebarProperty =
        AvaloniaProperty.Register<CodexSidebarRail, CodexSidebar?>(nameof(TargetSidebar));

    static CodexSidebarRail()
    {
        TargetProviderProperty.Changed.AddClassHandler<CodexSidebarRail>((rail, _) => rail.SyncResolvedState());
        TargetSidebarProperty.Changed.AddClassHandler<CodexSidebarRail>((rail, _) => rail.SyncResolvedState());
        CommandProperty.Changed.AddClassHandler<CodexSidebarRail>((rail, args) => rail.OnCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        CommandParameterProperty.Changed.AddClassHandler<CodexSidebarRail>((rail, _) => rail.SyncToggleClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexSidebarRail>((rail, _) => rail.SyncToggleClasses());
    }

    public CodexSidebarRail()
    {
        SyncResolvedState();
    }

    public CodexSidebarProvider? TargetProvider
    {
        get => GetValue(TargetProviderProperty);
        set => SetValue(TargetProviderProperty, value);
    }

    public CodexSidebar? TargetSidebar
    {
        get => GetValue(TargetSidebarProperty);
        set => SetValue(TargetSidebarProperty, value);
    }

    internal bool CanToggle => IsEnabled && (Command?.CanExecute(CommandParameter) ?? true);

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SyncResolvedState();
    }

    protected override void OnClick()
    {
        if (!CanToggle)
        {
            return;
        }

        if (ResolveProvider() is { } provider)
        {
            provider.ToggleOpen();
            SyncSidebarState(provider.ResolveState());
        }
        else if (ResolveSidebar() is { } sidebar)
        {
            sidebar.ToggleOpen();
            SyncSidebarState(CodexSidebarState.FromSidebar(sidebar));
        }

        base.OnClick();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
            _subscribedCommand = null;
        }

        base.OnDetachedFromVisualTree(e);
    }

    internal void SyncSidebarState(CodexSidebarState state)
    {
        CodexSidebarClassSync.Apply(Classes, state);
        SyncToggleClasses();
    }

    private void SyncResolvedState()
    {
        if (ResolveProvider() is { } provider)
        {
            SyncSidebarState(provider.ResolveState());
            return;
        }

        if (ResolveSidebar() is { } sidebar)
        {
            SyncSidebarState(CodexSidebarState.FromSidebar(sidebar));
            return;
        }

        SyncSidebarState(new CodexSidebarState(true, CodexSidebarCollapsible.Offcanvas, CodexSidebarVariant.Sidebar, CodexSidebarSide.Left));
    }

    private CodexSidebarProvider? ResolveProvider()
    {
        return TargetProvider ?? this.GetLogicalAncestors().OfType<CodexSidebarProvider>().FirstOrDefault();
    }

    private CodexSidebar? ResolveSidebar()
    {
        return TargetSidebar
            ?? this.GetLogicalAncestors().OfType<CodexSidebar>().FirstOrDefault()
            ?? ResolveProvider()?.GetLogicalDescendants().OfType<CodexSidebar>().FirstOrDefault();
    }

    private void SyncToggleClasses()
    {
        Classes.Set("can-toggle", CanToggle);
        Classes.Set("command-blocked", Command is not null && IsEnabled && !CanToggle);
    }

    private void OnCommandChanged(ICommand? oldCommand, ICommand? newCommand)
    {
        if (ReferenceEquals(oldCommand, newCommand))
        {
            return;
        }

        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
        }

        _subscribedCommand = newCommand;

        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged += OnCommandCanExecuteChanged;
        }

        SyncToggleClasses();
    }

    private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
    {
        SyncToggleClasses();
    }
}

public class CodexSidebarInset : CodexFrame
{
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexSidebarInset, bool>(nameof(IsOpen), true);

    static CodexSidebarInset()
    {
        IsOpenProperty.Changed.AddClassHandler<CodexSidebarInset>((inset, _) => inset.SyncClasses());
    }

    public CodexSidebarInset()
    {
        SyncClasses();
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);

        var provider = this.GetLogicalAncestors().OfType<CodexSidebarProvider>().FirstOrDefault();
        if (provider is not null)
        {
            SyncSidebarState(provider.ResolveState());
        }
    }

    internal void SyncSidebarState(CodexSidebarState state)
    {
        if (IsOpen != state.IsOpen)
        {
            SetCurrentValue(IsOpenProperty, state.IsOpen);
        }

        CodexSidebarClassSync.Apply(Classes, state);
    }

    private void SyncClasses()
    {
        Classes.Set("sidebar-inset", true);
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("state-expanded", IsOpen);
        Classes.Set("state-collapsed", !IsOpen);
    }
}

internal readonly record struct CodexSidebarState(
    bool IsOpen,
    CodexSidebarCollapsible Collapsible,
    CodexSidebarVariant Variant,
    CodexSidebarSide Side)
{
    public bool IsCollapsed => Collapsible != CodexSidebarCollapsible.None && !IsOpen;

    public static CodexSidebarState FromSidebar(CodexSidebar sidebar)
    {
        return new CodexSidebarState(
            sidebar.Collapsible == CodexSidebarCollapsible.None || sidebar.IsOpen,
            sidebar.Collapsible,
            sidebar.Variant,
            sidebar.Side);
    }
}

internal static class CodexSidebarClassSync
{
    public static void Apply(Classes classes, CodexSidebarState state)
    {
        var collapsed = state.IsCollapsed;

        classes.Set("open", !collapsed);
        classes.Set("closed", collapsed);
        classes.Set("state-expanded", !collapsed);
        classes.Set("state-collapsed", collapsed);
        classes.Set("side-left", state.Side == CodexSidebarSide.Left);
        classes.Set("side-right", state.Side == CodexSidebarSide.Right);
        classes.Set("variant-sidebar", state.Variant == CodexSidebarVariant.Sidebar);
        classes.Set("variant-floating", state.Variant == CodexSidebarVariant.Floating);
        classes.Set("variant-inset", state.Variant == CodexSidebarVariant.Inset);
        classes.Set("collapsible-offcanvas", state.Collapsible == CodexSidebarCollapsible.Offcanvas);
        classes.Set("collapsible-icon", state.Collapsible == CodexSidebarCollapsible.Icon);
        classes.Set("collapsible-none", state.Collapsible == CodexSidebarCollapsible.None);
        classes.Set("offcanvas", collapsed && state.Collapsible == CodexSidebarCollapsible.Offcanvas);
        classes.Set("icon", collapsed && state.Collapsible == CodexSidebarCollapsible.Icon);
    }
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
