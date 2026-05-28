using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using System.Linq;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public sealed class CodexBreadcrumbLinkActivatedEventArgs(
    CodexBreadcrumbLink link,
    CodexBreadcrumbItem? item,
    int index,
    string? href,
    object? content,
    CodexBreadcrumbLinkActivationSource source = CodexBreadcrumbLinkActivationSource.Programmatic) : EventArgs
{
    public CodexBreadcrumbLink Link { get; } = link;

    public CodexBreadcrumbItem? Item { get; } = item;

    public int Index { get; } = index;

    public string? Href { get; } = href;

    public object? Content { get; } = content;

    public CodexBreadcrumbLinkActivationSource Source { get; } = source;
}

public enum CodexBreadcrumbLinkActivationSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public class CodexBreadcrumb : ContentControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexBreadcrumb, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<CodexBreadcrumb, string>(nameof(Label), "Breadcrumb");

    static CodexBreadcrumb()
    {
        SizeProperty.Changed.AddClassHandler<CodexBreadcrumb>((breadcrumb, _) => breadcrumb.SyncClasses());
        LabelProperty.Changed.AddClassHandler<CodexBreadcrumb>((breadcrumb, _) => breadcrumb.SyncAutomation());
    }

    public CodexBreadcrumb()
    {
        SyncClasses();
        SyncAutomation();
    }

    public event EventHandler<CodexBreadcrumbLinkActivatedEventArgs>? LinkActivated;

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("breadcrumb", true);
        CodexClassSync.SetSize(Classes, Size);
    }

    private void SyncAutomation()
    {
        AutomationProperties.SetName(this, Label);
        AutomationProperties.SetIsControlElementOverride(this, true);
    }

    internal void NotifyLinkActivated(
        CodexBreadcrumbLink link,
        CodexBreadcrumbLinkActivationSource source = CodexBreadcrumbLinkActivationSource.Programmatic)
    {
        var item = link.GetLogicalAncestors().OfType<CodexBreadcrumbItem>().FirstOrDefault()
            ?? link.GetVisualAncestors().OfType<CodexBreadcrumbItem>().FirstOrDefault();
        var items = GetBreadcrumbItems();
        var index = IndexOf(items, item);

        LinkActivated?.Invoke(
            this,
            new CodexBreadcrumbLinkActivatedEventArgs(link, item, index, link.Href, link.Content, source));
    }

    private IReadOnlyList<CodexBreadcrumbItem> GetBreadcrumbItems()
    {
        var logical = this.GetLogicalDescendants()
            .OfType<CodexBreadcrumbItem>()
            .ToList();
        if (logical.Count > 0)
        {
            return logical;
        }

        var visual = this.GetVisualDescendants()
            .OfType<CodexBreadcrumbItem>()
            .ToList();
        if (visual.Count > 0)
        {
            return visual;
        }

        return Content is CodexBreadcrumbItem item ? [item] : [];
    }

    private static int IndexOf(IReadOnlyList<CodexBreadcrumbItem> items, CodexBreadcrumbItem? item)
    {
        if (item is null)
        {
            return -1;
        }

        for (var index = 0; index < items.Count; index++)
        {
            if (ReferenceEquals(items[index], item))
            {
                return index;
            }
        }

        return -1;
    }
}

public class CodexBreadcrumbList : ItemsControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexBreadcrumbList, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexBreadcrumbList()
    {
        SizeProperty.Changed.AddClassHandler<CodexBreadcrumbList>((list, _) => list.SyncClasses());
    }

    public CodexBreadcrumbList()
    {
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("breadcrumb-list", true);
        CodexClassSync.SetSize(Classes, Size);
    }
}

public class CodexBreadcrumbItem : ContentControl
{
    public static readonly StyledProperty<bool> IsCurrentProperty =
        AvaloniaProperty.Register<CodexBreadcrumbItem, bool>(nameof(IsCurrent));

    static CodexBreadcrumbItem()
    {
        IsCurrentProperty.Changed.AddClassHandler<CodexBreadcrumbItem>((item, _) => item.SyncClasses());
    }

    public CodexBreadcrumbItem()
    {
        SyncClasses();
    }

