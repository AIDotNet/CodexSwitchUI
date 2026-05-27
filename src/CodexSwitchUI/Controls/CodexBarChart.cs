using System.Collections.Specialized;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Themes;

namespace CodexSwitchUI.Controls;

public interface ICodexBarChartItem
{
    string Label { get; }

    double Value { get; }

    string ValueText { get; }

    string DetailText { get; }

    IBrush? AccentBrush { get; }
}

public sealed record CodexBarChartItem(
    string Label,
    double Value,
    string ValueText = "",
    string DetailText = "",
    IBrush? AccentBrush = null) : ICodexBarChartItem;

public sealed class CodexBarChartActiveItemChangedEventArgs(
    int oldIndex,
    int newIndex,
    ICodexBarChartItem? oldItem,
    ICodexBarChartItem? newItem)
    : EventArgs
{
    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public ICodexBarChartItem? OldItem { get; } = oldItem;

    public ICodexBarChartItem? NewItem { get; } = newItem;
}

public class CodexBarChart : TemplatedControl
{
    private static readonly TimeSpan AnimationFrameInterval = TimeSpan.FromMilliseconds(16);
    private static readonly TimeSpan DefaultAnimationDuration = CodexSwitchThemeOptions.ShadcnDefault.MotionDurationSlow;
    private const double AnimationSnapThreshold = 0.002d;
    private const double HoverLerpFactor = 0.24d;
    private const double TooltipLerpFactor = 0.28d;
    private static readonly IBrush DefaultForegroundBrush = Brushes.Black;
    private static readonly IBrush DefaultMutedForegroundBrush = Brushes.Gray;
    private static readonly IBrush DefaultGridBrush = new ImmutableSolidColorBrush(Color.FromArgb(34, 120, 120, 120));
    private static readonly IBrush DefaultBarBrush = new ImmutableSolidColorBrush(Color.Parse("#3B82F6"));
    private static readonly IBrush DefaultActiveBarBrush = new ImmutableSolidColorBrush(Color.Parse("#2563EB"));
    private static readonly IBrush DefaultTooltipBackgroundBrush = new ImmutableSolidColorBrush(Color.FromArgb(242, 24, 24, 27));
    private static readonly IBrush DefaultTooltipForegroundBrush = Brushes.White;
    private static readonly IBrush DefaultTooltipBorderBrush = new ImmutableSolidColorBrush(Color.FromArgb(52, 255, 255, 255));
    private static readonly IBrush EmptyTextBrush = new ImmutableSolidColorBrush(Color.Parse("#9CA3AF"));

    public static readonly StyledProperty<IEnumerable<ICodexBarChartItem>?> ItemsSourceProperty =
        AvaloniaProperty.Register<CodexBarChart, IEnumerable<ICodexBarChartItem>?>(nameof(ItemsSource));

    public static readonly StyledProperty<string> EmptyTextProperty =
        AvaloniaProperty.Register<CodexBarChart, string>(nameof(EmptyText), "No data");

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexBarChart, Orientation>(nameof(Orientation), Orientation.Vertical);

    public static readonly StyledProperty<bool> ShowGridLinesProperty =
        AvaloniaProperty.Register<CodexBarChart, bool>(nameof(ShowGridLines), true);

    public static readonly StyledProperty<bool> ShowAxisLabelsProperty =
        AvaloniaProperty.Register<CodexBarChart, bool>(nameof(ShowAxisLabels), true);

    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<CodexBarChart, bool>(nameof(IsCompact));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexBarChart, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<int> ActiveIndexProperty =
        AvaloniaProperty.Register<CodexBarChart, int>(nameof(ActiveIndex), -1);

    public static readonly StyledProperty<double> BarRadiusProperty =
        AvaloniaProperty.Register<CodexBarChart, double>(nameof(BarRadius), 6d);

    public static readonly StyledProperty<double> BarGapRatioProperty =
        AvaloniaProperty.Register<CodexBarChart, double>(nameof(BarGapRatio), 0.34d);

    public static readonly StyledProperty<IBrush?> MutedForegroundProperty =
        AvaloniaProperty.Register<CodexBarChart, IBrush?>(nameof(MutedForeground));

    public static readonly StyledProperty<IBrush?> GridBrushProperty =
        AvaloniaProperty.Register<CodexBarChart, IBrush?>(nameof(GridBrush));

