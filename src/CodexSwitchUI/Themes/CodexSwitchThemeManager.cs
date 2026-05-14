using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using CodexSwitchUI.Tokens;

namespace CodexSwitchUI.Themes;

/// <summary>
/// Applies CodexSwitchUI semantic tokens to an Avalonia application at startup or runtime.
/// </summary>
public sealed class CodexSwitchThemeManager
{
    public static CodexSwitchThemeManager Current { get; } = new();

    public event EventHandler? ThemeChanged;

    public CodexSwitchThemeMode Mode { get; private set; } = CodexSwitchThemeMode.Light;

    public CodexSwitchThemeOptions Options { get; private set; } = CodexSwitchThemeOptions.ShadcnDefault;

    public void ApplyToCurrent(CodexSwitchThemeMode mode, CodexSwitchThemeOptions? options = null)
    {
        if (Application.Current is null)
        {
            throw new InvalidOperationException("No Avalonia Application.Current is available.");
        }

        Apply(Application.Current, mode, options);
    }

    public void Apply(Application application, CodexSwitchThemeMode mode, CodexSwitchThemeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(application);

        Mode = mode;
        Options = options ?? Options;

        application.RequestedThemeVariant = mode switch
        {
            CodexSwitchThemeMode.Dark => ThemeVariant.Dark,
            CodexSwitchThemeMode.Light => ThemeVariant.Light,
            CodexSwitchThemeMode.Custom => ThemeVariant.Light,
            _ => ThemeVariant.Default
        };

        ApplyResources(application, Options.ResolvePalette(ResolveResourceMode(application, mode)), Options);
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    private static CodexSwitchThemeMode ResolveResourceMode(Application application, CodexSwitchThemeMode mode)
    {
        if (mode != CodexSwitchThemeMode.System)
        {
            return mode;
        }

        return application.ActualThemeVariant == ThemeVariant.Dark
            ? CodexSwitchThemeMode.Dark
            : CodexSwitchThemeMode.Light;
    }

    public static void ApplyResources(Application application, CodexSwitchPalette palette, CodexSwitchThemeOptions options)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(palette);
        ArgumentNullException.ThrowIfNull(options);

        SetBrush(application, CodexSwitchResourceKeys.BackgroundBrush, palette.Background);
        SetBrush(application, CodexSwitchResourceKeys.ForegroundBrush, palette.Foreground);
        SetBrush(application, CodexSwitchResourceKeys.CardBrush, palette.Card);
        SetBrush(application, CodexSwitchResourceKeys.CardForegroundBrush, palette.CardForeground);
        SetBrush(application, CodexSwitchResourceKeys.PopoverBrush, palette.Popover);
        SetBrush(application, CodexSwitchResourceKeys.PopoverForegroundBrush, palette.PopoverForeground);
        SetBrush(application, CodexSwitchResourceKeys.PrimaryBrush, palette.Primary);
        SetBrush(application, CodexSwitchResourceKeys.PrimaryForegroundBrush, palette.PrimaryForeground);
        SetBrush(application, CodexSwitchResourceKeys.SecondaryBrush, palette.Secondary);
        SetBrush(application, CodexSwitchResourceKeys.SecondaryForegroundBrush, palette.SecondaryForeground);
        SetBrush(application, CodexSwitchResourceKeys.MutedBrush, palette.Muted);
        SetBrush(application, CodexSwitchResourceKeys.MutedForegroundBrush, palette.MutedForeground);
        SetBrush(application, CodexSwitchResourceKeys.AccentBrush, palette.Accent);
        SetBrush(application, CodexSwitchResourceKeys.AccentForegroundBrush, palette.AccentForeground);
        SetBrush(application, CodexSwitchResourceKeys.DestructiveBrush, palette.Destructive);
        SetBrush(application, CodexSwitchResourceKeys.DestructiveForegroundBrush, palette.DestructiveForeground);
        SetBrush(application, CodexSwitchResourceKeys.BorderBrush, palette.Border);
        SetBrush(application, CodexSwitchResourceKeys.InputBrush, palette.Input);
        SetBrush(application, CodexSwitchResourceKeys.RingBrush, palette.Ring);
        SetBrush(application, CodexSwitchResourceKeys.SuccessBrush, palette.Success);
        SetBrush(application, CodexSwitchResourceKeys.SuccessForegroundBrush, palette.SuccessForeground);
        SetBrush(application, CodexSwitchResourceKeys.WarningBrush, palette.Warning);
        SetBrush(application, CodexSwitchResourceKeys.WarningForegroundBrush, palette.WarningForeground);

        application.Resources[CodexSwitchResourceKeys.FontFamily] = new FontFamily(options.FontFamily);
        application.Resources[CodexSwitchResourceKeys.FontSizeSm] = 12d;
        application.Resources[CodexSwitchResourceKeys.FontSizeMd] = 14d;
        application.Resources[CodexSwitchResourceKeys.FontSizeLg] = 16d;
        application.Resources[CodexSwitchResourceKeys.RadiusSm] = new CornerRadius(Math.Max(2, options.Radius - 2));
        application.Resources[CodexSwitchResourceKeys.RadiusMd] = new CornerRadius(options.Radius);
        application.Resources[CodexSwitchResourceKeys.RadiusLg] = new CornerRadius(options.Radius + 2);
        application.Resources[CodexSwitchResourceKeys.FocusThickness] = new Thickness(2);
        application.Resources[CodexSwitchResourceKeys.RingOffset] = options.RingOffset;

        var motionDurationFast = options.ReducedMotion ? TimeSpan.Zero : options.MotionDurationFast;
        var motionDurationDefault = options.ReducedMotion ? TimeSpan.Zero : options.MotionDurationDefault;
        var motionDurationSlow = options.ReducedMotion ? TimeSpan.Zero : options.MotionDurationSlow;
        var enterOffsetScale = options.ReducedMotion ? 0d : 1d;
        var skeletonShimmerOpacity = options.ReducedMotion ? 0d : options.SkeletonShimmerOpacity;

        application.Resources[CodexSwitchResourceKeys.MotionDurationFast] = motionDurationFast;
        application.Resources[CodexSwitchResourceKeys.MotionDurationDefault] = motionDurationDefault;
        application.Resources[CodexSwitchResourceKeys.MotionDurationSlow] = motionDurationSlow;
        application.Resources[CodexSwitchResourceKeys.MotionEaseOut] = options.MotionEaseOut;
        application.Resources[CodexSwitchResourceKeys.MotionEaseInOut] = options.MotionEaseInOut;
        application.Resources[CodexSwitchResourceKeys.DisabledOpacity] = options.DisabledOpacity;
        application.Resources[CodexSwitchResourceKeys.OverlayOpacity] = options.OverlayOpacity;
        application.Resources[CodexSwitchResourceKeys.PopoverEnterOffset] = options.PopoverEnterOffset * enterOffsetScale;
        application.Resources[CodexSwitchResourceKeys.DialogEnterOffset] = options.DialogEnterOffset * enterOffsetScale;
        application.Resources[CodexSwitchResourceKeys.ToastEnterOffset] = options.ToastEnterOffset * enterOffsetScale;
        application.Resources[CodexSwitchResourceKeys.SkeletonShimmerDuration] = options.ReducedMotion ? TimeSpan.Zero : options.SkeletonShimmerDuration;
        application.Resources[CodexSwitchResourceKeys.SkeletonShimmerOpacity] = skeletonShimmerOpacity;
        application.Resources[CodexSwitchResourceKeys.ReducedMotion] = options.ReducedMotion;
        SetBrush(application, CodexSwitchResourceKeys.SkeletonShimmerBrush, palette.Foreground, skeletonShimmerOpacity);

        var (sm, md, lg, padSm, padMd, padLg) = options.Density switch
        {
            CodexSwitchDensity.Compact => (30d, 36d, 42d, new Thickness(10, 5), new Thickness(12, 7), new Thickness(14, 9)),
            CodexSwitchDensity.Comfortable => (36d, 44d, 52d, new Thickness(14, 8), new Thickness(16, 10), new Thickness(20, 12)),
            _ => (32d, 40d, 48d, new Thickness(12, 6), new Thickness(14, 8), new Thickness(16, 10))
        };

        application.Resources[CodexSwitchResourceKeys.ControlHeightSm] = sm;
        application.Resources[CodexSwitchResourceKeys.ControlHeightMd] = md;
        application.Resources[CodexSwitchResourceKeys.ControlHeightLg] = lg;
        application.Resources[CodexSwitchResourceKeys.ControlPaddingSm] = padSm;
        application.Resources[CodexSwitchResourceKeys.ControlPaddingMd] = padMd;
        application.Resources[CodexSwitchResourceKeys.ControlPaddingLg] = padLg;
    }

    private static void SetBrush(Application application, string key, string color)
    {
        application.Resources[key] = new SolidColorBrush(Color.Parse(color));
    }

    private static void SetBrush(Application application, string key, string color, double opacity)
    {
        var parsed = Color.Parse(color);
        var alpha = (byte)Math.Clamp(Math.Round(opacity * byte.MaxValue), byte.MinValue, byte.MaxValue);
        application.Resources[key] = new SolidColorBrush(Color.FromArgb(alpha, parsed.R, parsed.G, parsed.B));
    }
}
