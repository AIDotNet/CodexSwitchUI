using Avalonia;
using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;

public static class SkeletonInteractionSample
{
    public static Control BuildSkeletonInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Animated skeletons pulse and shimmer without taking focus or hit testing."
        };
        var headline = new CodexSkeleton
        {
            Width = 280,
            Height = 18,
            PulseLowOpacity = 0.48,
            PulseHighOpacity = 1,
            ShimmerHighOpacity = 0.18
        };

        var toggleAnimation = new CodexButton
        {
            Content = "Toggle animation",
            Size = CodexControlSize.Small
        };
        toggleAnimation.Click += (_, _) =>
        {
            headline.IsAnimated = !headline.IsAnimated;
            status.Text = headline.IsAnimated
                ? "Pulse animation restored."
                : "Static reduced-motion fallback applied.";
        };

        var reduceDuration = new CodexButton
        {
            Content = "Zero duration",
            Variant = CodexControlVariant.Secondary,
            Size = CodexControlSize.Small
        };
        reduceDuration.Click += (_, _) =>
        {
            headline.PulseDuration = headline.PulseDuration == TimeSpan.Zero
                ? TimeSpan.FromSeconds(1.6)
                : TimeSpan.Zero;
            status.Text = headline.PulseDuration == TimeSpan.Zero
                ? "PulseDuration=0 renders the static frame."
                : "PulseDuration restored.";
        };

        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                status,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 12,
                    Children =
                    {
                        new CodexSkeleton { Width = 64, Height = 64, CornerRadius = new CornerRadius(32), ShimmerHighOpacity = 0.22 },
                        new StackPanel
                        {
                            Spacing = 8,
                            Children =
                            {
                                headline,
                                new CodexSkeleton { Width = 360, Height = 18, PulseLowOpacity = 0.48, PulseHighOpacity = 1 },
                                new CodexSkeleton { Width = 220, Height = 18, PulseLowOpacity = 0.48, PulseHighOpacity = 1 }
                            }
                        }
                    }
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { toggleAnimation, reduceDuration }
                },
                new CodexSkeleton { Width = 360, Height = 110, CornerRadius = new CornerRadius(8), IsAnimated = false },
                new CodexSkeleton { Width = 180, Height = 18, IsAnimated = false },
                new CodexText
                {
                    Role = CodexTextRole.Muted,
                    Text = "Static skeletons preserve layout while honoring reduced motion."
                }
            }
        };
    }
}