    public static readonly StyledProperty<IBrush?> BarBrushProperty =
        AvaloniaProperty.Register<CodexBarChart, IBrush?>(nameof(BarBrush));

    public static readonly StyledProperty<IBrush?> ActiveBarBrushProperty =
        AvaloniaProperty.Register<CodexBarChart, IBrush?>(nameof(ActiveBarBrush));

    public static readonly StyledProperty<IBrush?> TooltipBackgroundProperty =
        AvaloniaProperty.Register<CodexBarChart, IBrush?>(nameof(TooltipBackground));

    public static readonly StyledProperty<IBrush?> TooltipForegroundProperty =
        AvaloniaProperty.Register<CodexBarChart, IBrush?>(nameof(TooltipForeground));

    public static readonly StyledProperty<IBrush?> TooltipBorderBrushProperty =
        AvaloniaProperty.Register<CodexBarChart, IBrush?>(nameof(TooltipBorderBrush));

    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<CodexBarChart, TimeSpan>(nameof(AnimationDuration), DefaultAnimationDuration);

    private ICodexBarChartItem[] _items = [];
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

    static CodexBarChart()
    {
        AffectsRender<CodexBarChart>(
            ItemsSourceProperty,
            EmptyTextProperty,
            OrientationProperty,
            ShowGridLinesProperty,
            ShowAxisLabelsProperty,
            ActiveIndexProperty,
            BarRadiusProperty,
            BarGapRatioProperty,
            MutedForegroundProperty,
            GridBrushProperty,
            BarBrushProperty,
            ActiveBarBrushProperty,
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

        AffectsMeasure<CodexBarChart>(
            ItemsSourceProperty,
            OrientationProperty,
            IsCompactProperty,
            PaddingProperty,
            BorderThicknessProperty,
            FontSizeProperty);

        ItemsSourceProperty.Changed.AddClassHandler<CodexBarChart>((chart, args) =>
            chart.OnItemsSourceChanged(
                args.OldValue as IEnumerable<ICodexBarChartItem>,
                args.NewValue as IEnumerable<ICodexBarChartItem>));
        OrientationProperty.Changed.AddClassHandler<CodexBarChart>((chart, _) => chart.SyncClasses());
        ShowGridLinesProperty.Changed.AddClassHandler<CodexBarChart>((chart, _) => chart.SyncClasses());
        ShowAxisLabelsProperty.Changed.AddClassHandler<CodexBarChart>((chart, _) => chart.SyncClasses());
        IsCompactProperty.Changed.AddClassHandler<CodexBarChart>((chart, _) => chart.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexBarChart>((chart, _) => chart.SyncClasses());
        ActiveIndexProperty.Changed.AddClassHandler<CodexBarChart>((chart, args) => chart.OnActiveIndexChanged(args));
        AnimationDurationProperty.Changed.AddClassHandler<CodexBarChart>((chart, _) => chart.SyncChartAnimationDuration());
    }

    public CodexBarChart()
    {
        ClipToBounds = true;
        Focusable = false;
        PointerMoved += OnPointerMoved;
        PointerExited += OnPointerExited;
        SyncClasses();
    }

    public event EventHandler<CodexBarChartActiveItemChangedEventArgs>? ActiveItemChanged;

    public IEnumerable<ICodexBarChartItem>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string EmptyText
    {
        get => GetValue(EmptyTextProperty);
        set => SetValue(EmptyTextProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public bool ShowGridLines
    {
        get => GetValue(ShowGridLinesProperty);
        set => SetValue(ShowGridLinesProperty, value);
    }

    public bool ShowAxisLabels
    {
        get => GetValue(ShowAxisLabelsProperty);
        set => SetValue(ShowAxisLabelsProperty, value);
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

    public double BarRadius
    {
        get => GetValue(BarRadiusProperty);
        set => SetValue(BarRadiusProperty, value);
    }

    public double BarGapRatio
    {
        get => GetValue(BarGapRatioProperty);
        set => SetValue(BarGapRatioProperty, value);
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

    public IBrush? BarBrush
    {
        get => GetValue(BarBrushProperty);
        set => SetValue(BarBrushProperty, value);
    }

    public IBrush? ActiveBarBrush
    {
        get => GetValue(ActiveBarBrushProperty);
        set => SetValue(ActiveBarBrushProperty, value);
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

    public ICodexBarChartItem? ActiveItem
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
        var contentHeight = Orientation == Orientation.Horizontal
            ? Math.Max(IsCompact ? 118d : 150d, _items.Length * (IsCompact ? 34d : 42d))
            : IsCompact ? 148d : 190d;
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
        if (plot.Width <= 0 || plot.Height <= 0)
            return;

        var scale = CreateScale();
        var progress = EaseOutCubic(_chartProgress);

        if (ShowGridLines)
            DrawGrid(context, plot, scale);

        DrawBars(context, content, plot, scale, progress);
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

    private void DrawGrid(DrawingContext context, Rect plot, (double Min, double Max) scale)
    {
        var pen = new Pen(GridBrush ?? DefaultGridBrush, 1d);
        var gridCount = IsCompact ? 2 : 3;
        if (Orientation == Orientation.Horizontal)
        {
            for (var index = 0; index <= gridCount; index++)
            {
                var x = plot.X + plot.Width * index / gridCount;
                context.DrawLine(pen, new Point(x, plot.Y), new Point(x, plot.Bottom));
            }

            DrawZeroLine(context, plot, scale);
            return;
        }

        for (var index = 0; index <= gridCount; index++)
        {
            var y = plot.Y + plot.Height * index / gridCount;
            context.DrawLine(pen, new Point(plot.X, y), new Point(plot.Right, y));
        }

        DrawZeroLine(context, plot, scale);
    }

    private void DrawZeroLine(DrawingContext context, Rect plot, (double Min, double Max) scale)
    {
        if (scale.Min >= 0 || scale.Max <= 0)
            return;

        var pen = new Pen(GridBrush ?? DefaultGridBrush, 1.2d);
        if (Orientation == Orientation.Horizontal)
        {
            var x = GetX(plot, 0, scale.Min, scale.Max);
            context.DrawLine(pen, new Point(x, plot.Y), new Point(x, plot.Bottom));
            return;
        }

        var y = GetY(plot, 0, scale.Min, scale.Max);
        context.DrawLine(pen, new Point(plot.X, y), new Point(plot.Right, y));
    }

    private void DrawBars(
        DrawingContext context,
        Rect content,
        Rect plot,
        (double Min, double Max) scale,
        double progress)
    {
        if (Orientation == Orientation.Horizontal)
        {
            DrawHorizontalBars(context, content, plot, scale, progress);
            return;
        }

        DrawVerticalBars(context, content, plot, scale, progress);
    }

    private void DrawVerticalBars(
        DrawingContext context,
        Rect content,
        Rect plot,
        (double Min, double Max) scale,
        double progress)
    {
        var slot = plot.Width / _items.Length;
        var gapRatio = Math.Clamp(BarGapRatio, 0.08d, 0.72d);
        var barWidth = Math.Clamp(slot * (1d - gapRatio), 5d, IsCompact ? 28d : 48d);
        var zeroY = GetY(plot, 0, scale.Min, scale.Max);

        for (var index = 0; index < _items.Length; index++)
        {
            var item = _items[index];
            var animatedValue = item.Value * progress;
            var valueY = GetY(plot, animatedValue, scale.Min, scale.Max);
            var x = plot.X + slot * index + (slot - barWidth) / 2d;
            var y = Math.Min(zeroY, valueY);
            var height = Math.Max(1d, Math.Abs(valueY - zeroY));
            var rect = new Rect(x, y, barWidth, height);
            DrawBar(context, rect, ResolveBarBrush(index));

            if (index == ActiveIndex && _hoverProgress > 0.01d)
            {
                using var activeOpacity = context.PushOpacity(EaseOutCubic(_hoverProgress));
                DrawBar(context, rect.Inflate(1.5d), ResolveActiveBarBrush(index));
            }
        }

        DrawVerticalAxisText(context, content, plot, scale, slot);
    }

    private void DrawHorizontalBars(
        DrawingContext context,
        Rect content,
        Rect plot,
        (double Min, double Max) scale,
        double progress)
    {
        var slot = plot.Height / _items.Length;
        var gapRatio = Math.Clamp(BarGapRatio, 0.08d, 0.72d);
        var barHeight = Math.Clamp(slot * (1d - gapRatio), 5d, IsCompact ? 18d : 26d);
        var zeroX = GetX(plot, 0, scale.Min, scale.Max);

        for (var index = 0; index < _items.Length; index++)
        {
            var item = _items[index];
            var animatedValue = item.Value * progress;
            var valueX = GetX(plot, animatedValue, scale.Min, scale.Max);
            var y = plot.Y + slot * index + (slot - barHeight) / 2d;
            var x = Math.Min(zeroX, valueX);
            var width = Math.Max(1d, Math.Abs(valueX - zeroX));
            var rect = new Rect(x, y, width, barHeight);
            DrawBar(context, rect, ResolveBarBrush(index));

            if (index == ActiveIndex && _hoverProgress > 0.01d)
            {
                using var activeOpacity = context.PushOpacity(EaseOutCubic(_hoverProgress));
                DrawBar(context, rect.Inflate(1.5d), ResolveActiveBarBrush(index));
            }
        }

        DrawHorizontalAxisText(context, content, plot, slot);
    }

    private void DrawBar(DrawingContext context, Rect rect, IBrush brush)
    {
        var radius = Math.Max(0d, BarRadius);
        context.DrawRectangle(brush, null, rect, radius, radius);
    }

    private void DrawVerticalAxisText(
        DrawingContext context,
        Rect content,
        Rect plot,
        (double Min, double Max) scale,
        double slot)
    {
        if (!ShowAxisLabels || IsCompact)
            return;

        var muted = MutedForeground ?? DefaultMutedForegroundBrush;
        var fontSize = Math.Max(10d, FontSize - 1d);
        var typeface = CreateTypeface(FontWeight.Normal);
        for (var index = 0; index < _items.Length; index++)
        {
            if (slot < 42d && index != 0 && index != _items.Length - 1)
                continue;

            var centerX = plot.X + slot * index + slot / 2d;
            DrawTextLayout(
                context,
                CreateTextLayout(_items[index].Label, typeface, fontSize, muted, Math.Max(24d, slot)),
                new Point(centerX, plot.Bottom + 8d),
                TextAlignment.Center);
        }

        DrawTextLayout(
            context,
            CreateTextLayout(FormatValue(scale.Max), typeface, fontSize, muted, 72d),
            new Point(content.Right, plot.Y - 2d),
            TextAlignment.Right);
    }

    private void DrawHorizontalAxisText(DrawingContext context, Rect content, Rect plot, double slot)
    {
        if (!ShowAxisLabels || IsCompact)
            return;

        var muted = MutedForeground ?? DefaultMutedForegroundBrush;
        var fontSize = Math.Max(10d, FontSize - 1d);
        var typeface = CreateTypeface(FontWeight.Normal);
        for (var index = 0; index < _items.Length; index++)
        {
            var centerY = plot.Y + slot * index + slot / 2d;
            var item = _items[index];
            var label = CreateTextLayout(item.Label, typeface, fontSize, muted, Math.Max(1d, plot.X - content.X - 8d));
            DrawTextLayout(context, label, new Point(plot.X - 8d, centerY - label.Height / 2d), TextAlignment.Right);

            var valueText = string.IsNullOrWhiteSpace(item.ValueText) ? FormatValue(item.Value) : item.ValueText;
            DrawTextLayout(
                context,
                CreateTextLayout(valueText, typeface, fontSize, muted, Math.Max(1d, content.Right - plot.Right - 8d)),
                new Point(plot.Right + 8d, centerY - label.Height / 2d));
        }
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

        var markerBrush = item.AccentBrush ?? BarBrush ?? DefaultBarBrush;
        context.DrawRectangle(markerBrush, null, new Rect(rect.X + 8d, rect.Y + 10.5d, 9d, 9d), 3d, 3d);
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
            content.Width);
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

        if (Orientation == Orientation.Horizontal)
        {
            if (position.Y < plot.Y || position.Y > plot.Bottom)
                return -1;

            var slot = plot.Height / _items.Length;
            return Math.Clamp((int)Math.Floor((position.Y - plot.Y) / slot), 0, _items.Length - 1);
        }

        if (position.X < plot.X || position.X > plot.Right)
            return -1;

        var xSlot = plot.Width / _items.Length;
        return Math.Clamp((int)Math.Floor((position.X - plot.X) / xSlot), 0, _items.Length - 1);
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
        IEnumerable<ICodexBarChartItem>? oldValue,
        IEnumerable<ICodexBarChartItem>? newValue)
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

        var oldItem = oldIndex >= 0 && oldIndex < _items.Length ? _items[oldIndex] : null;
        var newItem = newIndex >= 0 && newIndex < _items.Length ? _items[newIndex] : null;
        SyncClasses();
        if (oldIndex != newIndex)
        {
            ActiveItemChanged?.Invoke(this, new CodexBarChartActiveItemChangedEventArgs(oldIndex, newIndex, oldItem, newItem));
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

    private Rect GetPlotRect(Rect content)
    {
        if (Orientation == Orientation.Horizontal)
        {
            var labelInset = ShowAxisLabels && !IsCompact ? 78d : 6d;
            var valueInset = ShowAxisLabels && !IsCompact ? 52d : 6d;
            return new Rect(
                content.X + labelInset,
                content.Y + 4d,
                Math.Max(1d, content.Width - labelInset - valueInset),
                Math.Max(1d, content.Height - 8d));
        }

        var axisHeight = ShowAxisLabels && !IsCompact ? 28d : 4d;
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
        min = Math.Min(0d, min);
        max = Math.Max(0d, max);

        if (Math.Abs(max - min) < 0.0001d)
        {
            max += 1d;
            min -= 1d;
        }

        var padding = (max - min) * 0.08d;
        if (min < 0)
            min -= padding;
        if (max > 0)
            max += padding;

        return (min, max);
    }

    private double GetY(Rect plot, double value, double minValue, double maxValue)
    {
        var normalized = Math.Clamp((value - minValue) / (maxValue - minValue), 0d, 1d);
        return plot.Bottom - plot.Height * normalized;
    }

    private double GetX(Rect plot, double value, double minValue, double maxValue)
    {
        var normalized = Math.Clamp((value - minValue) / (maxValue - minValue), 0d, 1d);
        return plot.X + plot.Width * normalized;
    }

    private IBrush ResolveBarBrush(int index)
    {
        return _items.Length > index && _items[index].AccentBrush is { } accent
            ? accent
            : BarBrush ?? DefaultBarBrush;
    }

    private IBrush ResolveActiveBarBrush(int index)
    {
        return ActiveBarBrush ?? _items[index].AccentBrush ?? BarBrush ?? DefaultActiveBarBrush;
    }

    private void SyncClasses()
    {
        Classes.Set("bar-chart", true);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("grid", ShowGridLines);
        Classes.Set("no-grid", !ShowGridLines);
        Classes.Set("axis-labels", ShowAxisLabels);
        Classes.Set("no-axis-labels", !ShowAxisLabels);
        Classes.Set("compact", IsCompact);
        Classes.Set("empty", _items.Length == 0);
        Classes.Set("has-active-bar", ActiveIndex >= 0);
        Classes.Set("has-negative", _items.Any(item => item.Value < 0));
        CodexClassSync.SetSize(Classes, Size);
    }

    private static ICodexBarChartItem[] ToArray(IEnumerable<ICodexBarChartItem>? source)
    {
        return source switch
        {
            null => [],
            ICodexBarChartItem[] array => array.Where(IsValidItem).ToArray(),
            ICollection<ICodexBarChartItem> collection => ToArray(collection),
            IReadOnlyCollection<ICodexBarChartItem> collection => ToArray(collection),
            _ => source.Where(IsValidItem).ToArray()
        };
    }

    private static ICodexBarChartItem[] ToArray(ICollection<ICodexBarChartItem> collection)
    {
        var array = new ICodexBarChartItem[collection.Count];
        collection.CopyTo(array, 0);
        return array.Where(IsValidItem).ToArray();
    }

    private static ICodexBarChartItem[] ToArray(IReadOnlyCollection<ICodexBarChartItem> collection)
    {
        var array = new ICodexBarChartItem[collection.Count];
        var index = 0;
        foreach (var item in collection)
            array[index++] = item;
        return array.Where(IsValidItem).ToArray();
    }

    private static bool IsValidItem(ICodexBarChartItem item)
    {
        return !double.IsNaN(item.Value) && !double.IsInfinity(item.Value);
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
            <= -1_000_000 => $"{value / 1_000_000d:0.#}M",
            >= 1_000 => $"{value / 1_000d:0.#}K",
            <= -1_000 => $"{value / 1_000d:0.#}K",
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
