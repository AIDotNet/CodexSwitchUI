using System.Collections.Specialized;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public interface ICodexUsagePieChartItem
{
    string Label { get; }

    double Value { get; }

    string ValueText { get; }

    string DetailText { get; }

    IBrush? AccentBrush { get; }
}

public sealed record CodexUsagePieChartItem(
    string Label,
    double Value,
    string ValueText,
    string DetailText = "",
    IBrush? AccentBrush = null) : ICodexUsagePieChartItem;

public class CodexUsagePieChart : TemplatedControl
{
    private static readonly TimeSpan AnimationFrameInterval = TimeSpan.FromMilliseconds(16);
    private static readonly TimeSpan ChartAnimationDuration = TimeSpan.FromMilliseconds(520);
    private const double HoverLerpFactor = 0.24d;
    private const double TooltipLerpFactor = 0.28d;
    private const double AnimationSnapThreshold = 0.002d;
    private const double InnerRadiusRatio = 0.58d;
    private const double CenterLabelSpacing = 2d;
    private const double CenterTextHorizontalInset = 8d;
    private static readonly IBrush DefaultForegroundBrush = Brushes.Black;
    private static readonly IBrush DefaultMutedForegroundBrush = Brushes.Gray;
    private static readonly IBrush DefaultTrackBrush = new SolidColorBrush(Color.FromArgb(28, 120, 120, 120));
    private static readonly IBrush DefaultSliceBorderBrush = new SolidColorBrush(Color.FromArgb(120, 9, 9, 11));
    private static readonly IBrush DefaultTooltipBackgroundBrush = new SolidColorBrush(Color.FromArgb(242, 24, 24, 27));
    private static readonly IBrush DefaultTooltipForegroundBrush = Brushes.White;
    private static readonly IBrush DefaultTooltipBorderBrush = new SolidColorBrush(Color.FromArgb(52, 255, 255, 255));
    private static readonly IBrush EmptyTextBrush = new SolidColorBrush(Color.Parse("#9CA3AF"));
    private static readonly IBrush[] Palette =
    [
        Brush("#60A5FA"),
        Brush("#34D399"),
        Brush("#F59E0B"),
        Brush("#F472B6"),
        Brush("#22D3EE"),
        Brush("#A78BFA")
    ];

    public static readonly StyledProperty<IEnumerable<ICodexUsagePieChartItem>?> ItemsSourceProperty =
        AvaloniaProperty.Register<CodexUsagePieChart, IEnumerable<ICodexUsagePieChartItem>?>(nameof(ItemsSource));

    public static readonly StyledProperty<string> EmptyTextProperty =
        AvaloniaProperty.Register<CodexUsagePieChart, string>(nameof(EmptyText), "No data");

    public static readonly StyledProperty<string> TotalLabelProperty =
        AvaloniaProperty.Register<CodexUsagePieChart, string>(nameof(TotalLabel), "Total");

    public static readonly StyledProperty<string> TotalValueProperty =
        AvaloniaProperty.Register<CodexUsagePieChart, string>(nameof(TotalValue), "");

    public static readonly StyledProperty<IBrush?> MutedForegroundProperty =
        AvaloniaProperty.Register<CodexUsagePieChart, IBrush?>(nameof(MutedForeground));

    public static readonly StyledProperty<IBrush?> TrackBrushProperty =
        AvaloniaProperty.Register<CodexUsagePieChart, IBrush?>(nameof(TrackBrush));

    public static readonly StyledProperty<IBrush?> SliceBorderBrushProperty =
        AvaloniaProperty.Register<CodexUsagePieChart, IBrush?>(nameof(SliceBorderBrush));

    public static readonly StyledProperty<IBrush?> CenterFillBrushProperty =
        AvaloniaProperty.Register<CodexUsagePieChart, IBrush?>(nameof(CenterFillBrush));

    public static readonly StyledProperty<IBrush?> TooltipBackgroundProperty =
        AvaloniaProperty.Register<CodexUsagePieChart, IBrush?>(nameof(TooltipBackground));

