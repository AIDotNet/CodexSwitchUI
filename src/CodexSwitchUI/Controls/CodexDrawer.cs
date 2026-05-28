using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace CodexSwitchUI.Controls;

public enum CodexDrawerDirection
{
    Bottom,
    Top,
    Right,
    Left
}

public sealed class CodexDrawerDragCompletedEventArgs(double dragOffset, bool dismissed)
    : EventArgs
{
    public double DragOffset { get; } = dragOffset;

    public bool Dismissed { get; } = dismissed;
}

public class CodexDrawer : CodexDialog
{
    private Control? _handle;
    private Point? _dragStart;
    private IPointer? _dragPointer;

    public static readonly StyledProperty<CodexDrawerDirection> DirectionProperty =
        AvaloniaProperty.Register<CodexDrawer, CodexDrawerDirection>(nameof(Direction), CodexDrawerDirection.Bottom);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexDrawer, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsHandleVisibleProperty =
        AvaloniaProperty.Register<CodexDrawer, bool>(nameof(IsHandleVisible), true);

    public static readonly StyledProperty<bool> ShouldScaleBackgroundProperty =
        AvaloniaProperty.Register<CodexDrawer, bool>(nameof(ShouldScaleBackground), true);

    public static readonly StyledProperty<bool> CloseOnDragDismissProperty =
        AvaloniaProperty.Register<CodexDrawer, bool>(nameof(CloseOnDragDismiss), true);

    public static readonly StyledProperty<double> DragDismissThresholdProperty =
        AvaloniaProperty.Register<CodexDrawer, double>(nameof(DragDismissThreshold), 96);

    public static readonly StyledProperty<double> DragOffsetProperty =
        AvaloniaProperty.Register<CodexDrawer, double>(nameof(DragOffset));

    public static readonly StyledProperty<bool> IsDraggingProperty =
        AvaloniaProperty.Register<CodexDrawer, bool>(nameof(IsDragging));

    public static readonly StyledProperty<bool> IsDragDismissReadyProperty =
        AvaloniaProperty.Register<CodexDrawer, bool>(nameof(IsDragDismissReady));

    static CodexDrawer()
    {
        DirectionProperty.Changed.AddClassHandler<CodexDrawer>((drawer, _) => drawer.SyncDrawerClasses());
        SizeProperty.Changed.AddClassHandler<CodexDrawer>((drawer, _) => drawer.SyncDrawerClasses());
        IsHandleVisibleProperty.Changed.AddClassHandler<CodexDrawer>((drawer, _) => drawer.SyncDrawerClasses());
        ShouldScaleBackgroundProperty.Changed.AddClassHandler<CodexDrawer>((drawer, _) => drawer.SyncDrawerClasses());
        CloseOnDragDismissProperty.Changed.AddClassHandler<CodexDrawer>((drawer, _) => drawer.SyncDrawerClasses());
        DragDismissThresholdProperty.Changed.AddClassHandler<CodexDrawer>((drawer, _) => drawer.SyncDragState());
        DragOffsetProperty.Changed.AddClassHandler<CodexDrawer>((drawer, _) => drawer.SyncDragState());
        IsDraggingProperty.Changed.AddClassHandler<CodexDrawer>((drawer, _) => drawer.SyncDrawerClasses());
        IsOpenProperty.Changed.AddClassHandler<CodexDrawer>((drawer, _) => drawer.ResetDrag());
    }

    public CodexDrawer()
    {
        SyncDrawerClasses();
        SyncDragState();
    }

    public event EventHandler<CodexDrawerDragCompletedEventArgs>? DragCompleted;

