using System.Collections.Specialized;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Themes;

namespace CodexSwitchUI.Controls;

public interface ICodexLineChartPoint
{
    string Label { get; }

    double Value { get; }

    string ValueText { get; }

    string DetailText { get; }

    IBrush? AccentBrush { get; }
}

public sealed record CodexLineChartPoint(
    string Label,
    double Value,
    string ValueText = "",
    string DetailText = "",
    IBrush? AccentBrush = null) : ICodexLineChartPoint;

public sealed class CodexLineChartActivePointChangedEventArgs(
    int oldIndex,
    int newIndex,
    ICodexLineChartPoint? oldPoint,
    ICodexLineChartPoint? newPoint)
    : EventArgs
{
    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public ICodexLineChartPoint? OldPoint { get; } = oldPoint;

    public ICodexLineChartPoint? NewPoint { get; } = newPoint;
}

public class CodexLineChart : TemplatedControl
{
    private static readonly TimeSpan AnimationFrameInterval = TimeSpan.FromMilliseconds(16);
    private static readonly TimeSpan DefaultAnimationDuration = CodexSwitchThemeOptions.ShadcnDefault.MotionDurationSlow;
    private const double AnimationSnapThreshold = 0.002d;
    private const double HoverLerpFactor = 0.24d;
    private const double TooltipLerpFactor = 0.28d;
    private static readonly IBrush DefaultForegroundBrush = Brushes.Black;
    private static readonly IBrush DefaultMutedForegroundBrush = Brushes.Gray;
    private static readonly IBrush DefaultGridBrush = new ImmutableSolidColorBrush(Color.FromArgb(34, 120, 120, 120));
    private static readonly IBrush DefaultLineBrush = new ImmutableSolidColorBrush(Color.Parse("#3B82F6"));
    private static readonly IBrush DefaultAreaBrush = new ImmutableSolidColorBrush(Color.FromArgb(42, 59, 130, 246));
    private static readonly IBrush DefaultTooltipBackgroundBrush = new ImmutableSolidColorBrush(Color.FromArgb(242, 24, 24, 27));
    private static readonly IBrush DefaultTooltipForegroundBrush = Brushes.White;
    private static readonly IBrush DefaultTooltipBorderBrush = new ImmutableSolidColorBrush(Color.FromArgb(52, 255, 255, 255));
    private static readonly IBrush EmptyTextBrush = new ImmutableSolidColorBrush(Color.Parse("#9CA3AF"));

    public static readonly StyledProperty<IEnumerable<ICodexLineChartPoint>?> ItemsSourceProperty =
        AvaloniaProperty.Register<CodexLineChart, IEnumerable<ICodexLineChartPoint>?>(nameof(ItemsSource));

    public static readonly StyledProperty<string> EmptyTextProperty =
        AvaloniaProperty.Register<CodexLineChart, string>(nameof(EmptyText), "No data");

    public static readonly StyledProperty<bool> ShowAreaProperty =
        AvaloniaProperty.Register<CodexLineChart, bool>(nameof(ShowArea), true);

    public static readonly StyledProperty<bool> ShowDotsProperty =
        AvaloniaProperty.Register<CodexLineChart, bool>(nameof(ShowDots), true);

    public static readonly StyledProperty<bool> ShowGridLinesProperty =
        AvaloniaProperty.Register<CodexLineChart, bool>(nameof(ShowGridLines), true);

    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<CodexLineChart, bool>(nameof(IsCompact));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexLineChart, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<int> ActiveIndexProperty =
        AvaloniaProperty.Register<CodexLineChart, int>(nameof(ActiveIndex), -1);

    public static readonly StyledProperty<IBrush?> MutedForegroundProperty =
        AvaloniaProperty.Register<CodexLineChart, IBrush?>(nameof(MutedForeground));

    public static readonly StyledProperty<IBrush?> GridBrushProperty =
        AvaloniaProperty.Register<CodexLineChart, IBrush?>(nameof(GridBrush));

    public static readonly StyledProperty<IBrush?> LineBrushProperty =
        AvaloniaProperty.Register<CodexLineChart, IBrush?>(nameof(LineBrush));

    public static readonly StyledProperty<IBrush?> AreaBrushProperty =
        AvaloniaProperty.Register<CodexLineChart, IBrush?>(nameof(AreaBrush));

    public static readonly StyledProperty<IBrush?> DotBrushProperty =
        AvaloniaProperty.Register<CodexLineChart, IBrush?>(nameof(DotBrush));

