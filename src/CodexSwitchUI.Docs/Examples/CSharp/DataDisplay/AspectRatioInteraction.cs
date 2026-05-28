using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class AspectRatioInteractionSample
{
    public static Control BuildAspectRatioInteractionPreview()
    {
        var status = Muted("Change the ratio, switch fit mode, or toggle content.");
        var aspectRatio = new CodexAspectRatio
        {
            Ratio = 16d / 9d,
            Width = 360,
            Content = MediaContent("Interactive media", "16:9")
        };
        aspectRatio.RatioChanged += (_, args) =>
        {
            status.Text = $"RatioChanged: {args.RatioText} ({args.OldRatio:0.###} -> {args.NewRatio:0.###}).";
        };

        var video = Button("16:9");
        video.Click += (_, _) => aspectRatio.Ratio = 16d / 9d;

        var square = Button("1:1");
        square.Click += (_, _) => aspectRatio.Ratio = 1d;

        var portrait = Button("9:16");
        portrait.Click += (_, _) => aspectRatio.Ratio = 9d / 16d;

        var invalid = Button("Invalid", CodexControlVariant.Ghost);
        invalid.Click += (_, _) => aspectRatio.Ratio = -1d;

        var fit = Button("Fit: width");
        fit.Click += (_, _) =>
        {
            aspectRatio.FitMode = aspectRatio.FitMode switch
            {
                CodexAspectRatioFitMode.Width => CodexAspectRatioFitMode.Height,
                CodexAspectRatioFitMode.Height => CodexAspectRatioFitMode.Contain,
                _ => CodexAspectRatioFitMode.Width
            };
            fit.Content = $"Fit: {aspectRatio.FitMode.ToString().ToLowerInvariant()}";
            status.Text = $"FitMode switched to {aspectRatio.FitMode}.";
        };

        var toggleContent = Button("Hide content");
        toggleContent.Click += (_, _) =>
        {
            var shouldHide = aspectRatio.Content is not null;
            aspectRatio.Content = shouldHide ? null : MediaContent("Interactive media", "restored");
            toggleContent.Content = shouldHide ? "Restore content" : "Hide content";
            status.Text = shouldHide
                ? "Content removed; empty and RatioText placeholder state is visible."
                : "Content restored; has-content state is active again.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                aspectRatio,
                Row(video, square, portrait, invalid),
                Row(fit, toggleContent),
                new CodexAspectRatio
                {
                    Ratio = double.NaN,
                    Width = 230,
                    Size = CodexControlSize.Small
                }
            }
        };
    }

    private static Control MediaContent(string title, string badge)
    {
        return new StackPanel
        {
            Margin = new Avalonia.Thickness(18),
            VerticalAlignment = VerticalAlignment.Bottom,
            Spacing = 8,
            Children =
            {
                new CodexBadge { Content = badge, Variant = CodexControlVariant.Secondary, HorizontalAlignment = HorizontalAlignment.Left },
                Text(title, CodexTextRole.Subtitle),
                Muted("Content is clipped inside the measured viewport.")
            }
        };
    }

    private static CodexButton Button(string label, CodexControlVariant variant = CodexControlVariant.Secondary)
    {
        return new CodexButton
        {
            Content = label,
            Size = CodexControlSize.Small,
            Variant = variant
        };
    }

    private static StackPanel Row(params Control[] children)
    {
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        foreach (var child in children)
            row.Children.Add(child);
        return row;
    }

    private static CodexText Muted(string text) => Text(text, CodexTextRole.Muted);

    private static CodexText Text(string text, CodexTextRole role)
    {
        return new CodexText { Role = role, Text = text };
    }
}
