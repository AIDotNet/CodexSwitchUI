using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public enum CodexScrollAreaType
{
    Auto,
    Always,
    Hover,
    Scroll
}

public class CodexScrollArea : ContentControl
{
    private static readonly TimeSpan ScrollIdleDelay = TimeSpan.FromMilliseconds(650);

    private ScrollViewer? _viewport;
    private readonly DispatcherTimer _scrollIdleTimer;

    public static readonly StyledProperty<CodexScrollAreaType> TypeProperty =
        AvaloniaProperty.Register<CodexScrollArea, CodexScrollAreaType>(nameof(Type), CodexScrollAreaType.Auto);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexScrollArea, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<CodexScrollArea, ScrollBarVisibility>(nameof(HorizontalScrollBarVisibility), ScrollBarVisibility.Disabled);

    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<CodexScrollArea, ScrollBarVisibility>(nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Auto);

    public static readonly StyledProperty<bool> AllowAutoHideProperty =
        AvaloniaProperty.Register<CodexScrollArea, bool>(nameof(AllowAutoHide), true);

    public static readonly StyledProperty<bool> IsScrollChainingEnabledProperty =
        AvaloniaProperty.Register<CodexScrollArea, bool>(nameof(IsScrollChainingEnabled), true);

    public static readonly StyledProperty<bool> IsInsetContentProperty =
        AvaloniaProperty.Register<CodexScrollArea, bool>(nameof(IsInsetContent));

    public static readonly StyledProperty<Vector> OffsetProperty =
        AvaloniaProperty.Register<CodexScrollArea, Vector>(nameof(Offset));

    public static readonly StyledProperty<Size> ExtentProperty =
        AvaloniaProperty.Register<CodexScrollArea, Size>(nameof(Extent));

    public static readonly StyledProperty<Size> ViewportProperty =
        AvaloniaProperty.Register<CodexScrollArea, Size>(nameof(Viewport));

    public static readonly StyledProperty<bool> IsScrollingProperty =
        AvaloniaProperty.Register<CodexScrollArea, bool>(nameof(IsScrolling));

    public static readonly StyledProperty<bool> CanScrollHorizontallyProperty =
        AvaloniaProperty.Register<CodexScrollArea, bool>(nameof(CanScrollHorizontally));

    public static readonly StyledProperty<bool> CanScrollVerticallyProperty =
        AvaloniaProperty.Register<CodexScrollArea, bool>(nameof(CanScrollVertically));

    public static readonly StyledProperty<bool> IsAtStartProperty =
        AvaloniaProperty.Register<CodexScrollArea, bool>(nameof(IsAtStart), true);

    public static readonly StyledProperty<bool> IsAtEndProperty =
        AvaloniaProperty.Register<CodexScrollArea, bool>(nameof(IsAtEnd), true);

    public static readonly StyledProperty<bool> IsAtTopProperty =
        AvaloniaProperty.Register<CodexScrollArea, bool>(nameof(IsAtTop), true);

    public static readonly StyledProperty<bool> IsAtBottomProperty =
        AvaloniaProperty.Register<CodexScrollArea, bool>(nameof(IsAtBottom), true);

    static CodexScrollArea()
    {
        TypeProperty.Changed.AddClassHandler<CodexScrollArea>((area, _) => area.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexScrollArea>((area, _) => area.SyncClasses());
        HorizontalScrollBarVisibilityProperty.Changed.AddClassHandler<CodexScrollArea>((area, _) => area.SyncClasses());
        VerticalScrollBarVisibilityProperty.Changed.AddClassHandler<CodexScrollArea>((area, _) => area.SyncClasses());
        IsInsetContentProperty.Changed.AddClassHandler<CodexScrollArea>((area, _) => area.SyncClasses());
        IsScrollingProperty.Changed.AddClassHandler<CodexScrollArea>((area, _) => area.SyncClasses());
        CanScrollHorizontallyProperty.Changed.AddClassHandler<CodexScrollArea>((area, _) => area.SyncClasses());
        CanScrollVerticallyProperty.Changed.AddClassHandler<CodexScrollArea>((area, _) => area.SyncClasses());
        IsAtStartProperty.Changed.AddClassHandler<CodexScrollArea>((area, _) => area.SyncClasses());
        IsAtEndProperty.Changed.AddClassHandler<CodexScrollArea>((area, _) => area.SyncClasses());
        IsAtTopProperty.Changed.AddClassHandler<CodexScrollArea>((area, _) => area.SyncClasses());
        IsAtBottomProperty.Changed.AddClassHandler<CodexScrollArea>((area, _) => area.SyncClasses());
    }

    public CodexScrollArea()
    {
        ClipToBounds = true;
        _scrollIdleTimer = new DispatcherTimer { Interval = ScrollIdleDelay };
        _scrollIdleTimer.Tick += OnScrollIdleTimerTick;
        SyncClasses();
    }

    public event EventHandler<ScrollChangedEventArgs>? ScrollChanged;

    public CodexScrollAreaType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
        get => GetValue(HorizontalScrollBarVisibilityProperty);
        set => SetValue(HorizontalScrollBarVisibilityProperty, value);
    }

    public ScrollBarVisibility VerticalScrollBarVisibility
    {
        get => GetValue(VerticalScrollBarVisibilityProperty);
        set => SetValue(VerticalScrollBarVisibilityProperty, value);
    }

    public bool AllowAutoHide
    {
        get => GetValue(AllowAutoHideProperty);
        set => SetValue(AllowAutoHideProperty, value);
    }

    public bool IsScrollChainingEnabled
    {
        get => GetValue(IsScrollChainingEnabledProperty);
        set => SetValue(IsScrollChainingEnabledProperty, value);
    }

