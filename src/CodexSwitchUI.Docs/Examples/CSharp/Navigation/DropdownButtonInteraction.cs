using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class DropdownButtonInteractionSample
{
    public static Control BuildDropdownInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "OpenChanged: dropdown starts closed."
        };
        var trigger = new CodexButton
        {
            Content = "Restore target",
            Size = CodexControlSize.Small
        };

        var dropdown = new CodexDropdownButton
        {
            Content = "Provider actions",
            IsArrowVisible = true,
            Align = CodexDropdownAlign.Start,
            RestoreFocusElement = trigger,
            DropDownContent = new StackPanel
            {
                Width = 188,
                Spacing = 6,
                Children =
                {
                    new CodexButton { Content = "Rename", Variant = CodexControlVariant.Ghost, HorizontalAlignment = HorizontalAlignment.Stretch },
                    new CodexButton { Content = "Duplicate", Variant = CodexControlVariant.Ghost, HorizontalAlignment = HorizontalAlignment.Stretch },
                    new CodexButton { Content = "Delete", Variant = CodexControlVariant.Destructive, HorizontalAlignment = HorizontalAlignment.Stretch }
                }
            }
        };
        dropdown.OpenChanged += (_, args) =>
        {
            status.Text = $"OpenChanged: dropdown {(args.IsOpen ? "opened" : "closed")} (source={args.Source}).";
        };
        dropdown.RestoreFocusRequested += (_, _) =>
        {
            status.Text = "RestoreFocusRequested returned focus to the trigger.";
        };

        var keepSurface = new CodexDropdownButton
        {
            Content = "Keep surface",
            CloseOnItemSelected = false,
            Align = CodexDropdownAlign.End,
            DropDownContent = new StackPanel
            {
                Width = 196,
                Children =
                {
                    new CodexButton { Content = "Copy ID", Variant = CodexControlVariant.Ghost, HorizontalAlignment = HorizontalAlignment.Stretch },
                    new CodexButton { Content = "Open logs", Variant = CodexControlVariant.Ghost, HorizontalAlignment = HorizontalAlignment.Stretch }
                }
            }
        };

        var open = new CodexButton
        {
            Content = "Open",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        open.Click += (_, _) => dropdown.Open();

        var dismiss = new CodexButton
        {
            Content = "Dismiss",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        dismiss.Click += (_, _) => dropdown.Dismiss();

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                trigger,
                status,
                dropdown,
                keepSurface,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        open,
                        dismiss
                    }
                },
                new CodexDropdownButton
                {
                    Content = "Loading",
                    IsLoading = true,
                    DropDownContent = "Loading blocks open and child action close."
                },
                new CodexDropdownButton
                {
                    Content = "Disabled",
                    IsEnabled = false,
                    DropDownContent = "Disabled trigger cannot open the surface."
                }
            }
        };
    }
}
