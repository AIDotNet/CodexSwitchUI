using System.Collections.Specialized;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;

namespace CodexSwitchUI.Controls;

public interface ICodexRankedBarChartItem
{
    string Label { get; }

    double Value { get; }

    string ValueText { get; }

    string DetailText { get; }

    IBrush? AccentBrush { get; }
}

public sealed record CodexRankedBarChartItem(
    string Label,
    double Value,
    string ValueText,
    string DetailText = "",
    IBrush? AccentBrush = null) : ICodexRankedBarChartItem;

public sealed class CodexRankedBarChartActiveItemChangedEventArgs(
    int oldIndex,
    int newIndex,
    ICodexRankedBarChartItem? oldItem,
    ICodexRankedBarChartItem? newItem)
    : EventArgs
{
    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    public ICodexRankedBarChartItem? OldItem { get; } = oldItem;

    public ICodexRankedBarChartItem? NewItem { get; } = newItem;
}

public class CodexRankedBarChart : TemplatedControl
{
    private static readonly IBrush DefaultForegroundBrush = Brushes.Black;
    private static readonly IBrush DefaultMutedForegroundBrush = Brushes.Gray;
    private static readonly IBrush DefaultTrackBrush = new ImmutableSolidColorBrush(Color.FromArgb(28, 120, 120, 120));
    private static readonly IBrush DefaultAccentBrush = new ImmutableSolidColorBrush(Color.Parse("#3B82F6"));
    private static readonly IBrush DefaultSecondaryAccentBrush = new ImmutableSolidColorBrush(Color.Parse("#10B981"));
    private static readonly IBrush DefaultTertiaryAccentBrush = new ImmutableSolidColorBrush(Color.Parse("#F59E0B"));

    public static readonly StyledProperty<IEnumerable<ICodexRankedBarChartItem>?> ItemsSourceProperty =
        AvaloniaProperty.Register<CodexRankedBarChart, IEnumerable<ICodexRankedBarChartItem>?>(nameof(ItemsSource));

    public static readonly StyledProperty<string> EmptyTextProperty =
        AvaloniaProperty.Register<CodexRankedBarChart, string>(nameof(EmptyText), "No data");

    public static readonly StyledProperty<int> MaxVisibleItemsProperty =
        AvaloniaProperty.Register<CodexRankedBarChart, int>(nameof(MaxVisibleItems), 6);

    public static readonly StyledProperty<double> RowHeightProperty =
        AvaloniaProperty.Register<CodexRankedBarChart, double>(nameof(RowHeight), 34);

    public static readonly StyledProperty<double> RowSpacingProperty =
        AvaloniaProperty.Register<CodexRankedBarChart, double>(nameof(RowSpacing), 10);

    public static readonly StyledProperty<IBrush?> MutedForegroundProperty =
        AvaloniaProperty.Register<CodexRankedBarChart, IBrush?>(nameof(MutedForeground));

    public static readonly StyledProperty<IBrush?> TrackBrushProperty =
        AvaloniaProperty.Register<CodexRankedBarChart, IBrush?>(nameof(TrackBrush));

    public static readonly StyledProperty<IBrush?> AccentBrushProperty =
        AvaloniaProperty.Register<CodexRankedBarChart, IBrush?>(nameof(AccentBrush));

    public static readonly StyledProperty<IBrush?> SecondaryAccentBrushProperty =
        AvaloniaProperty.Register<CodexRankedBarChart, IBrush?>(nameof(SecondaryAccentBrush));

    public static readonly StyledProperty<IBrush?> TertiaryAccentBrushProperty =
        AvaloniaProperty.Register<CodexRankedBarChart, IBrush?>(nameof(TertiaryAccentBrush));

    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<CodexRankedBarChart, bool>(nameof(IsCompact));

    public static readonly StyledProperty<int> ActiveIndexProperty =
        AvaloniaProperty.Register<CodexRankedBarChart, int>(nameof(ActiveIndex), -1);

    private ICodexRankedBarChartItem[] _items = [];
    private INotifyCollectionChanged? _observedItemsSource;
    private bool _itemsDirty = true;
    private bool _refreshQueued;

