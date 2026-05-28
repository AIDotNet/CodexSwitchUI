using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public sealed class CodexResizableLayoutChangedEventArgs(IReadOnlyList<double> panelSizes, int handleIndex) : EventArgs
{
    public IReadOnlyList<double> PanelSizes { get; } = panelSizes;

    public int HandleIndex { get; } = handleIndex;
}

public class CodexResizablePanelGroup : Panel
{
    private const double DefaultKeyboardStep = 10d;
    private const double DefaultMinPanelSize = 5d;
    private const double DefaultMaxPanelSize = 95d;

    private int _activeHandleIndex = -1;

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexResizablePanelGroup, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexResizablePanelGroup, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<IBrush?> BorderBrushProperty =
        AvaloniaProperty.Register<CodexResizablePanelGroup, IBrush?>(nameof(BorderBrush));

    public static readonly StyledProperty<Thickness> BorderThicknessProperty =
        AvaloniaProperty.Register<CodexResizablePanelGroup, Thickness>(nameof(BorderThickness));

    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<CodexResizablePanelGroup, CornerRadius>(nameof(CornerRadius));

    public static readonly StyledProperty<Thickness> PaddingProperty =
        AvaloniaProperty.Register<CodexResizablePanelGroup, Thickness>(nameof(Padding));

    public static readonly StyledProperty<bool> IsDraggingProperty =
        AvaloniaProperty.Register<CodexResizablePanelGroup, bool>(nameof(IsDragging));

    public static readonly StyledProperty<int> PanelCountProperty =
        AvaloniaProperty.Register<CodexResizablePanelGroup, int>(nameof(PanelCount));

    public static readonly StyledProperty<string> LayoutSummaryProperty =
        AvaloniaProperty.Register<CodexResizablePanelGroup, string>(nameof(LayoutSummary), "0 panels");

    static CodexResizablePanelGroup()
    {
        OrientationProperty.Changed.AddClassHandler<CodexResizablePanelGroup>((group, _) =>
        {
            group.SyncStructure();
            group.InvalidateMeasure();
        });
        SizeProperty.Changed.AddClassHandler<CodexResizablePanelGroup>((group, _) => group.SyncStructure());
        IsDraggingProperty.Changed.AddClassHandler<CodexResizablePanelGroup>((group, _) => group.SyncClasses());
        AffectsMeasure<CodexResizablePanelGroup>(OrientationProperty, PaddingProperty, BorderThicknessProperty);
        AffectsRender<CodexResizablePanelGroup>(
            BackgroundProperty,
            BorderBrushProperty,
            BorderThicknessProperty,
            CornerRadiusProperty);
    }

    public CodexResizablePanelGroup()
    {
        ClipToBounds = true;
        Children.CollectionChanged += OnChildrenCollectionChanged;
        SyncStructure();
    }

