using Avalonia.Controls;
using Avalonia.Input;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class ComboboxInteractionSample
{
    public static Control BuildComboboxInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "OpenChanged, SelectionChanged, and InputValueChanged update this status."
        };
        var combobox = new CodexCombobox
        {
            ItemsSource = Frameworks(),
            Text = "n",
            IsOpen = true,
            AutoHighlight = true,
            MinWidth = 240
        };
        combobox.OpenChanged += (_, args) =>
        {
            status.Text = $"OpenChanged: popup {(args.IsOpen ? "opened" : "closed")} (source={args.Source}).";
        };
        combobox.SelectionChanged += (_, args) =>
        {
            status.Text = args.NewItem is null
                ? $"SelectionChanged: {args.Source} cleared [{args.OldIndex}] {args.OldValue ?? "none"}."
                : $"SelectionChanged: {args.Source} [{args.OldIndex}] {args.OldValue ?? "none"} -> [{args.NewIndex}] {args.NewValue ?? "none"}.";
        };
        combobox.InputValueChanged += (_, args) =>
        {
            status.Text = $"InputValueChanged: {args.OldValue ?? "empty"} -> {args.NewValue ?? "empty"}.";
        };

        var moveHighlight = new CodexButton
        {
            Content = "Arrow down",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        moveHighlight.Click += (_, _) => combobox.TryHandleInputKey(Key.Down);

        var commit = new CodexButton
        {
            Content = "Enter",
            Size = CodexControlSize.Small
        };
        commit.Click += (_, _) => combobox.TryHandleInputKey(Key.Enter);

        var clear = new CodexButton
        {
            Content = "Clear",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        clear.Click += (_, _) => combobox.ClearSelection();

        var close = new CodexButton
        {
            Content = "Close",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        close.Click += (_, _) => combobox.Close();

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                combobox,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        moveHighlight,
                        commit,
                        clear,
                        close
                    }
                },
                new CodexCombobox
                {
                    ItemsSource = Frameworks(),
                    Text = "re",
                    IsOpen = true,
                    CloseOnSelect = false,
                    MinWidth = 240
                },
                new CodexCombobox
                {
                    ItemsSource = Frameworks(),
                    IsLoading = true,
                    LoadingContent = "Loading frameworks...",
                    MinWidth = 240
                }
            }
        };
    }

    private static string[] Frameworks()
    {
        return ["Next.js", "SvelteKit", "Nuxt.js", "Remix", "Astro"];
    }
}
