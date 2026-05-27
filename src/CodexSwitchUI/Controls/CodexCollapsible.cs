using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Themes;

namespace CodexSwitchUI.Controls;

public sealed class CodexCollapsibleOpenChangedEventArgs(bool isOpen) : EventArgs
{
    public bool IsOpen { get; } = isOpen;
}

public class CodexCollapsible : CodexFrame
{
    private static readonly TimeSpan FrameInterval = TimeSpan.FromMilliseconds(16);
    private static readonly TimeSpan DefaultAnimationDuration = CodexSwitchThemeOptions.ShadcnDefault.MotionDurationSlow;

    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<CodexCollapsible, object?>(nameof(Header));

    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<CodexCollapsible, IDataTemplate?>(nameof(HeaderTemplate));

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexCollapsible, bool>(nameof(IsOpen));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexCollapsible, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<Thickness> ContentPaddingProperty =
        AvaloniaProperty.Register<CodexCollapsible, Thickness>(nameof(ContentPadding), new Thickness(0, 6, 0, 0));

    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<CodexCollapsible, TimeSpan>(nameof(AnimationDuration), DefaultAnimationDuration);

    public static readonly StyledProperty<double> AnimatedHeightProperty =
        AvaloniaProperty.Register<CodexCollapsible, double>(nameof(AnimatedHeight));

    public static readonly StyledProperty<double> ContentMeasuredHeightProperty =
        AvaloniaProperty.Register<CodexCollapsible, double>(nameof(ContentMeasuredHeight));

    public static readonly StyledProperty<bool> IsContentVisibleProperty =
        AvaloniaProperty.Register<CodexCollapsible, bool>(nameof(IsContentVisible));

    public static readonly StyledProperty<bool> HasHeaderProperty =
        AvaloniaProperty.Register<CodexCollapsible, bool>(nameof(HasHeader));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexCollapsible, bool>(nameof(HasContent));

    private readonly Stopwatch _heightAnimationStopwatch = new();
    private Button? _trigger;
    private Control? _triggerLayout;
    private Control? _contentMeasure;
    private DispatcherTimer? _heightAnimationTimer;
    private double _animationFrom;
    private double _animationTo;
    private bool _collapseWhenAnimationCompletes;
    private bool _isMeasureQueued;
    private bool _isAttached;

    static CodexCollapsible()
    {
        HeaderProperty.Changed.AddClassHandler<CodexCollapsible>((collapsible, _) => collapsible.SyncSlotStates());
        ContentControl.ContentProperty.Changed.AddClassHandler<CodexCollapsible>((collapsible, _) =>
        {
            collapsible.SyncSlotStates();
            collapsible.RequestContentMeasure();
        });
        IsOpenProperty.Changed.AddClassHandler<CodexCollapsible>((collapsible, args) => collapsible.OnOpenChanged(args));
        SizeProperty.Changed.AddClassHandler<CodexCollapsible>((collapsible, _) => collapsible.SyncClasses());
        ContentPaddingProperty.Changed.AddClassHandler<CodexCollapsible>((collapsible, _) => collapsible.RequestContentMeasure());
        AnimationDurationProperty.Changed.AddClassHandler<CodexCollapsible>((collapsible, _) =>
        {
            if (collapsible.IsOpen)
            {
                collapsible.RequestContentMeasure();
            }
        });
    }

    public CodexCollapsible()
    {
        Focusable = false;
        SetValue(IsContentVisibleProperty, IsOpen);
        SyncClasses();
        SyncSlotStates();
    }

    public event EventHandler<CodexCollapsibleOpenChangedEventArgs>? OpenChanged;

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public Thickness ContentPadding
    {
        get => GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }

    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    public double AnimatedHeight => GetValue(AnimatedHeightProperty);

    public double ContentMeasuredHeight => GetValue(ContentMeasuredHeightProperty);

    public bool IsContentVisible => GetValue(IsContentVisibleProperty);

    public bool HasHeader => GetValue(HasHeaderProperty);

    public bool HasContent => GetValue(HasContentProperty);

