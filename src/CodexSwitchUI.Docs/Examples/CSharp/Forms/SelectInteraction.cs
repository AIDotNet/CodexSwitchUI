using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class SelectInteractionSample
{
    public static Control BuildSelectInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "OpenChanged and source-aware ValueChanged update this status."
        };
        var select = new CodexSelect
        {
            ItemsSource = new[] { "OpenAI", "Claude", "Responses" },
            SelectedIndex = 0,
            IsDropDownOpen = true,
            MinWidth = 240
        };
        select.OpenChanged += (_, args) =>
        {
            status.Text = $"OpenChanged: popup {(args.IsOpen ? "opened" : "closed")} (source={args.Source}).";
        };
        select.ValueChanged += (_, args) =>
        {
            status.Text = $"ValueChanged: {args.OldValue ?? "none"} -> {args.NewValue ?? "none"} (source={args.Source}).";
        };

        var chooseClaude = new CodexButton
        {
            Content = "Claude",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        chooseClaude.Click += (_, _) => select.SelectedIndex = 1;

        var closePopup = new CodexButton
        {
            Content = "Close",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        closePopup.Click += (_, _) => select.IsDropDownOpen = false;

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
                        chooseClaude,
                        closePopup
                    }
                },
                new CodexSelect
                {
                    ItemsSource = new[] { "Primary", "Fallback" },
                    SelectedIndex = 1,
                    Intent = CodexControlIntent.Warning,
                    Size = CodexControlSize.Small,
                    MinWidth = 240
                },
                new CodexSelect
                {
                    ItemsSource = new[] { "Locked" },
                    SelectedIndex = 0,
                    IsEnabled = false,
                    MinWidth = 240
                }
            }
        };
    }
}
