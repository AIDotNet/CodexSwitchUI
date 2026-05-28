using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class KbdInteractionSample
{
    public static Control BuildKbdInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Shortcut: Cmd + K."
        };
        var first = new CodexKbd { Content = "Cmd", Size = CodexControlSize.Small };
        var second = new CodexKbd { Content = "K", Size = CodexControlSize.Small };
        var sequenceStep = 0;

        var switchSequence = new CodexButton
        {
            Content = "Switch shortcut",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        switchSequence.Click += (_, _) =>
        {
            sequenceStep = (sequenceStep + 1) % 3;
            switch (sequenceStep)
            {
                case 1:
                    first.Content = "Esc";
                    second.Content = null;
                    status.Text = "Shortcut: Esc.";
                    break;
                case 2:
                    first.Content = "Shift";
                    second.Content = "P";
                    status.Text = "Shortcut: Shift + P.";
                    break;
                default:
                    first.Content = "Cmd";
                    second.Content = "K";
                    status.Text = "Shortcut: Cmd + K.";
                    break;
            }
        };

        var toggleDensity = new CodexButton
        {
            Content = "Toggle density",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        toggleDensity.Click += (_, _) =>
        {
            first.Size = first.Size == CodexControlSize.Small ? CodexControlSize.Large : CodexControlSize.Small;
            second.Size = first.Size;
            status.Text = $"Shortcut density changed to {first.Size}.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                new CodexKbdGroup
                {
                    Size = CodexControlSize.Small,
                    Items = { first, second }
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { switchSequence, toggleDensity }
                },
                new CodexCommand
                {
                    Content = new CodexCommandList
                    {
                        Items =
                        {
                            new CodexCommandItem { Content = "Open command palette", Shortcut = "Cmd K", IsActive = true },
                            new CodexCommandItem { Content = "Switch provider", Shortcut = "Shift P" }
                        }
                    }
                },
                new CodexKbdGroup
                {
                    Size = CodexControlSize.Small,
                    Items =
                    {
                        new CodexKbd { Content = "Cmd", Size = CodexControlSize.Small },
                        new CodexKbd { Content = "K", Size = CodexControlSize.Small },
                        new CodexKbd { Content = "then", Size = CodexControlSize.Small },
                        new CodexKbd { Content = "P", Size = CodexControlSize.Small }
                    }
                },
                new CodexButton
                {
                    Content = "Run shortcut",
                    TrailingIcon = new CodexKbd { Content = "Enter", Size = CodexControlSize.Small },
                    IsEnabled = false
                }
            }
        };
    }
}
