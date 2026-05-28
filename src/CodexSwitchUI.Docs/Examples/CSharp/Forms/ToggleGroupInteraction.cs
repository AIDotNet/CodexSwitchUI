using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class ToggleGroupInteractionSample
{
    public static Control BuildToggleGroupInteractionPreview()
    {
        var singleStatus = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Single value: center (source=Programmatic)."
        };
        var single = new CodexToggleGroup
        {
            Spacing = 0,
            SelectedValue = "center",
            Items =
            {
                new CodexToggleGroupItem { Content = "Left", Value = "left" },
                new CodexToggleGroupItem { Content = "Center", Value = "center" },
                new CodexToggleGroupItem { Content = "Right", Value = "right" }
            }
        };
        single.ValueChanged += (_, args) =>
        {
            singleStatus.Text = string.IsNullOrWhiteSpace(args.NewValue)
                ? $"Single value cleared (source={args.Source})."
                : $"Single value: {args.NewValue} (source={args.Source}).";
        };

        var multiStatus = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Multiple values: bold, italic (source=Programmatic)."
        };
        var multiple = new CodexToggleGroup
        {
            Type = CodexToggleGroupType.Multiple,
            Variant = CodexControlVariant.Outline,
            SelectedValues = ["bold", "italic"],
            Items =
            {
                new CodexToggleGroupItem { Content = "Bold", Value = "bold" },
                new CodexToggleGroupItem { Content = "Italic", Value = "italic" },
                new CodexToggleGroupItem { Content = "Underline", Value = "underline" }
            }
        };
        multiple.ValueChanged += (_, args) =>
        {
            multiStatus.Text = args.NewValues.Count == 0
                ? $"Multiple values cleared (source={args.Source})."
                : $"Multiple values: {string.Join(", ", args.NewValues)} (source={args.Source}).";
        };

        var selectLeft = new CodexButton
        {
            Content = "Select left",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        selectLeft.Click += (_, _) => single.SelectedValue = "left";

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                single,
                singleStatus,
                selectLeft,
                multiple,
                multiStatus,
                new CodexToggleGroup
                {
                    IsLoop = false,
                    SelectedValue = "preview",
                    Items =
                    {
                        new CodexToggleGroupItem { Content = "Preview", Value = "preview" },
                        new CodexToggleGroupItem { Content = "Code", Value = "code" },
                        new CodexToggleGroupItem { Content = "Disabled", Value = "disabled", IsEnabled = false },
                        new CodexToggleGroupItem { Content = "Events", Value = "events" }
                    }
                },
                new CodexToggleGroup
                {
                    Orientation = Orientation.Vertical,
                    Variant = CodexControlVariant.Outline,
                    SelectedValue = "list",
                    Items =
                    {
                        new CodexToggleGroupItem { Content = "List", Value = "list" },
                        new CodexToggleGroupItem { Content = "Grid", Value = "grid" },
                        new CodexToggleGroupItem { Content = "Cards", Value = "cards" }
                    }
                }
            }
        };
    }
}
