using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class SliderInteractionSample
{
    public static Control BuildSliderInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "ValueChanging and ValueCommitted update this status."
        };
        var slider = new CodexSlider
        {
            Minimum = 0,
            Maximum = 100,
            Value = 48,
            TickFrequency = 1,
            MinWidth = 320
        };
        slider.ValueChanging += (_, args) =>
        {
            status.Text = $"ValueChanging: {args.OldValue:0.##} -> {args.NewValue:0.##}.";
        };
        slider.ValueCommitted += (_, args) =>
        {
            status.Text = $"ValueCommitted ({args.Source}): {args.OldValue:0.##} -> {args.NewValue:0.##}.";
        };

        var setLow = new CodexButton
        {
            Content = "Set 24",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        setLow.Click += (_, _) => slider.Value = 24;

        var setHigh = new CodexButton
        {
            Content = "Set 76",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        setHigh.Click += (_, _) => slider.Value = 76;

        var commit = new CodexButton
        {
            Content = "Commit",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        commit.Click += (_, _) =>
        {
            if (!slider.CommitValue())
            {
                status.Text = $"ValueCommitted skipped: {slider.Value:0.##} is already committed.";
            }
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                slider,
                status,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { setLow, setHigh, commit }
                },
                new CodexSlider
                {
                    Minimum = 0,
                    Maximum = 2,
                    Value = 0.8,
                    TickFrequency = 0.1,
                    Size = CodexControlSize.Large,
                    Intent = CodexControlIntent.Success,
                    MinWidth = 320
                },
                new CodexSlider
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = 66,
                    Orientation = Orientation.Vertical,
                    Intent = CodexControlIntent.Warning,
                    Height = 130
                },
                new CodexSlider
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = 72,
                    IsEnabled = false,
                    MinWidth = 320
                }
            }
        };
    }
}
