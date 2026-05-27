using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Themes;

namespace CodexSwitchUI.Controls;

public class CodexSkeleton : CodexFrame
{
    private static readonly TimeSpan FrameInterval = TimeSpan.FromMilliseconds(16);
    private static readonly TimeSpan DefaultPulseDuration = CodexSwitchThemeOptions.ShadcnDefault.SkeletonShimmerDuration;

    public static readonly StyledProperty<bool> IsAnimatedProperty =
        AvaloniaProperty.Register<CodexSkeleton, bool>(nameof(IsAnimated), true);

    public static readonly StyledProperty<double> PulseOpacityProperty =
        AvaloniaProperty.Register<CodexSkeleton, double>(nameof(PulseOpacity), 1);

    public static readonly StyledProperty<double> PulseLowOpacityProperty =
        AvaloniaProperty.Register<CodexSkeleton, double>(nameof(PulseLowOpacity), 0.5);

    public static readonly StyledProperty<double> PulseHighOpacityProperty =
        AvaloniaProperty.Register<CodexSkeleton, double>(nameof(PulseHighOpacity), 1);

    public static readonly StyledProperty<TimeSpan> PulseDurationProperty =
        AvaloniaProperty.Register<CodexSkeleton, TimeSpan>(nameof(PulseDuration), DefaultPulseDuration);

    public static readonly StyledProperty<double> ShimmerOpacityProperty =
        AvaloniaProperty.Register<CodexSkeleton, double>(nameof(ShimmerOpacity), 0);

    public static readonly StyledProperty<double> ShimmerLowOpacityProperty =
        AvaloniaProperty.Register<CodexSkeleton, double>(nameof(ShimmerLowOpacity), 0);

    public static readonly StyledProperty<double> ShimmerHighOpacityProperty =
        AvaloniaProperty.Register<CodexSkeleton, double>(nameof(ShimmerHighOpacity), 0);

    public static readonly StyledProperty<IBrush?> ShimmerBrushProperty =
        AvaloniaProperty.Register<CodexSkeleton, IBrush?>(nameof(ShimmerBrush));

    private readonly Stopwatch _pulseStopwatch = new();
    private DispatcherTimer? _animationTimer;
    private bool _isAttached;

    static CodexSkeleton()
    {
        IsAnimatedProperty.Changed.AddClassHandler<CodexSkeleton>((skeleton, _) =>
        {
            skeleton.SyncClasses();
            skeleton.SyncAnimation(restart: true);
        });
        PulseDurationProperty.Changed.AddClassHandler<CodexSkeleton>((skeleton, _) => skeleton.SyncAnimation(restart: true));
        PulseLowOpacityProperty.Changed.AddClassHandler<CodexSkeleton>((skeleton, _) => skeleton.ApplyPulseFrame());
        PulseHighOpacityProperty.Changed.AddClassHandler<CodexSkeleton>((skeleton, _) => skeleton.ApplyPulseFrame());
        ShimmerLowOpacityProperty.Changed.AddClassHandler<CodexSkeleton>((skeleton, _) => skeleton.ApplyPulseFrame());
        ShimmerHighOpacityProperty.Changed.AddClassHandler<CodexSkeleton>((skeleton, _) => skeleton.ApplyPulseFrame());
    }

    public CodexSkeleton()
    {
        Focusable = false;
        IsHitTestVisible = false;
        SyncClasses();
        ApplyStaticFrame();
    }

    public bool IsAnimated
    {
        get => GetValue(IsAnimatedProperty);
        set => SetValue(IsAnimatedProperty, value);
    }

    public double PulseOpacity
    {
        get => GetValue(PulseOpacityProperty);
        set => SetValue(PulseOpacityProperty, value);
    }

    public double PulseLowOpacity
    {
        get => GetValue(PulseLowOpacityProperty);
        set => SetValue(PulseLowOpacityProperty, value);
    }