    public event EventHandler<CodexResizableLayoutChangedEventArgs>? LayoutChanged;

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public IBrush? BorderBrush
    {
        get => GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    public Thickness BorderThickness
    {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public Thickness Padding
    {
        get => GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public bool IsDragging
    {
        get => GetValue(IsDraggingProperty);
        private set => SetValue(IsDraggingProperty, value);
    }

    public int PanelCount => GetValue(PanelCountProperty);

    public string LayoutSummary => GetValue(LayoutSummaryProperty);

    public bool ResizeHandleByPercent(CodexResizableHandle handle, double deltaPercent)
    {
        if (!TryGetPanelPair(handle, out var previous, out var next, out var handleIndex))
        {
            return false;
        }

        EnsurePanelSizes();

        var previousSize = previous.PanelSize;
        var nextSize = next.PanelSize;
        var delta = ClampDelta(previous, next, deltaPercent);
        if (Math.Abs(delta) < 0.001)
        {
            return false;
        }

        SetPanelSize(previous, previousSize + delta);
        SetPanelSize(next, nextSize - delta);
        SyncStructure();
        InvalidateMeasure();
        RaiseLayoutChanged(handleIndex);
        return true;
    }

    public bool TryHandleResizeKey(CodexResizableHandle handle, Key key)
    {
        var delta = key switch
        {
            Key.Left when Orientation == Orientation.Horizontal => -DefaultKeyboardStep,
            Key.Right when Orientation == Orientation.Horizontal => DefaultKeyboardStep,
            Key.Up when Orientation == Orientation.Vertical => -DefaultKeyboardStep,
            Key.Down when Orientation == Orientation.Vertical => DefaultKeyboardStep,
            Key.PageUp => -DefaultKeyboardStep,
            Key.PageDown => DefaultKeyboardStep,
            Key.Home => -100d,
            Key.End => 100d,
            _ => 0d
        };

        return Math.Abs(delta) > 0.001 && ResizeHandleByPercent(handle, delta);
    }

    internal bool BeginResize(CodexResizableHandle handle)
    {
        if (!TryGetPanelPair(handle, out _, out _, out var handleIndex))
        {
            return false;
        }

        _activeHandleIndex = handleIndex;
        IsDragging = true;
        handle.IsDragging = true;
        SyncStructure();
        return true;
    }

    internal bool ResizeHandleByPixels(CodexResizableHandle handle, double deltaPixels)
    {
        var usableLength = GetUsableMainLength(Bounds.Size);
        if (usableLength <= 0)
        {
            return false;
        }

        return ResizeHandleByPercent(handle, deltaPixels / usableLength * 100d);
    }

    internal bool EndResize(CodexResizableHandle handle)
    {
        if (!IsDragging && !handle.IsDragging)
        {
            return false;
        }

        _activeHandleIndex = -1;
        IsDragging = false;
        handle.IsDragging = false;
        SyncStructure();
        return true;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        EnsurePanelSizes();
        SyncStructure();

        var innerSize = Deflate(availableSize, BorderThickness + Padding);
        var usableLength = GetUsableMainLength(innerSize);
        var desiredCross = 0d;

        foreach (var child in Children)
        {
            if (child is CodexResizablePanel panel)
            {
                var panelLength = double.IsInfinity(usableLength) ? double.PositiveInfinity : Math.Max(0, usableLength * panel.PanelSize / 100d);
                var constraint = Orientation == Orientation.Horizontal
                    ? new Size(panelLength, innerSize.Height)
                    : new Size(innerSize.Width, panelLength);
                panel.Measure(constraint);
                desiredCross = Math.Max(desiredCross, Orientation == Orientation.Horizontal ? panel.DesiredSize.Height : panel.DesiredSize.Width);
            }
            else if (child is CodexResizableHandle handle)
            {
                var thickness = HandleThickness(handle);
                var constraint = Orientation == Orientation.Horizontal
                    ? new Size(thickness, innerSize.Height)
                    : new Size(innerSize.Width, thickness);
                handle.Measure(constraint);
                desiredCross = Math.Max(desiredCross, Orientation == Orientation.Horizontal ? handle.DesiredSize.Height : handle.DesiredSize.Width);
            }
            else
            {
                child.Measure(innerSize);
                desiredCross = Math.Max(desiredCross, Orientation == Orientation.Horizontal ? child.DesiredSize.Height : child.DesiredSize.Width);
            }
        }

        var main = Orientation == Orientation.Horizontal
            ? ResolveDesiredMain(availableSize.Width, Children.Sum(child => child.DesiredSize.Width))
            : ResolveDesiredMain(availableSize.Height, Children.Sum(child => child.DesiredSize.Height));
        var borderAndPadding = BorderThickness + Padding;
        return Orientation == Orientation.Horizontal
            ? new Size(main + borderAndPadding.Left + borderAndPadding.Right, desiredCross + borderAndPadding.Top + borderAndPadding.Bottom)
            : new Size(desiredCross + borderAndPadding.Left + borderAndPadding.Right, main + borderAndPadding.Top + borderAndPadding.Bottom);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        EnsurePanelSizes();

        var borderAndPadding = BorderThickness + Padding;
        var content = new Rect(finalSize).Deflate(borderAndPadding);
        var usableLength = GetUsableMainLength(content.Size);
        var offset = Orientation == Orientation.Horizontal ? content.X : content.Y;

        foreach (var child in Children)
        {
            var length = child switch
            {
                CodexResizablePanel panel => Math.Max(0, usableLength * panel.PanelSize / 100d),
                CodexResizableHandle handle => HandleThickness(handle),
                _ => 0d
            };

            var rect = Orientation == Orientation.Horizontal
                ? new Rect(offset, content.Y, length, content.Height)
                : new Rect(content.X, offset, content.Width, length);
            child.Arrange(rect);
            offset += length;
        }

        return finalSize;
    }

    private void OnChildrenCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncStructure();
        InvalidateMeasure();
    }

    private void EnsurePanelSizes()
    {
        var panels = Panels().ToArray();
        if (panels.Length == 0)
        {
            SetValue(PanelCountProperty, 0);
            SetValue(LayoutSummaryProperty, "0 panels");
            return;
        }

        var uninitialized = panels.Count(panel => panel.PanelSize <= 0);
        var initializedTotal = panels.Where(panel => panel.PanelSize > 0).Sum(panel => panel.PanelSize);
        var fallback = Math.Max(0, 100d - initializedTotal) / Math.Max(1, uninitialized);

        foreach (var panel in panels)
        {
            if (panel.PanelSize <= 0)
            {
                var defaultSize = panel.DefaultSize > 0 ? panel.DefaultSize : fallback;
                SetPanelSize(panel, Math.Clamp(defaultSize, MinSize(panel), MaxSize(panel)));
            }
        }

        NormalizePanels(panels);
        SetValue(PanelCountProperty, panels.Length);
        SetValue(LayoutSummaryProperty, string.Join(" / ", panels.Select(panel => $"{Math.Round(panel.PanelSize)}%")));
    }

    private void NormalizePanels(IReadOnlyList<CodexResizablePanel> panels)
    {
        var total = panels.Sum(panel => panel.PanelSize);
        if (total <= 0)
        {
            var equal = 100d / panels.Count;
            foreach (var panel in panels)
            {
                SetPanelSize(panel, equal);
            }

            return;
        }

        foreach (var panel in panels)
        {
            SetPanelSize(panel, Math.Clamp(panel.PanelSize / total * 100d, MinSize(panel), MaxSize(panel)));
        }
    }

    private void SyncStructure()
    {
        var panels = Panels().ToArray();
        var handles = Handles().ToArray();
        SetValue(PanelCountProperty, panels.Length);
        SetValue(LayoutSummaryProperty, panels.Length == 0
            ? "0 panels"
            : string.Join(" / ", panels.Select(panel => $"{Math.Round(panel.PanelSize)}%")));
        SyncClasses(panels.Length, handles.Length);

        for (var index = 0; index < panels.Length; index++)
        {
            panels[index].SetGroupState(this, index, index == 0, index == panels.Length - 1);
        }

        for (var index = 0; index < handles.Length; index++)
        {
            handles[index].SetGroupState(this, index, index == _activeHandleIndex);
        }
    }

    private void SyncClasses()
    {
        SyncClasses(Panels().Count(), Handles().Count());
    }

    private void SyncClasses(int panelCount, int handleCount)
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("resizable-panel-group", true);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("dragging", IsDragging);
        Classes.Set("idle", !IsDragging);
        Classes.Set("has-handle", handleCount > 0);
        Classes.Set("empty", panelCount == 0);
    }

    private bool TryGetPanelPair(
        CodexResizableHandle handle,
        out CodexResizablePanel previous,
        out CodexResizablePanel next,
        out int handleIndex)
    {
        previous = null!;
        next = null!;
        handleIndex = -1;

        var childIndex = Children.IndexOf(handle);
        if (childIndex < 0)
        {
            return false;
        }

        for (var index = childIndex - 1; index >= 0; index--)
        {
            if (Children[index] is CodexResizablePanel panel)
            {
                previous = panel;
                break;
            }
        }

        for (var index = childIndex + 1; index < Children.Count; index++)
        {
            if (Children[index] is CodexResizablePanel panel)
            {
                next = panel;
                break;
            }
        }

        if (previous is null || next is null)
        {
            return false;
        }

        handleIndex = Handles().ToList().IndexOf(handle);
        return handleIndex >= 0;
    }

    private double ClampDelta(CodexResizablePanel previous, CodexResizablePanel next, double deltaPercent)
    {
        var maxGrowPrevious = MaxSize(previous) - previous.PanelSize;
        var maxShrinkPrevious = previous.PanelSize - MinSize(previous);
        var maxGrowNext = MaxSize(next) - next.PanelSize;
        var maxShrinkNext = next.PanelSize - MinSize(next);

        return deltaPercent >= 0
            ? Math.Min(deltaPercent, Math.Min(maxGrowPrevious, maxShrinkNext))
            : Math.Max(deltaPercent, -Math.Min(maxShrinkPrevious, maxGrowNext));
    }

    private IEnumerable<CodexResizablePanel> Panels()
    {
        return Children.OfType<CodexResizablePanel>();
    }

    private IEnumerable<CodexResizableHandle> Handles()
    {
        return Children.OfType<CodexResizableHandle>();
    }

    private void RaiseLayoutChanged(int handleIndex)
    {
        LayoutChanged?.Invoke(this, new CodexResizableLayoutChangedEventArgs(Panels().Select(panel => panel.PanelSize).ToArray(), handleIndex));
    }

    private double GetUsableMainLength(Size size)
    {
        var main = Orientation == Orientation.Horizontal ? size.Width : size.Height;
        if (double.IsInfinity(main))
        {
            return main;
        }

        var handleTotal = Handles().Sum(HandleThickness);
        return Math.Max(0, main - handleTotal);
    }

    private static Size Deflate(Size size, Thickness thickness)
    {
        return new Size(
            double.IsInfinity(size.Width) ? size.Width : Math.Max(0, size.Width - thickness.Left - thickness.Right),
            double.IsInfinity(size.Height) ? size.Height : Math.Max(0, size.Height - thickness.Top - thickness.Bottom));
    }

    private static double ResolveDesiredMain(double availableMain, double desiredMain)
    {
        return double.IsInfinity(availableMain) ? desiredMain : availableMain;
    }

    private static double MinSize(CodexResizablePanel panel)
    {
        return Math.Clamp(panel.MinSize <= 0 ? DefaultMinPanelSize : panel.MinSize, 0, 100);
    }

    private static double MaxSize(CodexResizablePanel panel)
    {
        return Math.Clamp(panel.MaxSize <= 0 ? DefaultMaxPanelSize : panel.MaxSize, MinSize(panel), 100);
    }

    private static void SetPanelSize(CodexResizablePanel panel, double value)
    {
        panel.SetCurrentValue(CodexResizablePanel.PanelSizeProperty, Math.Clamp(value, MinSize(panel), MaxSize(panel)));
    }

    private static double HandleThickness(CodexResizableHandle handle)
    {
        return handle.Size switch
        {
            CodexControlSize.Small => 6,
            CodexControlSize.Large => 12,
            CodexControlSize.Icon => 6,
            _ => 8
        };
    }
}

public class CodexResizablePanel : ContentControl
{
    public static readonly StyledProperty<double> DefaultSizeProperty =
        AvaloniaProperty.Register<CodexResizablePanel, double>(nameof(DefaultSize));

