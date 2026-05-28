using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public class CodexTooltipProvider : ContentControl
{
    public static readonly StyledProperty<TimeSpan> DelayDurationProperty =
        AvaloniaProperty.Register<CodexTooltipProvider, TimeSpan>(nameof(DelayDuration), TimeSpan.FromMilliseconds(700));

    public static readonly StyledProperty<TimeSpan> SkipDelayDurationProperty =
        AvaloniaProperty.Register<CodexTooltipProvider, TimeSpan>(nameof(SkipDelayDuration), TimeSpan.FromMilliseconds(300));

    public static readonly StyledProperty<bool> DisableHoverableContentProperty =
        AvaloniaProperty.Register<CodexTooltipProvider, bool>(nameof(DisableHoverableContent));

    static CodexTooltipProvider()
    {
        DelayDurationProperty.Changed.AddClassHandler<CodexTooltipProvider>((provider, _) => provider.SyncClasses());
        SkipDelayDurationProperty.Changed.AddClassHandler<CodexTooltipProvider>((provider, _) => provider.SyncClasses());
        DisableHoverableContentProperty.Changed.AddClassHandler<CodexTooltipProvider>((provider, _) => provider.SyncClasses());
    }

    public CodexTooltipProvider()
    {
        Focusable = false;
        SyncClasses();
    }

    public TimeSpan DelayDuration
    {
        get => GetValue(DelayDurationProperty);
        set => SetValue(DelayDurationProperty, value);
    }

    public TimeSpan SkipDelayDuration
    {
        get => GetValue(SkipDelayDurationProperty);
        set => SetValue(SkipDelayDurationProperty, value);
    }

    public bool DisableHoverableContent
    {
        get => GetValue(DisableHoverableContentProperty);
        set => SetValue(DisableHoverableContentProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("tooltip-provider", true);
        Classes.Set("instant-open", DelayDuration <= TimeSpan.Zero);
        Classes.Set("skip-delay", SkipDelayDuration > TimeSpan.Zero);
        Classes.Set("hoverable-disabled", DisableHoverableContent);
    }
}

public enum CodexTooltipOpenChangeSource
{
    Programmatic,
    Pointer,
    Focus,
    Keyboard
}

public sealed class CodexTooltipOpenChangedEventArgs(
    bool isOpen,
    CodexTooltipOpenChangeSource source = CodexTooltipOpenChangeSource.Programmatic) : EventArgs
{
    public bool IsOpen { get; } = isOpen;

    public CodexTooltipOpenChangeSource Source { get; } = source;
}

public class CodexTooltip : ContentControl
{
    private DispatcherTimer? _openTimer;
    private DispatcherTimer? _closeTimer;
    private CodexTooltipOpenChangeSource? _pendingOpenChangeSource;

    public static readonly StyledProperty<object?> TriggerProperty =
        AvaloniaProperty.Register<CodexTooltip, object?>(nameof(Trigger));

    public static readonly StyledProperty<IDataTemplate?> TriggerTemplateProperty =
        AvaloniaProperty.Register<CodexTooltip, IDataTemplate?>(nameof(TriggerTemplate));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexTooltip, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.Register<CodexTooltip, PlacementMode>(nameof(Placement), PlacementMode.Top);

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexTooltip, bool>(nameof(IsOpen));

    public static readonly StyledProperty<bool> IsArrowVisibleProperty =
        AvaloniaProperty.Register<CodexTooltip, bool>(nameof(IsArrowVisible));

    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<CodexTooltip, bool>(nameof(CloseOnEscape), true);

    public static readonly StyledProperty<bool> DisableHoverableContentProperty =
        AvaloniaProperty.Register<CodexTooltip, bool>(nameof(DisableHoverableContent));

    public static readonly StyledProperty<TimeSpan> OpenDelayProperty =
        AvaloniaProperty.Register<CodexTooltip, TimeSpan>(nameof(OpenDelay), TimeSpan.FromMilliseconds(700));

    public static readonly StyledProperty<TimeSpan> CloseDelayProperty =
        AvaloniaProperty.Register<CodexTooltip, TimeSpan>(nameof(CloseDelay));

    public static readonly StyledProperty<bool> HasTriggerProperty =
        AvaloniaProperty.Register<CodexTooltip, bool>(nameof(HasTrigger));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexTooltip, bool>(nameof(HasContent));

    static CodexTooltip()
    {
        TriggerProperty.Changed.AddClassHandler<CodexTooltip>((tooltip, _) => tooltip.SyncSlotStates());
        SizeProperty.Changed.AddClassHandler<CodexTooltip>((tooltip, _) => tooltip.SyncClasses());
        PlacementProperty.Changed.AddClassHandler<CodexTooltip>((tooltip, _) => tooltip.SyncClasses());
        IsOpenProperty.Changed.AddClassHandler<CodexTooltip>((tooltip, args) => tooltip.OnOpenChanged(args));
        IsArrowVisibleProperty.Changed.AddClassHandler<CodexTooltip>((tooltip, _) => tooltip.SyncClasses());
        DisableHoverableContentProperty.Changed.AddClassHandler<CodexTooltip>((tooltip, _) => tooltip.SyncClasses());
        OpenDelayProperty.Changed.AddClassHandler<CodexTooltip>((tooltip, _) => tooltip.SyncClasses());
        CloseDelayProperty.Changed.AddClassHandler<CodexTooltip>((tooltip, _) => tooltip.SyncClasses());
        ContentProperty.Changed.AddClassHandler<CodexTooltip>((tooltip, _) => tooltip.SyncSlotStates());
    }

    public CodexTooltip()
    {
        Focusable = false;
        IsHitTestVisible = false;
        SyncClasses();
        SyncSlotStates();
    }

    public event EventHandler<CodexTooltipOpenChangedEventArgs>? OpenChanged;

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

    public bool DisableHoverableContent
    {
        get => GetValue(DisableHoverableContentProperty);
        set => SetValue(DisableHoverableContentProperty, value);
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

    public bool HasTrigger => GetValue(HasTriggerProperty);

    public bool HasContent => GetValue(HasContentProperty);

    public void Open()
    {
        Open(CodexTooltipOpenChangeSource.Programmatic);
    }

    internal void Open(CodexTooltipOpenChangeSource source)
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
        return Dismiss(CodexTooltipOpenChangeSource.Programmatic);
    }

    internal bool Dismiss(CodexTooltipOpenChangeSource source)
    {
        StopTimers();

        if (!IsOpen)
        {
            return false;
        }

        RunWithOpenChangeSource(source, () => IsOpen = false);
        return true;
    }

    internal bool RequestOpen(CodexTooltipOpenChangeSource source = CodexTooltipOpenChangeSource.Pointer)
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

        var openDelay = EffectiveOpenDelay();
        if (openDelay <= TimeSpan.Zero)
        {
            Open(source);
            return true;
        }

        StartTimer(ref _openTimer, openDelay, () => Open(source));
        return true;
    }

    internal bool RequestFocusOpen()
    {
        if (!IsEnabled)
        {
            return false;
        }

        StopTimers();

        if (IsOpen)
        {
            return false;
        }

        Open(CodexTooltipOpenChangeSource.Focus);
        return true;
    }

    internal bool RequestClose(CodexTooltipOpenChangeSource source = CodexTooltipOpenChangeSource.Pointer)
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
        if (!IsOpen)
        {
            return false;
        }

        return key switch
        {
            Key.Escape => CloseOnEscape && Dismiss(CodexTooltipOpenChangeSource.Keyboard),
            Key.Enter or Key.Space => Dismiss(CodexTooltipOpenChangeSource.Keyboard),
            _ => false
        };
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        if (HasTrigger)
        {
            RequestOpen(CodexTooltipOpenChangeSource.Pointer);
        }
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        if (HasTrigger)
        {
            RequestClose(CodexTooltipOpenChangeSource.Pointer);
        }
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        if (HasTrigger)
        {
            RequestFocusOpen();
        }
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        if (HasTrigger)
        {
            RequestClose(CodexTooltipOpenChangeSource.Focus);
        }
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

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        StopTimers();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SyncClasses();
    }

    private void SyncClasses()
    {
        var effectiveOpenDelay = EffectiveOpenDelay();

        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("has-arrow", IsArrowVisible);
        Classes.Set("instant-open", effectiveOpenDelay <= TimeSpan.Zero);
        Classes.Set("delayed-open", effectiveOpenDelay > TimeSpan.Zero);
        Classes.Set("instant-close", CloseDelay <= TimeSpan.Zero);
        Classes.Set("delayed-close", CloseDelay > TimeSpan.Zero);
        Classes.Set("hoverable-disabled", EffectiveDisableHoverableContent());
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
            OpenChanged?.Invoke(this, new CodexTooltipOpenChangedEventArgs(IsOpen, CurrentOpenChangeSource));
        }
    }

    private void SyncSlotStates()
    {
        SetValue(HasTriggerProperty, HasValue(Trigger));
        SetValue(HasContentProperty, HasValue(Content));
        Classes.Set("has-trigger", HasTrigger);
        Classes.Set("has-content", HasContent);
        SetCurrentValue(IsHitTestVisibleProperty, HasTrigger);
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

    private TimeSpan EffectiveOpenDelay()
    {
        if (IsSet(OpenDelayProperty))
        {
            return OpenDelay;
        }

        return FindProvider()?.DelayDuration ?? OpenDelay;
    }

    private bool EffectiveDisableHoverableContent()
    {
        if (DisableHoverableContent)
        {
            return true;
        }

        return !IsSet(DisableHoverableContentProperty) && FindProvider()?.DisableHoverableContent == true;
    }

    private CodexTooltipProvider? FindProvider()
    {
        return this.GetLogicalAncestors().OfType<CodexTooltipProvider>().FirstOrDefault();
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }

    private CodexTooltipOpenChangeSource CurrentOpenChangeSource =>
        _pendingOpenChangeSource ?? CodexTooltipOpenChangeSource.Programmatic;

    private void RunWithOpenChangeSource(CodexTooltipOpenChangeSource source, Action action)
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
