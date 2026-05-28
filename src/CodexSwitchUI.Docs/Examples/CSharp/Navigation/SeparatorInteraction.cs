using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class SeparatorInteractionSample
{
    public static Control BuildSeparatorInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Separator is horizontal and medium."
        };
        var separator = new CodexSeparator { Size = CodexControlSize.Medium };

        var toggleOrientation = new CodexButton
        {
            Content = "Toggle orientation",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        toggleOrientation.Click += (_, _) =>
        {
            separator.Orientation = separator.Orientation == Orientation.Horizontal
                ? Orientation.Vertical
                : Orientation.Horizontal;
            separator.Width = separator.Orientation == Orientation.Vertical ? 1 : double.NaN;
            separator.Height = separator.Orientation == Orientation.Vertical ? 56 : double.NaN;
            status.Text = separator.Orientation == Orientation.Vertical
                ? "Separator switched to vertical with explicit height."
                : "Separator switched to horizontal and stretches across the row.";
        };

        var cycleSize = new CodexButton
        {
            Content = "Cycle size",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        cycleSize.Click += (_, _) =>
        {
            separator.Size = separator.Size == CodexControlSize.Small
                ? CodexControlSize.Large
                : separator.Size == CodexControlSize.Large
                    ? CodexControlSize.Medium
                    : CodexControlSize.Small;
            status.Text = $"Separator size changed to {separator.Size}.";
        };

        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                status,
                separator,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { toggleOrientation, cycleSize }
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 12,
                    Children =
                    {
                        new CodexButton { Content = "Undo", Size = CodexControlSize.Small },
                        new CodexSeparator { Orientation = Orientation.Vertical, Height = 36 },
                        new CodexButton { Content = "Redo", Size = CodexControlSize.Small, Variant = CodexControlVariant.Secondary },
                        new CodexSeparator { Orientation = Orientation.Vertical, Height = 36, Size = CodexControlSize.Large },
                        new CodexButton { Content = "Save", Size = CodexControlSize.Small }
                    }
                },
                new StackPanel
                {
                    Spacing = 10,
                    IsEnabled = false,
                    Children =
                    {
                        new CodexText { Role = CodexTextRole.Subtitle, Text = "Disabled host composition" },
                        new CodexSeparator { Size = CodexControlSize.Small },
                        new CodexText { Role = CodexTextRole.Muted, Text = "The divider remains visible while the command cluster is disabled." }
                    }
                }
            }
        };
    }
}