    static CodexRankedBarChart()
    {
        AffectsRender<CodexRankedBarChart>(
            ItemsSourceProperty,
            EmptyTextProperty,
            MaxVisibleItemsProperty,
            RowHeightProperty,
            RowSpacingProperty,
            MutedForegroundProperty,
            TrackBrushProperty,
            AccentBrushProperty,
            SecondaryAccentBrushProperty,
            TertiaryAccentBrushProperty,
            ActiveIndexProperty,
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

        AffectsMeasure<CodexRankedBarChart>(
            ItemsSourceProperty,
            MaxVisibleItemsProperty,
            RowHeightProperty,
            RowSpacingProperty,
            PaddingProperty);

        ItemsSourceProperty.Changed.AddClassHandler<CodexRankedBarChart>((chart, args) =>
            chart.OnItemsSourceChanged(
                args.OldValue as IEnumerable<ICodexRankedBarChartItem>,
                args.NewValue as IEnumerable<ICodexRankedBarChartItem>));
        IsCompactProperty.Changed.AddClassHandler<CodexRankedBarChart>((chart, _) => chart.SyncClasses());
        ActiveIndexProperty.Changed.AddClassHandler<CodexRankedBarChart>((chart, args) => chart.OnActiveIndexChanged(args));
    }

    public CodexRankedBarChart()
    {
        ClipToBounds = true;
        Focusable = false;
        PointerMoved += OnPointerMoved;
        PointerExited += OnPointerExited;
        SyncClasses();
    }

    public event EventHandler<CodexRankedBarChartActiveItemChangedEventArgs>? ActiveItemChanged;

    public IEnumerable<ICodexRankedBarChartItem>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string EmptyText
    {
        get => GetValue(EmptyTextProperty);
        set => SetValue(EmptyTextProperty, value);
    }

    public int MaxVisibleItems
    {
        get => GetValue(MaxVisibleItemsProperty);
        set => SetValue(MaxVisibleItemsProperty, value);
    }

    public double RowHeight
    {
        get => GetValue(RowHeightProperty);
        set => SetValue(RowHeightProperty, value);
    }

    public double RowSpacing
    {
        get => GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
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

    public IBrush? AccentBrush
    {
        get => GetValue(AccentBrushProperty);
        set => SetValue(AccentBrushProperty, value);
    }

    public IBrush? SecondaryAccentBrush
    {
        get => GetValue(SecondaryAccentBrushProperty);
        set => SetValue(SecondaryAccentBrushProperty, value);
    }

    public IBrush? TertiaryAccentBrush
    {
        get => GetValue(TertiaryAccentBrushProperty);
        set => SetValue(TertiaryAccentBrushProperty, value);
    }

    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    public int ActiveIndex
    {
        get => GetValue(ActiveIndexProperty);
        set => SetValue(ActiveIndexProperty, value);
    }

    public ICodexRankedBarChartItem? ActiveItem
    {
        get
        {
            EnsureItems();
            return ActiveIndex >= 0 && ActiveIndex < GetVisibleCount() ? _items[ActiveIndex] : null;
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        EnsureItems();

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var borderThickness = BorderThickness;
        var borderPen = BorderBrush is null || borderThickness.Left <= 0
            ? null
            : new Pen(BorderBrush, Math.Max(1, borderThickness.Left));
        var radius = Math.Max(0, CornerRadius.TopLeft);
        context.DrawRectangle(Background, borderPen, bounds.Deflate(borderThickness.Left / 2d), radius, radius);

        var content = bounds.Deflate(borderThickness).Deflate(Padding);
        if (content.Width <= 0 || content.Height <= 0)
            return;

        var visibleCount = GetVisibleCount();
        if (visibleCount == 0)
        {
            DrawEmptyState(context, content);
            return;
        }

        var maxValue = 1d;
        for (var index = 0; index < visibleCount; index++)
            maxValue = Math.Max(maxValue, Math.Max(0d, _items[index].Value));

        var rowHeight = Math.Max(24d, RowHeight);
        var rowSpacing = Math.Max(0d, RowSpacing);
        var y = content.Y;

        for (var index = 0; index < visibleCount; index++)
        {
            var item = _items[index];
            DrawRow(context, item, index, content.X, y, content.Width, rowHeight, maxValue);
            y += rowHeight + rowSpacing;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        EnsureItems();

        var visibleCount = GetVisibleCount();
        var rowHeight = Math.Max(24d, RowHeight);
        var rowSpacing = Math.Max(0d, RowSpacing);
        var contentHeight = visibleCount == 0
            ? 76d
            : visibleCount * rowHeight + Math.Max(0, visibleCount - 1) * rowSpacing;
        var desiredHeight = Padding.Top + Padding.Bottom + BorderThickness.Top + BorderThickness.Bottom + contentHeight;
        var desiredWidth = double.IsInfinity(availableSize.Width) ? 320d : availableSize.Width;

        return new Size(desiredWidth, desiredHeight);
    }

    private void DrawRow(
        DrawingContext context,
        ICodexRankedBarChartItem item,
        int index,
        double x,
        double y,
        double width,
        double rowHeight,
        double maxValue)
    {
        var foreground = Foreground ?? DefaultForegroundBrush;
        var muted = MutedForeground ?? DefaultMutedForegroundBrush;
        var track = TrackBrush ?? DefaultTrackBrush;
        var accent = item.AccentBrush ?? ResolveAccent(index);
        var fontSize = Math.Max(10d, FontSize);
        var labelTypeface = new Typeface(FontFamily, FontStyle, FontWeight.SemiBold, FontStretch);
        var valueTypeface = new Typeface(FontFamily, FontStyle, FontWeight.SemiBold, FontStretch);
        var detailTypeface = new Typeface(FontFamily, FontStyle, FontWeight.Normal, FontStretch);
        var valueWidth = Math.Min(120d, Math.Max(72d, width * 0.34d));
        var labelWidth = Math.Max(48d, width - valueWidth - 12d);
        var labelLayout = CreateTextLayout(item.Label, labelTypeface, fontSize, foreground, labelWidth);
        var valueLayout = CreateTextLayout(item.ValueText, valueTypeface, fontSize, foreground, valueWidth, TextAlignment.Right);

        if (index == ActiveIndex)
        {
            using var activeOpacity = context.PushOpacity(0.55d);
            context.DrawRectangle(track, null, new Rect(x - 7d, y - 5d, width + 14d, rowHeight), 8d, 8d);
        }

        DrawTextLayout(context, labelLayout, new Point(x, y));
        DrawTextLayout(context, valueLayout, new Point(x + width, y), TextAlignment.Right);

        var barTop = y + Math.Min(rowHeight - 10d, fontSize + 10d);
        var barHeight = IsCompact ? 5d : 6d;
        var detailTop = barTop + barHeight + 5d;
        var trackRect = new Rect(x, barTop, width, barHeight);
        var progress = Math.Clamp(Math.Max(0d, item.Value) / maxValue, 0d, 1d);
        var fillRect = new Rect(x, barTop, Math.Max(barHeight, width * progress), barHeight);

        context.DrawRectangle(track, null, trackRect, barHeight / 2d, barHeight / 2d);
        context.DrawRectangle(accent, null, fillRect, barHeight / 2d, barHeight / 2d);

        if (!string.IsNullOrWhiteSpace(item.DetailText) && detailTop + fontSize <= y + rowHeight + 2d)
        {
            var detailLayout = CreateTextLayout(item.DetailText, detailTypeface, Math.Max(10d, fontSize - 1d), muted, width);
            DrawTextLayout(context, detailLayout, new Point(x, detailTop));
        }
    }

    private void DrawEmptyState(DrawingContext context, Rect content)
    {
        var text = string.IsNullOrWhiteSpace(EmptyText) ? "No data" : EmptyText;
        var brush = MutedForeground ?? DefaultMutedForegroundBrush;
        var layout = CreateTextLayout(
            text,
            new Typeface(FontFamily, FontStyle, FontWeight.Normal, FontStretch),
            Math.Max(11d, FontSize),
            brush,
            content.Width,
            TextAlignment.Center);
        DrawTextLayout(
            context,
            layout,
            new Point(content.Center.X, content.Center.Y - layout.Height / 2d),
            TextAlignment.Center);
    }

    private IBrush ResolveAccent(int index)
    {
        return (index % 3) switch
        {
            1 => SecondaryAccentBrush ?? DefaultSecondaryAccentBrush,
            2 => TertiaryAccentBrush ?? DefaultTertiaryAccentBrush,
            _ => AccentBrush ?? DefaultAccentBrush
        };
    }

    private int GetVisibleCount()
    {
        var maxItems = Math.Max(0, MaxVisibleItems);
        return maxItems == 0 ? 0 : Math.Min(maxItems, _items.Length);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        EnsureItems();
        ActiveIndex = HitTest(e.GetPosition(this));
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        ActiveIndex = -1;
    }

    private int HitTest(Point position)
    {
        var visibleCount = GetVisibleCount();
        if (visibleCount == 0)
            return -1;

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return -1;

        var content = bounds.Deflate(BorderThickness).Deflate(Padding);
        if (content.Width <= 0 || content.Height <= 0 || position.X < content.X || position.X > content.Right)
            return -1;

        var rowHeight = Math.Max(24d, RowHeight);
        var rowSpacing = Math.Max(0d, RowSpacing);
        for (var index = 0; index < visibleCount; index++)
        {
            var rowTop = content.Y + index * (rowHeight + rowSpacing);
            var rowBottom = rowTop + rowHeight;
            if (position.Y >= rowTop && position.Y <= rowBottom)
                return index;
        }

        return -1;
    }

    private void EnsureItems()
    {
        if (!_itemsDirty)
            return;

        _items = ToArray(ItemsSource);
        _itemsDirty = false;
        SyncClasses();
    }

    private void OnItemsSourceChanged(
        IEnumerable<ICodexRankedBarChartItem>? oldValue,
        IEnumerable<ICodexRankedBarChartItem>? newValue)
    {
        if (_observedItemsSource is not null)
            _observedItemsSource.CollectionChanged -= OnObservedItemsSourceChanged;

        _observedItemsSource = newValue as INotifyCollectionChanged;
        if (_observedItemsSource is not null)
            _observedItemsSource.CollectionChanged += OnObservedItemsSourceChanged;

        ActiveIndex = -1;
        _itemsDirty = true;
        InvalidateMeasure();
        InvalidateVisual();
    }

    private void OnObservedItemsSourceChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ActiveIndex = -1;
        _itemsDirty = true;

        if (_refreshQueued)
            return;

        _refreshQueued = true;
        Dispatcher.UIThread.Post(() =>
        {
            _refreshQueued = false;
            InvalidateMeasure();
            InvalidateVisual();
        }, DispatcherPriority.Background);
    }

    private void OnActiveIndexChanged(AvaloniaPropertyChangedEventArgs args)
    {
        EnsureItems();

        var oldIndex = args.OldValue is int oldValue ? oldValue : -1;
        var visibleCount = GetVisibleCount();
        var newIndex = ActiveIndex >= 0 && ActiveIndex < visibleCount ? ActiveIndex : -1;
        if (ActiveIndex != newIndex)
        {
            SetCurrentValue(ActiveIndexProperty, newIndex);
            return;
        }

        var oldItem = oldIndex >= 0 && oldIndex < visibleCount ? _items[oldIndex] : null;
        var newItem = newIndex >= 0 && newIndex < visibleCount ? _items[newIndex] : null;
        SyncClasses();
        if (oldIndex != newIndex)
        {
            ActiveItemChanged?.Invoke(
                this,
                new CodexRankedBarChartActiveItemChangedEventArgs(oldIndex, newIndex, oldItem, newItem));
        }

        InvalidateVisual();
    }

    private void SyncClasses()
    {
        Classes.Set("ranked-bar-chart", true);
        Classes.Set("compact", IsCompact);
        Classes.Set("empty", _items.Length == 0 || GetVisibleCount() == 0);
        Classes.Set("has-active-row", ActiveIndex >= 0);
    }

    private static ICodexRankedBarChartItem[] ToArray(IEnumerable<ICodexRankedBarChartItem>? source)
    {
        return source switch
        {
            null => [],
            ICodexRankedBarChartItem[] array => array,
            ICollection<ICodexRankedBarChartItem> collection => ToArray(collection),
            IReadOnlyCollection<ICodexRankedBarChartItem> collection => ToArray(collection),
            _ => source
                .Where(item => item.Value > 0 || !string.IsNullOrWhiteSpace(item.Label))
                .OrderByDescending(item => item.Value)
                .ToArray()
        };
    }

    private static ICodexRankedBarChartItem[] ToArray(ICollection<ICodexRankedBarChartItem> collection)
    {
        var array = new ICodexRankedBarChartItem[collection.Count];
        collection.CopyTo(array, 0);
        return array.OrderByDescending(item => item.Value).ToArray();
    }

    private static ICodexRankedBarChartItem[] ToArray(IReadOnlyCollection<ICodexRankedBarChartItem> collection)
    {
        var array = new ICodexRankedBarChartItem[collection.Count];
        var index = 0;
        foreach (var item in collection)
            array[index++] = item;
        return array.OrderByDescending(item => item.Value).ToArray();
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
}
