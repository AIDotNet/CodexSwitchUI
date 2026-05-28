using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class InputOtpInteractionSample
{
    public static Control BuildInputOtpInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "TryInsertText filters input and advances ActiveIndex."
        };
        var input = BuildInputOtp("12", 6, CodexInputOtp.DigitsPattern, activeIndex: 2);

        var paste = new CodexButton
        {
            Content = "Paste code",
            Size = CodexControlSize.Small
        };
        paste.Click += (_, _) =>
        {
            input.Clear();
            input.TryInsertText("491826");
            status.Text = $"Pasted: {input.Text}, complete={input.IsComplete}.";
        };

        var focusSlot = new CodexButton
        {
            Content = "Focus slot 3",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        focusSlot.Click += (_, _) =>
        {
            input.FocusSlot(2);
            status.Text = $"ActiveIndex moved to {input.ActiveIndex}.";
        };

        var clear = new CodexButton
        {
            Content = "Clear",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        clear.Click += (_, _) =>
        {
            input.Clear();
            status.Text = "Code cleared and ActiveIndex reset.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                input,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { paste, focusSlot, clear }
                },
                BuildInputOtp("A9C4", 6, CodexInputOtp.DigitsAndLettersPattern, CodexControlIntent.Warning),
                BuildInputOtp("123", 6, CodexInputOtp.DigitsPattern, isEnabled: false)
            }
        };
    }

    private static CodexInputOtp BuildInputOtp(
        string text,
        int maxLength,
        string? pattern,
        CodexControlIntent intent = CodexControlIntent.Default,
        bool isEnabled = true,
        int activeIndex = 0)
    {
        var input = new CodexInputOtp
        {
            Text = text,
            MaxLength = maxLength,
            Pattern = pattern,
            Intent = intent,
            IsEnabled = isEnabled,
            ActiveIndex = activeIndex
        };

        var first = new CodexInputOtpGroup();
        var second = new CodexInputOtpGroup();
        for (var index = 0; index < maxLength; index++)
        {
            var slot = new CodexInputOtpSlot { Index = index };
            if (index < 3)
            {
                first.Items.Add(slot);
            }
            else
            {
                second.Items.Add(slot);
            }
        }

        input.Items.Add(first);
        input.Items.Add(new CodexInputOtpSeparator());
        input.Items.Add(second);
        return input;
    }
}