    public virtual void Toggle()
    {
        if (IsEnabled)
        {
            IsOpen = !IsOpen;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_trigger is not null)
        {
            _trigger.RemoveHandler(InputElement.KeyDownEvent, OnTriggerKeyDown);
        }

        if (_triggerLayout is not null)
        {
            _triggerLayout.RemoveHandler(InputElement.PointerPressedEvent, OnTriggerPointerPressed);
        }

        base.OnApplyTemplate(e);

        _triggerLayout = e.NameScope.Find<Control>("PART_TriggerLayout");
        _trigger = e.NameScope.Find<Button>("PART_Trigger");
        _contentMeasure = e.NameScope.Find<Control>("PART_ContentMeasure");

        if (_triggerLayout is not null)
        {
            _triggerLayout.AddHandler(
                InputElement.PointerPressedEvent,
                OnTriggerPointerPressed,
                RoutingStrategies.Bubble,
                handledEventsToo: true);
        }

        if (_trigger is not null)
        {
            _trigger.AddHandler(
                InputElement.KeyDownEvent,
                OnTriggerKeyDown,
                RoutingStrategies.Bubble,
                handledEventsToo: true);
        }

        SyncOpenState(immediate: true);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;
        SyncOpenState(immediate: true);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        StopHeightAnimation();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == BoundsProperty && IsOpen)
        {
            RequestContentMeasure();
        }
    }

    private void OnTriggerPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(_triggerLayout).Properties.IsLeftButtonPressed)
        {
            return;
        }

        _trigger?.Focus(NavigationMethod.Pointer, KeyModifiers.None);
        Toggle();
        e.Handled = true;
    }

    private void OnTriggerKeyDown(object? sender, KeyEventArgs e)
    {
        if (!TryHandleTriggerKey(e.Key))
        {
            return;
        }

        e.Handled = true;
    }

    internal virtual bool TryHandleTriggerKey(Key key)
    {
        if (key is not (Key.Enter or Key.Space))
        {
            return false;
        }

        Toggle();
        return true;
    }

    internal bool FocusTrigger(NavigationMethod method = NavigationMethod.Directional)
    {
        return _trigger?.Focus(method, KeyModifiers.None) == true;
    }

    private void OnOpenChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncOpenState(immediate: false);

        if (args.OldValue is bool oldValue && args.NewValue is bool newValue && oldValue != newValue)
        {
            OpenChanged?.Invoke(this, new CodexCollapsibleOpenChangedEventArgs(newValue));
        }
    }

    private void SyncOpenState(bool immediate)
    {
        SyncClasses();

        if (IsOpen)
        {
            ExpandContent(immediate);
        }
        else
        {
            CollapseContent(immediate);
        }
    }

    private void ExpandContent(bool immediate)
    {
        SetValue(IsContentVisibleProperty, true);

        var targetHeight = MeasureContentHeight();
        if (targetHeight <= 0)
        {
            RequestContentMeasure();
        }

        if (immediate || !_isAttached)
        {
            SetAnimatedHeight(targetHeight);
            return;
        }

        StartHeightAnimation(AnimatedHeight, targetHeight, collapseWhenDone: false);
    }

    private void CollapseContent(bool immediate)
    {
        var measuredHeight = MeasureContentHeight();
        var fromHeight = AnimatedHeight > 0
            ? AnimatedHeight
            : measuredHeight;

        SetValue(IsContentVisibleProperty, true);

        if (immediate || !_isAttached)
        {
            StopHeightAnimation();
            SetAnimatedHeight(0);
            SetValue(IsContentVisibleProperty, false);
            return;
        }

        StartHeightAnimation(fromHeight, 0, collapseWhenDone: true);
    }

    private void RequestContentMeasure()
    {
        if (_isMeasureQueued)
        {
            return;
        }

        _isMeasureQueued = true;
        Dispatcher.UIThread.Post(() =>
        {
            _isMeasureQueued = false;
            UpdateMeasuredHeight();
        }, DispatcherPriority.Loaded);
    }

    private void UpdateMeasuredHeight()
    {
        var measuredHeight = MeasureContentHeight();

        if (!IsOpen)
        {
            if (!IsContentVisible)
            {
                SetAnimatedHeight(0);
            }

            return;
        }

        if (!_isAttached || AnimationDuration <= TimeSpan.Zero)
        {
            SetAnimatedHeight(measuredHeight);
            return;
        }

        if (Math.Abs(AnimatedHeight - measuredHeight) > 0.5)
        {
            StartHeightAnimation(AnimatedHeight, measuredHeight, collapseWhenDone: false);
        }
    }

    private double MeasureContentHeight()
    {
        if (_contentMeasure is null)
        {
            SetValue(ContentMeasuredHeightProperty, 0d);
            return 0;
        }

        var availableWidth = Bounds.Width;
        if (double.IsNaN(availableWidth) || availableWidth <= 0 || double.IsInfinity(availableWidth))
        {
            availableWidth = double.PositiveInfinity;
        }

        _contentMeasure.Measure(new Size(availableWidth, double.PositiveInfinity));
        var height = Math.Ceiling(Math.Max(0, _contentMeasure.DesiredSize.Height));
        SetValue(ContentMeasuredHeightProperty, height);
        return height;
    }

    private void StartHeightAnimation(double from, double to, bool collapseWhenDone)
    {
        StopHeightAnimation();

        from = NormalizeHeight(from);
        to = NormalizeHeight(to);
        _animationFrom = from;
        _animationTo = to;
        _collapseWhenAnimationCompletes = collapseWhenDone;
        SetValue(IsContentVisibleProperty, true);
        SetAnimatedHeight(from);

        if (AnimationDuration <= TimeSpan.Zero || Math.Abs(from - to) <= 0.5)
        {
            CompleteHeightAnimation();
            return;
        }

        if (_heightAnimationTimer is null)
        {
            _heightAnimationTimer = new DispatcherTimer
            {
                Interval = FrameInterval
            };
            _heightAnimationTimer.Tick += OnHeightAnimationTick;
        }

        _heightAnimationStopwatch.Restart();
        _heightAnimationTimer.Start();
    }

    private void StopHeightAnimation()
    {
        _heightAnimationTimer?.Stop();
        _heightAnimationStopwatch.Reset();
        _collapseWhenAnimationCompletes = false;
    }

    private void OnHeightAnimationTick(object? sender, EventArgs e)
    {
        var duration = AnimationDuration.TotalMilliseconds;
        if (duration <= 0)
        {
            CompleteHeightAnimation();
            return;
        }

        var progress = Math.Clamp(_heightAnimationStopwatch.Elapsed.TotalMilliseconds / duration, 0, 1);
        var easedProgress = CssEaseOut(progress);
        SetAnimatedHeight(Lerp(_animationFrom, _animationTo, easedProgress));

        if (progress >= 1)
        {
            CompleteHeightAnimation();
        }
    }

    private void CompleteHeightAnimation()
    {
        _heightAnimationTimer?.Stop();
        _heightAnimationStopwatch.Reset();
        SetAnimatedHeight(_animationTo);

        if (_collapseWhenAnimationCompletes && !IsOpen)
        {
            SetValue(IsContentVisibleProperty, false);
        }

        _collapseWhenAnimationCompletes = false;
    }

    private void SetAnimatedHeight(double height)
    {
        SetValue(AnimatedHeightProperty, NormalizeHeight(height));
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
    }

    private void SyncSlotStates()
    {
        var hasHeader = HasValue(Header);
        var hasContent = HasValue(Content);

        SetValue(HasHeaderProperty, hasHeader);
        SetValue(HasContentProperty, hasContent);
        Classes.Set("has-header", hasHeader);
        Classes.Set("has-content", hasContent);
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }

    private static double NormalizeHeight(double height)
    {
        if (double.IsNaN(height) || double.IsInfinity(height) || height < 0)
        {
            return 0;
        }

        return height;
    }

    private static double Lerp(double start, double end, double progress)
    {
        return start + ((end - start) * progress);
    }

    private static double CssEaseOut(double progress)
    {
        return CubicBezier(progress, 0, 0, 0.2, 1);
    }

    private static double CubicBezier(double progress, double x1, double y1, double x2, double y2)
    {
        progress = Math.Clamp(progress, 0, 1);
        var t = progress;

        for (var i = 0; i < 6; i++)
        {
            var x = SampleCurve(t, x1, x2) - progress;
            var derivative = SampleCurveDerivative(t, x1, x2);
            if (Math.Abs(x) < 0.000001 || Math.Abs(derivative) < 0.000001)
            {
                break;
            }

            var next = t - (x / derivative);
            if (next < 0 || next > 1)
            {
                break;
            }

            t = next;
        }

        if (Math.Abs(SampleCurve(t, x1, x2) - progress) > 0.000001)
        {
            var min = 0d;
            var max = 1d;
            t = progress;

            for (var i = 0; i < 10; i++)
            {
                var x = SampleCurve(t, x1, x2);
                if (Math.Abs(x - progress) < 0.000001)
                {
                    break;
                }

                if (x < progress)
                {
                    min = t;
                }
                else
                {
                    max = t;
                }

                t = (min + max) / 2d;
            }
        }

        return SampleCurve(t, y1, y2);
    }

    private static double SampleCurve(double t, double control1, double control2)
    {
        var inverse = 1 - t;
        return (3 * inverse * inverse * t * control1)
            + (3 * inverse * t * t * control2)
            + (t * t * t);
    }

    private static double SampleCurveDerivative(double t, double control1, double control2)
    {
        var inverse = 1 - t;
        return (3 * inverse * inverse * control1)
            + (6 * inverse * t * (control2 - control1))
            + (3 * t * t * (1 - control2));
    }
}
