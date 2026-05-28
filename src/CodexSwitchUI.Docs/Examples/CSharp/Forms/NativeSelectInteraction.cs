using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class NativeSelectInteractionSample
{
    public static Control BuildNativeSelectInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Native select events update this status with source metadata."
        };
        var select = new CodexNativeSelect
        {
            SelectedIndex = 1,
            MinWidth = 240,
            Items =
            {
                new CodexNativeSelectOption { Value = "", Content = "Select provider" },
                new CodexNativeSelectOption { Value = "openai", Content = "OpenAI" },
                new CodexNativeSelectOption { Value = "claude", Content = "Claude" },
                new CodexNativeSelectOption { Value = "responses", Content = "Responses" }
            }
        };
        select.OpenChanged += (_, args) =>
        {
            status.Text = $"OpenChanged: popup {(args.IsOpen ? "opened" : "closed")} (source={args.Source}).";
        };
        select.ValueChanged += (_, args) =>
        {
            status.Text = $"ValueChanged: {args.OldValue ?? "none"} -> {args.NewValue ?? "none"} (source={args.Source}).";
        };

        var open = new CodexButton
        {
            Content = "Open",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        open.Click += (_, _) => select.IsDropDownOpen = true;

        var chooseClaude = new CodexButton
        {
            Content = "Claude",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        chooseClaude.Click += (_, _) => select.SelectedIndex = 2;

        var close = new CodexButton
        {
            Content = "Close",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        close.Click += (_, _) => select.IsDropDownOpen = false;

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                select,
                status,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        open,
                        chooseClaude,
                        close
                    }
                },
                new CodexNativeSelect
                {
                    SelectedIndex = 1,
                    MinWidth = 240,
                    Items =
                    {
                        new CodexNativeSelectOption { Value = "", Content = "Select model" },
                        new CodexNativeSelectOption { Value = "gpt-5", Content = "gpt-5" },
                        new CodexNativeSelectOption { Value = "legacy", Content = "Legacy model", IsEnabled = false },
                        new CodexNativeSelectOption { Value = "mini", Content = "gpt-5-mini" }
                    }
                },
                new CodexNativeSelect
                {
                    SelectedIndex = 2,
                    Intent = CodexControlIntent.Warning,
                    Size = CodexControlSize.Small,
                    MinWidth = 240,
                    Items =
                    {
                        new CodexNativeSelectOption { Value = "", Content = "Select route" },
                        new CodexNativeSelectOption { Value = "primary", Content = "Primary" },
                        new CodexNativeSelectOption { Value = "fallback", Content = "Fallback" }
                    }
                }
            }
        };
    }
}
