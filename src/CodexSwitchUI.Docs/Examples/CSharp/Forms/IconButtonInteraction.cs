using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class IconButtonInteractionSample
{
    public static Control BuildIconButtonInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Icon-only actions keep fixed dimensions across hover, press, and loading."
        };
        var pin = new CodexIconButton
        {
            Content = "p",
            IsRound = true,
            Variant = CodexControlVariant.Secondary
        };
        pin.Click += (_, _) =>
        {
            pin.Variant = pin.Variant == CodexControlVariant.Secondary
                ? CodexControlVariant.Default
                : CodexControlVariant.Secondary;
            status.Text = pin.Variant == CodexControlVariant.Default ? "Pinned." : "Unpinned.";
        };

        var refresh = new CodexIconButton
        {
            Content = ">",
            LoadingContent = "..."
        };
        refresh.Click += (_, _) =>
        {
            refresh.IsLoading = !refresh.IsLoading;
            status.Text = refresh.IsLoading ? "Refresh is loading." : "Refresh is ready.";
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
                    Spacing = 8,
                    Children =
                    {
                        pin,
                        refresh,
                        new CodexIconButton { Content = "...", Variant = CodexControlVariant.Ghost },
                        new CodexIconButton { Content = "x", Variant = CodexControlVariant.Destructive }
                    }
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        new CodexIconButton { Content = "+", IsRound = true },
                        new CodexIconButton { Content = "i", IsRound = true, Variant = CodexControlVariant.Secondary },
                        new CodexIconButton { Content = "?", IsRound = true, Variant = CodexControlVariant.Ghost }
                    }
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        new CodexIconButton { Content = ">", IsLoading = true, LoadingContent = "..." },
                        new CodexIconButton { Content = "i", IsEnabled = false },
                        new CodexIconButton { Content = "x", Variant = CodexControlVariant.Destructive, IsEnabled = false }
                    }
                }
            }
        };
    }
}
