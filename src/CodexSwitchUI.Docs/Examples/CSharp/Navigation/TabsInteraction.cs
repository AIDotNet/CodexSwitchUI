using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class TabsInteractionSample
{
    public static Control BuildTabsInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "ValueChanged reports old value, new value, and source metadata."
        };

        var valueTabs = new CodexTabs
        {
            SelectedValue = "code",
            Items =
            {
                new CodexTabItem { Header = "Preview", Value = "preview", Content = "Preview content" },
                new CodexTabItem { Header = "Code", Value = "code", Content = "Selected by value." },
                new CodexTabItem { Header = "Disabled", Value = "disabled", IsEnabled = false },
                new CodexTabItem { Header = "Events", Value = "events", Content = "End selects the final enabled tab." }
            }
        };
        valueTabs.ValueChanged += (_, args) =>
        {
            status.Text = $"ValueChanged: {args.OldValue ?? "none"} -> {args.NewValue ?? "none"} ({args.Source}).";
        };

        var selectPreview = new CodexButton
        {
            Content = "Select Preview",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        selectPreview.Click += (_, _) => valueTabs.SelectedValue = "preview";

        var manualTabs = new CodexTabs
        {
            Variant = CodexTabsVariant.Line,
            ActivationMode = CodexTabsActivationMode.Manual,
            SelectedValue = "home",
            Items =
            {
                new CodexTabItem { Header = "Home", Value = "home", Content = "Manual mode starts here." },
                new CodexTabItem { Header = "State", Value = "state", Content = "Arrow keys move focus first." },
                new CodexTabItem { Header = "Disabled", Value = "disabled", IsEnabled = false },
                new CodexTabItem { Header = "End", Value = "end", Content = "Enter or Space activates the focused trigger." }
            }
        };
        manualTabs.ValueChanged += (_, args) =>
        {
            status.Text = $"Manual ValueChanged: {args.OldValue ?? "none"} -> {args.NewValue ?? "none"} ({args.Source}).";
        };

        var verticalTabs = new CodexTabs
        {
            Orientation = Orientation.Vertical,
            IsLoop = false,
            SelectedValue = "selected",
            Items =
            {
                new CodexTabItem { Header = "Up", Value = "up" },
                new CodexTabItem { Header = "Selected", Value = "selected" },
                new CodexTabItem { Header = "Disabled", Value = "disabled", IsEnabled = false },
                new CodexTabItem { Header = "Down", Value = "down" }
            }
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                valueTabs,
                selectPreview,
                manualTabs,
                verticalTabs,
                status
            }
        };
    }
}
