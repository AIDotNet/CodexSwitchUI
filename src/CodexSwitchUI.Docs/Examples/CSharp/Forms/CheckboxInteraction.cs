using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class CheckboxInteractionSample
{
    public static Control BuildCheckboxInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "CheckedStateChanged: routing is checked (source=Programmatic)."
        };
        var routing = new CodexCheckBox
        {
            Content = "Enable provider routing",
            IsChecked = true
        };
        routing.CheckedStateChanged += (_, args) =>
        {
            status.Text = $"Routing changed from {Format(args.OldValue)} to {Format(args.NewValue)} (source={args.Source}).";
        };

        var archived = new CodexCheckBox { Content = "Sync archived sessions" };
        archived.CheckedStateChanged += (_, args) =>
        {
            status.Text = $"Archive sync changed from {Format(args.OldValue)} to {Format(args.NewValue)} (source={args.Source}).";
        };

        var chooseIndeterminate = new CodexButton
        {
            Content = "Set mixed",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        chooseIndeterminate.Click += (_, _) => routing.IsChecked = null;

        var chooseChecked = new CodexButton
        {
            Content = "Check",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        chooseChecked.Click += (_, _) => routing.IsChecked = true;

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                routing,
                archived,
                status,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { chooseIndeterminate, chooseChecked }
                },
                new CodexCheckBox
                {
                    Content = "Mixed provider selection",
                    IsThreeState = true,
                    IsChecked = null
                },
                new CodexCheckBox
                {
                    Content = "Locked on",
                    IsChecked = true,
                    IsEnabled = false
                }
            }
        };
    }

    private static string Format(bool? value)
    {
        return value switch
        {
            true => "checked",
            false => "unchecked",
            _ => "indeterminate"
        };
    }
}
