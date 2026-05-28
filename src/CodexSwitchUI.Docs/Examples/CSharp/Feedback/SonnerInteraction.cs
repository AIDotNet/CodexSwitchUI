using Avalonia;
using Avalonia.Controls;
using CodexSwitchUI.Controls;

public static class SonnerInteractionSample
{
public static Control BuildSonnerInteractionPreview()
{
    CodexSonnerService.Clear();

    CodexSonnerService.Success("Provider saved", new CodexSonnerOptions
    {
        Description = "Success toast auto-dismisses after the configured duration.",
        Action = new CodexSonnerAction("Undo", () => { })
    });

    CodexSonnerService.Warning("Fallback active", new CodexSonnerOptions
    {
        Description = "Warning toast keeps the close affordance visible.",
        Cancel = new CodexSonnerAction("Dismiss", () => { })
    });

    CodexSonnerService.Loading("Refreshing usage", new CodexSonnerOptions
    {
        Description = "Loading toast remains until the host dismisses it.",
        CloseButton = false
    });

    var expandedViewport = new Border
    {
        MinHeight = 240,
        Child = new CodexSonner
        {
            Position = CodexSonnerPosition.TopRight,
            RichColors = true,
            Expand = true,
            VisibleToasts = 3,
            CloseButton = true,
            Offset = new Thickness(0)
        }
    };

    var compactViewport = new Border
    {
        MinHeight = 240,
        Child = new CodexSonner
        {
            Position = CodexSonnerPosition.BottomLeft,
            RichColors = false,
            Expand = false,
            VisibleToasts = 2,
            CloseButton = false,
            Offset = new Thickness(0)
        }
    };
    Grid.SetColumn(compactViewport, 1);

    return new Grid
    {
        ColumnDefinitions =
        {
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star)
        },
        ColumnSpacing = 14,
        Children =
        {
            expandedViewport,
            compactViewport
        }
    };
}
}
