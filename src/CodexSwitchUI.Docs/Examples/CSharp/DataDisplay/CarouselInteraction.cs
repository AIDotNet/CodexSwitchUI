using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class CarouselInteractionSample
{
    public static Control BuildCarouselInteractionPreview()
    {
        var status = Muted("SelectionChanged updates this status when commands move the slide.");
        var carousel = BuildCarousel(
            selectedIndex: 0,
            slides:
            [
                CarouselSlide("Start", "at-start / previous-disabled", "1", CodexControlVariant.Secondary),
                CarouselSlide("Middle", "Arrow keys move here", "2", CodexControlVariant.Success),
                CarouselSlide("End", "at-end / next-disabled", "3", CodexControlVariant.Warning)
            ]);
        carousel.SelectionChanged += (_, args) =>
        {
            status.Text = $"SelectionChanged: {args.Source} moved {args.OldIndex + 1} -> {args.NewIndex + 1}.";
        };

        var previous = new CodexButton
        {
            Content = "Previous",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary,
            Command = carousel.PreviousCommand
        };
        var next = new CodexButton
        {
            Content = "Next",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary,
            Command = carousel.NextCommand
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                carousel,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { previous, next }
                },
                BuildCarousel(selectedIndex: 1, slides:
                [
                    CarouselSlide("Home", "Home selects first", "H", CodexControlVariant.Secondary),
                    CarouselSlide("Arrow", "Left/Right moves", "A", CodexControlVariant.Success),
                    CarouselSlide("End", "End selects last", "E", CodexControlVariant.Outline)
                ]),
                BuildCarousel(selectedIndex: 2, loop: true, slides:
                [
                    CarouselSlide("First", "Wrapped target", "1", CodexControlVariant.Secondary),
                    CarouselSlide("Middle", "Loop stays enabled", "2", CodexControlVariant.Outline),
                    CarouselSlide("Last", "Next wraps", "3", CodexControlVariant.Warning)
                ]),
                BuildCarousel(
                    orientation: Orientation.Vertical,
                    selectedIndex: 1,
                    size: CodexControlSize.Small,
                    slides:
                    [
                        CarouselSlide("Up", "Up/PageUp", "1", CodexControlVariant.Secondary, height: 116),
                        CarouselSlide("Selected", "Current", "2", CodexControlVariant.Success, height: 116),
                        CarouselSlide("Down", "Down/PageDown", "3", CodexControlVariant.Outline, height: 116)
                    ])
            }
        };
    }

    private static CodexCarousel BuildCarousel(
        int selectedIndex = 0,
        Orientation orientation = Orientation.Horizontal,
        bool loop = false,
        CodexControlSize size = CodexControlSize.Medium,
        IReadOnlyList<CodexCarouselItem>? slides = null)
    {
        var carousel = new CodexCarousel
        {
            SelectedIndex = selectedIndex,
            Orientation = orientation,
            Loop = loop,
            Size = size,
            MinWidth = orientation == Orientation.Horizontal ? 360 : 300,
            MaxWidth = orientation == Orientation.Horizontal ? 560 : 340,
            MaxHeight = orientation == Orientation.Vertical ? 360 : double.PositiveInfinity
        };

        foreach (var slide in slides ?? [])
        {
            carousel.Items.Add(slide);
        }

        return carousel;
    }

    private static CodexCarouselItem CarouselSlide(
        string title,
        string description,
        string value,
        CodexControlVariant variant,
        double width = 280,
        double height = 160)
    {
        return new CodexCarouselItem
        {
            Width = width,
            MinHeight = height,
            Content = new CodexCard
            {
                Title = title,
                Description = description,
                Content = Text(value, CodexTextRole.Subtitle),
                Footer = new CodexBadge
                {
                    Content = variant == CodexControlVariant.Outline ? "queued" : "active",
                    Variant = variant,
                    HorizontalAlignment = HorizontalAlignment.Left
                }
            }
        };
    }

    private static CodexText Muted(string text)
    {
        return Text(text, CodexTextRole.Muted);
    }

    private static CodexText Text(string text, CodexTextRole role)
    {
        return new CodexText
        {
            Role = role,
            Text = text
        };
    }
}
