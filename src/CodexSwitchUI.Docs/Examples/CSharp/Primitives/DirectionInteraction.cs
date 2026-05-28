using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class DirectionInteractionSample
{
    public static Control BuildDirectionInteractionPreview()
    {
        var status = Text("Direction is LeftToRight.", CodexTextRole.Muted);
        var provider = new CodexDirection
        {
            Direction = CodexDirectionMode.LeftToRight,
            Content = new CodexCard
            {
                Title = "Runtime language surface",
                Description = "Switch direction without rebuilding the form.",
                Content = new StackPanel
                {
                    Spacing = 10,
                    Children =
                    {
                        new CodexField
                        {
                            Label = "Provider",
                            Description = "Child fields and action rows inherit the active direction.",
                            Content = new CodexTextBox
                            {
                                Width = 260,
                                Text = "Localized value"
                            }
                        },
                        DirectionRow("Action row", "Cancel", "Continue")
                    }
                }
            }
        };
        provider.DirectionChanged += (_, args) =>
        {
            status.Text = $"DirectionChanged -> {args.NewDirection}; FlowDirection={args.FlowDirection}.";
        };

        var toggleDirection = new CodexButton
        {
            Content = "Switch direction",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        toggleDirection.Click += (_, _) =>
        {
            provider.Direction = provider.Direction == CodexDirectionMode.LeftToRight
                ? CodexDirectionMode.RightToLeft
                : CodexDirectionMode.LeftToRight;
        };

        var forceLtr = new CodexButton
        {
            Content = "Force LTR",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        forceLtr.Click += (_, _) => provider.Direction = CodexDirectionMode.LeftToRight;

        var forceRtl = new CodexButton
        {
            Content = "Force RTL",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        forceRtl.Click += (_, _) => provider.Direction = CodexDirectionMode.RightToLeft;

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
                    Label = "Provider event",
                    Description = "Direction changes update FlowDirection, classes, and event status.",
                    Content = new StackPanel
                    {
                        Spacing = 10,
                        Children =
                        {
                            status,
                            provider,
                            new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                Spacing = 8,
                                Children = { toggleDirection, forceLtr, forceRtl }
                            }
                        }
                    }
                },
                GridCell(new CodexField
                {
                    Label = "RTL form alignment",
                    Description = "Fields, inputs, and action rows inherit direction like Web document islands.",
                    Content = new CodexDirection
                    {
                        Direction = CodexDirectionMode.RightToLeft,
                        Content = new StackPanel
                        {
                            Spacing = 10,
                            Children =
                            {
                                new CodexField
                                {
                                    Label = "Email",
                                    Description = "Trailing controls mirror with the provider.",
                                    Content = new CodexTextBox
                                    {
                                        Width = 260,
                                        Text = "member@example.com"
                                    }
                                },
                                DirectionRow("Actions", "Cancel", "Save")
                            }
                        }
                    }
                }, row: 0, column: 1),
                GridCell(new CodexField
                {
                    Label = "Nested code island",
                    Description = "RTL shells can keep command snippets and provider routes LTR.",
                    Content = new CodexDirection
                    {
                        Direction = CodexDirectionMode.RightToLeft,
                        Content = new StackPanel
                        {
                            Spacing = 10,
                            Children =
                            {
                                Text("Outer action order follows RTL.", CodexTextRole.Muted),
                                DirectionRow("Command", "Previous", "Next"),
                                new CodexDirection
                                {
                                    Direction = CodexDirectionMode.LeftToRight,
                                    Content = Text("curl https://api.example.test/v1/models", CodexTextRole.Code)
                                }
                            }
                        }
                    }
                }, row: 1, column: 0),
                GridCell(new CodexDirection
                {
                    Direction = CodexDirectionMode.RightToLeft,
                    IsEnabled = false,
                    Content = DirectionSurface("Disabled RTL", CodexDirectionMode.RightToLeft, "Opacity changes; layout direction remains RTL.")
                }, row: 1, column: 1)
            }
        };
    }

    private static CodexDirection DirectionSurface(string title, CodexDirectionMode direction, string description)
    {
        return new CodexDirection
        {
            Direction = direction,
            Content = new CodexCard
            {
                Title = title,
                Description = description,
                Content = new StackPanel
                {
                    Spacing = 10,
                    Children =
                    {
                        new CodexField
                        {
                            Label = "Provider",
                            Description = direction == CodexDirectionMode.RightToLeft
                                ? "Right-to-left content inherits mirrored layout."
                                : "Left-to-right content uses the default layout.",
                            Content = new CodexTextBox
                            {
                                Width = 260,
                                Text = direction == CodexDirectionMode.RightToLeft ? "RTL value" : "LTR value"
                            }
                        },
                        DirectionRow("Action row", "Cancel", "Continue")
                    }
                }
            }
        };
    }

    private static StackPanel DirectionRow(string label, string secondary, string primary)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                new CodexBadge
                {
                    Content = label,
                    Variant = CodexControlVariant.Secondary
                },
                new CodexButton
                {
                    Content = secondary,
                    Size = CodexControlSize.Small,
                    Variant = CodexControlVariant.Outline
                },
                new CodexButton
                {
                    Content = primary,
                    Size = CodexControlSize.Small
                }
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

    private static Control GridCell(Control control, int row, int column)
    {
        Grid.SetRow(control, row);
        Grid.SetColumn(control, column);
        return control;
    }
}
