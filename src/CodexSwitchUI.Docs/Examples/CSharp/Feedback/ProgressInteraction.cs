using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;

public static class ProgressInteractionSample
{
    public static Control BuildProgressInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Value changes animate the determinate indicator width."
        };
        var progress = new CodexProgress
        {
            Minimum = 0,
            Maximum = 100,
            Value = 36,
            ShowProgressText = true,
            MinWidth = 360
        };

        var step = new CodexButton
        {
            Content = "Step value",
            Size = CodexControlSize.Small
        };
        step.Click += (_, _) =>
        {
            progress.Value = progress.Value >= 84 ? 24 : progress.Value + 12;
            status.Text = $"Progress value changed to {progress.Value:0}%.";
        };

        var indeterminate = new CodexProgress
        {
            IsIndeterminate = true,
            Variant = CodexControlVariant.Success,
            MinWidth = 360
        };
        var reduceMotion = new CodexButton
        {
            Content = "Toggle motion",
            Variant = CodexControlVariant.Secondary,
            Size = CodexControlSize.Small
        };
        reduceMotion.Click += (_, _) =>
        {
            indeterminate.IndeterminateAnimationDuration =
                indeterminate.IndeterminateAnimationDuration == TimeSpan.Zero
                    ? TimeSpan.FromSeconds(1.4)
                    : TimeSpan.Zero;
            status.Text = indeterminate.IndeterminateAnimationDuration == TimeSpan.Zero
                ? "IndeterminateAnimationDuration=0 keeps the segment static."
                : "Indeterminate animation restored.";
        };

        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                status,
                progress,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { step, reduceMotion }
                },
                indeterminate,
                new CodexProgress
                {
                    IsIndeterminate = true,
                    IndeterminateAnimationDuration = TimeSpan.Zero,
                    Variant = CodexControlVariant.Warning,
                    Size = CodexControlSize.Large,
                    MinWidth = 360
                },
                new CodexProgress
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = 68,
                    IsEnabled = false,
                    MinWidth = 360
                }
            }
        };
    }
}
