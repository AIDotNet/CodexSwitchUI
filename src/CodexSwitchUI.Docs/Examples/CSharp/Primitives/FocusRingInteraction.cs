using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using CodexSwitchUI.Tokens;

public static class FocusRingInteractionSample
{
    public static Control BuildFocusRingInteractionPreview()
    {
        var target = new CodexButton { Content = "Focusable target" };
        var ring = new CodexFocusRing
        {
            IsRingVisible = true,
            RingThickness = new Thickness(2),
            RingOffset = new Thickness(2),
            Content = target
        };
        var status = Text("Ring is visible with 2px thickness and 2px offset.", CodexTextRole.Muted);
        var isLargeRing = false;
        var isSuccessBrush = false;

        var toggleVisible = new CodexButton
        {
            Content = "Toggle ring",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        toggleVisible.Click += (_, _) =>
        {
            ring.IsRingVisible = !ring.IsRingVisible;
            status.Text = ring.IsRingVisible
                ? "Ring visibility restored for focus-visible state."
                : "Ring hidden while content remains interactive.";
        };

        var cycleGeometry = new CodexButton
        {
            Content = "Cycle geometry",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        cycleGeometry.Click += (_, _) =>
        {
            isLargeRing = !isLargeRing;
            ring.RingThickness = isLargeRing ? new Thickness(4) : new Thickness(2);
            ring.RingOffset = isLargeRing ? new Thickness(5) : new Thickness(2);
            status.Text = isLargeRing
                ? "Ring thickness and offset animated to the large focus target."
                : "Ring geometry returned to the default focus target.";
        };

        var requestFocus = new CodexButton
        {
            Content = "Request focus",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        requestFocus.Click += (_, _) =>
        {
            ring.IsRingVisible = true;
            target.Focus();
            status.Text = "Focus requested on the wrapped target and ring visibility stayed host-controlled.";
        };

        var switchAccent = new CodexButton
        {
            Content = "Switch accent",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        switchAccent.Click += (_, _) =>
        {
            isSuccessBrush = !isSuccessBrush;
            ring.RingBrush = ThemeBrush(isSuccessBrush ? CodexSwitchResourceKeys.SuccessBrush : CodexSwitchResourceKeys.RingBrush);
            status.Text = isSuccessBrush
                ? "Ring brush switched to success accent."
                : "Ring brush restored to the theme focus token.";
        };

        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 16,
            RowSpacing = 14,
            Children =
            {
                new CodexField
                {
                    Label = "Visibility and geometry",
                    Description = "Focus ring visibility, thickness, offset, and brush update through component properties.",
                    Content = new StackPanel
                    {
                        Spacing = 12,
                        Children =
                        {
                            status,
                            ring,
                            new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                Spacing = 8,
                                Children = { toggleVisible, cycleGeometry, requestFocus, switchAccent }
                            }
                        }
                    }
                },
                GridCell(new CodexField
                {
                    Label = "Wrapped input target",
                    Description = "Ring chrome wraps arbitrary focusable content without changing the child template.",
                    Content = new CodexFocusRing
                    {
                        RingOffset = new Thickness(4),
                        Content = new CodexTextBox
                        {
                            Width = 260,
                            Text = "Keyboard focus"
                        }
                    }
                }, row: 0, column: 1),
                GridCell(new CodexField
                {
                    Label = "Disabled target",
                    Description = "Disabled child controls remain visually wrapped while ignoring pointer activation.",
                    Content = new CodexFocusRing
                    {
                        IsRingVisible = true,
                        Content = new CodexButton
                        {
                            Content = "Disabled action",
                            IsEnabled = false
                        }
                    }
                }, row: 1, column: 0)
            }
        };
    }

    private static CodexText Text(string value, CodexTextRole role)
    {
        return new CodexText
        {
            Role = role,
            Text = value
        };
    }

    private static IBrush ThemeBrush(string key)
    {
        if (Application.Current?.TryFindResource(key, null, out var value) == true && value is IBrush brush)
        {
            return brush;
        }

        return Brushes.Transparent;
    }

    private static Control GridCell(Control control, int row, int column)
    {
        Grid.SetRow(control, row);
        Grid.SetColumn(control, column);
        return control;
    }
}
