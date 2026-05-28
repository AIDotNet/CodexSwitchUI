using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class RadioInteractionSample
{
    public static Control BuildRadioInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Fastest healthy provider is checked."
        };
        var fastest = new CodexRadio
        {
            GroupName = "provider-route",
            Content = "Fastest healthy provider",
            IsChecked = true
        };
        var lowestCost = new CodexRadio
        {
            GroupName = "provider-route",
            Content = "Lowest cost route"
        };
        var pinned = new CodexRadio
        {
            GroupName = "provider-route",
            Content = "Pinned provider",
            Intent = CodexControlIntent.Warning
        };

        fastest.Checked += (_, _) => status.Text = "Fastest healthy provider is checked.";
        lowestCost.Checked += (_, _) => status.Text = "Lowest cost route is checked.";
        pinned.Checked += (_, _) => status.Text = "Pinned provider is checked.";

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                fastest,
                lowestCost,
                pinned,
                status,
                new CodexRadio
                {
                    GroupName = "provider-route-disabled",
                    Content = "Locked checked",
                    IsChecked = true,
                    IsEnabled = false
                },
                new CodexRadio
                {
                    GroupName = "provider-route-disabled",
                    Content = "Locked unchecked",
                    Size = CodexControlSize.Small,
                    IsEnabled = false
                }
            }
        };
    }
}
