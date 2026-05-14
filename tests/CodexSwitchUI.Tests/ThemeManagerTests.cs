using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using CodexSwitchUI.Themes;
using CodexSwitchUI.Tokens;
using Xunit;

namespace CodexSwitchUI.Tests;

public class ThemeManagerTests
{
    [Fact]
    public void ApplyWritesLightThemeResources()
    {
        var app = new Application();

        CodexSwitchThemeManager.Current.Apply(app, CodexSwitchThemeMode.Light, CodexSwitchThemeOptions.ShadcnDefault);

        Assert.Equal(ThemeVariant.Light, app.RequestedThemeVariant);
        AssertBrush(app, CodexSwitchResourceKeys.BackgroundBrush, "#FFFFFFFF");
        AssertBrush(app, CodexSwitchResourceKeys.PrimaryBrush, "#FF18181B");
        Assert.Equal(TimeSpan.FromMilliseconds(120), AssertResource<TimeSpan>(app, CodexSwitchResourceKeys.MotionDurationFast));
        Assert.Equal(TimeSpan.FromMilliseconds(150), AssertResource<TimeSpan>(app, CodexSwitchResourceKeys.MotionDurationDefault));
        Assert.Equal(TimeSpan.FromMilliseconds(220), AssertResource<TimeSpan>(app, CodexSwitchResourceKeys.MotionDurationSlow));
        Assert.IsType<CubicEaseOut>(AssertAssignableResource<Easing>(app, CodexSwitchResourceKeys.MotionEaseOut));
        Assert.IsType<CubicEaseInOut>(AssertAssignableResource<Easing>(app, CodexSwitchResourceKeys.MotionEaseInOut));
        Assert.Equal(0.5, AssertResource<double>(app, CodexSwitchResourceKeys.DisabledOpacity));
        Assert.Equal(new Thickness(2), AssertResource<Thickness>(app, CodexSwitchResourceKeys.RingOffset));
        Assert.Equal(0.8, AssertResource<double>(app, CodexSwitchResourceKeys.OverlayOpacity));
        Assert.Equal(4d, AssertResource<double>(app, CodexSwitchResourceKeys.PopoverEnterOffset));
        Assert.Equal(8d, AssertResource<double>(app, CodexSwitchResourceKeys.DialogEnterOffset));
        Assert.Equal(8d, AssertResource<double>(app, CodexSwitchResourceKeys.ToastEnterOffset));
        Assert.Equal(TimeSpan.FromSeconds(2), AssertResource<TimeSpan>(app, CodexSwitchResourceKeys.SkeletonShimmerDuration));
        Assert.Equal(0.14, AssertResource<double>(app, CodexSwitchResourceKeys.SkeletonShimmerOpacity));
        Assert.False(AssertResource<bool>(app, CodexSwitchResourceKeys.ReducedMotion));
        AssertBrush(app, CodexSwitchResourceKeys.SkeletonShimmerBrush, "#2409090B");
    }

    [Fact]
    public void ApplyWritesCustomThemeResources()
    {
        var app = new Application();
        var options = CodexSwitchThemeOptions.ShadcnDefault with
        {
            Radius = 12,
            Density = CodexSwitchDensity.Compact,
            CustomPalette = CodexSwitchPalette.Light with
            {
                Primary = "#FF2563EB",
                PrimaryForeground = "#FFFFFFFF"
            }
        };

        CodexSwitchThemeManager.Current.Apply(app, CodexSwitchThemeMode.Custom, options);

        Assert.Equal(ThemeVariant.Light, app.RequestedThemeVariant);
        AssertBrush(app, CodexSwitchResourceKeys.PrimaryBrush, "#FF2563EB");
        Assert.Equal(new CornerRadius(12), app.Resources[CodexSwitchResourceKeys.RadiusMd]);
        Assert.Equal(36d, app.Resources[CodexSwitchResourceKeys.ControlHeightMd]);
    }

    [Fact]
    public void ApplyWritesReducedMotionResources()
    {
        var app = new Application();
        var options = CodexSwitchThemeOptions.ShadcnDefault with
        {
            ReducedMotion = true,
            PopoverEnterOffset = 12,
            DialogEnterOffset = 16,
            ToastEnterOffset = 20,
            SkeletonShimmerOpacity = 0.4
        };

        CodexSwitchThemeManager.Current.Apply(app, CodexSwitchThemeMode.Light, options);

        Assert.True(AssertResource<bool>(app, CodexSwitchResourceKeys.ReducedMotion));
        Assert.Equal(TimeSpan.Zero, AssertResource<TimeSpan>(app, CodexSwitchResourceKeys.MotionDurationFast));
        Assert.Equal(TimeSpan.Zero, AssertResource<TimeSpan>(app, CodexSwitchResourceKeys.MotionDurationDefault));
        Assert.Equal(TimeSpan.Zero, AssertResource<TimeSpan>(app, CodexSwitchResourceKeys.MotionDurationSlow));
        Assert.Equal(0d, AssertResource<double>(app, CodexSwitchResourceKeys.PopoverEnterOffset));
        Assert.Equal(0d, AssertResource<double>(app, CodexSwitchResourceKeys.DialogEnterOffset));
        Assert.Equal(0d, AssertResource<double>(app, CodexSwitchResourceKeys.ToastEnterOffset));
        Assert.Equal(TimeSpan.Zero, AssertResource<TimeSpan>(app, CodexSwitchResourceKeys.SkeletonShimmerDuration));
        Assert.Equal(0d, AssertResource<double>(app, CodexSwitchResourceKeys.SkeletonShimmerOpacity));
        AssertBrush(app, CodexSwitchResourceKeys.SkeletonShimmerBrush, "#0009090B");
    }

    private static void AssertBrush(Application app, string key, string expected)
    {
        var brush = AssertResource<SolidColorBrush>(app, key);
        Assert.Equal(Color.Parse(expected), brush.Color);
    }

    private static T AssertResource<T>(Application app, string key)
    {
        Assert.True(app.Resources.TryGetResource(key, null, out var value));
        return Assert.IsType<T>(value);
    }

    private static T AssertAssignableResource<T>(Application app, string key)
    {
        Assert.True(app.Resources.TryGetResource(key, null, out var value));
        return Assert.IsAssignableFrom<T>(value);
    }
}