    public static readonly StyledProperty<IBrush?> TooltipForegroundProperty =
        AvaloniaProperty.Register<CodexUsagePieChart, IBrush?>(nameof(TooltipForeground));

    public static readonly StyledProperty<IBrush?> TooltipBorderBrushProperty =
        AvaloniaProperty.Register<CodexUsagePieChart, IBrush?>(nameof(TooltipBorderBrush));

    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<CodexUsagePieChart, bool>(nameof(IsCompact));

    private ICodexUsagePieChartItem[] _items = [];
    private PieSlice[] _slices = [];
    private Geometry?[] _sliceGeometries = [];
    private LegendTextCache[] _legendTextCaches = [];
    private TooltipTextCache[] _tooltipTextCaches = [];
    private CenterTextCache? _centerTextCache;
    private INotifyCollectionChanged? _observedItemsSource;
    private DispatcherTimer? _animationTimer;
    private DateTimeOffset _chartAnimationStartedAt = DateTimeOffset.UtcNow;
    private Rect _cachedGeometryPieRect;
    private Rect _cachedTextPieRect;
    private Rect _cachedTextLegendRect;
    private Point? _targetPointerPosition;
    private Point? _tooltipPosition;
    private int _hoveredIndex = -1;
    private double _totalValue;
    private double _chartProgress = 1d;
    private double _hoverProgress;
    private double _targetHoverProgress;
    private bool _itemsDirty = true;
    private bool _geometryDirty = true;
    private bool _textDirty = true;
    private bool _refreshQueued;

