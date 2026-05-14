using CodexSwitchUI.Tokens;
using Avalonia;
using Avalonia.Animation.Easings;

namespace CodexSwitchUI.Themes;

public enum CodexSwitchThemeMode
{
    Light,
    Dark,
    System,
    Custom
}

public enum CodexSwitchDensity
{
    Compact,
    Default,
    Comfortable
}

/// <summary>
/// Runtime theme settings exposed to XAML resources and Avalonia controls.
/// </summary>
public sealed record CodexSwitchThemeOptions
{
    public CodexSwitchPalette LightPalette { get; init; } = CodexSwitchPalette.Light;
    public CodexSwitchPalette DarkPalette { get; init; } = CodexSwitchPalette.Dark;
    public CodexSwitchPalette? CustomPalette { get; init; }
    public double Radius { get; init; } = 6;
    public string FontFamily { get; init; } = "Inter, Segoe UI, sans-serif";
    public CodexSwitchDensity Density { get; init; } = CodexSwitchDensity.Default;
    public TimeSpan MotionDurationFast { get; init; } = TimeSpan.FromMilliseconds(120);
    public TimeSpan MotionDurationDefault { get; init; } = TimeSpan.FromMilliseconds(150);
    public TimeSpan MotionDurationSlow { get; init; } = TimeSpan.FromMilliseconds(220);
    public Easing MotionEaseOut { get; init; } = new CubicEaseOut();
    public Easing MotionEaseInOut { get; init; } = new CubicEaseInOut();
    public double DisabledOpacity { get; init; } = 0.5;
    public Thickness RingOffset { get; init; } = new(2);
    public double OverlayOpacity { get; init; } = 0.8;
    public double PopoverEnterOffset { get; init; } = 4;
    public double DialogEnterOffset { get; init; } = 8;
    public double ToastEnterOffset { get; init; } = 8;
    public TimeSpan SkeletonShimmerDuration { get; init; } = TimeSpan.FromSeconds(2);
    public double SkeletonShimmerOpacity { get; init; } = 0.14;
    public bool ReducedMotion { get; init; }

    public static CodexSwitchThemeOptions ShadcnDefault { get; } = new();

    public CodexSwitchPalette ResolvePalette(CodexSwitchThemeMode mode)
    {
        return mode switch
        {
            CodexSwitchThemeMode.Dark => DarkPalette,
            CodexSwitchThemeMode.Custom when CustomPalette is not null => CustomPalette,
            _ => LightPalette
        };
    }
}
