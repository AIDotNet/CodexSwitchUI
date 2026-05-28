using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class SwitchInteractionSample
{
    public static Control BuildSwitchInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Streaming is on (source=Programmatic)."
        };
        var streaming = new CodexSwitch
        {
            Content = "Streaming",
            IsChecked = true
        };
        streaming.CheckedChanged += (_, args) =>
        {
            status.Text = $"Streaming changed from {Label(args.OldValue)} to {Label(args.NewValue)} (source={args.Source}).";
        };

        var fallback = new CodexSwitch { Content = "Fallback routing" };
        fallback.CheckedChanged += (_, args) =>
        {
            status.Text = args.NewValue
                ? $"Fallback routing enabled (source={args.Source})."
                : $"Fallback routing disabled (source={args.Source}).";
        };

        var turnOff = new CodexButton
        {
            Content = "Turn off",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        turnOff.Click += (_, _) => streaming.IsChecked = false;

        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                streaming,
                fallback,
                status,
                turnOff,
                new CodexSwitch
                {
                    Content = "Large success",
                    Size = CodexControlSize.Large,
                    Intent = CodexControlIntent.Success,
                    IsChecked = true
                },
                new CodexSwitch
                {
                    Content = "Locked on",
                    IsChecked = true,
                    IsEnabled = false
                }
            }
        };
    }

    private static string Label(bool value)
    {
        return value ? "on" : "off";
    }
}
