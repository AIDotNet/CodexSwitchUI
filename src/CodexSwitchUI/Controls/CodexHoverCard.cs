using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public enum CodexHoverCardAlign
{
    Center,
    Start,
    End
}

public enum CodexHoverCardOpenChangeSource
{
    Programmatic,
    Pointer,
    Focus,
    Keyboard
}

public sealed class CodexHoverCardOpenChangedEventArgs(
    bool isOpen,
    CodexHoverCardOpenChangeSource source = CodexHoverCardOpenChangeSource.Programmatic) : EventArgs
{
    public bool IsOpen { get; } = isOpen;

    public CodexHoverCardOpenChangeSource Source { get; } = source;
}

public class CodexHoverCard : ContentControl
{
    private DispatcherTimer? _openTimer;
    private DispatcherTimer? _closeTimer;
    private CodexHoverCardOpenChangeSource? _pendingOpenChangeSource;
    private Border? _surface;

    public static readonly StyledProperty<object?> TriggerProperty =
        AvaloniaProperty.Register<CodexHoverCard, object?>(nameof(Trigger));

    public static readonly StyledProperty<IDataTemplate?> TriggerTemplateProperty =
        AvaloniaProperty.Register<CodexHoverCard, IDataTemplate?>(nameof(TriggerTemplate));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexHoverCard, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.Register<CodexHoverCard, PlacementMode>(nameof(Placement), PlacementMode.Bottom);

    public static readonly StyledProperty<PlacementMode> EffectivePlacementProperty =
        AvaloniaProperty.Register<CodexHoverCard, PlacementMode>(nameof(EffectivePlacement), PlacementMode.Bottom);

    public static readonly StyledProperty<CodexHoverCardAlign> AlignProperty =
        AvaloniaProperty.Register<CodexHoverCard, CodexHoverCardAlign>(nameof(Align), CodexHoverCardAlign.Center);

    public static readonly StyledProperty<TimeSpan> OpenDelayProperty =
        AvaloniaProperty.Register<CodexHoverCard, TimeSpan>(nameof(OpenDelay), TimeSpan.FromMilliseconds(700));

    public static readonly StyledProperty<TimeSpan> CloseDelayProperty =
        AvaloniaProperty.Register<CodexHoverCard, TimeSpan>(nameof(CloseDelay), TimeSpan.FromMilliseconds(300));

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexHoverCard, bool>(nameof(IsOpen));

    public static readonly StyledProperty<bool> IsArrowVisibleProperty =
        AvaloniaProperty.Register<CodexHoverCard, bool>(nameof(IsArrowVisible), true);

    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<CodexHoverCard, bool>(nameof(CloseOnEscape), true);

    public static readonly StyledProperty<bool> HasTriggerProperty =
        AvaloniaProperty.Register<CodexHoverCard, bool>(nameof(HasTrigger));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexHoverCard, bool>(nameof(HasContent));

    static CodexHoverCard()
    {
        TriggerProperty.Changed.AddClassHandler<CodexHoverCard>((card, _) => card.SyncSlotStates());
        ContentProperty.Changed.AddClassHandler<CodexHoverCard>((card, _) => card.SyncSlotStates());
        SizeProperty.Changed.AddClassHandler<CodexHoverCard>((card, _) => card.SyncClasses());
        PlacementProperty.Changed.AddClassHandler<CodexHoverCard>((card, _) => card.SyncClasses());
        AlignProperty.Changed.AddClassHandler<CodexHoverCard>((card, _) => card.SyncClasses());
        OpenDelayProperty.Changed.AddClassHandler<CodexHoverCard>((card, _) => card.SyncClasses());
        CloseDelayProperty.Changed.AddClassHandler<CodexHoverCard>((card, _) => card.SyncClasses());
        IsOpenProperty.Changed.AddClassHandler<CodexHoverCard>((card, args) => card.OnOpenChanged(args));
        IsArrowVisibleProperty.Changed.AddClassHandler<CodexHoverCard>((card, _) => card.SyncClasses());
    }

    public CodexHoverCard()
    {
        Focusable = false;
        SyncClasses();
        SyncSlotStates();
    }

