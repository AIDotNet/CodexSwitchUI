using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class ToggleInteractionSample
{
    public static Control BuildToggleInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Bookmark is on (source=Programmatic)."
        };
        var bookmark = new CodexToggle
        {
            Content = "Bookmark",
            IsPressed = true
        };
        bookmark.PressedChanged += (_, args) =>
        {
            status.Text = args.NewValue
                ? $"Bookmark is on (source={args.Source})."
                : $"Bookmark is off (source={args.Source}).";
        };

        var bold = new CodexToggle
        {
            Content = "Bold",
            Variant = CodexControlVariant.Outline
        };
        bold.PressedChanged += (_, args) =>
        {
            status.Text = args.NewValue
                ? $"Bold enabled (source={args.Source})."
                : $"Bold disabled (source={args.Source}).";
        };

        var clear = new CodexButton
        {
            Content = "Clear pressed",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        clear.Click += (_, _) => bookmark.IsPressed = false;

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        bookmark,
                        bold,
                        new CodexToggle { Content = "Disabled", IsEnabled = false }
                    }
                },
                status,
                clear,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        new CodexToggle { Content = "S", Size = CodexControlSize.Small },
                        new CodexToggle { Content = "Default", IsPressed = true },
                        new CodexToggle { Content = "Large", Size = CodexControlSize.Large, Variant = CodexControlVariant.Outline }
                    }
                }
            }
        };
    }
}
