using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;

public static class SpinnerInteractionSample
{
    public static Control BuildSpinnerInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Active spinners start their timer when attached to the visual tree."
        };
        var spinner = new CodexSpinner
        {
            Label = "Default loading"
        };

        var toggleActive = new CodexButton
        {
            Content = "Toggle active",
            Size = CodexControlSize.Small
        };
        toggleActive.Click += (_, _) =>
        {
            spinner.IsActive = !spinner.IsActive;
            status.Text = spinner.IsActive ? "Spinner animation resumed." : "Spinner paused and reports idle automation status.";
        };

        var reduceMotion = new CodexButton
        {
            Content = "Reduce motion",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        reduceMotion.Click += (_, _) =>
        {
            spinner.RotationDuration = spinner.RotationDuration == TimeSpan.Zero
                ? TimeSpan.FromSeconds(1)
                : TimeSpan.Zero;
            status.Text = spinner.RotationDuration == TimeSpan.Zero
                ? "RotationDuration=0 renders a static frame."
                : "RotationDuration restored to the default loop.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 14,
                    Children =
                    {
                        new CodexSpinner { Size = CodexControlSize.Small, Label = "Small loading" },
                        spinner,
                        new CodexSpinner { Size = CodexControlSize.Large, Label = "Large loading" }
                    }
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { toggleActive, reduceMotion }
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 14,
                    Children =
                    {
                        new CodexSpinner { IsActive = false, Label = "Paused loading" },
                        new CodexSpinner { RotationDuration = TimeSpan.Zero, Label = "Reduced motion" },
                        new CodexButton { Content = "Refreshing", IsLoading = true }
                    }
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        new CodexSpinner { Label = "Syncing" },
                        new CodexBadge { Content = "syncing", Variant = CodexControlVariant.Secondary }
                    }
                }
            }
        };
    }
}