    public double PulseHighOpacity
    {
        get => GetValue(PulseHighOpacityProperty);
        set => SetValue(PulseHighOpacityProperty, value);
    }

    public TimeSpan PulseDuration
    {
        get => GetValue(PulseDurationProperty);
        set => SetValue(PulseDurationProperty, value);
    }

    public double ShimmerOpacity
    {
        get => GetValue(ShimmerOpacityProperty);
        set => SetValue(ShimmerOpacityProperty, value);
    }

    public double ShimmerLowOpacity
    {
        get => GetValue(ShimmerLowOpacityProperty);
        set => SetValue(ShimmerLowOpacityProperty, value);
    }

    public double ShimmerHighOpacity
    {
        get => GetValue(ShimmerHighOpacityProperty);
        set => SetValue(ShimmerHighOpacityProperty, value);
    }

    public IBrush? ShimmerBrush
    {
        get => GetValue(ShimmerBrushProperty);
        set => SetValue(ShimmerBrushProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;
        SyncAnimation(restart: true);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        StopAnimation();
        base.OnDetachedFromVisualTree(e);
    }

    private void SyncAnimation(bool restart = false)
    {
        if (!_isAttached)
        {
            return;
        }

        if (IsAnimated && PulseDuration > TimeSpan.Zero)
        {
            StartAnimation(restart);
        }
        else
        {
            StopAnimation();
            ApplyStaticFrame();
        }
    }

    private void StartAnimation(bool restart)
    {
        if (_animationTimer is null)
        {
            _animationTimer = new DispatcherTimer
            {
                Interval = FrameInterval
            };
            _animationTimer.Tick += OnAnimationTimerTick;
        }

        if (restart || !_pulseStopwatch.IsRunning)
        {
            _pulseStopwatch.Restart();
        }

        ApplyPulseFrame();
        _animationTimer.Start();
    }

    private void StopAnimation()
    {
        _animationTimer?.Stop();
        _pulseStopwatch.Reset();
    }

    private void OnAnimationTimerTick(object? sender, EventArgs e)
    {
        ApplyPulseFrame();
    }

    private void ApplyPulseFrame()
    {
        if (!_pulseStopwatch.IsRunning || PulseDuration <= TimeSpan.Zero)
        {
            ApplyStaticFrame();
            return;
        }

        var progress = (_pulseStopwatch.Elapsed.TotalMilliseconds % PulseDuration.TotalMilliseconds)
            / PulseDuration.TotalMilliseconds;

        ApplyPulseProgress(progress);
    }

    private void ApplyPulseProgress(double progress)
    {
        var lowPulse = ClampOpacity(PulseLowOpacity);
        var highPulse = ClampOpacity(PulseHighOpacity);
        var lowShimmer = ClampOpacity(ShimmerLowOpacity);
        var highShimmer = ClampOpacity(ShimmerHighOpacity);

        var segmentProgress = progress <= 0.5
            ? progress * 2
            : (progress - 0.5) * 2;
        var eased = CssPulseEase(segmentProgress);

        PulseOpacity = progress <= 0.5
            ? Lerp(highPulse, lowPulse, eased)
            : Lerp(lowPulse, highPulse, eased);
        ShimmerOpacity = progress <= 0.5
            ? Lerp(highShimmer, lowShimmer, eased)
            : Lerp(lowShimmer, highShimmer, eased);
    }

    private void ApplyStaticFrame()
    {
        PulseOpacity = ClampOpacity(PulseHighOpacity);
        ShimmerOpacity = 0;
    }

    private void SyncClasses()
    {
        Classes.Set("animated", IsAnimated);
        Classes.Set("static", !IsAnimated);
    }

    private static double ClampOpacity(double value)
    {
        return Math.Clamp(value, 0, 1);
    }

    private static double Lerp(double start, double end, double progress)
    {
        return start + ((end - start) * progress);
    }

    private static double CssPulseEase(double progress)
    {
        return CubicBezier(progress, 0.4, 0, 0.6, 1);
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
