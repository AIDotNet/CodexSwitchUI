using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using CodexSwitchUI.Themes;

namespace CodexSwitchUI.Tokens;

/// <summary>
/// Resolves theme-backed motion resources for runtime animations that cannot use AXAML DynamicResource bindings directly.
/// </summary>
public static class CodexMotion
{
    public static TimeSpan ResolveFastDuration(Control? target = null)
    {
        return ResolveDuration(target, CodexSwitchResourceKeys.MotionDurationFast, CodexSwitchThemeOptions.ShadcnDefault.MotionDurationFast);
    }

    public static TimeSpan ResolveDefaultDuration(Control? target = null)
    {
        return ResolveDuration(target, CodexSwitchResourceKeys.MotionDurationDefault, CodexSwitchThemeOptions.ShadcnDefault.MotionDurationDefault);
    }

    public static TimeSpan ResolveSlowDuration(Control? target = null)
    {
        return ResolveDuration(target, CodexSwitchResourceKeys.MotionDurationSlow, CodexSwitchThemeOptions.ShadcnDefault.MotionDurationSlow);
    }

    public static TimeSpan ResolveDuration(Control? target, string key, TimeSpan fallback)
    {
        if (target?.TryFindResource(key, null, out var value) == true && value is TimeSpan duration)
        {
            return duration;
        }

        if (Application.Current?.TryFindResource(key, null, out value) == true && value is TimeSpan appDuration)
        {
            return appDuration;
        }

        return fallback;
    }

    public static Easing ResolveEaseOut(Control? target = null)
    {
        if (target?.TryFindResource(CodexSwitchResourceKeys.MotionEaseOut, null, out var value) == true && value is Easing easing)
        {
            return easing;
        }

        if (Application.Current?.TryFindResource(CodexSwitchResourceKeys.MotionEaseOut, null, out value) == true && value is Easing appEasing)
        {
            return appEasing;
        }

        return CodexSwitchThemeOptions.ShadcnDefault.MotionEaseOut;
    }

    public static void ApplyOpacityTransition(Control target, TimeSpan duration, Easing easing)
    {
        target.Transitions =
        [
            new DoubleTransition
            {
                Property = Visual.OpacityProperty,
                Duration = duration,
                Easing = easing
            }
        ];
    }

    public static void ApplyTranslateYTransition(TranslateTransform transform, TimeSpan duration, Easing easing)
    {
        transform.Transitions =
        [
            new DoubleTransition
            {
                Property = TranslateTransform.YProperty,
                Duration = duration,
                Easing = easing
            }
        ];
    }
}