    public static readonly StyledProperty<IBrush?> ActiveDotBrushProperty =
        AvaloniaProperty.Register<CodexLineChart, IBrush?>(nameof(ActiveDotBrush));

    public static readonly StyledProperty<IBrush?> TooltipBackgroundProperty =
        AvaloniaProperty.Register<CodexLineChart, IBrush?>(nameof(TooltipBackground));

    public static readonly StyledProperty<IBrush?> TooltipForegroundProperty =
        AvaloniaProperty.Register<CodexLineChart, IBrush?>(nameof(TooltipForeground));

    public static readonly StyledProperty<IBrush?> TooltipBorderBrushProperty =
        AvaloniaProperty.Register<CodexLineChart, IBrush?>(nameof(TooltipBorderBrush));

    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<CodexLineChart, TimeSpan>(nameof(AnimationDuration), DefaultAnimationDuration);

    private ICodexLineChartPoint[] _items = [];
    private INotifyCollectionChanged? _observedItemsSource;
    private DispatcherTimer? _animationTimer;
    private DateTimeOffset _chartAnimationStartedAt = DateTimeOffset.UtcNow;
    private Point? _targetPointerPosition;
    private Point? _tooltipPosition;
    private double _chartProgress = 1d;
    private double _hoverProgress;
    private double _targetHoverProgress;
    private bool _itemsDirty = true;
    private bool _refreshQueued;