    public static readonly StyledProperty<double> MinSizeProperty =
        AvaloniaProperty.Register<CodexResizablePanel, double>(nameof(MinSize), 5d);

    public static readonly StyledProperty<double> MaxSizeProperty =
        AvaloniaProperty.Register<CodexResizablePanel, double>(nameof(MaxSize), 95d);

    public static readonly StyledProperty<double> PanelSizeProperty =
        AvaloniaProperty.Register<CodexResizablePanel, double>(nameof(PanelSize));

    static CodexResizablePanel()
    {
        DefaultSizeProperty.Changed.AddClassHandler<CodexResizablePanel>((panel, _) => panel.SyncClasses());
        MinSizeProperty.Changed.AddClassHandler<CodexResizablePanel>((panel, _) => panel.SyncClasses());
        MaxSizeProperty.Changed.AddClassHandler<CodexResizablePanel>((panel, _) => panel.SyncClasses());
        PanelSizeProperty.Changed.AddClassHandler<CodexResizablePanel>((panel, _) => panel.SyncClasses());
    }

    public CodexResizablePanel()
    {
        SyncClasses();
    }

    public double DefaultSize
    {
        get => GetValue(DefaultSizeProperty);
        set => SetValue(DefaultSizeProperty, value);
    }

    public double MinSize
    {
        get => GetValue(MinSizeProperty);
        set => SetValue(MinSizeProperty, value);
    }

