using System.Diagnostics;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace CodexSwitchUI.Controls;

public class CodexSpinner : TemplatedControl
{
    private static readonly (Point Start, Point End)[] Spokes =
    [
        (new Point(12, 2), new Point(12, 6)),
        (new Point(16.2, 7.8), new Point(19.1, 4.9)),
        (new Point(18, 12), new Point(22, 12)),
        (new Point(16.2, 16.2), new Point(19.1, 19.1)),
        (new Point(12, 18), new Point(12, 22)),
        (new Point(4.9, 19.1), new Point(7.8, 16.2)),
        (new Point(2, 12), new Point(6, 12)),
        (new Point(4.9, 4.9), new Point(7.8, 7.8))
    ];

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexSpinner, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<CodexSpinner, bool>(nameof(IsActive), true);

    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<CodexSpinner, string>(nameof(Label), "Loading");

    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<CodexSpinner, double>(nameof(StrokeThickness), 2);

    public static readonly StyledProperty<TimeSpan> RotationDurationProperty =
        AvaloniaProperty.Register<CodexSpinner, TimeSpan>(nameof(RotationDuration), TimeSpan.FromSeconds(1));

    private readonly Stopwatch _stopwatch = new();
    private DispatcherTimer? _animationTimer;
    private bool _isAttached;
    private double _angle;

    static CodexSpinner()
    {
        SizeProperty.Changed.AddClassHandler<CodexSpinner>((spinner, _) =>
        {
            spinner.SyncClasses();
            spinner.InvalidateMeasure();
        });
        IsActiveProperty.Changed.AddClassHandler<CodexSpinner>((spinner, _) =>
        {
            spinner.SyncClasses();
            spinner.SyncAutomation();
            spinner.SyncAnimation();
        });
        LabelProperty.Changed.AddClassHandler<CodexSpinner>((spinner, _) => spinner.SyncAutomation());
        StrokeThicknessProperty.Changed.AddClassHandler<CodexSpinner>((spinner, _) => spinner.InvalidateVisual());
        RotationDurationProperty.Changed.AddClassHandler<CodexSpinner>((spinner, _) => spinner.SyncAnimation());
        ForegroundProperty.Changed.AddClassHandler<CodexSpinner>((spinner, _) => spinner.InvalidateVisual());
    }

    public CodexSpinner()
    {
        Focusable = false;
        IsHitTestVisible = false;
        SyncClasses();
        SyncAutomation();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public TimeSpan RotationDuration
    {
        get => GetValue(RotationDurationProperty);
        set => SetValue(RotationDurationProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Foreground is null || Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            return;
        }

        var side = Math.Min(Bounds.Width, Bounds.Height);
        var scale = side / 24d;
        var offset = new Point((Bounds.Width - side) / 2d, (Bounds.Height - side) / 2d);
        var stroke = Math.Max(1, Math.Min(StrokeThickness, side / 3d));
        var pen = new Pen(Foreground, stroke, null, PenLineCap.Round, PenLineJoin.Round);
        var radians = _angle * Math.PI / 180d;

        foreach (var spoke in Spokes)
        {
            context.DrawLine(
                pen,
                TransformPoint(spoke.Start, scale, offset, radians),
                TransformPoint(spoke.End, scale, offset, radians));
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;
        SyncAnimation();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        StopAnimation();
        base.OnDetachedFromVisualTree(e);
    }

    private void SyncAnimation()
    {
        if (!_isAttached)
        {
            return;
        }

        if (IsActive && RotationDuration > TimeSpan.Zero)
        {
            StartAnimation();
        }
        else
        {
            StopAnimation();
            _angle = 0;
            InvalidateVisual();
        }
    }

    private void StartAnimation()
    {
        if (_animationTimer is null)
        {
            _animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _animationTimer.Tick += OnAnimationTick;
        }

        if (!_stopwatch.IsRunning)
        {
            _stopwatch.Start();
        }

        _animationTimer.Start();
    }

    private void StopAnimation()
    {
        _animationTimer?.Stop();
        _stopwatch.Reset();
    }

    private void OnAnimationTick(object? sender, EventArgs e)
    {
        var duration = RotationDuration.TotalMilliseconds;
        _angle = duration <= 0
            ? 0
            : (_stopwatch.Elapsed.TotalMilliseconds % duration) / duration * 360d;

        InvalidateVisual();
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("active", IsActive);
        Classes.Set("paused", !IsActive);
    }

    private void SyncAutomation()
    {
        AutomationProperties.SetName(this, Label);
        AutomationProperties.SetItemStatus(this, IsActive ? "loading" : "idle");
        AutomationProperties.SetIsControlElementOverride(this, true);
        AutomationProperties.SetLiveSetting(this, AutomationLiveSetting.Polite);
    }

    private static Point TransformPoint(Point point, double scale, Point offset, double radians)
    {
        var x = point.X - 12d;
        var y = point.Y - 12d;
        var cos = Math.Cos(radians);
        var sin = Math.Sin(radians);

        return new Point(
            offset.X + ((x * cos - y * sin) + 12d) * scale,
            offset.Y + ((x * sin + y * cos) + 12d) * scale);
    }
}
