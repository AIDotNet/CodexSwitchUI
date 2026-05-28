using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class TextBoxInteractionSample
{
    public static Control BuildTextBoxInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "TextChanged, selection, slots, read-only, and validation stay on the owned TextBox template."
        };
        var input = new CodexTextBox
        {
            Text = "OpenAI",
            CaretIndex = 6,
            SelectionStart = 0,
            SelectionEnd = 6,
            MinWidth = 260
        };
        input.TextChanged += (_, _) => status.Text = $"TextChanged: {input.Text}";

        var validate = new CodexButton
        {
            Content = "Validate",
            Size = CodexControlSize.Small
        };
        validate.Click += (_, _) =>
        {
            var isValid = !string.IsNullOrWhiteSpace(input.Text);
            input.Intent = isValid ? CodexControlIntent.Success : CodexControlIntent.Error;
            status.Text = isValid ? "Provider name is valid." : "Provider name is required.";
        };

        var clear = new CodexButton
        {
            Content = "Clear",
            Variant = CodexControlVariant.Ghost,
            Size = CodexControlSize.Small
        };
        clear.Click += (_, _) => input.Text = string.Empty;

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                input,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { validate, clear }
                },
                new CodexTextBox
                {
                    PlaceholderText = "Provider alias",
                    InnerLeftContent = "cs://",
                    InnerRightContent = ".local",
                    MinWidth = 260
                },
                new CodexTextBox
                {
                    Text = "/v1/chat/completions",
                    Intent = CodexControlIntent.Error,
                    SelectionStart = 0,
                    SelectionEnd = 3,
                    MinWidth = 260
                },
                new CodexTextBox { Text = "https://api.openai.com/v1", IsReadOnly = true, MinWidth = 260 },
                new CodexTextBox { Text = "Locked", IsEnabled = false, MinWidth = 260 }
            }
        };
    }
}