    public double MaxSize
    {
        get => GetValue(MaxSizeProperty);
        set => SetValue(MaxSizeProperty, value);
    }

    public double PanelSize => GetValue(PanelSizeProperty);

    internal void SetGroupState(CodexResizablePanelGroup group, int index, bool isFirst, bool isLast)
    {
        Classes.Set("horizontal", group.Orientation == Orientation.Horizontal);
        Classes.Set("vertical", group.Orientation == Orientation.Vertical);
        Classes.Set("first", isFirst);
        Classes.Set("last", isLast);
        Classes.Set("middle", !isFirst && !isLast);
        Classes.Set("collapsed", PanelSize <= MinSize + 0.001);
        Classes.Set("expanded", PanelSize > MinSize + 0.001);
        Classes.Set($"panel-index-{index}", true);
        SyncClasses();
    }

    private void SyncClasses()
    {
        Classes.Set("resizable-panel", true);
        Classes.Set("has-default-size", DefaultSize > 0);
        Classes.Set("has-min-size", MinSize > 0);
        Classes.Set("has-max-size", MaxSize > 0);
    }
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexResizableHandle : TemplatedControl
{
    private Point _lastPoint;

    public static readonly StyledProperty<bool> WithHandleProperty =
        AvaloniaProperty.Register<CodexResizableHandle, bool>(nameof(WithHandle));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexResizableHandle, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexResizableHandle, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<int> HandleIndexProperty =
        AvaloniaProperty.Register<CodexResizableHandle, int>(nameof(HandleIndex), -1);

    public static readonly StyledProperty<bool> IsDraggingProperty =
        AvaloniaProperty.Register<CodexResizableHandle, bool>(nameof(IsDragging));

    static CodexResizableHandle()
    {
        WithHandleProperty.Changed.AddClassHandler<CodexResizableHandle>((handle, _) => handle.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexResizableHandle>((handle, _) => handle.SyncClasses());
        OrientationProperty.Changed.AddClassHandler<CodexResizableHandle>((handle, _) => handle.SyncClasses());
        HandleIndexProperty.Changed.AddClassHandler<CodexResizableHandle>((handle, _) => handle.SyncClasses());
        IsDraggingProperty.Changed.AddClassHandler<CodexResizableHandle>((handle, _) => handle.SyncClasses());
    }

    public CodexResizableHandle()
    {
        Focusable = true;
        SyncClasses();
    }

    public bool WithHandle
    {
        get => GetValue(WithHandleProperty);
        set => SetValue(WithHandleProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        private set => SetValue(OrientationProperty, value);
    }

    public int HandleIndex
    {
        get => GetValue(HandleIndexProperty);
        private set => SetValue(HandleIndexProperty, value);
    }

    public bool IsDragging
    {
        get => GetValue(IsDraggingProperty);
        internal set => SetValue(IsDraggingProperty, value);
    }

    public bool TryHandleResizeKey(Key key)
    {
        return Owner()?.TryHandleResizeKey(this, key) == true;
    }

    internal bool TryBeginResize(PointerUpdateKind updateKind, Point startPoint, CodexResizablePanelGroup? owner = null)
    {
        if (updateKind != PointerUpdateKind.LeftButtonPressed || !IsEnabled)
        {
            return false;
        }

        owner ??= Owner();
        if (owner is null)
        {
            return false;
        }

        _lastPoint = startPoint;
        return owner.BeginResize(this);
    }

    internal bool TryEndResize(PointerUpdateKind updateKind, CodexResizablePanelGroup? owner = null)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased)
        {
            return false;
        }

        owner ??= Owner();
        return owner?.EndResize(this) == true;
    }

    internal void SetGroupState(CodexResizablePanelGroup group, int index, bool isActive)
    {
        Orientation = group.Orientation;
        Size = group.Size;
        HandleIndex = index;
        Classes.Set("active", isActive);
        SyncClasses();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (Owner() is not { } owner)
        {
            return;
        }

        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        var startPoint = e.GetPosition(owner);
        if (!TryBeginResize(updateKind, startPoint, owner))
        {
            return;
        }

        Focus();
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!IsDragging || Owner() is not { } owner)
        {
            return;
        }

        var point = e.GetPosition(owner);
        var delta = Orientation == Orientation.Horizontal ? point.X - _lastPoint.X : point.Y - _lastPoint.Y;
        if (owner.ResizeHandleByPixels(this, delta))
        {
            _lastPoint = point;
        }

        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;

        if (!TryEndResize(updateKind))
        {
            return;
        }

        e.Pointer.Capture(null);
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (TryHandleResizeKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
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

    private CodexResizablePanelGroup? Owner()
    {
        return Parent as CodexResizablePanelGroup
               ?? this.GetVisualParent<CodexResizablePanelGroup>();
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("resizable-handle", true);
        Classes.Set("with-handle", WithHandle);
        Classes.Set("no-handle", !WithHandle);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("dragging", IsDragging);
        Classes.Set("idle", !IsDragging);
        Classes.Set("indexed", HandleIndex >= 0);
    }
}
