using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class RadioGroupInteractionSample
{
    public static Control BuildRadioGroupInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Selected value is balanced."
        };
        var group = new CodexRadioGroup
        {
            SelectedValue = "balanced",
            Items =
            {
                new CodexRadioGroupItem { Value = "balanced", Content = "Balanced" },
                new CodexRadioGroupItem { Value = "reasoning", Content = "Reasoning" },
                new CodexRadioGroupItem { Value = "latency", Content = "Low latency" }
            }
        };
        group.ValueChanged += (_, args) =>
        {
            status.Text = $"ValueChanged({args.Source}): [{args.OldIndex}] {args.OldValue ?? "none"} -> [{args.NewIndex}] {args.NewValue ?? "none"}.";
        };

        var cycleValue = new CodexButton
        {
            Content = "Cycle value",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        cycleValue.Click += (_, _) =>
        {
            group.SelectedValue = group.SelectedValue switch
            {
                "balanced" => "reasoning",
                "reasoning" => "latency",
                _ => "balanced"
            };
        };

        var toggleLoading = new CodexButton
        {
            Content = "Toggle loading",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        toggleLoading.Click += (_, _) => group.IsLoading = !group.IsLoading;

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                group,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { cycleValue, toggleLoading }
                },
                new CodexRadioGroup
                {
                    IsLoop = false,
                    SelectedValue = "first",
                    Items =
                    {
                        new CodexRadioGroupItem { Value = "first", Content = "First boundary" },
                        new CodexRadioGroupItem { Value = "middle", Content = "Middle" },
                        new CodexRadioGroupItem { Value = "last", Content = "Last boundary" }
                    }
                },
                new CodexRadioGroup
                {
                    Orientation = Orientation.Horizontal,
                    SelectedValue = "primary",
                    Items =
                    {
                        new CodexRadioGroupItem { Value = "primary", Content = "Primary route" },
                        new CodexRadioGroupItem { Value = "blocked", Content = "Disabled route", IsEnabled = false },
                        new CodexRadioGroupItem { Value = "fallback", Content = "Fallback route" }
                    }
                }
            }
        };
    }
}