    public bool IsCurrent
    {
        get => GetValue(IsCurrentProperty);
        set => SetValue(IsCurrentProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("breadcrumb-item", true);
        Classes.Set("current", IsCurrent);
    }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexBreadcrumbLink : Button
{
    private ICommand? _subscribedCommand;
    private bool _hasPrimaryPointerPress;
    private PointerUpdateKind? _pendingPointerReleaseKind;
    private bool _activationHandledByPointerRelease;
    private CodexBreadcrumbLinkActivationSource? _pendingActivationSource;

    public static readonly StyledProperty<bool> IsCurrentProperty =
        AvaloniaProperty.Register<CodexBreadcrumbLink, bool>(nameof(IsCurrent));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexBreadcrumbLink, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<string?> HrefProperty =
        AvaloniaProperty.Register<CodexBreadcrumbLink, string?>(nameof(Href));

    static CodexBreadcrumbLink()
    {
        IsCurrentProperty.Changed.AddClassHandler<CodexBreadcrumbLink>((link, _) => link.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexBreadcrumbLink>((link, _) => link.SyncClasses());
        HrefProperty.Changed.AddClassHandler<CodexBreadcrumbLink>((link, _) => link.SyncClasses());
        CommandProperty.Changed.AddClassHandler<CodexBreadcrumbLink>((link, args) => link.OnCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        CommandParameterProperty.Changed.AddClassHandler<CodexBreadcrumbLink>((link, _) => link.SyncClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexBreadcrumbLink>((link, _) => link.SyncClasses());
    }

    public CodexBreadcrumbLink()
    {
        SyncClasses();
    }

    public bool IsCurrent
    {
        get => GetValue(IsCurrentProperty);
        set => SetValue(IsCurrentProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public string? Href
    {
        get => GetValue(HrefProperty);
        set => SetValue(HrefProperty, value);
    }

    public bool CanActivate => !IsCurrent && IsEnabled && (Command?.CanExecute(CommandParameter) ?? true);

    public bool TryActivate()
    {
        return TryActivate(CodexBreadcrumbLinkActivationSource.Programmatic);
    }

    internal bool TryActivate(CodexBreadcrumbLinkActivationSource source)
    {
        if (!CanActivate)
        {
            return false;
        }

        _pendingActivationSource = source;
        try
        {
            OnClick();
        }
        finally
        {
            _pendingActivationSource = null;
        }

        return true;
    }

    internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased)
        {
            return false;
        }

        return TryActivate(CodexBreadcrumbLinkActivationSource.Pointer);
    }

    protected override void OnClick()
    {
        if (_pendingPointerReleaseKind is { } updateKind)
        {
            if (updateKind != PointerUpdateKind.LeftButtonReleased || !CanActivate)
            {
                return;
            }

            NotifyOwner(_pendingActivationSource ?? CodexBreadcrumbLinkActivationSource.Pointer);
            _activationHandledByPointerRelease = true;
            base.OnClick();
            return;
        }

        if (!CanActivate)
        {
            return;
        }

        NotifyOwner(_pendingActivationSource ?? CodexBreadcrumbLinkActivationSource.Keyboard);
        base.OnClick();
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, CodexFocusVisible.FromFocusChange(e));
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        _hasPrimaryPointerPress = e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed;
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        var canActivateFromPointer = _hasPrimaryPointerPress && IsPointerOver;
        _hasPrimaryPointerPress = false;

        if (canActivateFromPointer)
        {
            _pendingPointerReleaseKind = updateKind;
            try
            {
                base.OnPointerReleased(e);
            }
            finally
            {
                _pendingPointerReleaseKind = null;
            }

            if (_activationHandledByPointerRelease)
            {
                _activationHandledByPointerRelease = false;
                e.Handled = true;
            }

            return;
        }

        base.OnPointerReleased(e);
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

    private void SyncClasses()
    {
        Classes.Set("breadcrumb-link", true);
        Classes.Set("current", IsCurrent);
        Classes.Set("has-href", !string.IsNullOrWhiteSpace(Href));
        Classes.Set("can-activate", CanActivate);
        Classes.Set("command-blocked", Command is not null && !IsCurrent && IsEnabled && !CanActivate);
        CodexClassSync.SetSize(Classes, Size);
    }

    private void NotifyOwner(CodexBreadcrumbLinkActivationSource source)
    {
        var owner = this.GetLogicalAncestors().OfType<CodexBreadcrumb>().FirstOrDefault()
            ?? this.GetVisualAncestors().OfType<CodexBreadcrumb>().FirstOrDefault();
        owner?.NotifyLinkActivated(this, source);
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

        SyncClasses();
    }

    private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
    {
        SyncClasses();
    }
}

public class CodexBreadcrumbPage : ContentControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexBreadcrumbPage, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexBreadcrumbPage()
    {
        SizeProperty.Changed.AddClassHandler<CodexBreadcrumbPage>((page, _) => page.SyncClasses());
    }

    public CodexBreadcrumbPage()
    {
        Focusable = false;
        IsHitTestVisible = false;
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("breadcrumb-page", true);
        Classes.Set("current", true);
        CodexClassSync.SetSize(Classes, Size);
    }
}

public class CodexBreadcrumbSeparator : ContentControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexBreadcrumbSeparator, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexBreadcrumbSeparator()
    {
        SizeProperty.Changed.AddClassHandler<CodexBreadcrumbSeparator>((separator, _) => separator.SyncClasses());
    }

    public CodexBreadcrumbSeparator()
    {
        Content = ">";
        Focusable = false;
        IsHitTestVisible = false;
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("breadcrumb-separator", true);
        CodexClassSync.SetSize(Classes, Size);
    }
}

public class CodexBreadcrumbEllipsis : ContentControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexBreadcrumbEllipsis, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<CodexBreadcrumbEllipsis, string>(nameof(Label), "More");

    static CodexBreadcrumbEllipsis()
    {
        SizeProperty.Changed.AddClassHandler<CodexBreadcrumbEllipsis>((ellipsis, _) => ellipsis.SyncClasses());
        LabelProperty.Changed.AddClassHandler<CodexBreadcrumbEllipsis>((ellipsis, _) => ellipsis.SyncAutomation());
    }

    public CodexBreadcrumbEllipsis()
    {
        Content = "...";
        Focusable = false;
        IsHitTestVisible = false;
        SyncClasses();
        SyncAutomation();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("breadcrumb-ellipsis", true);
        CodexClassSync.SetSize(Classes, Size);
    }

    private void SyncAutomation()
    {
        AutomationProperties.SetName(this, Label);
        AutomationProperties.SetIsControlElementOverride(this, true);
    }
}