    static CodexLineChart()
    {
        AffectsRender<CodexLineChart>(
            ItemsSourceProperty,
            EmptyTextProperty,
            ShowAreaProperty,
            ShowDotsProperty,
            ShowGridLinesProperty,
            ActiveIndexProperty,
            MutedForegroundProperty,
            GridBrushProperty,
            LineBrushProperty,
            AreaBrushProperty,
            DotBrushProperty,
            ActiveDotBrushProperty,
            TooltipBackgroundProperty,
            TooltipForegroundProperty,
            TooltipBorderBrushProperty,
            BackgroundProperty,
            BorderBrushProperty,
            BorderThicknessProperty,
            CornerRadiusProperty,
            FontFamilyProperty,
            FontSizeProperty,
            FontStyleProperty,
            FontWeightProperty,
            FontStretchProperty,
            ForegroundProperty,
            PaddingProperty);

        AffectsMeasure<CodexLineChart>(
            ItemsSourceProperty,
            IsCompactProperty,
            PaddingProperty,
            BorderThicknessProperty,
            FontSizeProperty);

        ItemsSourceProperty.Changed.AddClassHandler<CodexLineChart>((chart, args) =>
            chart.OnItemsSourceChanged(
                args.OldValue as IEnumerable<ICodexLineChartPoint>,
                args.NewValue as IEnumerable<ICodexLineChartPoint>));
        ShowAreaProperty.Changed.AddClassHandler<CodexLineChart>((chart, _) => chart.SyncClasses());
        ShowDotsProperty.Changed.AddClassHandler<CodexLineChart>((chart, _) => chart.SyncClasses());
        ShowGridLinesProperty.Changed.AddClassHandler<CodexLineChart>((chart, _) => chart.SyncClasses());
        IsCompactProperty.Changed.AddClassHandler<CodexLineChart>((chart, _) => chart.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexLineChart>((chart, _) => chart.SyncClasses());
        ActiveIndexProperty.Changed.AddClassHandler<CodexLineChart>((chart, args) => chart.OnActiveIndexChanged(args));
        AnimationDurationProperty.Changed.AddClassHandler<CodexLineChart>((chart, _) => chart.SyncChartAnimationDuration());
    }

    public CodexLineChart()
    {
        ClipToBounds = true;
        Focusable = false;
        PointerMoved += OnPointerMoved;
        PointerExited += OnPointerExited;
        SyncClasses();
    }

    public event EventHandler<CodexLineChartActivePointChangedEventArgs>? ActivePointChanged;

    public IEnumerable<ICodexLineChartPoint>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string EmptyText
    {
        get => GetValue(EmptyTextProperty);
        set => SetValue(EmptyTextProperty, value);
    }

    public bool ShowArea
    {
        get => GetValue(ShowAreaProperty);
        set => SetValue(ShowAreaProperty, value);
    }

    public bool ShowDots
    {
        get => GetValue(ShowDotsProperty);
        set => SetValue(ShowDotsProperty, value);
    }

    public bool ShowGridLines
    {
        get => GetValue(ShowGridLinesProperty);
        set => SetValue(ShowGridLinesProperty, value);
    }

    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public int ActiveIndex
    {
        get => GetValue(ActiveIndexProperty);
        set => SetValue(ActiveIndexProperty, value);
    }

    public IBrush? MutedForeground
    {
        get => GetValue(MutedForegroundProperty);
        set => SetValue(MutedForegroundProperty, value);
    }

    public IBrush? GridBrush
    {
        get => GetValue(GridBrushProperty);
        set => SetValue(GridBrushProperty, value);
    }

    public IBrush? LineBrush
    {
        get => GetValue(LineBrushProperty);
        set => SetValue(LineBrushProperty, value);
    }

    public IBrush? AreaBrush
    {
        get => GetValue(AreaBrushProperty);
        set => SetValue(AreaBrushProperty, value);
    }

    public IBrush? DotBrush
    {
        get => GetValue(DotBrushProperty);
        set => SetValue(DotBrushProperty, value);
    }

    public IBrush? ActiveDotBrush
    {
        get => GetValue(ActiveDotBrushProperty);
        set => SetValue(ActiveDotBrushProperty, value);
    }

    public IBrush? TooltipBackground
    {
        get => GetValue(TooltipBackgroundProperty);
        set => SetValue(TooltipBackgroundProperty, value);
    }

    public IBrush? TooltipForeground
    {
        get => GetValue(TooltipForegroundProperty);
        set => SetValue(TooltipForegroundProperty, value);
    }

    public IBrush? TooltipBorderBrush
    {
        get => GetValue(TooltipBorderBrushProperty);
        set => SetValue(TooltipBorderBrushProperty, value);
    }

    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    public ICodexLineChartPoint? ActivePoint
    {
        get
        {
            EnsureItems();
            return ActiveIndex >= 0 && ActiveIndex < _items.Length ? _items[ActiveIndex] : null;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (_observedItemsSource is null && ItemsSource is INotifyCollectionChanged observed)
        {
            _observedItemsSource = observed;
            _observedItemsSource.CollectionChanged += OnObservedItemsSourceChanged;
        }

        if (_chartProgress < 1d || Math.Abs(_targetHoverProgress - _hoverProgress) > AnimationSnapThreshold)
            StartAnimationTimer();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopAnimationTimer();
        if (_observedItemsSource is not null)
            _observedItemsSource.CollectionChanged -= OnObservedItemsSourceChanged;
        _observedItemsSource = null;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        EnsureItems();

        var desiredWidth = double.IsInfinity(availableSize.Width) ? 360d : availableSize.Width;
        var contentHeight = IsCompact ? 148d : 190d;
        if (_items.Length == 0)
            contentHeight = IsCompact ? 104d : 126d;

        return new Size(
            desiredWidth,
            contentHeight + Padding.Top + Padding.Bottom + BorderThickness.Top + BorderThickness.Bottom);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        EnsureItems();

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        DrawSurface(context, bounds);

        var content = bounds.Deflate(BorderThickness).Deflate(Padding);
        if (content.Width <= 0 || content.Height <= 0)
            return;

        if (_items.Length == 0)
        {
            DrawEmptyState(context, content);
            return;
        }

        var plot = GetPlotRect(content);
        var scale = CreateScale();
        var progress = EaseOutCubic(_chartProgress);
        var points = CreatePoints(plot, scale.Min, scale.Max, progress);

        if (ShowGridLines)
            DrawGrid(context, plot);

        DrawSeries(context, plot, points, progress);
        DrawAxesText(context, content, plot, scale);
        DrawActivePoint(context, plot, points);
        DrawTooltip(context, bounds);
    }

    private void DrawSurface(DrawingContext context, Rect bounds)
    {
        var borderThickness = BorderThickness;
        var borderPen = BorderBrush is null || borderThickness.Left <= 0
            ? null
            : new Pen(BorderBrush, Math.Max(1, borderThickness.Left));
        var radius = Math.Max(0, CornerRadius.TopLeft);
        context.DrawRectangle(Background, borderPen, bounds.Deflate(borderThickness.Left / 2d), radius, radius);
    }

    private void DrawGrid(DrawingContext context, Rect plot)
    {
        var pen = new Pen(GridBrush ?? DefaultGridBrush, 1d);
        var rows = IsCompact ? 2 : 3;
        for (var index = 0; index <= rows; index++)
        {
            var y = plot.Y + plot.Height * index / rows;
            context.DrawLine(pen, new Point(plot.X, y), new Point(plot.Right, y));
        }
    }

    private void DrawSeries(DrawingContext context, Rect plot, Point[] points, double progress)
    {
        if (points.Length == 0)
            return;

        var clip = new Rect(plot.X, plot.Y - 8d, plot.Width * progress, plot.Height + 16d);
        using var reveal = context.PushClip(clip);

        if (ShowArea && points.Length >= 2)
        {
            var areaGeometry = BuildAreaGeometry(points, plot.Bottom);
            context.DrawGeometry(AreaBrush ?? DefaultAreaBrush, null, areaGeometry);
        }

        if (points.Length == 1)
        {
            DrawPoint(context, points[0], ResolveLineBrush(0), 4d);
            return;
        }

        var lineGeometry = BuildLineGeometry(points);
        if (lineGeometry is not null)
        {
            var linePen = new Pen(LineBrush ?? ResolveLineBrush(0), IsCompact ? 2d : 2.4d);
            context.DrawGeometry(null, linePen, lineGeometry);
        }

        if (!ShowDots)
            return;

        var dotRadius = IsCompact ? 3d : 3.6d;
        for (var index = 0; index < points.Length; index++)
        {
            DrawPoint(context, points[index], _items[index].AccentBrush ?? DotBrush ?? LineBrush ?? DefaultLineBrush, dotRadius);
        }
    }

    private void DrawAxesText(DrawingContext context, Rect content, Rect plot, (double Min, double Max) scale)
    {
        if (IsCompact || _items.Length == 0)
            return;

        var muted = MutedForeground ?? DefaultMutedForegroundBrush;
        var fontSize = Math.Max(10d, FontSize - 1d);
        var typeface = CreateTypeface(FontWeight.Normal);
        var first = _items[0].Label;
        var last = _items[^1].Label;
        DrawTextLayout(
            context,
            CreateTextLayout(first, typeface, fontSize, muted, plot.Width * 0.48d),
            new Point(plot.X, plot.Bottom + 8d));
        DrawTextLayout(
            context,
            CreateTextLayout(last, typeface, fontSize, muted, plot.Width * 0.48d, TextAlignment.Right),
            new Point(plot.Right, plot.Bottom + 8d),
            TextAlignment.Right);

        DrawTextLayout(
            context,
            CreateTextLayout(FormatValue(scale.Max), typeface, fontSize, muted, 72d, TextAlignment.Right),
            new Point(content.Right, plot.Y - 2d),
            TextAlignment.Right);
    }

    private void DrawActivePoint(DrawingContext context, Rect plot, Point[] points)
    {
        if (ActiveIndex < 0 || ActiveIndex >= points.Length || _hoverProgress <= 0.01d)
            return;

        var opacity = EaseOutCubic(_hoverProgress);
        var point = points[ActiveIndex];
        using var activeOpacity = context.PushOpacity(opacity);
        context.DrawLine(
            new Pen(GridBrush ?? DefaultGridBrush, 1d),
            new Point(point.X, plot.Y),
            new Point(point.X, plot.Bottom));
        DrawPoint(context, point, ActiveDotBrush ?? _items[ActiveIndex].AccentBrush ?? LineBrush ?? DefaultLineBrush, IsCompact ? 5d : 5.8d);
        DrawPoint(context, point, Background ?? Brushes.White, IsCompact ? 2.2d : 2.8d);
    }

    private void DrawTooltip(DrawingContext context, Rect bounds)
    {
        if (ActiveIndex < 0 ||
            ActiveIndex >= _items.Length ||
            _tooltipPosition is not { } pointer ||
            _hoverProgress <= 0.01d)
        {
            return;
        }

        var item = _items[ActiveIndex];
        var foreground = TooltipForeground ?? DefaultTooltipForegroundBrush;
        var muted = TooltipForeground ?? DefaultTooltipForegroundBrush;
        var typeface = CreateTypeface(FontWeight.SemiBold);
        var valueTypeface = CreateTypeface(FontWeight.Normal);
        var label = CreateTextLayout(item.Label, typeface, Math.Max(11d, FontSize), foreground, 160d);
        var valueText = string.IsNullOrWhiteSpace(item.ValueText) ? FormatValue(item.Value) : item.ValueText;
        var value = CreateTextLayout(valueText, valueTypeface, Math.Max(11d, FontSize), foreground, 160d);
        var detail = string.IsNullOrWhiteSpace(item.DetailText)
            ? null
            : CreateTextLayout(item.DetailText, valueTypeface, Math.Max(10d, FontSize - 1d), muted, 160d);
        var width = Math.Max(label.Width, Math.Max(value.Width, detail?.Width ?? 0d)) + 24d;
        var height = 14d + label.Height + value.Height + (detail?.Height ?? 0d) + (detail is null ? 8d : 14d);
        var x = pointer.X + 14d;
        var y = pointer.Y + 14d;

        if (x + width > bounds.Right - 4d)
            x = pointer.X - width - 14d;
        if (y + height > bounds.Bottom - 4d)
            y = pointer.Y - height - 14d;

        x = Math.Clamp(x, bounds.X + 4d, Math.Max(bounds.X + 4d, bounds.Right - width - 4d));
        y = Math.Clamp(y, bounds.Y + 4d, Math.Max(bounds.Y + 4d, bounds.Bottom - height - 4d));

        using var tooltipOpacity = context.PushOpacity(EaseOutCubic(_hoverProgress));
        var rect = new Rect(x, y, width, height);
        context.DrawRectangle(
            TooltipBackground ?? DefaultTooltipBackgroundBrush,
            new Pen(TooltipBorderBrush ?? DefaultTooltipBorderBrush, 1d),
            rect,
            8d,
            8d);

        var markerBrush = item.AccentBrush ?? LineBrush ?? DefaultLineBrush;
        context.DrawEllipse(markerBrush, null, new Point(rect.X + 12d, rect.Y + 15d), 4.5d, 4.5d);
        DrawTextLayout(context, label, new Point(rect.X + 23d, rect.Y + 8d));
        DrawTextLayout(context, value, new Point(rect.X + 12d, rect.Y + 10d + label.Height));

        if (detail is not null)
            DrawTextLayout(context, detail, new Point(rect.X + 12d, rect.Y + 14d + label.Height + value.Height));
    }

    private void DrawEmptyState(DrawingContext context, Rect content)
    {
        var layout = CreateTextLayout(
            string.IsNullOrWhiteSpace(EmptyText) ? "No data" : EmptyText,
            CreateTypeface(FontWeight.Normal),
            Math.Max(11d, FontSize),
            MutedForeground ?? EmptyTextBrush,
            content.Width,
            TextAlignment.Center);
        DrawTextLayout(
            context,
            layout,
            new Point(content.Center.X, content.Center.Y - layout.Height / 2d),
            TextAlignment.Center);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        EnsureItems();
        var position = e.GetPosition(this);
        var nextIndex = HitTest(position);

        if (nextIndex >= 0)
        {
            ActiveIndex = nextIndex;
            _targetHoverProgress = 1d;
            _targetPointerPosition = position;
            _tooltipPosition ??= position;
            StartAnimationTimer();
            return;
        }

        _targetHoverProgress = 0d;
        _targetPointerPosition = position;
        StartAnimationTimer();
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        _targetHoverProgress = 0d;
        StartAnimationTimer();
    }

    private int HitTest(Point position)
    {
        if (_items.Length == 0)
            return -1;

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return -1;

        var content = bounds.Deflate(BorderThickness).Deflate(Padding);
        var plot = GetPlotRect(content);
        if (plot.Width <= 0 || plot.Height <= 0)
            return -1;

        var horizontalPadding = _items.Length == 1 ? plot.Width / 2d : Math.Max(18d, plot.Width / Math.Max(1, _items.Length - 1) / 2d);
        if (position.X < plot.X - horizontalPadding || position.X > plot.Right + horizontalPadding)
            return -1;

        var step = _items.Length <= 1 ? 0d : plot.Width / (_items.Length - 1);
        return _items.Length == 1
            ? 0
            : Math.Clamp((int)Math.Round((position.X - plot.X) / step), 0, _items.Length - 1);
    }

    private void StartChartAnimation()
    {
        if (AnimationDuration <= TimeSpan.Zero)
        {
            _chartProgress = 1d;
            StopAnimationTimer();
            InvalidateVisual();
            return;
        }

        _chartAnimationStartedAt = DateTimeOffset.UtcNow;
        _chartProgress = 0d;
        StartAnimationTimer();
        InvalidateVisual();
    }

    private void SyncChartAnimationDuration()
    {
        if (AnimationDuration <= TimeSpan.Zero && _chartProgress < 1d)
        {
            _chartProgress = 1d;
            StopAnimationTimer();
            InvalidateVisual();
        }
    }

    private void StartAnimationTimer()
    {
        if (_animationTimer is not null)
            return;

        _animationTimer = new DispatcherTimer { Interval = AnimationFrameInterval };
        _animationTimer.Tick += OnAnimationTick;
        _animationTimer.Start();
    }

    private void StopAnimationTimer()
    {
        if (_animationTimer is null)
            return;

        _animationTimer.Stop();
        _animationTimer.Tick -= OnAnimationTick;
        _animationTimer = null;
    }

    private void OnAnimationTick(object? sender, EventArgs e)
    {
        var invalidated = false;
        if (_chartProgress < 1d)
        {
            var elapsed = DateTimeOffset.UtcNow - _chartAnimationStartedAt;
            var duration = AnimationDuration.TotalMilliseconds;
            _chartProgress = duration <= 0
                ? 1d
                : Math.Clamp(elapsed.TotalMilliseconds / duration, 0d, 1d);
            invalidated = true;
        }

        if (Math.Abs(_targetHoverProgress - _hoverProgress) > AnimationSnapThreshold)
        {
            _hoverProgress += (_targetHoverProgress - _hoverProgress) * HoverLerpFactor;
            if (Math.Abs(_targetHoverProgress - _hoverProgress) <= AnimationSnapThreshold)
                _hoverProgress = _targetHoverProgress;
            invalidated = true;
        }

        if (_targetPointerPosition is { } target && _tooltipPosition is { } current)
        {
            var next = Lerp(current, target, TooltipLerpFactor);
            if (Distance(next, target) < 0.5d)
                next = target;

            if (!SamePoint(current, next))
            {
                _tooltipPosition = next;
                invalidated = true;
            }
        }

        if (_targetHoverProgress <= 0d && _hoverProgress <= AnimationSnapThreshold)
        {
            _hoverProgress = 0d;
            ActiveIndex = -1;
            _targetPointerPosition = null;
            _tooltipPosition = null;
        }

        if (invalidated)
            InvalidateVisual();

        var tooltipSettled = _targetPointerPosition is null ||
            _tooltipPosition is null ||
            Distance(_tooltipPosition.Value, _targetPointerPosition.Value) < 0.5d;
        if (_chartProgress >= 1d &&
            Math.Abs(_targetHoverProgress - _hoverProgress) <= AnimationSnapThreshold &&
            tooltipSettled)
        {
            StopAnimationTimer();
        }
    }

    private void OnItemsSourceChanged(
        IEnumerable<ICodexLineChartPoint>? oldValue,
        IEnumerable<ICodexLineChartPoint>? newValue)
    {
        if (_observedItemsSource is not null)
            _observedItemsSource.CollectionChanged -= OnObservedItemsSourceChanged;

        _observedItemsSource = newValue as INotifyCollectionChanged;
        if (_observedItemsSource is not null)
            _observedItemsSource.CollectionChanged += OnObservedItemsSourceChanged;

        _itemsDirty = true;
        ActiveIndex = -1;
        ResetHover();
        StartChartAnimation();
        InvalidateMeasure();
        InvalidateVisual();
    }

    private void OnObservedItemsSourceChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _itemsDirty = true;
        ActiveIndex = -1;
        ResetHover();

        if (_refreshQueued)
            return;

        _refreshQueued = true;
        Dispatcher.UIThread.Post(() =>
        {
            _refreshQueued = false;
            StartChartAnimation();
            InvalidateMeasure();
            InvalidateVisual();
        }, DispatcherPriority.Background);
    }

    private void OnActiveIndexChanged(AvaloniaPropertyChangedEventArgs args)
    {
        EnsureItems();

        var oldIndex = args.OldValue is int oldValue ? oldValue : -1;
        var newIndex = ActiveIndex >= 0 && ActiveIndex < _items.Length ? ActiveIndex : -1;
        if (ActiveIndex != newIndex)
        {
            SetCurrentValue(ActiveIndexProperty, newIndex);
            return;
        }

        var oldPoint = oldIndex >= 0 && oldIndex < _items.Length ? _items[oldIndex] : null;
        var newPoint = newIndex >= 0 && newIndex < _items.Length ? _items[newIndex] : null;
        SyncClasses();
        if (oldIndex != newIndex)
        {
            ActivePointChanged?.Invoke(this, new CodexLineChartActivePointChangedEventArgs(oldIndex, newIndex, oldPoint, newPoint));
        }

        InvalidateVisual();
    }

    private void ResetHover()
    {
        _hoverProgress = 0d;
        _targetHoverProgress = 0d;
        _targetPointerPosition = null;
        _tooltipPosition = null;
    }

    private void EnsureItems()
    {
        if (!_itemsDirty)
            return;

        _items = ToArray(ItemsSource);
        _itemsDirty = false;
        SyncClasses();
    }

    private Point[] CreatePoints(Rect plot, double minValue, double maxValue, double progress)
    {
        var points = new Point[_items.Length];
        if (_items.Length == 0)
            return points;

        if (_items.Length == 1)
        {
            points[0] = new Point(plot.Center.X, GetY(plot, _items[0].Value, minValue, maxValue, progress));
            return points;
        }

        var step = plot.Width / (_items.Length - 1);
        for (var index = 0; index < _items.Length; index++)
        {
            points[index] = new Point(
                plot.X + step * index,
                GetY(plot, _items[index].Value, minValue, maxValue, progress));
        }

        return points;
    }

    private Rect GetPlotRect(Rect content)
    {
        var axisHeight = IsCompact ? 2d : 25d;
        var topInset = IsCompact ? 4d : 12d;
        return new Rect(
            content.X,
            content.Y + topInset,
            content.Width,
            Math.Max(1d, content.Height - topInset - axisHeight));
    }

    private (double Min, double Max) CreateScale()
    {
        var min = _items.Min(item => item.Value);
        var max = _items.Max(item => item.Value);
        if (min > 0)
            min = Math.Min(0, min * 0.92d);

        if (Math.Abs(max - min) < 0.0001d)
        {
            max += 1d;
            min -= 1d;
        }

        var padding = (max - min) * 0.08d;
        return (min - padding, max + padding);
    }

    private double GetY(Rect plot, double value, double minValue, double maxValue, double progress)
    {
        var normalized = Math.Clamp((value - minValue) / (maxValue - minValue), 0d, 1d);
        var target = plot.Bottom - plot.Height * normalized;
        return Lerp(plot.Bottom, target, progress);
    }

    private IBrush ResolveLineBrush(int index)
    {
        return _items.Length > index && _items[index].AccentBrush is { } accent
            ? accent
            : LineBrush ?? DefaultLineBrush;
    }

    private void SyncClasses()
    {
        Classes.Set("line-chart", true);
        Classes.Set("area", ShowArea);
        Classes.Set("line-only", !ShowArea);
        Classes.Set("dots", ShowDots);
        Classes.Set("no-dots", !ShowDots);
        Classes.Set("grid", ShowGridLines);
        Classes.Set("no-grid", !ShowGridLines);
        Classes.Set("compact", IsCompact);
        Classes.Set("empty", _items.Length == 0);
        Classes.Set("has-active-point", ActiveIndex >= 0);
        CodexClassSync.SetSize(Classes, Size);
    }

    private static ICodexLineChartPoint[] ToArray(IEnumerable<ICodexLineChartPoint>? source)
    {
        return source switch
        {
            null => [],
            ICodexLineChartPoint[] array => array.Where(IsValidPoint).ToArray(),
            ICollection<ICodexLineChartPoint> collection => ToArray(collection),
            IReadOnlyCollection<ICodexLineChartPoint> collection => ToArray(collection),
            _ => source.Where(IsValidPoint).ToArray()
        };
    }

    private static ICodexLineChartPoint[] ToArray(ICollection<ICodexLineChartPoint> collection)
    {
        var array = new ICodexLineChartPoint[collection.Count];
        collection.CopyTo(array, 0);
        return array.Where(IsValidPoint).ToArray();
    }

    private static ICodexLineChartPoint[] ToArray(IReadOnlyCollection<ICodexLineChartPoint> collection)
    {
        var array = new ICodexLineChartPoint[collection.Count];
        var index = 0;
        foreach (var item in collection)
            array[index++] = item;
        return array.Where(IsValidPoint).ToArray();
    }

    private static bool IsValidPoint(ICodexLineChartPoint item)
    {
        return !double.IsNaN(item.Value) && !double.IsInfinity(item.Value);
    }

    private static Geometry BuildAreaGeometry(IReadOnlyList<Point> points, double baseline)
    {
        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(new Point(points[0].X, baseline), isFilled: true);
            ctx.LineTo(points[0]);
            AddSmoothSegments(ctx, points, 0, points.Count - 1, 1);
            ctx.LineTo(new Point(points[^1].X, baseline));
            ctx.EndFigure(isClosed: true);
        }

        return geometry;
    }

