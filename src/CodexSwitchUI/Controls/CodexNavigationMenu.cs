using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

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

    private CodexNavigationMenuItem? _activeItem;
    private int _activeIndex = -1;

    static CodexNavigationMenu()
    {
        SizeProperty.Changed.AddClassHandler<CodexNavigationMenu>((menu, _) => menu.SyncClasses());
        OrientationProperty.Changed.AddClassHandler<CodexNavigationMenu>((menu, _) => menu.SyncClasses());
        IsViewportOpenProperty.Changed.AddClassHandler<CodexNavigationMenu>((menu, _) => menu.SyncClasses());
        IsMotionReversedProperty.Changed.AddClassHandler<CodexNavigationMenu>((menu, _) => menu.SyncClasses());
    }

    public CodexNavigationMenu()
    {
        SyncClasses();
    }

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

    public void ActivateItem(CodexNavigationMenuItem item)
    {
        if (!item.IsEnabled || !item.HasContent)
        {
            CloseViewport();
            return;
        }

        var nextIndex = IndexOf(item);
        var hasPrevious = _activeIndex >= 0;
        IsMotionReversed = hasPrevious && nextIndex >= 0 && nextIndex < _activeIndex;

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
    }

    public void CloseViewport()
    {
        _activeItem?.SetOpenState(false);
        _activeItem = null;
        _activeIndex = -1;
        ActiveItem = null;
        IsViewportOpen = false;
        SyncClasses();
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
        base.OnKeyDown(e);

        if (e.Key == Key.Escape)
        {
            CloseViewport();
            e.Handled = true;
        }
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
}

public class CodexNavigationMenuItem : HeaderedContentControl
{
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexNavigationMenuItem, object?>(nameof(Icon));

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
        IconProperty.Changed.AddClassHandler<CodexNavigationMenuItem>((item, _) => item.SyncClasses());
        IsOpenProperty.Changed.AddClassHandler<CodexNavigationMenuItem>((item, _) => item.SyncClasses());
        ContentProperty.Changed.AddClassHandler<CodexNavigationMenuItem>((item, _) => item.SyncClasses());
    }

    public CodexNavigationMenuItem()
    {
        Focusable = true;
        SyncClasses();
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool HasIcon => GetValue(HasIconProperty);

    public bool HasContent => GetValue(HasContentProperty);

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

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        Activate();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            Focus();
            Activate();
            e.Handled = true;
        }
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        Activate();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key is Key.Enter or Key.Space)
        {
            Activate();
            e.Handled = true;
        }
    }

    private void Activate()
    {
        var owner = ItemsControl.ItemsControlFromItemContainer(this) as CodexNavigationMenu
            ?? this.GetVisualAncestors().OfType<CodexNavigationMenu>().FirstOrDefault();

        owner?.ActivateItem(this);
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

public class CodexNavigationMenuLink : ContentControl
{
    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexNavigationMenuLink, string?>(nameof(Description));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexNavigationMenuLink, object?>(nameof(Icon));

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
        IsActiveProperty.Changed.AddClassHandler<CodexNavigationMenuLink>((link, _) => link.SyncClasses());
    }

    public CodexNavigationMenuLink()
    {
        Focusable = true;
        SyncClasses();
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

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasIcon => GetValue(HasIconProperty);

    private void SyncClasses()
    {
        SetValue(HasDescriptionProperty, HasValue(Description));
        SetValue(HasIconProperty, HasValue(Icon));
        Classes.Set("active", IsActive);
        Classes.Set("has-description", HasDescription);
        Classes.Set("has-icon", HasIcon);
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
