using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class TypographyInteractionSample
{
    public static Control BuildTypographyInteractionPreview()
    {
        var roles = new[]
        {
            CodexTextRole.Body,
            CodexTextRole.Title,
            CodexTextRole.Subtitle,
            CodexTextRole.Muted,
            CodexTextRole.Code
        };
        var roleIndex = 0;
        var status = Text("Role is Body and wrapping is enabled.", CodexTextRole.Muted);
        var sample = Text("Body copy wraps inside the component while host events change only the typography role.", roles[roleIndex]);
        sample.Width = 330;
        sample.TextWrapping = TextWrapping.Wrap;

        var cycleRole = new CodexButton
        {
            Content = "Cycle role",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        cycleRole.Click += (_, _) =>
        {
            roleIndex = (roleIndex + 1) % roles.Length;
            sample.Role = roles[roleIndex];
            sample.Text = sample.Role == CodexTextRole.Code
                ? "code-role: CodexMotion.ResolveDefaultDuration(target)"
                : $"{sample.Role} role updates class-driven typography without replacing the text node.";
            status.Text = $"Role changed to {sample.Role}.";
        };

        var toggleWrap = new CodexButton
        {
            Content = "Toggle wrap",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        toggleWrap.Click += (_, _) =>
        {
            sample.TextWrapping = sample.TextWrapping == TextWrapping.Wrap ? TextWrapping.NoWrap : TextWrapping.Wrap;
            sample.Width = sample.TextWrapping == TextWrapping.Wrap ? 330 : 260;
            status.Text = sample.TextWrapping == TextWrapping.Wrap
                ? "Wrapping restored for responsive content."
                : "Wrapping disabled to show single-line overflow behavior.";
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
                    Label = "Role and wrapping",
                    Description = "Host events update role classes and wrapping without replacing the text block.",
                    Content = new StackPanel
                    {
                        Spacing = 10,
                        Children =
                        {
                            status,
                            sample,
                            new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                Spacing = 8,
                                Children = { cycleRole, toggleWrap }
                            }
                        }
                    }
                },
                GridCell(new CodexField
                {
                    Label = "Code role",
                    Description = "Inline code keeps compact rhythm while inheriting tokenized color and background.",
                    Content = new StackPanel
                    {
                        Spacing = 8,
                        Children =
                        {
                            Text("const duration = CodexMotion.ResolveDefaultDuration(target);", CodexTextRole.Code),
                            Text("Code role stays readable inside command descriptions and API snippets.", CodexTextRole.Muted)
                        }
                    }
                }, row: 0, column: 1),
                GridCell(new CodexField
                {
                    Label = "Dense hierarchy",
                    Description = "Title, body, muted, and code roles compose in compact surfaces.",
                    Content = new StackPanel
                    {
                        Spacing = 7,
                        Children =
                        {
                            Text("Usage summary", CodexTextRole.Subtitle),
                            Text("42.7K tokens routed through the primary provider.", CodexTextRole.Body),
                            Text("route=openai/default", CodexTextRole.Code),
                            Text("Compact dashboards keep text rhythm token-driven.", CodexTextRole.Muted)
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

    private static Control GridCell(Control control, int row, int column)
    {
        Grid.SetRow(control, row);
        Grid.SetColumn(control, column);
        return control;
    }
}
