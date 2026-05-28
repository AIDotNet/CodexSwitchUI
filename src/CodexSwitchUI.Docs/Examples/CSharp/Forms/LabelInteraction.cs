using Avalonia.Controls;
using CodexSwitchUI.Controls;

public static class LabelInteractionSample
{
    public static Control BuildLabelInteractionPreview()
    {
        var terms = new CodexCheckBox { IsChecked = true };
        var disabled = new CodexCheckBox { IsEnabled = false };
        var provider = new CodexTextBox { Text = "OpenAI" };

        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 10,
                    Children =
                    {
                        terms,
                        new CodexLabel
                        {
                            Target = terms,
                            Content = "Clicking this label focuses the checkbox"
                        }
                    }
                },
                new StackPanel
                {
                    Spacing = 6,
                    Children =
                    {
                        new CodexLabel { Target = provider, Content = "_Provider", IsRequired = true },
                        provider
                    }
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 10,
                    Children =
                    {
                        disabled,
                        new CodexLabel
                        {
                            Target = disabled,
                            Content = "Disabled target uses muted opacity"
                        }
                    }
                },
                new StackPanel
                {
                    Spacing = 6,
                    Children =
                    {
                        new CodexLabel
                        {
                            Content = "Required endpoint",
                            Intent = CodexControlIntent.Error,
                            IsRequired = true
                        },
                        new CodexTextBox
                        {
                            Intent = CodexControlIntent.Error,
                            PlaceholderText = "https://api.example.com"
                        }
                    }
                }
            }
        };
    }
}