    static CodexUsagePieChart()
    {
        AffectsRender<CodexUsagePieChart>(
            ItemsSourceProperty,
            EmptyTextProperty,
            TotalLabelProperty,
            TotalValueProperty,
            MutedForegroundProperty,
            TrackBrushProperty,
            SliceBorderBrushProperty,
            CenterFillBrushProperty,
            TooltipBackgroundProperty,
            TooltipForegroundProperty,
            TooltipBorderBrushProperty,
            FontFamilyProperty,
            FontSizeProperty,
            FontStyleProperty,
            FontWeightProperty,
            FontStretchProperty,
            ForegroundProperty);

        AffectsMeasure<CodexUsagePieChart>(ItemsSourceProperty, FontSizeProperty);

        ItemsSourceProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, args) =>
            chart.OnItemsSourceChanged(
                args.OldValue as IEnumerable<ICodexUsagePieChartItem>,
                args.NewValue as IEnumerable<ICodexUsagePieChartItem>));
        EmptyTextProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, _) => chart.MarkTextDirty());
        TotalLabelProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, _) => chart.MarkTextDirty());
        TotalValueProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, _) => chart.MarkTextDirty());
        MutedForegroundProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, _) => chart.MarkTextDirty());
        TooltipForegroundProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, _) => chart.MarkTextDirty());
        FontFamilyProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, _) => chart.MarkTextDirty());
        FontSizeProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, _) => chart.MarkTextDirty());
        FontStyleProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, _) => chart.MarkTextDirty());
        FontWeightProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, _) => chart.MarkTextDirty());
        FontStretchProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, _) => chart.MarkTextDirty());
        ForegroundProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, _) => chart.MarkTextDirty());
        IsCompactProperty.Changed.AddClassHandler<CodexUsagePieChart>((chart, _) => chart.SyncClasses());
    }

    public CodexUsagePieChart()
    {
        ClipToBounds = true;
        Focusable = false;
        PointerMoved += OnPointerMoved;
        PointerExited += OnPointerExited;
        SyncClasses();
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

    public IEnumerable<ICodexUsagePieChartItem>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string EmptyText
    {
        get => GetValue(EmptyTextProperty);
        set => SetValue(EmptyTextProperty, value);
    }

    public string TotalLabel
    {
        get => GetValue(TotalLabelProperty);
        set => SetValue(TotalLabelProperty, value);
    }

    public string TotalValue
    {
        get => GetValue(TotalValueProperty);
        set => SetValue(TotalValueProperty, value);
    }

    public IBrush? MutedForeground
    {
        get => GetValue(MutedForegroundProperty);
        set => SetValue(MutedForegroundProperty, value);
    }

    public IBrush? TrackBrush
    {
        get => GetValue(TrackBrushProperty);
        set => SetValue(TrackBrushProperty, value);
    }

    public IBrush? SliceBorderBrush
    {
        get => GetValue(SliceBorderBrushProperty);
        set => SetValue(SliceBorderBrushProperty, value);
    }

    public IBrush? CenterFillBrush
    {
        get => GetValue(CenterFillBrushProperty);
        set => SetValue(CenterFillBrushProperty, value);
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

    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        EnsureItems();

        var desiredWidth = double.IsInfinity(availableSize.Width) ? 360d : availableSize.Width;
        var desiredHeight = _items.Length == 0 ? 118d : 206d;
        return new Size(desiredWidth, desiredHeight);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        EnsureItems();

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var content = bounds.Deflate(2);
        if (_items.Length == 0 || _totalValue <= 0d)
        {
            DrawEmptyState(context, content);
            return;
        }

        var layout = CreateLayout(content);
        EnsureGeometryCache(layout.PieRect);
        EnsureTextCache(layout.PieRect, layout.LegendRect);
        DrawPie(context, layout.PieRect);
        DrawCenterLabel(context, layout.PieRect);
        DrawLegend(context, layout.LegendRect);
        DrawTooltip(context, bounds);
    }

    private void DrawPie(DrawingContext context, Rect pieRect)
    {
        var center = pieRect.Center;
        var outerRadius = Math.Min(pieRect.Width, pieRect.Height) / 2d;
        var innerRadius = outerRadius * InnerRadiusRatio;
        var borderPen = new Pen(SliceBorderBrush ?? DefaultSliceBorderBrush, 1.2d);
        var hoverPen = new Pen(SliceBorderBrush ?? DefaultSliceBorderBrush, 2d);
        var chartProgress = EaseOutCubic(_chartProgress);

        context.DrawEllipse(TrackBrush ?? DefaultTrackBrush, null, center, outerRadius, outerRadius);

        if (chartProgress >= 0.999d)
        {
            for (var index = 0; index < _sliceGeometries.Length; index++)
            {
                if (_sliceGeometries[index] is { } geometry)
                    context.DrawGeometry(_items[index].AccentBrush ?? ResolveAccent(index), borderPen, geometry);
            }
        }
        else
        {
            var remainingAngle = 360d * chartProgress;
            foreach (var slice in _slices)
            {
                if (remainingAngle <= 0d)
                    break;

                var sweep = Math.Min(slice.SweepAngle, remainingAngle);
                if (sweep > 0d)
                {
                    var geometry = CreateDonutSlice(center, outerRadius, innerRadius, slice.StartAngle, sweep);
                    context.DrawGeometry(_items[slice.Index].AccentBrush ?? ResolveAccent(slice.Index), borderPen, geometry);
                }

                remainingAngle -= sweep;
            }
        }

        var hoverProgress = EaseOutCubic(_hoverProgress) * chartProgress;
        if (_hoveredIndex >= 0 && _hoveredIndex < _slices.Length && hoverProgress > 0.01d)
        {
            var slice = _slices[_hoveredIndex];
            var hoverOuterRadius = outerRadius + 4d * hoverProgress;
            var hoverInnerRadius = Math.Max(1d, innerRadius - hoverProgress);
            var geometry = CreateDonutSlice(center, hoverOuterRadius, hoverInnerRadius, slice.StartAngle, slice.SweepAngle);
            using var opacity = context.PushOpacity(hoverProgress);
            context.DrawGeometry(_items[_hoveredIndex].AccentBrush ?? ResolveAccent(_hoveredIndex), hoverPen, geometry);
        }

        context.DrawEllipse(CenterFillBrush ?? SliceBorderBrush, null, center, innerRadius - 0.5d, innerRadius - 0.5d);
    }

    private void DrawCenterLabel(DrawingContext context, Rect pieRect)
    {
        if (_centerTextCache is null)
            return;

        var chartProgress = EaseOutCubic(Math.Clamp((_chartProgress - 0.08d) / 0.92d, 0d, 1d));
        var layout = CalculateCenterLabelLayout(
            pieRect.Center,
            _centerTextCache.Value.Width,
            _centerTextCache.Value.Height,
            _centerTextCache.Label.Width,
            _centerTextCache.Label.Height,
            CenterLabelSpacing,
            (1d - chartProgress) * 8d);
        using var opacity = context.PushOpacity(chartProgress);
        DrawTextLayout(context, _centerTextCache.Value, layout.ValueOrigin);
        DrawTextLayout(context, _centerTextCache.Label, layout.LabelOrigin);
    }

    private void DrawLegend(DrawingContext context, Rect legendRect)
    {
        if (legendRect.Width <= 0 || legendRect.Height <= 0)
            return;

        var rowHeight = GetLegendRowHeight();
        var y = legendRect.Y;
        var chartProgress = EaseOutCubic(Math.Clamp((_chartProgress - 0.18d) / 0.82d, 0d, 1d));

        using (context.PushOpacity(chartProgress))
        {
            for (var index = 0; index < _items.Length && index < _legendTextCaches.Length && y + 24d <= legendRect.Bottom; index++)
            {
                var item = _items[index];
                var cache = _legendTextCaches[index];
                if (index == _hoveredIndex && _hoverProgress > 0.01d)
                {
                    var rowRect = new Rect(legendRect.X - 6d, y - 4d, legendRect.Width + 8d, Math.Min(rowHeight - 2d, 35d));
                    using var hoverOpacity = context.PushOpacity(EaseOutCubic(_hoverProgress));
                    context.DrawRectangle(TrackBrush ?? DefaultTrackBrush, null, rowRect, 7d, 7d);
                }

                var markerBrush = item.AccentBrush ?? ResolveAccent(index);
                var markerCenter = new Point(legendRect.X + 5d, y + 8d);
                context.DrawEllipse(markerBrush, null, markerCenter, 4.5d, 4.5d);

                var textX = legendRect.X + 18d;
                DrawTextLayout(context, cache.Label, new Point(textX, y));
                DrawTextLayout(context, cache.Value, new Point(legendRect.Right, y), TextAlignment.Right);

                if (cache.Detail is not null)
                    DrawTextLayout(context, cache.Detail, new Point(textX, y + cache.Label.Height + 3d));

                y += rowHeight;
            }
        }
    }

    private void DrawTooltip(DrawingContext context, Rect bounds)
    {
        if (_hoveredIndex < 0 ||
            _hoveredIndex >= _items.Length ||
            _hoveredIndex >= _tooltipTextCaches.Length ||
            _tooltipPosition is not { } pointer)
        {
            return;
        }

        var opacity = EaseOutCubic(_hoverProgress);
        if (opacity <= 0.01d)
            return;

        var item = _items[_hoveredIndex];
        var cache = _tooltipTextCaches[_hoveredIndex];
        var x = pointer.X + 14d;
        var y = pointer.Y + 14d;

        if (x + cache.Width > bounds.Right - 4d)
            x = pointer.X - cache.Width - 14d;
        if (y + cache.Height > bounds.Bottom - 4d)
            y = pointer.Y - cache.Height - 14d;

        x = Math.Clamp(x, bounds.X + 4d, Math.Max(bounds.X + 4d, bounds.Right - cache.Width - 4d));
        y = Math.Clamp(y, bounds.Y + 4d, Math.Max(bounds.Y + 4d, bounds.Bottom - cache.Height - 4d));

        using var tooltipOpacity = context.PushOpacity(opacity);
        var rect = new Rect(x, y, cache.Width, cache.Height);
        context.DrawRectangle(
            TooltipBackground ?? DefaultTooltipBackgroundBrush,
            new Pen(TooltipBorderBrush ?? DefaultTooltipBorderBrush, 1d),
            rect,
            8d,
            8d);

        var markerBrush = item.AccentBrush ?? ResolveAccent(_hoveredIndex);
        context.DrawEllipse(markerBrush, null, new Point(rect.X + 12d, rect.Y + 15d), 4.5d, 4.5d);
        DrawTextLayout(context, cache.Label, new Point(rect.X + 23d, rect.Y + 8d));
        DrawTextLayout(context, cache.Value, new Point(rect.X + 12d, rect.Y + 10d + cache.Label.Height));

        if (cache.Detail is not null)
            DrawTextLayout(context, cache.Detail, new Point(rect.X + 12d, rect.Y + 14d + cache.Label.Height + cache.Value.Height));
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
        var hoveredIndex = HitTest(position);

        if (hoveredIndex >= 0)
        {
            if (hoveredIndex != _hoveredIndex)
            {
                _hoveredIndex = hoveredIndex;
                _hoverProgress = Math.Min(_hoverProgress, 0.2d);
            }

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

        var layout = CreateLayout(bounds.Deflate(2));
        var pieIndex = HitTestPie(position, layout.PieRect);
        return pieIndex >= 0 ? pieIndex : HitTestLegend(position, layout.LegendRect);
    }

    private int HitTestPie(Point position, Rect pieRect)
    {
        var center = pieRect.Center;
        var outerRadius = Math.Min(pieRect.Width, pieRect.Height) / 2d;
        var innerRadius = outerRadius * InnerRadiusRatio;
        var deltaX = position.X - center.X;
        var deltaY = position.Y - center.Y;
        var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        if (distance < innerRadius - 2d || distance > outerRadius + 6d)
            return -1;

        var angle = Math.Atan2(deltaY, deltaX) * 180d / Math.PI;
        var relativeAngle = NormalizeAngle(angle + 90d);
        foreach (var slice in _slices)
        {
            var start = NormalizeAngle(slice.StartAngle + 90d);
            var end = start + slice.SweepAngle;
            if (relativeAngle >= start && relativeAngle <= end + 0.35d)
                return slice.Index;

            if (end > 360d && relativeAngle <= end - 360d + 0.35d)
                return slice.Index;
        }

        return -1;
    }

    private int HitTestLegend(Point position, Rect legendRect)
    {
        if (!legendRect.Contains(position))
            return -1;

        var rowHeight = GetLegendRowHeight();
        var index = (int)((position.Y - legendRect.Y) / rowHeight);
        if (index < 0 || index >= _items.Length)
            return -1;

        return legendRect.Y + index * rowHeight + 24d <= legendRect.Bottom ? index : -1;
    }

    private void StartChartAnimation()
    {
        _chartAnimationStartedAt = DateTimeOffset.UtcNow;
        _chartProgress = 0d;
        StartAnimationTimer();
        InvalidateVisual();
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
            _chartProgress = Math.Clamp(elapsed.TotalMilliseconds / ChartAnimationDuration.TotalMilliseconds, 0d, 1d);
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
            _hoveredIndex = -1;
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
        IEnumerable<ICodexUsagePieChartItem>? oldValue,
        IEnumerable<ICodexUsagePieChartItem>? newValue)
    {
        if (_observedItemsSource is not null)
            _observedItemsSource.CollectionChanged -= OnObservedItemsSourceChanged;

        _observedItemsSource = newValue as INotifyCollectionChanged;
        if (_observedItemsSource is not null)
            _observedItemsSource.CollectionChanged += OnObservedItemsSourceChanged;

        ResetHover();
        MarkDataDirty();
        StartChartAnimation();
        InvalidateMeasure();
    }

    private void OnObservedItemsSourceChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        MarkDataDirty();
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

    private void ResetHover()
    {
        _hoveredIndex = -1;
        _hoverProgress = 0d;
        _targetHoverProgress = 0d;
        _targetPointerPosition = null;
        _tooltipPosition = null;
    }

    private void MarkDataDirty()
    {
        _itemsDirty = true;
        _geometryDirty = true;
        _textDirty = true;
    }

    private void MarkTextDirty()
    {
        _textDirty = true;
        InvalidateVisual();
    }

    private void EnsureItems()
    {
        if (!_itemsDirty)
            return;

        _items = ToArray(ItemsSource);
        _totalValue = _items.Sum(item => Math.Max(0d, item.Value));
        _slices = CreateSlices(_items, _totalValue);
        _sliceGeometries = new Geometry?[_slices.Length];
        _legendTextCaches = [];
        _tooltipTextCaches = [];
        _centerTextCache = null;
        _itemsDirty = false;
        _geometryDirty = true;
        _textDirty = true;
    }

    private void EnsureGeometryCache(Rect pieRect)
    {
        if (!_geometryDirty && SameRect(_cachedGeometryPieRect, pieRect) && _sliceGeometries.Length == _slices.Length)
            return;

        var center = pieRect.Center;
        var outerRadius = Math.Min(pieRect.Width, pieRect.Height) / 2d;
        var innerRadius = outerRadius * InnerRadiusRatio;
        _sliceGeometries = new Geometry?[_slices.Length];
        for (var index = 0; index < _slices.Length; index++)
        {
            var slice = _slices[index];
            if (slice.SweepAngle > 0d)
                _sliceGeometries[index] = CreateDonutSlice(center, outerRadius, innerRadius, slice.StartAngle, slice.SweepAngle);
        }

        _cachedGeometryPieRect = pieRect;
        _geometryDirty = false;
    }

    private void EnsureTextCache(Rect pieRect, Rect legendRect)
    {
        if (!_textDirty &&
            SameRect(_cachedTextPieRect, pieRect) &&
            SameRect(_cachedTextLegendRect, legendRect) &&
            _legendTextCaches.Length == _items.Length &&
            _tooltipTextCaches.Length == _items.Length)
        {
            return;
        }

        var foreground = Foreground ?? DefaultForegroundBrush;
        var muted = MutedForeground ?? DefaultMutedForegroundBrush;
        var tooltipForeground = TooltipForeground ?? Foreground ?? DefaultTooltipForegroundBrush;
        var valueText = string.IsNullOrWhiteSpace(TotalValue) ? (_items.Length == 0 ? "" : _items[0].ValueText) : TotalValue;
        var labelText = string.IsNullOrWhiteSpace(TotalLabel) ? "Total" : TotalLabel;
        var innerDiameter = Math.Min(pieRect.Width, pieRect.Height) * InnerRadiusRatio;
        var centerTextWidth = Math.Max(1d, innerDiameter - CenterTextHorizontalInset);
        _centerTextCache = new CenterTextCache(
            CreateTextLayout(valueText, CreateTypeface(FontWeight.SemiBold), Math.Max(13d, FontSize + 2d), foreground, centerTextWidth, TextAlignment.Center),
            CreateTextLayout(labelText, CreateTypeface(FontWeight.Normal), Math.Max(10d, FontSize - 1d), muted, centerTextWidth, TextAlignment.Center));

        var labelTypeface = CreateTypeface(FontWeight.SemiBold);
        var detailTypeface = CreateTypeface(FontWeight.Normal);
        var valueWidth = Math.Min(70d, Math.Max(48d, legendRect.Width * 0.28d));
        var labelWidth = Math.Max(30d, legendRect.Width - valueWidth - 24d);
        _legendTextCaches = _items
            .Select(item => new LegendTextCache(
                CreateTextLayout(item.Label, labelTypeface, Math.Max(10d, FontSize), foreground, labelWidth),
                CreateTextLayout(item.ValueText, labelTypeface, Math.Max(10d, FontSize), foreground, valueWidth, TextAlignment.Right),
                string.IsNullOrWhiteSpace(item.DetailText)
                    ? null
                    : CreateTextLayout(item.DetailText, detailTypeface, Math.Max(9d, FontSize - 1d), muted, Math.Max(1d, legendRect.Width - 18d))))
            .ToArray();

        const double maxTooltipTextWidth = 210d;
        _tooltipTextCaches = _items
            .Select(item =>
            {
                var label = CreateTextLayout(item.Label, labelTypeface, Math.Max(11d, FontSize), tooltipForeground, maxTooltipTextWidth);
                var value = CreateTextLayout(item.ValueText, labelTypeface, Math.Max(10d, FontSize - 1d), tooltipForeground, maxTooltipTextWidth);
                var detail = string.IsNullOrWhiteSpace(item.DetailText)
                    ? null
                    : CreateTextLayout(item.DetailText, detailTypeface, Math.Max(9d, FontSize - 1d), muted, maxTooltipTextWidth);
                var width = Math.Min(
                    238d,
                    Math.Max(148d, Math.Max(label.Width + 34d, Math.Max(value.Width, detail?.Width ?? 0d) + 22d)));
                var height = 18d + label.Height + value.Height + (detail is null ? 0d : detail.Height + 4d);
                return new TooltipTextCache(label, value, detail, width, height);
            })
            .ToArray();

        _cachedTextPieRect = pieRect;
        _cachedTextLegendRect = legendRect;
        _textDirty = false;
    }

    private ChartLayout CreateLayout(Rect content)
    {
        var stacked = content.Width < 360d;
        var pieSize = stacked
            ? Math.Min(142d, Math.Min(content.Width, content.Height * 0.58d))
            : Math.Min(156d, Math.Min(content.Height - 10d, content.Width * 0.4d));
        pieSize = Math.Max(96d, pieSize);

        var pieRect = stacked
            ? new Rect(content.Center.X - pieSize / 2d, content.Y + 2d, pieSize, pieSize)
            : new Rect(content.X + 4d, content.Center.Y - pieSize / 2d, pieSize, pieSize);
        var legendRect = stacked
            ? new Rect(content.X, pieRect.Bottom + 12d, content.Width, Math.Max(0d, content.Bottom - pieRect.Bottom - 12d))
            : new Rect(pieRect.Right + 22d, content.Y + 2d, Math.Max(0d, content.Right - pieRect.Right - 22d), content.Height - 4d);

        return new ChartLayout(pieRect, legendRect);
    }

    private double GetLegendRowHeight()
    {
        return Math.Max(31d, FontSize * 2.4d);
    }

    private void SyncClasses()
    {
        Classes.Set("compact", IsCompact);
    }

    internal static CenterLabelLayout CalculateCenterLabelLayout(
        Point center,
        double valueWidth,
        double valueHeight,
        double labelWidth,
        double labelHeight,
        double spacing = CenterLabelSpacing,
        double verticalOffset = 0d)
    {
        var safeValueWidth = Math.Max(0d, valueWidth);
        var safeValueHeight = Math.Max(0d, valueHeight);
        var safeLabelWidth = Math.Max(0d, labelWidth);
        var safeLabelHeight = Math.Max(0d, labelHeight);
        var safeSpacing = Math.Max(0d, spacing);
        var combinedHeight = safeValueHeight + safeSpacing + safeLabelHeight;
        var valueTop = center.Y - combinedHeight / 2d + verticalOffset;
        var labelTop = valueTop + safeValueHeight + safeSpacing;

        return new CenterLabelLayout(
            new Point(center.X - safeValueWidth / 2d, valueTop),
            new Point(center.X - safeLabelWidth / 2d, labelTop),
            combinedHeight);
    }

    private static ICodexUsagePieChartItem[] ToArray(IEnumerable<ICodexUsagePieChartItem>? source)
    {
        return source switch
        {
            null => [],
            ICodexUsagePieChartItem[] array => array.Where(item => item.Value > 0).OrderByDescending(item => item.Value).ToArray(),
            ICollection<ICodexUsagePieChartItem> collection => ToArray(collection),
            IReadOnlyCollection<ICodexUsagePieChartItem> collection => ToArray(collection),
            _ => source.Where(item => item.Value > 0).OrderByDescending(item => item.Value).ToArray()
        };
    }

    private static ICodexUsagePieChartItem[] ToArray(ICollection<ICodexUsagePieChartItem> collection)
    {
        var array = new ICodexUsagePieChartItem[collection.Count];
        collection.CopyTo(array, 0);
        return array.Where(item => item.Value > 0).OrderByDescending(item => item.Value).ToArray();
    }

    private static ICodexUsagePieChartItem[] ToArray(IReadOnlyCollection<ICodexUsagePieChartItem> collection)
    {
        var array = new ICodexUsagePieChartItem[collection.Count];
        var index = 0;
        foreach (var item in collection)
            array[index++] = item;
        return array.Where(item => item.Value > 0).OrderByDescending(item => item.Value).ToArray();
    }

    private static PieSlice[] CreateSlices(IReadOnlyList<ICodexUsagePieChartItem> items, double total)
    {
        if (total <= 0d || items.Count == 0)
            return [];

        var slices = new PieSlice[items.Count];
        var startAngle = -90d;
        for (var index = 0; index < items.Count; index++)
        {
            var value = Math.Max(0d, items[index].Value);
            var sweep = value <= 0d ? 0d : Math.Max(0.1d, Math.Min(359.85d, value / total * 360d));
            slices[index] = new PieSlice(index, startAngle, sweep);
            startAngle += sweep;
        }

        return slices;
    }

    private IBrush ResolveAccent(int index)
    {
        return Palette[index % Palette.Length];
    }

    private Typeface CreateTypeface(FontWeight weight)
    {
        return new Typeface(FontFamily, FontStyle, weight, FontStretch);
    }

    private static Geometry CreateDonutSlice(
        Point center,
        double outerRadius,
        double innerRadius,
        double startAngle,
        double sweepAngle)
    {
        var endAngle = startAngle + sweepAngle;
        var startOuter = PointOnCircle(center, outerRadius, startAngle);
        var endOuter = PointOnCircle(center, outerRadius, endAngle);
        var startInner = PointOnCircle(center, innerRadius, startAngle);
        var endInner = PointOnCircle(center, innerRadius, endAngle);
        var isLargeArc = sweepAngle > 180d;
        var geometry = new StreamGeometry();

        using (var context = geometry.Open())
        {
            context.BeginFigure(startOuter, true);
            context.ArcTo(endOuter, new Size(outerRadius, outerRadius), 0d, isLargeArc, SweepDirection.Clockwise);
            context.LineTo(endInner);
            context.ArcTo(startInner, new Size(innerRadius, innerRadius), 0d, isLargeArc, SweepDirection.CounterClockwise);
            context.EndFigure(true);
        }

        return geometry;
    }

    private static Point PointOnCircle(Point center, double radius, double angle)
    {
        var radians = angle * Math.PI / 180d;
        return new Point(
            center.X + Math.Cos(radians) * radius,
            center.Y + Math.Sin(radians) * radius);
    }

    private static Point Lerp(Point current, Point target, double amount)
    {
        return new Point(
            current.X + (target.X - current.X) * amount,
            current.Y + (target.Y - current.Y) * amount);
    }

    private static double Distance(Point first, Point second)
    {
        var x = first.X - second.X;
        var y = first.Y - second.Y;
        return Math.Sqrt(x * x + y * y);
    }

    private static double NormalizeAngle(double angle)
    {
        var normalized = angle % 360d;
        return normalized < 0d ? normalized + 360d : normalized;
    }

    private static double EaseOutCubic(double value)
    {
        var normalized = Math.Clamp(value, 0d, 1d);
        return 1d - Math.Pow(1d - normalized, 3d);
    }

    private static bool SameRect(Rect first, Rect second)
    {
        return Math.Abs(first.X - second.X) < 0.5d &&
            Math.Abs(first.Y - second.Y) < 0.5d &&
            Math.Abs(first.Width - second.Width) < 0.5d &&
            Math.Abs(first.Height - second.Height) < 0.5d;
    }

    private static bool SamePoint(Point first, Point second)
    {
        return Math.Abs(first.X - second.X) < 0.25d &&
            Math.Abs(first.Y - second.Y) < 0.25d;
    }

    private static IBrush Brush(string value)
    {
        return new SolidColorBrush(Color.Parse(value));
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

    internal readonly record struct CenterLabelLayout(Point ValueOrigin, Point LabelOrigin, double CombinedHeight);

    private readonly record struct ChartLayout(Rect PieRect, Rect LegendRect);

    private readonly record struct PieSlice(int Index, double StartAngle, double SweepAngle);

    private sealed record CenterTextCache(TextLayout Value, TextLayout Label);

    private sealed record LegendTextCache(TextLayout Label, TextLayout Value, TextLayout? Detail);

    private sealed record TooltipTextCache(TextLayout Label, TextLayout Value, TextLayout? Detail, double Width, double Height);
}
