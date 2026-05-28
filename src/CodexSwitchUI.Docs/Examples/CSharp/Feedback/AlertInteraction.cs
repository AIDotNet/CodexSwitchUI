using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class AlertInteractionSample
{
    public static Control BuildAlertInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Action idle. Click the slotted action to update the alert."
        };
        var acknowledge = new CodexButton
        {
            Content = "Acknowledge",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        var liveAlert = new CodexAlert
        {
            Icon = "!",
            Title = "Fallback routing active",
            Description = "The action slot uses a normal button event path while the alert keeps its layout stable.",
            Variant = CodexControlVariant.Warning,
            Action = acknowledge
        };
        acknowledge.Click += (_, _) =>
        {
            liveAlert.Icon = "i";
            liveAlert.Title = "Fallback acknowledged";
            liveAlert.Description = "The slotted action clicked, variant changed, and slot classes stayed synchronized.";
            liveAlert.Variant = CodexControlVariant.Success;
            status.Text = $"Slotted action clicked; has-action={liveAlert.HasAction}.";
        };

        var reset = new CodexButton
        {
            Content = "Reset alert",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Outline
        };
        reset.Click += (_, _) =>
        {
            liveAlert.Icon = "!";
            liveAlert.Title = "Fallback routing active";
            liveAlert.Description = "The action slot uses a normal button event path while the alert keeps its layout stable.";
            liveAlert.Variant = CodexControlVariant.Warning;
            status.Text = "Alert reset to warning state.";
        };

        var inspect = new CodexButton
        {
            Content = "Inspect",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        inspect.Click += (_, _) =>
        {
            status.Text = "Rich content action clicked; content remained mounted.";
        };

        var dynamicAlert = new CodexAlert
        {
            Icon = "i",
            Title = "Dynamic description",
            Description = "Description is visible and contributes the has-description class.",
            Content = new CodexText
            {
                Role = CodexTextRole.Muted,
                Text = "Toggle the description to exercise slot presence changes."
            },
            Action = new CodexBadge
            {
                Content = "live",
                Variant = CodexControlVariant.Success,
                IsStatusVisible = true
            }
        };
        var toggleDescription = new CodexButton
        {
            Content = "Toggle description",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        toggleDescription.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(dynamicAlert.Description))
            {
                dynamicAlert.Description = "Description is visible and contributes the has-description class.";
            }
            else
            {
                dynamicAlert.Description = null;
            }

            status.Text = dynamicAlert.HasDescription
                ? "Description restored; alert added the description slot class."
                : "Description hidden; alert removed the description slot class.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                liveAlert,
                reset,
                new CodexAlert
                {
                    Icon = "i",
                    Title = "Usage threshold reached",
                    Description = "Structured child content can live below the description.",
                    Content = new StackPanel
                    {
                        Spacing = 6,
                        Children =
                        {
                            new CodexProgress { Value = 86, Variant = CodexControlVariant.Warning },
                            new CodexText { Role = CodexTextRole.Muted, Text = "86% of the monthly budget is currently reserved." }
                        }
                    },
                    Action = inspect
                },
                new CodexAlert
                {
                    Icon = "x",
                    Title = "Provider unavailable",
                    Description = "Retry remains disabled until the health check recovers.",
                    Variant = CodexControlVariant.Destructive,
                    Action = new CodexButton
                    {
                        Content = "Retry",
                        Size = CodexControlSize.Small,
                        Variant = CodexControlVariant.Outline,
                        IsEnabled = false
                    }
                },
                dynamicAlert,
                toggleDescription
            }
        };
    }
}