    private static Geometry? BuildLineGeometry(IReadOnlyList<Point> points)
    {
        if (points.Count < 2)
            return null;

        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(points[0], isFilled: false);
            AddSmoothSegments(ctx, points, 0, points.Count - 1, 1);
            ctx.EndFigure(isClosed: false);
        }

        return geometry;
    }

    private static void AddSmoothSegments(
        StreamGeometryContext context,
        IReadOnlyList<Point> points,
        int startIndex,
        int endIndex,
        int step)
    {
        if (points.Count < 2 || startIndex == endIndex)
            return;

        for (var index = startIndex; index != endIndex; index += step)
        {
            var nextIndex = index + step;
            var previousIndex = index == startIndex ? index : index - step;
            var afterNextIndex = nextIndex == endIndex ? nextIndex : nextIndex + step;
            var previous = points[previousIndex];
            var current = points[index];
            var next = points[nextIndex];
            var afterNext = points[afterNextIndex];
            var control1 = new Point(
                current.X + (next.X - previous.X) * 0.23d,
                current.Y + (next.Y - previous.Y) * 0.23d);
            var control2 = new Point(
                next.X - (afterNext.X - current.X) * 0.23d,
                next.Y - (afterNext.Y - current.Y) * 0.23d);

            var minY = Math.Min(current.Y, next.Y);
            var maxY = Math.Max(current.Y, next.Y);
            control1 = new Point(control1.X, Math.Clamp(control1.Y, minY, maxY));
            control2 = new Point(control2.X, Math.Clamp(control2.Y, minY, maxY));
            context.CubicBezierTo(control1, control2, next);
        }
    }

    private static void DrawPoint(DrawingContext context, Point point, IBrush brush, double radius)
    {
        context.DrawEllipse(brush, null, point, radius, radius);
    }

    private static TextLayout CreateTextLayout(
        string? text,
        Typeface typeface,
        double fontSize,
        IBrush brush,
        double maxWidth,
        TextAlignment alignment = TextAlignment.Left)
    {
        return new TextLayout(
            text ?? string.Empty,
            typeface,
            fontSize,
            brush,
            textAlignment: alignment,
            textWrapping: TextWrapping.NoWrap,
            textTrimming: TextTrimming.CharacterEllipsis,
            flowDirection: CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft
                ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight,
            maxWidth: Math.Max(1d, maxWidth),
            maxLines: 1);
    }

    private static void DrawTextLayout(
        DrawingContext context,
        TextLayout layout,
        Point origin,
        TextAlignment alignment = TextAlignment.Left)
    {
        var x = alignment switch
        {
            TextAlignment.Center => origin.X - layout.Width / 2d,
            TextAlignment.Right => origin.X - layout.Width,
            _ => origin.X
        };
        layout.Draw(context, new Point(Math.Round(x), Math.Round(origin.Y)));
    }

    private Typeface CreateTypeface(FontWeight weight)
    {
        return new Typeface(FontFamily, FontStyle, weight, FontStretch);
    }

    private static string FormatValue(double value)
    {
        return value switch
        {
            >= 1_000_000 => $"{value / 1_000_000d:0.#}M",
            >= 1_000 => $"{value / 1_000d:0.#}K",
            _ => value.ToString("0.#", CultureInfo.InvariantCulture)
        };
    }

    private static double EaseOutCubic(double value)
    {
        var clamped = Math.Clamp(value, 0d, 1d);
        return 1d - Math.Pow(1d - clamped, 3d);
    }

    private static Point Lerp(Point from, Point to, double amount)
    {
        return new Point(
            Lerp(from.X, to.X, amount),
            Lerp(from.Y, to.Y, amount));
    }

    private static double Lerp(double from, double to, double amount)
    {
        return from + (to - from) * amount;
    }

    private static double Distance(Point first, Point second)
    {
        var x = first.X - second.X;
        var y = first.Y - second.Y;
        return Math.Sqrt(x * x + y * y);
    }

    private static bool SamePoint(Point first, Point second)
    {
        return Math.Abs(first.X - second.X) < 0.01d && Math.Abs(first.Y - second.Y) < 0.01d;
    }
}
