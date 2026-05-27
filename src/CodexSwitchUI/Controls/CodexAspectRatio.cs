using System.Globalization;
using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

public enum CodexAspectRatioFitMode
{
    Width,
    Height,
    Contain
}

public sealed class CodexAspectRatioChangedEventArgs(double oldRatio, double newRatio, string ratioText)
    : EventArgs
{
    public double OldRatio { get; } = oldRatio;

    public double NewRatio { get; } = newRatio;

    public string RatioText { get; } = ratioText;
}

public class CodexAspectRatio : ContentControl
{
    private const double DefaultWidth = 320d;

    public static readonly StyledProperty<double> RatioProperty =
        AvaloniaProperty.Register<CodexAspectRatio, double>(nameof(Ratio), 16d / 9d);

    public static readonly StyledProperty<CodexAspectRatioFitMode> FitModeProperty =
        AvaloniaProperty.Register<CodexAspectRatio, CodexAspectRatioFitMode>(nameof(FitMode), CodexAspectRatioFitMode.Width);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexAspectRatio, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexAspectRatio, bool>(nameof(HasContent));

    public static readonly StyledProperty<string> RatioTextProperty =
        AvaloniaProperty.Register<CodexAspectRatio, string>(nameof(RatioText), "16:9");

    static CodexAspectRatio()
    {
        RatioProperty.Changed.AddClassHandler<CodexAspectRatio>((aspectRatio, args) => aspectRatio.OnRatioChanged(args));
        FitModeProperty.Changed.AddClassHandler<CodexAspectRatio>((aspectRatio, _) =>
        {
            aspectRatio.SyncClasses();
            aspectRatio.InvalidateMeasure();
        });
        SizeProperty.Changed.AddClassHandler<CodexAspectRatio>((aspectRatio, _) => aspectRatio.SyncClasses());
        ContentProperty.Changed.AddClassHandler<CodexAspectRatio>((aspectRatio, _) => aspectRatio.SyncClasses());
        AffectsMeasure<CodexAspectRatio>(RatioProperty, FitModeProperty, PaddingProperty, BorderThicknessProperty);
    }

    public CodexAspectRatio()
    {
        ClipToBounds = true;
        SyncClasses();
    }

    public event EventHandler<CodexAspectRatioChangedEventArgs>? RatioChanged;

    public double Ratio
    {
        get => GetValue(RatioProperty);
        set => SetValue(RatioProperty, NormalizeRatio(value));
    }

    public CodexAspectRatioFitMode FitMode
    {
        get => GetValue(FitModeProperty);
        set => SetValue(FitModeProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool HasContent => GetValue(HasContentProperty);

    public string RatioText => GetValue(RatioTextProperty);

    public static Size CalculateRatioSize(
        double ratio,
        CodexAspectRatioFitMode fitMode,
        Size availableSize,
        double explicitWidth = double.NaN,
        double explicitHeight = double.NaN)
    {
        ratio = NormalizeRatio(ratio);

        var width = PickSize(explicitWidth, availableSize.Width);
        var height = PickSize(explicitHeight, availableSize.Height);

        if (fitMode == CodexAspectRatioFitMode.Contain && width.HasValue && height.HasValue)
        {
            var candidateHeight = width.Value / ratio;
            if (candidateHeight <= height.Value)
            {
                return new Size(width.Value, candidateHeight);
            }

            return new Size(height.Value * ratio, height.Value);
        }

        if (fitMode == CodexAspectRatioFitMode.Height && height.HasValue)
        {
            return new Size(height.Value * ratio, height.Value);
        }

        if (width.HasValue)
        {
            return new Size(width.Value, width.Value / ratio);
        }

        if (height.HasValue)
        {
            return new Size(height.Value * ratio, height.Value);
        }

        return new Size(DefaultWidth, DefaultWidth / ratio);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var ratioSize = CalculateRatioSize(Ratio, FitMode, availableSize, Width, Height);
        base.MeasureOverride(ratioSize);
        return ratioSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var ratioSize = CalculateRatioSize(Ratio, FitMode, finalSize, Width, Height);
        base.ArrangeOverride(ratioSize);
        return ratioSize;
    }

    private void OnRatioChanged(AvaloniaPropertyChangedEventArgs args)
    {
        var normalized = NormalizeRatio(Ratio);
        if (!IsClose(Ratio, normalized))
        {
            SetCurrentValue(RatioProperty, normalized);
            return;
        }

        var oldRatio = args.OldValue is double oldValue ? NormalizeRatio(oldValue) : 16d / 9d;
        var ratioText = FormatRatio(normalized);
        SetValue(RatioTextProperty, ratioText);
        SyncClasses();
        InvalidateMeasure();
        RatioChanged?.Invoke(this, new CodexAspectRatioChangedEventArgs(oldRatio, normalized, ratioText));
    }

    private void SyncClasses()
    {
        var ratio = NormalizeRatio(Ratio);

        SetValue(HasContentProperty, HasValue(Content));
        SetValue(RatioTextProperty, FormatRatio(ratio));

        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("aspect-ratio", true);
        Classes.Set("has-content", HasContent);
        Classes.Set("empty", !HasContent);
        Classes.Set("ratio-square", IsClose(ratio, 1d));
        Classes.Set("ratio-video", IsClose(ratio, 16d / 9d));
        Classes.Set("ratio-portrait", ratio < 1d && !IsClose(ratio, 1d));
        Classes.Set("ratio-landscape", ratio > 1d && !IsClose(ratio, 16d / 9d));
        Classes.Set("fit-width", FitMode == CodexAspectRatioFitMode.Width);
        Classes.Set("fit-height", FitMode == CodexAspectRatioFitMode.Height);
        Classes.Set("fit-contain", FitMode == CodexAspectRatioFitMode.Contain);
    }

    private static double? PickSize(double explicitSize, double availableSize)
    {
        var explicitValue = CoerceSize(explicitSize);
        var availableValue = CoerceSize(availableSize);

        return explicitValue.HasValue && availableValue.HasValue
            ? Math.Min(explicitValue.Value, availableValue.Value)
            : explicitValue ?? availableValue;
    }

    private static double? CoerceSize(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return null;
        }

        return Math.Max(0d, value);
    }

    private static double NormalizeRatio(double ratio)
    {
        return double.IsNaN(ratio) || double.IsInfinity(ratio) || ratio <= 0d
            ? 1d
            : ratio;
    }

    private static string FormatRatio(double ratio)
    {
        if (IsClose(ratio, 16d / 9d))
        {
            return "16:9";
        }

        if (IsClose(ratio, 9d / 16d))
        {
            return "9:16";
        }

        if (IsClose(ratio, 4d / 3d))
        {
            return "4:3";
        }

        if (IsClose(ratio, 1d))
        {
            return "1:1";
        }

        return ratio.ToString("0.###", CultureInfo.InvariantCulture);
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }

    private static bool IsClose(double left, double right)
    {
        return Math.Abs(left - right) < 0.001d;
    }
}