    public CodexDrawerDirection Direction
    {
        get => GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool IsHandleVisible
    {
        get => GetValue(IsHandleVisibleProperty);
        set => SetValue(IsHandleVisibleProperty, value);
    }

    public bool ShouldScaleBackground
    {
        get => GetValue(ShouldScaleBackgroundProperty);
        set => SetValue(ShouldScaleBackgroundProperty, value);
    }

    public bool CloseOnDragDismiss
    {
        get => GetValue(CloseOnDragDismissProperty);
        set => SetValue(CloseOnDragDismissProperty, value);
    }

    public double DragDismissThreshold
    {
        get => GetValue(DragDismissThresholdProperty);
        set => SetValue(DragDismissThresholdProperty, value);
    }

    public double DragOffset
    {
        get => GetValue(DragOffsetProperty);
        private set => SetValue(DragOffsetProperty, value);
    }

    public bool IsDragging
    {
        get => GetValue(IsDraggingProperty);
        private set => SetValue(IsDraggingProperty, value);
    }

    public bool IsDragDismissReady => GetValue(IsDragDismissReadyProperty);

    public bool BeginDrag()
    {
        if (!CanDrag())
        {
            return false;
        }

        IsDragging = true;
        return true;
    }

    public bool DragBy(double outwardOffset)
    {
        if (!IsDragging && !BeginDrag())
        {
            return false;
        }

        DragOffset = Math.Max(0, outwardOffset);
        return true;
    }

    public bool CompleteDrag()
    {
        if (!IsDragging)
        {
            return false;
        }

        var offset = DragOffset;
        var shouldDismiss = IsDragDismissReady && CloseOnDragDismiss;
        ResetDrag();

        if (shouldDismiss)
        {
            Dismiss(CodexDialogOpenChangeSource.Pointer);
        }

        DragCompleted?.Invoke(this, new CodexDrawerDragCompletedEventArgs(offset, shouldDismiss));
        return shouldDismiss;
    }

    internal bool TryBeginHandleDrag(PointerUpdateKind updateKind, Point startPoint, IPointer? pointer = null)
    {
        if (updateKind != PointerUpdateKind.LeftButtonPressed || !BeginDrag())
        {
            return false;
        }

        _dragStart = startPoint;
        _dragPointer = pointer;
        return true;
    }

    internal bool TryCompleteHandleDrag(PointerUpdateKind updateKind, IPointer? pointer = null)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased || !IsDragging)
        {
            return false;
        }

        if (_dragPointer is not null && pointer is not null && !ReferenceEquals(pointer, _dragPointer))
        {
            return false;
        }

        CompleteDrag();
        _dragStart = null;
        _dragPointer = null;
        return true;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_handle is not null)
        {
            _handle.PointerPressed -= OnHandlePointerPressed;
            _handle.PointerMoved -= OnHandlePointerMoved;
            _handle.PointerReleased -= OnHandlePointerReleased;
            _handle.PointerCaptureLost -= OnHandlePointerCaptureLost;
        }

        base.OnApplyTemplate(e);

        _handle = e.NameScope.Find<Control>("PART_Handle");
        if (_handle is not null)
        {
            _handle.PointerPressed += OnHandlePointerPressed;
            _handle.PointerMoved += OnHandlePointerMoved;
            _handle.PointerReleased += OnHandlePointerReleased;
            _handle.PointerCaptureLost += OnHandlePointerCaptureLost;
        }
    }

    private bool CanDrag()
    {
        return IsOpen && IsEnabled && IsHandleVisible;
    }

    private void OnHandlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var updateKind = e.GetCurrentPoint(_handle).Properties.PointerUpdateKind;
        if (!TryBeginHandleDrag(updateKind, e.GetPosition(this), e.Pointer))
        {
            return;
        }

        e.Pointer.Capture(_handle);
        e.Handled = true;
    }

    private void OnHandlePointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragStart is null || _dragPointer is null || !ReferenceEquals(e.Pointer, _dragPointer))
        {
            return;
        }

        DragBy(OutwardOffset(_dragStart.Value, e.GetPosition(this)));
        e.Handled = true;
    }

    private void OnHandlePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var updateKind = e.GetCurrentPoint(_handle).Properties.PointerUpdateKind;
        if (!TryCompleteHandleDrag(updateKind, e.Pointer))
        {
            return;
        }

        e.Pointer.Capture(null);
        e.Handled = true;
    }

    private void OnHandlePointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        CompleteDrag();
        _dragStart = null;
        _dragPointer = null;
    }

    private double OutwardOffset(Point start, Point current)
    {
        return Direction switch
        {
            CodexDrawerDirection.Top => start.Y - current.Y,
            CodexDrawerDirection.Left => start.X - current.X,
            CodexDrawerDirection.Right => current.X - start.X,
            _ => current.Y - start.Y
        };
    }

    private void ResetDrag()
    {
        _dragStart = null;
        _dragPointer = null;
        IsDragging = false;
        DragOffset = 0;
    }

    private void SyncDragState()
    {
        var ready = DragOffset >= Math.Max(0, DragDismissThreshold);
        SetValue(IsDragDismissReadyProperty, ready);
        Classes.Set("drag-dismiss-ready", ready);
        Classes.Set("has-drag-offset", DragOffset > 0);
    }

    private void SyncDrawerClasses()
    {
        Classes.Set("drawer", true);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("direction-bottom", Direction == CodexDrawerDirection.Bottom);
        Classes.Set("direction-top", Direction == CodexDrawerDirection.Top);
        Classes.Set("direction-right", Direction == CodexDrawerDirection.Right);
        Classes.Set("direction-left", Direction == CodexDrawerDirection.Left);
        Classes.Set("has-handle", IsHandleVisible);
        Classes.Set("scale-background", ShouldScaleBackground);
        Classes.Set("dragging", IsDragging);
        Classes.Set("close-on-drag", CloseOnDragDismiss);
    }
}
