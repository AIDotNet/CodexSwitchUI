using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System.Linq;

public static class ResizableInteractionSample
{
    public static Control BuildResizableInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Resize events publish panel percentages."
        };
        var handle = new CodexResizableHandle { WithHandle = true };
        var group = new CodexResizablePanelGroup
        {
            Width = 640,
            Height = 220,
            Orientation = Orientation.Horizontal,
            Children =
            {
                Panel("Navigation", 30, 20, 60),
                handle,
                Panel("Workspace", 70, 35)
            }
        };
        group.LayoutChanged += (_, args) =>
        {
            status.Text = $"Handle {args.HandleIndex + 1}: {string.Join(" / ", args.PanelSizes.Select(size => $"{Math.Round(size)}%"))}";
        };

        var shrink = new CodexButton
        {
            Content = "Shrink left",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        shrink.Click += (_, _) => group.ResizeHandleByPercent(handle, -10);

        var grow = new CodexButton
        {
            Content = "Grow left",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        grow.Click += (_, _) => group.ResizeHandleByPercent(handle, 10);

        var keyboard = new CodexButton
        {
            Content = "Keyboard right",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        keyboard.Click += (_, _) => handle.TryHandleResizeKey(Avalonia.Input.Key.Right);

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                group,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { shrink, grow, keyboard }
                },
                new CodexResizablePanelGroup
                {
                    Width = 320,
                    Height = 260,
                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        Panel("Header", 40, 25),
                        new CodexResizableHandle { WithHandle = true },
                        Panel("Body", 60, 30)
                    }
                }
            }
        };
    }

    private static CodexResizablePanel Panel(string title, double defaultSize, double minSize, double maxSize = 95)
    {
        return new CodexResizablePanel
        {
            DefaultSize = defaultSize,
            MinSize = minSize,
            MaxSize = maxSize,
            Content = new CodexText
            {
                Role = CodexTextRole.Muted,
                Text = title
            }
        };
    }
}
