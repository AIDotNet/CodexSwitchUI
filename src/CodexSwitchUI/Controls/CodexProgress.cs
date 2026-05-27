using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CodexSwitchUI.Themes;
using System.Diagnostics;

namespace CodexSwitchUI.Controls;

public class CodexProgress : ProgressBar
{
    private readonly Stopwatch _indeterminateStopwatch = new();
    private DispatcherTimer? _indeterminateTimer;
    private bool _isAttached;
    private double _indeterminateProgress;

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexProgress, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexProgress, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<TimeSpan> IndeterminateAnimationDurationProperty =
        AvaloniaProperty.Register<CodexProgress, TimeSpan>(nameof(IndeterminateAnimationDuration), CodexSwitchThemeOptions.ShadcnDefault.SkeletonShimmerDuration);

    public static readonly StyledProperty<double> IndeterminateIndicatorWidthProperty =
        AvaloniaProperty.Register<CodexProgress, double>(nameof(IndeterminateIndicatorWidth), 72);

    public static readonly StyledProperty<Thickness> IndeterminateIndicatorMarginProperty =
        AvaloniaProperty.Register<CodexProgress, Thickness>(nameof(IndeterminateIndicatorMargin));

    static CodexProgress()
    {
        VariantProperty.Changed.AddClassHandler<CodexProgress>((progress, _) => progress.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexProgress>((progress, _) => progress.SyncClasses());
        IsIndeterminateProperty.Changed.AddClassHandler<CodexProgress>((progress, _) =>
        {
            progress.SyncClasses();
            progress.SyncIndeterminateAnimation();
        });
        IndeterminateAnimationDurationProperty.Changed.AddClassHandler<CodexProgress>((progress, _) => progress.SyncIndeterminateAnimation());
    }

    public CodexProgress()
    {
        SyncClasses();
        SyncIndeterminateFrame();
    }

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public TimeSpan IndeterminateAnimationDuration
    {
        get => GetValue(IndeterminateAnimationDurationProperty);
        set => SetValue(IndeterminateAnimationDurationProperty, value);
    }

    public double IndeterminateIndicatorWidth
    {
        get => GetValue(IndeterminateIndicatorWidthProperty);
        set => SetValue(IndeterminateIndicatorWidthProperty, value);
    }

    public Thickness IndeterminateIndicatorMargin
    {
        get => GetValue(IndeterminateIndicatorMarginProperty);
        set => SetValue(IndeterminateIndicatorMarginProperty, value);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        SyncIndeterminateFrame();
        return size;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;
        SyncIndeterminateAnimation();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        StopIndeterminateAnimation();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsEnabledProperty)
        {
            SyncIndeterminateAnimation();
        }
    }

    private void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("indeterminate", IsIndeterminate);
    }

    private void SyncIndeterminateAnimation()
    {
        if (!_isAttached)
        {
            SyncIndeterminateFrame();
            return;
        }

        if (IsIndeterminate && IsEnabled && IndeterminateAnimationDuration > TimeSpan.Zero)
        {
            StartIndeterminateAnimation();
        }
        else
        {
            StopIndeterminateAnimation();
            _indeterminateProgress = 0;
            SyncIndeterminateFrame();
        }
    }

    private void StartIndeterminateAnimation()
    {
        if (_indeterminateTimer is null)
        {
            _indeterminateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _indeterminateTimer.Tick += OnIndeterminateAnimationTick;
        }

        if (!_indeterminateStopwatch.IsRunning)
        {
            _indeterminateStopwatch.Start();
        }

        _indeterminateTimer.Start();
        SyncIndeterminateFrame();
    }

    private void StopIndeterminateAnimation()
    {
        _indeterminateTimer?.Stop();
        _indeterminateStopwatch.Reset();
    }

    private void OnIndeterminateAnimationTick(object? sender, EventArgs e)
    {
        var duration = IndeterminateAnimationDuration.TotalMilliseconds;
        _indeterminateProgress = duration <= 0
            ? 0
            : (_indeterminateStopwatch.Elapsed.TotalMilliseconds % duration) / duration;

        SyncIndeterminateFrame();
    }

    private void SyncIndeterminateFrame()
    {
        var trackWidth = Math.Max(0, Bounds.Width);
        var indicatorWidth = trackWidth <= 0
            ? 72
            : Math.Clamp(trackWidth * 0.36, 56, 140);
        var eased = EaseInOutCubic(_indeterminateProgress);
        var left = trackWidth <= 0
            ? 0
            : -indicatorWidth + ((trackWidth + indicatorWidth) * eased);

        IndeterminateIndicatorWidth = indicatorWidth;
        IndeterminateIndicatorMargin = new Thickness(left, 0, 0, 0);
    }

    private static double EaseInOutCubic(double progress)
    {
        var p = Math.Clamp(progress, 0, 1);
        return p < 0.5
            ? 4 * p * p * p
            : 1 - Math.Pow(-2 * p + 2, 3) / 2;
    }
}
