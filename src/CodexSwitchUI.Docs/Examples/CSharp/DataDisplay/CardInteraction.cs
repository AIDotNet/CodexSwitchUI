using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class CardInteractionSample
{
    public static Control BuildCardInteractionPreview()
    {
        var status = Muted("Click the interactive card or footer action.");
        var actionCount = 0;
        var card = new CodexCard
        {
            IsInteractive = true,
            Title = "Fallback routing",
            Description = "Interactive card surface",
            Content = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    Text("Claude Sonnet is the current fallback.", CodexTextRole.Body),
                    new CodexBadge { Content = "ready", Variant = CodexControlVariant.Success, IsStatusVisible = true }
                }
            }
        };
        card.PointerReleased += (_, args) =>
        {
            if (args.GetCurrentPoint(card).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonReleased)
            {
                return;
            }

            actionCount++;
            card.Title = "Fallback selected";
            card.Description = $"Card selected {actionCount} time(s).";
            status.Text = "Interactive card pointer release updated header text.";
            args.Handled = true;
        };

        var configure = new CodexButton
        {
            Content = "Configure",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        configure.Click += (_, _) =>
        {
            actionCount++;
            card.Footer = new CodexBadge
            {
                Content = $"configured {actionCount}",
                Variant = CodexControlVariant.Secondary
            };
            status.Text = "Footer action clicked; footer slot content updated.";
        };
        card.Footer = configure;

        var dynamicCard = new CodexCard
        {
            Title = "Dynamic slots",
            Description = "Content and footer can be removed without replacing the card.",
            Content = Muted("Content slot is currently visible."),
            Footer = new CodexBadge { Content = "visible", Variant = CodexControlVariant.Secondary }
        };
        var toggleSlots = new CodexButton
        {
            Content = "Toggle slots",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        toggleSlots.Click += (_, _) =>
        {
            var hide = dynamicCard.Content is not null;
            dynamicCard.Content = hide ? null : Muted("Content slot is currently visible.");
            dynamicCard.Footer = hide ? null : new CodexBadge { Content = "visible", Variant = CodexControlVariant.Secondary };
            status.Text = hide
                ? "Card content and footer slots hidden."
                : "Card content and footer slots restored.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                card,
                dynamicCard,
                toggleSlots,
                new CodexCard
                {
                    Title = "Paused provider",
                    Description = "The footer action is visible but locked.",
                    Content = Muted("Disabled hosts keep the same layout contract."),
                    Footer = new CodexButton
                    {
                        Content = "Resume",
                        Size = CodexControlSize.Small,
                        Variant = CodexControlVariant.Secondary,
                        IsEnabled = false
                    }
                }
            }
        };
    }

    private static CodexText Muted(string text)
    {
        return Text(text, CodexTextRole.Muted);
    }

    private static CodexText Text(string text, CodexTextRole role)
    {
        return new CodexText
        {
            Role = role,
            Text = text
        };
    }
}
