using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class TextareaInteractionSample
{
    public static Control BuildTextareaInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Multiline text keeps wrapping, selection, and scrolling inside the textarea template."
        };
        var textarea = new CodexTextarea
        {
            Text = "stream: true\ntemperature: 0.7\nreasoning: medium",
            CaretIndex = 22,
            SelectionStart = 4,
            SelectionEnd = 12,
            MinHeight = 118
        };
        textarea.TextChanged += (_, _) => status.Text = $"TextChanged: {textarea.Text?.Length ?? 0} characters.";

        var markInvalid = new CodexButton
        {
            Content = "Mark invalid",
            Size = CodexControlSize.Small
        };
        markInvalid.Click += (_, _) =>
        {
            textarea.Intent = CodexControlIntent.Error;
            status.Text = "Textarea intent changed to Error.";
        };

        var append = new CodexButton
        {
            Content = "Append line",
            Variant = CodexControlVariant.Secondary,
            Size = CodexControlSize.Small
        };
        append.Click += (_, _) => textarea.Text += "\ncache: warm";

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                textarea,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { markInvalid, append }
                },
                new CodexTextarea
                {
                    PlaceholderText = "Optional JSON payload",
                    MinLines = 5,
                    MinHeight = 132
                },
                new CodexTextarea
                {
                    Intent = CodexControlIntent.Error,
                    PlaceholderText = "JSON body",
                    MinHeight = 118
                },
                new CodexTextarea { Text = "request_id: cs_123\nstatus: complete", IsReadOnly = true, MinHeight = 90 },
                new CodexTextarea { Text = "Locked", IsEnabled = false, MinHeight = 90 }
            }
        };
    }
}
