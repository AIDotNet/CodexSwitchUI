using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;

public static class SonnerInteractionSample
{
public static Control BuildSonnerInteractionPreview()
{
    CodexSonnerService.Clear();

    var status = new CodexText
    {
        Role = CodexTextRole.Muted,
        Text = "Sonner viewport is empty by default. Trigger a toast to mount host rows."
    };

    var success = new CodexButton
    {
        Content = "Success",
        Size = CodexControlSize.Small,
        Variant = CodexControlVariant.Secondary
    };
    success.Click += (_, _) =>
    {
        CodexSonnerService.Success("Provider saved", new CodexSonnerOptions
        {
            Description = "Success toast auto-dismisses after the configured duration.",
            Action = new CodexSonnerAction("Undo", () => { })
        });
        status.Text = "Success toast queued through CodexSonnerService.";
    };

    var warning = new CodexButton
    {
        Content = "Warning",
        Size = CodexControlSize.Small,
        Variant = CodexControlVariant.Ghost
    };
    warning.Click += (_, _) =>
    {
        CodexSonnerService.Warning("Fallback active", new CodexSonnerOptions
        {
            Description = "Warning toast keeps the close affordance visible.",
            Cancel = new CodexSonnerAction("Dismiss", () => { })
        });
        status.Text = "Warning toast queued with cancel action.";
    };

    var loading = new CodexButton
    {
        Content = "Loading",
        Size = CodexControlSize.Small,
        Variant = CodexControlVariant.Ghost
    };
    loading.Click += (_, _) =>
    {
        CodexSonnerService.Loading("Refreshing usage", new CodexSonnerOptions
        {
            Description = "Loading toast remains until the host dismisses it.",
            CloseButton = false
        });
        status.Text = "Loading toast queued and remains until dismissed.";
    };

    var clear = new CodexButton
    {
        Content = "Clear",
        Size = CodexControlSize.Small,
        Variant = CodexControlVariant.Outline
    };
    clear.Click += (_, _) =>
    {
        CodexSonnerService.Clear();
        status.Text = "Sonner queue cleared; viewport returned to empty default.";
    };

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

    var viewportGrid = new Grid
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

    return new StackPanel
    {
        Spacing = 12,
        Children =
        {
            status,
            new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                Children =
                {
                    success,
                    warning,
                    loading,
                    clear
                }
            },
            viewportGrid
        }
    };
}
}
