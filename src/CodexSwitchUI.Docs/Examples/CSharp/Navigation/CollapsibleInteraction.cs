using Avalonia;
using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;

public static class CollapsibleInteractionSample
{
    public static Control BuildCollapsibleInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "OpenChanged reports programmatic, pointer, and keyboard disclosure source."
        };
        var collapsible = new CodexCollapsible
        {
            Header = "Measured disclosure",
            AnimationDuration = TimeSpan.FromMilliseconds(220),
            ContentPadding = new Thickness(0, 10, 0, 0),
            Content = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new CodexText { Role = CodexTextRole.Muted, Text = "Content height is measured before animating open." },
                    new CodexCheckBox { Content = "Enable regional fallback", IsChecked = true },
                    new CodexButton { Content = "Apply", Size = CodexControlSize.Small, HorizontalAlignment = HorizontalAlignment.Left }
                }
            }
        };
        collapsible.OpenChanged += (_, args) =>
        {
            status.Text = $"OpenChanged: {(args.IsOpen ? "open" : "closed")} (source={args.Source}).";
        };

        var toggle = new CodexButton
        {
            Content = "Toggle",
            Size = CodexControlSize.Small
        };
        toggle.Click += (_, _) => collapsible.Toggle();

        var reduceMotion = new CodexButton
        {
            Content = "Reduce motion",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        reduceMotion.Click += (_, _) =>
        {
            collapsible.AnimationDuration = collapsible.AnimationDuration == TimeSpan.Zero
                ? TimeSpan.FromMilliseconds(220)
                : TimeSpan.Zero;
            status.Text = collapsible.AnimationDuration == TimeSpan.Zero
                ? "AnimationDuration=0 jumps to measured height."
                : "Measured height animation restored.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                collapsible,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { toggle, reduceMotion }
                },
                new CodexCollapsible
                {
                    Header = "Closed",
                    IsOpen = false,
                    Content = new CodexText { Role = CodexTextRole.Muted, Text = "Closed state preserves content for measurement." }
                },
                new CodexCollapsible
                {
                    Header = "Disabled",
                    IsOpen = false,
                    IsEnabled = false,
                    Content = new CodexText { Role = CodexTextRole.Muted, Text = "Disabled trigger ignores pointer and keyboard toggle." }
                }
            }
        };
    }
}