    public event EventHandler<CodexHoverCardOpenChangedEventArgs>? OpenChanged;

    public object? Trigger
    {
        get => GetValue(TriggerProperty);
        set => SetValue(TriggerProperty, value);
    }

    public IDataTemplate? TriggerTemplate
    {
        get => GetValue(TriggerTemplateProperty);
        set => SetValue(TriggerTemplateProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    public PlacementMode EffectivePlacement
    {
        get => GetValue(EffectivePlacementProperty);
        private set => SetValue(EffectivePlacementProperty, value);
    }

    public CodexHoverCardAlign Align
    {
        get => GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }

    public TimeSpan OpenDelay
    {
        get => GetValue(OpenDelayProperty);
        set => SetValue(OpenDelayProperty, value);
    }

    public TimeSpan CloseDelay
    {
        get => GetValue(CloseDelayProperty);
        set => SetValue(CloseDelayProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool IsArrowVisible
    {
        get => GetValue(IsArrowVisibleProperty);
        set => SetValue(IsArrowVisibleProperty, value);
    }

    public bool CloseOnEscape
    {
        get => GetValue(CloseOnEscapeProperty);
        set => SetValue(CloseOnEscapeProperty, value);
    }

    public bool HasTrigger => GetValue(HasTriggerProperty);

    public bool HasContent => GetValue(HasContentProperty);

    public void Open()
    {
        Open(CodexHoverCardOpenChangeSource.Programmatic);
    }

    internal void Open(CodexHoverCardOpenChangeSource source)
    {
        if (!IsEnabled)
        {
            return;
        }

        StopTimers();
        RunWithOpenChangeSource(source, () => IsOpen = true);
    }

    public bool Dismiss()
    {
        return Dismiss(CodexHoverCardOpenChangeSource.Programmatic);
    }

    internal bool Dismiss(CodexHoverCardOpenChangeSource source)
    {
        StopTimers();

        if (!IsOpen)
        {
            return false;
        }

        RunWithOpenChangeSource(source, () => IsOpen = false);
        return true;
    }

    internal bool RequestOpen(CodexHoverCardOpenChangeSource source = CodexHoverCardOpenChangeSource.Pointer)
    {
        if (!IsEnabled)
        {
            return false;
        }

        StopCloseTimer();

        if (IsOpen)
        {
            return false;
        }

        if (OpenDelay <= TimeSpan.Zero)
        {
            Open(source);
            return true;
        }

        StartTimer(ref _openTimer, OpenDelay, () => Open(source));
        return true;
    }

    internal bool RequestClose(CodexHoverCardOpenChangeSource source = CodexHoverCardOpenChangeSource.Pointer)
    {
        StopOpenTimer();

        if (!IsOpen)
        {
            return false;
        }

        if (CloseDelay <= TimeSpan.Zero)
        {
            return Dismiss(source);
        }

        StartTimer(ref _closeTimer, CloseDelay, () => Dismiss(source));
        return true;
    }

    internal bool TryHandleDismissKey(Key key)
    {
        return key == Key.Escape && CloseOnEscape && Dismiss(CodexHoverCardOpenChangeSource.Keyboard);
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        RequestOpen(CodexHoverCardOpenChangeSource.Pointer);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        RequestClose(CodexHoverCardOpenChangeSource.Pointer);
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        RequestOpen(CodexHoverCardOpenChangeSource.Focus);
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        RequestClose(CodexHoverCardOpenChangeSource.Focus);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (TryHandleDismissKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_surface is not null)
        {
            _surface.PointerEntered -= OnSurfacePointerEntered;
            _surface.PointerExited -= OnSurfacePointerExited;
        }

        base.OnApplyTemplate(e);

        _surface = e.NameScope.Find<Border>("PART_Surface");

        if (_surface is not null)
        {
            _surface.PointerEntered += OnSurfacePointerEntered;
            _surface.PointerExited += OnSurfacePointerExited;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_surface is not null)
        {
            _surface.PointerEntered -= OnSurfacePointerEntered;
            _surface.PointerExited -= OnSurfacePointerExited;
            _surface = null;
        }

        StopTimers();
        base.OnDetachedFromVisualTree(e);
    }

    private void OnSurfacePointerEntered(object? sender, PointerEventArgs e)
    {
        RequestOpen(CodexHoverCardOpenChangeSource.Pointer);
    }

    private void OnSurfacePointerExited(object? sender, PointerEventArgs e)
    {
        RequestClose(CodexHoverCardOpenChangeSource.Pointer);
    }

    private void SyncClasses()
    {
        EffectivePlacement = CodexPopupPlacement.Resolve(Placement, Align);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("has-arrow", IsArrowVisible);
        Classes.Set("instant-open", OpenDelay <= TimeSpan.Zero);
        Classes.Set("delayed-open", OpenDelay > TimeSpan.Zero);
        Classes.Set("instant-close", CloseDelay <= TimeSpan.Zero);
        Classes.Set("delayed-close", CloseDelay > TimeSpan.Zero);
        Classes.Set("align-center", Align == CodexHoverCardAlign.Center);
        Classes.Set("align-start", Align == CodexHoverCardAlign.Start);
        Classes.Set("align-end", Align == CodexHoverCardAlign.End);
        Classes.Set("side-top", Placement is PlacementMode.Top or PlacementMode.TopEdgeAlignedLeft or PlacementMode.TopEdgeAlignedRight);
        Classes.Set("side-left", Placement is PlacementMode.Left or PlacementMode.LeftEdgeAlignedTop or PlacementMode.LeftEdgeAlignedBottom);
        Classes.Set("side-right", Placement is PlacementMode.Right or PlacementMode.RightEdgeAlignedTop or PlacementMode.RightEdgeAlignedBottom);
        Classes.Set("side-bottom", Placement is not (PlacementMode.Top or PlacementMode.TopEdgeAlignedLeft or PlacementMode.TopEdgeAlignedRight
            or PlacementMode.Left or PlacementMode.LeftEdgeAlignedTop or PlacementMode.LeftEdgeAlignedBottom
            or PlacementMode.Right or PlacementMode.RightEdgeAlignedTop or PlacementMode.RightEdgeAlignedBottom));
    }

    private void OnOpenChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncClasses();

        if (args.OldValue is bool oldValue && oldValue != IsOpen)
        {
            OpenChanged?.Invoke(this, new CodexHoverCardOpenChangedEventArgs(IsOpen, CurrentOpenChangeSource));
        }
    }

    private void SyncSlotStates()
    {
        SetValue(HasTriggerProperty, HasValue(Trigger));
        SetValue(HasContentProperty, HasValue(Content));
        Classes.Set("has-trigger", HasTrigger);
        Classes.Set("has-content", HasContent);
    }

    private void StartTimer(ref DispatcherTimer? timer, TimeSpan delay, Action tick)
    {
        timer ??= new DispatcherTimer { Interval = delay };
        timer.Stop();
        timer.Interval = delay;
        timer.Tick -= OnTick;
        timer.Tick += OnTick;
        timer.Start();

        void OnTick(object? sender, EventArgs args)
        {
            if (sender is DispatcherTimer activeTimer)
            {
                activeTimer.Stop();
                activeTimer.Tick -= OnTick;
            }

            tick();
        }
    }

    private void StopTimers()
    {
        StopOpenTimer();
        StopCloseTimer();
    }

    private void StopOpenTimer()
    {
        _openTimer?.Stop();
    }

    private void StopCloseTimer()
    {
        _closeTimer?.Stop();
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }

    private CodexHoverCardOpenChangeSource CurrentOpenChangeSource =>
        _pendingOpenChangeSource ?? CodexHoverCardOpenChangeSource.Programmatic;

    private void RunWithOpenChangeSource(CodexHoverCardOpenChangeSource source, Action action)
    {
        var previousSource = _pendingOpenChangeSource;
        _pendingOpenChangeSource = source;
        try
        {
            action();
        }
        finally
        {
            _pendingOpenChangeSource = previousSource;
        }
    }
}
