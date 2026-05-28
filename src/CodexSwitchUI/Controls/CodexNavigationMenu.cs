using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public sealed class CodexNavigationMenuActiveItemChangedEventArgs(
    CodexNavigationMenuItem? oldItem,
    CodexNavigationMenuItem? newItem,
    string? value) : EventArgs
{
    public CodexNavigationMenuItem? OldItem { get; } = oldItem;

    public CodexNavigationMenuItem? NewItem { get; } = newItem;

    public string? Value { get; } = value;
}

public sealed class CodexNavigationMenuActivatedEventArgs(object? commandParameter) : EventArgs
{
    public object? CommandParameter { get; } = commandParameter;
}

public class CodexNavigationMenu : ItemsControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexNavigationMenu, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexNavigationMenu, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<bool> IsViewportOpenProperty =
        AvaloniaProperty.Register<CodexNavigationMenu, bool>(nameof(IsViewportOpen));

    public static readonly StyledProperty<bool> IsMotionReversedProperty =
        AvaloniaProperty.Register<CodexNavigationMenu, bool>(nameof(IsMotionReversed));

    public static readonly StyledProperty<object?> ViewportContentProperty =
        AvaloniaProperty.Register<CodexNavigationMenu, object?>(nameof(ViewportContent));

    public static readonly StyledProperty<double> ViewportWidthProperty =
        AvaloniaProperty.Register<CodexNavigationMenu, double>(nameof(ViewportWidth), 420);

    public static readonly StyledProperty<double> ViewportMinHeightProperty =
        AvaloniaProperty.Register<CodexNavigationMenu, double>(nameof(ViewportMinHeight), 188);

    public static readonly StyledProperty<CodexNavigationMenuItem?> ActiveItemProperty =
        AvaloniaProperty.Register<CodexNavigationMenu, CodexNavigationMenuItem?>(nameof(ActiveItem));

    public static readonly StyledProperty<string?> ActiveValueProperty =
        AvaloniaProperty.Register<CodexNavigationMenu, string?>(nameof(ActiveValue));

    private CodexNavigationMenuItem? _activeItem;
    private int _activeIndex = -1;
    private bool _isSyncingActiveValue;

    static CodexNavigationMenu()
    {
        SizeProperty.Changed.AddClassHandler<CodexNavigationMenu>((menu, _) => menu.SyncClasses());
        OrientationProperty.Changed.AddClassHandler<CodexNavigationMenu>((menu, _) => menu.SyncClasses());
        IsViewportOpenProperty.Changed.AddClassHandler<CodexNavigationMenu>((menu, _) => menu.SyncClasses());
        IsMotionReversedProperty.Changed.AddClassHandler<CodexNavigationMenu>((menu, _) => menu.SyncClasses());
        ActiveValueProperty.Changed.AddClassHandler<CodexNavigationMenu>((menu, args) => menu.OnActiveValueChanged(args.NewValue as string));
    }

    public CodexNavigationMenu()
    {
        SyncClasses();
    }

    public event EventHandler<CodexNavigationMenuActiveItemChangedEventArgs>? ActiveItemChanged;

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public bool IsViewportOpen
    {
        get => GetValue(IsViewportOpenProperty);
        private set => SetValue(IsViewportOpenProperty, value);
    }

    public bool IsMotionReversed
    {
        get => GetValue(IsMotionReversedProperty);
        private set => SetValue(IsMotionReversedProperty, value);
    }

    public object? ViewportContent
    {
        get => GetValue(ViewportContentProperty);
        private set => SetValue(ViewportContentProperty, value);
    }

    public double ViewportWidth
    {
        get => GetValue(ViewportWidthProperty);
        private set => SetValue(ViewportWidthProperty, value);
    }

    public double ViewportMinHeight
    {
        get => GetValue(ViewportMinHeightProperty);
        private set => SetValue(ViewportMinHeightProperty, value);
    }

    public CodexNavigationMenuItem? ActiveItem
    {
        get => GetValue(ActiveItemProperty);
        private set => SetValue(ActiveItemProperty, value);
    }

    public string? ActiveValue
    {
        get => GetValue(ActiveValueProperty);
        set => SetValue(ActiveValueProperty, value);
    }

    public void ActivateItem(CodexNavigationMenuItem item, bool activateLink = false)
    {
        if (!item.IsEnabled)
        {
            return;
        }

        if (!item.HasContent)
        {
            if (activateLink)
            {
                item.TryActivateLink();
            }

            CloseViewport();
            return;
        }

        var nextIndex = IndexOf(item);
        var hasPrevious = _activeIndex >= 0;
        IsMotionReversed = hasPrevious && nextIndex >= 0 && nextIndex < _activeIndex;
        var oldItem = _activeItem;

        if (!ReferenceEquals(_activeItem, item))
        {
            _activeItem?.SetOpenState(false);
            _activeItem = item;
            _activeIndex = nextIndex;
        }

        item.SetOpenState(true);
        ActiveItem = item;
        ViewportContent = item.Content;
        ViewportWidth = item.ViewportWidth;
        ViewportMinHeight = item.ViewportMinHeight;
        IsViewportOpen = true;
        SyncClasses();

        var value = item.ResolveValue();
        SetActiveValue(value);
        if (!ReferenceEquals(oldItem, item))
        {
            ActiveItemChanged?.Invoke(this, new CodexNavigationMenuActiveItemChangedEventArgs(oldItem, item, value));
        }
    }

    public void CloseViewport()
    {
        var oldItem = _activeItem;
        _activeItem?.SetOpenState(false);
        _activeItem = null;
        _activeIndex = -1;
        ActiveItem = null;
        SetActiveValue(null);
        IsViewportOpen = false;
        SyncClasses();

        if (oldItem is not null)
        {
            ActiveItemChanged?.Invoke(this, new CodexNavigationMenuActiveItemChangedEventArgs(oldItem, null, null));
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        ApplyActiveValue(ActiveValue);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);

        var position = e.GetPosition(this);
        if (position.X < 0
            || position.Y < 0
            || position.X > Bounds.Width
            || position.Y > Bounds.Height)
        {
            CloseViewport();
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (TryHandleNavigationKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    internal bool TryHandleNavigationKey(Key key)
    {
        if (key != Key.Escape)
        {
            return false;
        }

        CloseViewport();
        return true;
    }

    internal bool TryHandleItemNavigationKey(CodexNavigationMenuItem currentItem, Key key, bool moveFocus = true)
    {
        if (ItemsView.Count == 0)
        {
            return false;
        }

        var currentIndex = IndexOf(currentItem);
        if (currentIndex < 0)
        {
            currentIndex = _activeIndex >= 0 ? _activeIndex : FirstActivatableIndex();
        }

        var nextIndex = key switch
        {
            Key.Home => FirstActivatableIndex(),
            Key.End => LastActivatableIndex(),
            Key.Right when Orientation == Orientation.Horizontal => NextActivatableIndex(currentIndex, 1),
            Key.Left when Orientation == Orientation.Horizontal => NextActivatableIndex(currentIndex, -1),
            Key.Down when Orientation == Orientation.Vertical => NextActivatableIndex(currentIndex, 1),
            Key.Up when Orientation == Orientation.Vertical => NextActivatableIndex(currentIndex, -1),
            _ => -1
        };

        if (nextIndex < 0)
        {
            return false;
        }

        var item = NavigationItemAt(nextIndex);
        if (item is null)
        {
            return false;
        }

        if (moveFocus)
        {
            item.Focus(NavigationMethod.Directional, KeyModifiers.None);
        }

        ActivateItem(item);
        return true;
    }

    private int IndexOf(CodexNavigationMenuItem item)
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (ReferenceEquals(ItemsView[index], item)
                || ReferenceEquals(ContainerFromIndex(index), item))
            {
                return index;
            }
        }

        return -1;
    }

    private int FirstActivatableIndex()
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (NavigationItemAt(index) is { IsEnabled: true })
            {
                return index;
            }
        }

        return -1;
    }

    private int LastActivatableIndex()
    {
        for (var index = ItemsView.Count - 1; index >= 0; index--)
        {
            if (NavigationItemAt(index) is { IsEnabled: true })
            {
                return index;
            }
        }

        return -1;
    }

    private int NextActivatableIndex(int currentIndex, int step)
    {
        var count = ItemsView.Count;
        for (var offset = 1; offset <= count; offset++)
        {
            var index = (currentIndex + (offset * step) + count) % count;
            if (NavigationItemAt(index) is { IsEnabled: true })
            {
                return index;
            }
        }

        return -1;
    }

    private CodexNavigationMenuItem? NavigationItemAt(int index)
    {
        if (index < 0 || index >= ItemsView.Count)
        {
            return null;
        }

        return ItemsView[index] as CodexNavigationMenuItem
            ?? ContainerFromIndex(index) as CodexNavigationMenuItem;
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("open", IsViewportOpen);
        Classes.Set("closed", !IsViewportOpen);
        Classes.Set("motion-from-start", IsMotionReversed);
        Classes.Set("motion-from-end", IsViewportOpen && !IsMotionReversed);
    }

    private void OnActiveValueChanged(string? value)
    {
        if (_isSyncingActiveValue)
        {
            return;
        }

        ApplyActiveValue(value);
    }

    private void ApplyActiveValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            CloseViewport();
            return;
        }

        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (NavigationItemAt(index) is { } item
                && string.Equals(item.ResolveValue(), value, StringComparison.Ordinal))
            {
                ActivateItem(item);
                return;
            }
        }
    }

    private void SetActiveValue(string? value)
    {
        _isSyncingActiveValue = true;
        try
        {
            SetValue(ActiveValueProperty, value);
        }
        finally
        {
            _isSyncingActiveValue = false;
        }
    }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexNavigationMenuItem : HeaderedContentControl
{
    private ICommand? _subscribedCommand;

    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CodexNavigationMenuItem, string?>(nameof(Value));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexNavigationMenuItem, object?>(nameof(Icon));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<CodexNavigationMenuItem, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<CodexNavigationMenuItem, object?>(nameof(CommandParameter));

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexNavigationMenuItem, bool>(nameof(IsOpen));

    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<CodexNavigationMenuItem, bool>(nameof(HasIcon));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexNavigationMenuItem, bool>(nameof(HasContent));

    public static readonly StyledProperty<double> ViewportWidthProperty =
        AvaloniaProperty.Register<CodexNavigationMenuItem, double>(nameof(ViewportWidth), 420);

    public static readonly StyledProperty<double> ViewportMinHeightProperty =
        AvaloniaProperty.Register<CodexNavigationMenuItem, double>(nameof(ViewportMinHeight), 188);

    static CodexNavigationMenuItem()
    {
        CommandProperty.Changed.AddClassHandler<CodexNavigationMenuItem>((item, args) => item.OnCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        CommandParameterProperty.Changed.AddClassHandler<CodexNavigationMenuItem>((item, _) => item.SyncClasses());
        IconProperty.Changed.AddClassHandler<CodexNavigationMenuItem>((item, _) => item.SyncClasses());
        IsOpenProperty.Changed.AddClassHandler<CodexNavigationMenuItem>((item, _) => item.SyncClasses());
        ContentProperty.Changed.AddClassHandler<CodexNavigationMenuItem>((item, _) => item.SyncClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexNavigationMenuItem>((item, _) => item.SyncClasses());
    }

    public CodexNavigationMenuItem()
    {
        Focusable = true;
        SyncClasses();
    }

    public event EventHandler<CodexNavigationMenuActivatedEventArgs>? Activated;

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool HasIcon => GetValue(HasIconProperty);

    public bool HasContent => GetValue(HasContentProperty);

    public bool CanActivateLink => IsEnabled && !HasContent && (Command?.CanExecute(CommandParameter) ?? true);

    public double ViewportWidth
    {
        get => GetValue(ViewportWidthProperty);
        set => SetValue(ViewportWidthProperty, value);
    }

    public double ViewportMinHeight
    {
        get => GetValue(ViewportMinHeightProperty);
        set => SetValue(ViewportMinHeightProperty, value);
    }

    internal void SetOpenState(bool isOpen)
    {
        SetValue(IsOpenProperty, isOpen);
    }

    public bool TryActivateLink()
    {
        if (!CanActivateLink)
        {
            return false;
        }

        Command?.Execute(CommandParameter);
        Activated?.Invoke(this, new CodexNavigationMenuActivatedEventArgs(CommandParameter));
        return true;
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        Activate();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        base.OnPointerPressed(e);

        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        if (updateKind == PointerUpdateKind.LeftButtonPressed)
        {
            Focus(NavigationMethod.Pointer, KeyModifiers.None);
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        if (TryHandlePointerRelease(updateKind))
        {
            e.Handled = true;
        }
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, CodexFocusVisible.FromFocusChange(e));
        Activate();
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        var owner = FindOwner();
        if (TryHandleActivationKey(e.Key, owner)
            || owner?.TryHandleItemNavigationKey(this, e.Key) == true)
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    internal bool TryHandleActivationKey(Key key, CodexNavigationMenu? owner = null)
    {
        if (key is not (Key.Enter or Key.Space) || !IsEnabled)
        {
            return false;
        }

        if (!HasContent)
        {
            if (!TryActivateLink())
            {
                return false;
            }

            owner?.CloseViewport();
        }
        else
        {
            Activate(owner);
        }

        return true;
    }

    internal bool TryHandlePointerRelease(PointerUpdateKind updateKind, CodexNavigationMenu? owner = null)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased || !IsEnabled)
        {
            return false;
        }

        owner ??= FindOwner();
        if (!HasContent)
        {
            if (!TryActivateLink())
            {
                return false;
            }

            owner?.CloseViewport();
        }
        else
        {
            Activate(owner);
        }

        return true;
    }

    internal string? ResolveValue()
    {
        var value = string.IsNullOrWhiteSpace(Value)
            ? Header?.ToString()
            : Value;

        return value?.Trim();
    }

    private void Activate(CodexNavigationMenu? owner = null)
    {
        owner ??= FindOwner();
        owner?.ActivateItem(this);
    }

    private CodexNavigationMenu? FindOwner()
    {
        return ItemsControl.ItemsControlFromItemContainer(this) as CodexNavigationMenu
            ?? this.GetVisualAncestors().OfType<CodexNavigationMenu>().FirstOrDefault();
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
        var hasContent = HasValue(Content);
        SetValue(HasIconProperty, HasValue(Icon));
        SetValue(HasContentProperty, hasContent);
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("has-icon", HasIcon);
        Classes.Set("has-content", hasContent);
        Classes.Set("link", !hasContent);
        Classes.Set("can-activate", CanActivateLink);
        Classes.Set("command-blocked", !hasContent && !CanActivateLink);
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

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}

public class CodexNavigationMenuContent : ItemsControl
{
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<CodexNavigationMenuContent, object?>(nameof(Header));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexNavigationMenuContent, string?>(nameof(Description));

    public static readonly StyledProperty<bool> HasHeaderProperty =
        AvaloniaProperty.Register<CodexNavigationMenuContent, bool>(nameof(HasHeader));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexNavigationMenuContent, bool>(nameof(HasDescription));

    static CodexNavigationMenuContent()
    {
        HeaderProperty.Changed.AddClassHandler<CodexNavigationMenuContent>((content, _) => content.SyncClasses());
        DescriptionProperty.Changed.AddClassHandler<CodexNavigationMenuContent>((content, _) => content.SyncClasses());
    }

    public CodexNavigationMenuContent()
    {
        SyncClasses();
    }

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public bool HasHeader => GetValue(HasHeaderProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    private void SyncClasses()
    {
        SetValue(HasHeaderProperty, HasValue(Header));
        SetValue(HasDescriptionProperty, HasValue(Description));
        Classes.Set("has-header", HasHeader);
        Classes.Set("has-description", HasDescription);
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexNavigationMenuLink : ContentControl
{
    private ICommand? _subscribedCommand;

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexNavigationMenuLink, string?>(nameof(Description));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexNavigationMenuLink, object?>(nameof(Icon));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<CodexNavigationMenuLink, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<CodexNavigationMenuLink, object?>(nameof(CommandParameter));

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexNavigationMenuLink, bool>(nameof(IsActive));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexNavigationMenuLink, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<CodexNavigationMenuLink, bool>(nameof(HasIcon));

    static CodexNavigationMenuLink()
    {
        DescriptionProperty.Changed.AddClassHandler<CodexNavigationMenuLink>((link, _) => link.SyncClasses());
        IconProperty.Changed.AddClassHandler<CodexNavigationMenuLink>((link, _) => link.SyncClasses());
        CommandProperty.Changed.AddClassHandler<CodexNavigationMenuLink>((link, args) => link.OnCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        CommandParameterProperty.Changed.AddClassHandler<CodexNavigationMenuLink>((link, _) => link.SyncClasses());
        IsActiveProperty.Changed.AddClassHandler<CodexNavigationMenuLink>((link, _) => link.SyncClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexNavigationMenuLink>((link, _) => link.SyncClasses());
    }

    public CodexNavigationMenuLink()
    {
        Focusable = true;
        SyncClasses();
    }

    public event EventHandler<CodexNavigationMenuActivatedEventArgs>? Activated;

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

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasIcon => GetValue(HasIconProperty);

    public bool CanActivate => IsEnabled && (Command?.CanExecute(CommandParameter) ?? true);

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
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        if (TryHandlePointerActivation(updateKind))
        {
            e.Handled = true;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key is (Key.Enter or Key.Space) && TryActivate())
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased)
        {
            return false;
        }

        return TryActivate();
    }

    public bool TryActivate()
    {
        if (!CanActivate)
        {
            return false;
        }

        Command?.Execute(CommandParameter);
        Activated?.Invoke(this, new CodexNavigationMenuActivatedEventArgs(CommandParameter));
        return true;
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
        SetValue(HasDescriptionProperty, HasValue(Description));
        SetValue(HasIconProperty, HasValue(Icon));
        Classes.Set("active", IsActive);
        Classes.Set("has-description", HasDescription);
        Classes.Set("has-icon", HasIcon);
        Classes.Set("can-activate", CanActivate);
        Classes.Set("command-blocked", !CanActivate);
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

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
