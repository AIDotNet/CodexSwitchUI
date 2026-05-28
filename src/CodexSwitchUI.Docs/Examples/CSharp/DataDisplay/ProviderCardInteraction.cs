using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System.Windows.Input;

public static class ProviderCardInteractionSample
{
    public static Control BuildProviderCardInteractionPreview()
    {
        var selected = Muted("Selected: OpenAI (initial)");
        var openAi = ProviderCard("OpenAI", "Primary Responses route", true);
        var claude = ProviderCard("Claude", "Fallback route", false);
        var gemini = ProviderCard("Gemini", "Cost-aware route", false);
        var cards = new[]
        {
            (Card: openAi, Name: "OpenAI"),
            (Card: claude, Name: "Claude"),
            (Card: gemini, Name: "Gemini")
        };

        foreach (var item in cards)
        {
            item.Card.Selected += (_, args) =>
            {
                foreach (var sibling in cards)
                {
                    sibling.Card.Meta = ProviderCardMeta(ReferenceEquals(sibling.Card, item.Card));
                }

                selected.Text = $"Selected: {item.Name}, source={args.Source}.";
            };
        }

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                selected,
                openAi,
                claude,
                gemini,
                ProviderCard("Dragging route", "Reordering provider priority", false, isDragging: true),
                new CodexProviderCard
                {
                    Leading = new CodexBadge { Content = "Default", Variant = CodexControlVariant.Secondary },
                    Header = Text("Local proxy", CodexTextRole.Body),
                    Meta = ProviderCardMeta(false),
                    Description = "Action slot remains mounted during click feedback.",
                    Icon = "L",
                    Status = new CodexBadge { Content = "Healthy", Variant = CodexControlVariant.Success },
                    Usage = new CodexMetric { Label = "Tokens", Value = Text("9.8K", CodexTextRole.Body) },
                    Actions = new CodexIconButton { Content = "...", Variant = CodexControlVariant.Ghost }
                },
                ProviderCard("Locked route", "Credentials are required before selection", false, isEnabled: false),
                new CodexProviderCard
                {
                    Header = Text("Staged route", CodexTextRole.Body),
                    Meta = new CodexBadge { Content = "Blocked", Variant = CodexControlVariant.Warning },
                    Description = "Validation must pass before this provider row can become active.",
                    Icon = "S",
                    Status = new CodexBadge { Content = "Waiting", Variant = CodexControlVariant.Warning },
                    Usage = new CodexMetric { Label = "Tokens", Value = Text("0", CodexTextRole.Body) },
                    Command = new SampleCommand(() => selected.Text = "Selected: Staged route", () => false)
                }
            }
        };
    }

    private static CodexProviderCard ProviderCard(
        string name,
        string description,
        bool isActive,
        bool isDragging = false,
        bool isEnabled = true)
    {
        return new CodexProviderCard
        {
            Header = Text(name, CodexTextRole.Body),
            Meta = ProviderCardMeta(isActive),
            Description = description,
            Icon = name[..1],
            Status = new CodexBadge
            {
                Content = isActive ? "Healthy" : "Ready",
                Variant = isActive ? CodexControlVariant.Success : CodexControlVariant.Secondary
            },
            Usage = new CodexMetric
            {
                Label = "Tokens",
                Value = Text(isActive ? "42.7K" : "7.1K", CodexTextRole.Body)
            },
            IsActive = isActive,
            IsDragging = isDragging,
            IsEnabled = isEnabled
        };
    }

    private static CodexBadge ProviderCardMeta(bool active)
    {
        return new CodexBadge
        {
            Content = active ? "Active" : "Ready",
            Variant = active ? CodexControlVariant.Success : CodexControlVariant.Secondary
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

    private sealed class SampleCommand(Action execute, Func<bool>? canExecute = null) : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => execute();
    }
}
