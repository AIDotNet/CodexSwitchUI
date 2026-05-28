using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class TooltipInteractionSample
{
    public static Control BuildTooltipInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "OpenChanged: tooltip starts closed."
        };
        var tooltip = new CodexTooltip
        {
            Trigger = new CodexButton
            {
                Content = "Billing target",
                Size = CodexControlSize.Small,
                HorizontalAlignment = HorizontalAlignment.Left
            },
            Content = "Usage refreshes every minute.",
            Placement = PlacementMode.Bottom,
            IsArrowVisible = true,
            Size = CodexControlSize.Small,
            OpenDelay = TimeSpan.Zero
        };
        tooltip.OpenChanged += (_, args) =>
        {
            status.Text = args.IsOpen
                ? $"OpenChanged: tooltip opened by {args.Source}."
                : $"OpenChanged: tooltip closed by {args.Source}.";
        };

        var openNow = new CodexButton
        {
            Content = "Open now",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        openNow.Click += (_, _) => tooltip.Open();

        var dismiss = new CodexButton
        {
            Content = "Dismiss",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Outline
        };
        dismiss.Click += (_, _) => tooltip.Dismiss();

        return new CodexTooltipProvider
        {
            DelayDuration = TimeSpan.FromMilliseconds(700),
            Content = new StackPanel
            {
                Spacing = 10,
                Children =
                {
                    tooltip,
                    status,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 8,
                        Children =
                        {
                            openNow,
                            dismiss
                        }
                    },
                    new CodexTooltip
                    {
                        Content = "Escape dismissal is disabled for persistent hints.",
                        Placement = PlacementMode.Right,
                        CloseOnEscape = false,
                        IsOpen = true,
                        IsArrowVisible = true
                    },
                    new CodexTooltip
                    {
                        Trigger = new CodexButton { Content = "Save", Variant = CodexControlVariant.Secondary },
                        Content = "Large top-aligned tooltip with arrow.",
                        Placement = PlacementMode.Top,
                        IsOpen = true,
                        IsArrowVisible = true,
                        Size = CodexControlSize.Large
                    },
                    new CodexTooltip
                    {
                        Content = "Closed hint keeps side and arrow state.",
                        Placement = PlacementMode.Left,
                        IsOpen = false,
                        IsArrowVisible = true
                    }
                }
            }
        };
    }
}