    public bool IsInsetContent
    {
        get => GetValue(IsInsetContentProperty);
        set => SetValue(IsInsetContentProperty, value);
    }

    public Vector Offset => GetValue(OffsetProperty);

    public Size Extent => GetValue(ExtentProperty);

    public Size Viewport => GetValue(ViewportProperty);

    public bool IsScrolling => GetValue(IsScrollingProperty);

    public bool CanScrollHorizontally => GetValue(CanScrollHorizontallyProperty);

    public bool CanScrollVertically => GetValue(CanScrollVerticallyProperty);

    public bool IsAtStart => GetValue(IsAtStartProperty);

    public bool IsAtEnd => GetValue(IsAtEndProperty);

    public bool IsAtTop => GetValue(IsAtTopProperty);

    public bool IsAtBottom => GetValue(IsAtBottomProperty);

    public bool ScrollToTop()
    {
        return SetOffset(new Vector(Offset.X, 0));
    }

    public bool ScrollToBottom()
    {
        return SetOffset(new Vector(Offset.X, Math.Max(0, Extent.Height - Viewport.Height)));
    }

    public bool ScrollToStart()
    {
        return SetOffset(new Vector(0, Offset.Y));
    }

    public bool ScrollToEnd()
    {
        return SetOffset(new Vector(Math.Max(0, Extent.Width - Viewport.Width), Offset.Y));
    }

    public bool SetOffset(Vector offset)
    {
        if (_viewport is null)
        {
            return false;
        }

        var maxX = Math.Max(0, _viewport.Extent.Width - _viewport.Viewport.Width);
        var maxY = Math.Max(0, _viewport.Extent.Height - _viewport.Viewport.Height);
        _viewport.Offset = new Vector(Math.Clamp(offset.X, 0, maxX), Math.Clamp(offset.Y, 0, maxY));
        SyncScrollMetrics();
        return true;
    }

    internal void SyncScrollMetricsForTests(Vector offset, Size extent, Size viewport)
    {
        SetValue(OffsetProperty, offset);
        SetValue(ExtentProperty, extent);
        SetValue(ViewportProperty, viewport);
        SyncScrollBoundaryState(offset, extent, viewport);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_viewport is not null)
        {
            _viewport.ScrollChanged -= OnViewportScrollChanged;
        }

        base.OnApplyTemplate(e);

        _viewport = e.NameScope.Find<ScrollViewer>("PART_Viewport");

        if (_viewport is not null)
        {
            _viewport.ScrollChanged += OnViewportScrollChanged;
            SyncScrollMetrics();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _scrollIdleTimer.Stop();
        base.OnDetachedFromVisualTree(e);
    }

    private void OnViewportScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        SetValue(IsScrollingProperty, true);
        _scrollIdleTimer.Stop();
        _scrollIdleTimer.Start();
        SyncScrollMetrics();
        ScrollChanged?.Invoke(this, e);
    }

    private void OnScrollIdleTimerTick(object? sender, EventArgs e)
    {
        _scrollIdleTimer.Stop();
        SetValue(IsScrollingProperty, false);
    }

    private void SyncScrollMetrics()
    {
        if (_viewport is null)
        {
            SyncScrollBoundaryState(Offset, Extent, Viewport);
            return;
        }

        SetValue(OffsetProperty, _viewport.Offset);
        SetValue(ExtentProperty, _viewport.Extent);
        SetValue(ViewportProperty, _viewport.Viewport);
        SyncScrollBoundaryState(_viewport.Offset, _viewport.Extent, _viewport.Viewport);
    }

    private void SyncScrollBoundaryState(Vector offset, Size extent, Size viewport)
    {
        var maxX = Math.Max(0, extent.Width - viewport.Width);
        var maxY = Math.Max(0, extent.Height - viewport.Height);

        SetValue(CanScrollHorizontallyProperty, maxX > 0.5);
        SetValue(CanScrollVerticallyProperty, maxY > 0.5);
        SetValue(IsAtStartProperty, offset.X <= 0.5);
        SetValue(IsAtEndProperty, offset.X >= maxX - 0.5);
        SetValue(IsAtTopProperty, offset.Y <= 0.5);
        SetValue(IsAtBottomProperty, offset.Y >= maxY - 0.5);
        SyncClasses();
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("type-auto", Type == CodexScrollAreaType.Auto);
        Classes.Set("type-always", Type == CodexScrollAreaType.Always);
        Classes.Set("type-hover", Type == CodexScrollAreaType.Hover);
        Classes.Set("type-scroll", Type == CodexScrollAreaType.Scroll);
        Classes.Set("horizontal-disabled", HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled);
        Classes.Set("horizontal-auto", HorizontalScrollBarVisibility == ScrollBarVisibility.Auto);
        Classes.Set("horizontal-visible", HorizontalScrollBarVisibility == ScrollBarVisibility.Visible);
        Classes.Set("vertical-disabled", VerticalScrollBarVisibility == ScrollBarVisibility.Disabled);
        Classes.Set("vertical-auto", VerticalScrollBarVisibility == ScrollBarVisibility.Auto);
        Classes.Set("vertical-visible", VerticalScrollBarVisibility == ScrollBarVisibility.Visible);
        Classes.Set("inset-content", IsInsetContent);
        Classes.Set("scrolling", IsScrolling);
        Classes.Set("can-scroll-x", CanScrollHorizontally);
        Classes.Set("can-scroll-y", CanScrollVertically);
        Classes.Set("at-start", IsAtStart);
        Classes.Set("at-end", IsAtEnd);
        Classes.Set("at-top", IsAtTop);
        Classes.Set("at-bottom", IsAtBottom);
    }
}
