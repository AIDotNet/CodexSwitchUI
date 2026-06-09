using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class SplitButtonInteractionSample
{
    public static Control BuildSplitButtonInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "OpenChanged reports pointer, keyboard, selection, or programmatic source."
        };
        var split = new CodexSplitButton
        {
            Content = "Run sync",
            IsArrowVisible = true,
            Align = CodexDropdownAlign.Start,
            DropDownContent = ActionMenu("Run once", "Schedule", "Stop")
        };
        split.Click += (_, _) => status.Text = "Primary action executed.";
        split.OpenChanged += (_, args) =>
        {
            status.Text = $"OpenChanged: {(args.IsOpen ? "open" : "closed")} (source={args.Source}).";
        };
        split.RestoreFocusRequested += (_, _) => status.Text = "RestoreFocusRequested after dismissal.";

        var open = new CodexButton
        {
            Content = "Open",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        open.Click += (_, _) => split.Open();

        var dismiss = new CodexButton
        {
            Content = "Dismiss",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        dismiss.Click += (_, _) => split.Dismiss();

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                split,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { open, dismiss }
                },
                new CodexSplitButton
                {
                    Content = "Keep open",
                    CloseOnItemSelected = false,
                    DropDownContent = ActionMenu("Preview", "Copy command")
                },
                new CodexSplitButton
                {
                    Content = "Loading",
                    IsLoading = true,
                    DropDownContent = new CodexText
                    {
                        Role = CodexTextRole.Muted,
                        Text = "Loading blocks the primary action and menu trigger."
                    }
                },
                new CodexSplitButton
                {
                    Content = "End aligned",
                    Align = CodexDropdownAlign.End,
                    Variant = CodexControlVariant.Secondary,
                    DropDownContent = ActionMenu("Rename", "Duplicate", "Archive")
                }
            }
        };
    }

    private static StackPanel ActionMenu(params string[] labels)
    {
        var menu = new StackPanel { Width = 190, Spacing = 6 };
        foreach (var label in labels)
        {
            menu.Children.Add(new CodexButton
            {
                Content = label,
                Variant = CodexControlVariant.Ghost,
                HorizontalAlignment = HorizontalAlignment.Stretch
            });
        }

        return menu;
    }
}
