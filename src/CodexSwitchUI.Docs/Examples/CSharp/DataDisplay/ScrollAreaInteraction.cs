using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class ScrollAreaInteractionSample
{
    public static Control BuildScrollAreaInteractionPreview()
    {
        var status = Muted("Offset: top. Use the buttons or pointer wheel to exercise boundary classes.");
        var area = new CodexScrollArea
        {
            Type = CodexScrollAreaType.Scroll,
            IsInsetContent = true,
            Width = 320,
            Height = 190,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = ScrollAreaRows("OpenAI", "Claude", "Gemini", "Local proxy", "Fallback", "Archive", "Usage", "Settings", "Billing", "Audit")
        };
        area.ScrollChanged += (_, _) =>
        {
            var maxY = Math.Max(0, area.Extent.Height - area.Viewport.Height);
            status.Text = $"Offset: {Math.Round(area.Offset.Y)} / {Math.Round(maxY)}";
        };

        var top = Button("Top");
        top.Click += (_, _) =>
        {
            area.ScrollToTop();
            status.Text = "Requested top boundary.";
        };

        var bottom = Button("Bottom");
        bottom.Click += (_, _) =>
        {
            area.ScrollToBottom();
            status.Text = "Requested bottom boundary.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                area,
                Row(top, bottom),
                new CodexScrollArea
                {
                    Type = CodexScrollAreaType.Hover,
                    IsInsetContent = true,
                    Width = 300,
                    Height = 120,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Content = ScrollAreaRows("Hover", "Reveals", "On pointer", "Without layout shift")
                },
                new CodexScrollArea
                {
                    Type = CodexScrollAreaType.Always,
                    IsEnabled = false,
                    Width = 300,
                    Height = 120,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Content = ScrollAreaRows("Disabled", "Keeps", "Boundary", "State")
                }
            }
        };
    }

    private static StackPanel ScrollAreaRows(params string[] rows)
    {
        var stack = new StackPanel { Spacing = 8 };
        foreach (var row in rows)
        {
            stack.Children.Add(new CodexButton
            {
                Content = row,
                Variant = CodexControlVariant.Ghost,
                HorizontalAlignment = HorizontalAlignment.Stretch
            });
        }

        return stack;
    }

    private static CodexButton Button(string label)
    {
        return new CodexButton { Content = label, Size = CodexControlSize.Small, Variant = CodexControlVariant.Secondary };
    }

    private static StackPanel Row(params Control[] children)
    {
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        foreach (var child in children)
            row.Children.Add(child);
        return row;
    }

    private static CodexText Muted(string text)
    {
        return new CodexText { Role = CodexTextRole.Muted, Text = text };
    }
}
