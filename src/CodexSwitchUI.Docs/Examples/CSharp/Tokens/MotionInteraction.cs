using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using CodexSwitchUI.Tokens;
using System;

public static class MotionInteractionSample
{
    public static Control BuildMotionInteractionPreview()
    {
        var status = Text("Default duration resolved from theme motion tokens.", CodexTextRole.Muted);
        var transform = new TranslateTransform();
        var motionSurface = new CodexCard
        {
            Width = 300,
            Title = "Animated surface",
            Description = "Opacity and translate transitions share Codex motion tokens.",
            RenderTransform = transform,
            Content = new CodexBadge
            {
                Content = "Motion ready",
                Variant = CodexControlVariant.Secondary
            }
        };
        CodexMotion.ApplyOpacityTransition(motionSurface, CodexMotion.ResolveDefaultDuration(motionSurface), CodexMotion.ResolveEaseOut(motionSurface));
        CodexMotion.ApplyTranslateYTransition(transform, CodexMotion.ResolveDefaultDuration(motionSurface), CodexMotion.ResolveEaseOut(motionSurface));

        var isDimmed = false;
        var isShifted = false;
        var isReducedMotion = false;

        var opacity = new CodexButton
        {
            Content = "Animate opacity",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        opacity.Click += (_, _) =>
        {
            isDimmed = !isDimmed;
            motionSurface.Opacity = isDimmed ? 0.58 : 1;
            status.Text = isDimmed
                ? "Opacity transitioned to the dimmed state."
                : "Opacity transitioned back to the resting state.";
        };

        var translate = new CodexButton
        {
            Content = "Translate Y",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        translate.Click += (_, _) =>
        {
            isShifted = !isShifted;
            transform.Y = isShifted ? 12 : 0;
            status.Text = isShifted
                ? "Translate Y moved through the runtime transition helper."
                : "Translate Y returned to zero using the same easing.";
        };

        var reduce = new CodexButton
        {
            Content = "Reduce motion",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        reduce.Click += (_, _) =>
        {
            isReducedMotion = !isReducedMotion;
            var duration = isReducedMotion ? TimeSpan.Zero : CodexMotion.ResolveDefaultDuration(motionSurface);
            CodexMotion.ApplyOpacityTransition(motionSurface, duration, CodexMotion.ResolveEaseOut(motionSurface));
            CodexMotion.ApplyTranslateYTransition(transform, duration, CodexMotion.ResolveEaseOut(motionSurface));
            reduce.Content = isReducedMotion ? "Restore motion" : "Reduce motion";
            status.Text = isReducedMotion
                ? "Motion reduced to zero-duration handoff."
                : "Motion restored to the default theme duration.";
        };

        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 16,
            RowSpacing = 14,
            Children =
            {
                new CodexField
                {
                    Label = "Runtime transition",
                    Description = "Runtime surfaces resolve duration and easing tokens before applying transitions.",
                    Content = new StackPanel
                    {
                        Spacing = 10,
                        Children =
                        {
                            status,
                            motionSurface,
                            new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                Spacing = 8,
                                Children = { opacity, translate, reduce }
                            }
                        }
                    }
                },
                GridCell(new CodexField
                {
                    Label = "Duration tokens",
                    Description = "Fast, default, and slow durations map to Web-style interaction tiers.",
                    Content = new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Star)
                        },
                        ColumnSpacing = 12,
                        Children =
                        {
                            MotionToken("Fast", $"{CodexMotion.ResolveFastDuration().TotalMilliseconds:0}ms", "focus/hover", 0),
                            MotionToken("Default", $"{CodexMotion.ResolveDefaultDuration().TotalMilliseconds:0}ms", "surface", 1),
                            MotionToken("Slow", $"{CodexMotion.ResolveSlowDuration().TotalMilliseconds:0}ms", "overlay", 2)
                        }
                    }
                }, row: 0, column: 1),
                GridCell(new CodexField
                {
                    Label = "Reduced handoff",
                    Description = "Zero duration keeps final state deterministic for reduced-motion flows.",
                    Content = new StackPanel
                    {
                        Spacing = 8,
                        Children =
                        {
                            new CodexKbd { Content = "0ms", Size = CodexControlSize.Small },
                            Text("Reduced motion", CodexTextRole.Subtitle),
                            Text("State changes complete immediately while controls keep the same event contract.", CodexTextRole.Muted)
                        }
                    }
                }, row: 1, column: 0)
            }
        };
    }

    private static Control MotionToken(string label, string value, string use, int column)
    {
        var token = new CodexCard
        {
            Title = label,
            Description = use,
            Content = Text(value, CodexTextRole.Code)
        };
        Grid.SetColumn(token, column);
        return token;
    }

    private static CodexText Text(string value, CodexTextRole role)
    {
        return new CodexText
        {
            Role = role,
            Text = value
        };
    }

    private static Control GridCell(Control control, int row, int column)
    {
        Grid.SetRow(control, row);
        Grid.SetColumn(control, column);
        return control;
    }
}
