using Avalonia.Controls;
using Avalonia.Input;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System.Windows.Input;

public static class BadgeInteractionSample
{
    public static Control BuildBadgeInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Badge status: healthy."
        };
        var statusBadge = new CodexBadge
        {
            Content = "Healthy",
            Variant = CodexControlVariant.Success,
            StatusVariant = CodexControlVariant.Success,
            IsStatusVisible = true
        };
        var statusStep = 0;
        var rotateStatus = new CodexButton
        {
            Content = "Rotate status",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        rotateStatus.Click += (_, _) =>
        {
            statusStep = (statusStep + 1) % 3;
            statusBadge.Content = statusStep switch
            {
                1 => "Fallback",
                2 => "Blocked",
                _ => "Healthy"
            };
            statusBadge.Variant = statusStep switch
            {
                1 => CodexControlVariant.Warning,
                2 => CodexControlVariant.Destructive,
                _ => CodexControlVariant.Success
            };
            statusBadge.StatusVariant = statusBadge.Variant;
            status.Text = $"Badge status: {statusBadge.Content}.";
        };

        var count = 2;
        var countBadge = new CodexBadge
        {
            Content = "2 routes",
            Variant = CodexControlVariant.Secondary
        };
        var increment = new CodexButton
        {
            Content = "Add route",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        increment.Click += (_, _) =>
        {
            count++;
            countBadge.Content = count >= 9 ? "9+ routes" : $"{count} routes";
            status.Text = $"Route badge updated to {countBadge.Content}.";
        };

        var densityBadge = new CodexBadge
        {
            Content = "Medium",
            Size = CodexControlSize.Medium,
            Variant = CodexControlVariant.Outline
        };
        var toggleSize = new CodexButton
        {
            Content = "Toggle size",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        toggleSize.Click += (_, _) =>
        {
            densityBadge.Size = densityBadge.Size == CodexControlSize.Small
                ? CodexControlSize.Large
                : densityBadge.Size == CodexControlSize.Large
                    ? CodexControlSize.Medium
                    : CodexControlSize.Small;
            densityBadge.Content = densityBadge.Size.ToString();
            status.Text = $"Badge size changed to {densityBadge.Size}.";
        };

        var linkBadge = new CodexBadge
        {
            Content = "Open provider",
            Variant = CodexControlVariant.Link,
            IsInteractive = true,
            CommandParameter = "provider-route",
            Command = new SampleCommand(() => status.Text = "Badge command executed.")
        };
        linkBadge.Activated += (_, args) =>
        {
            status.Text = $"Link badge activated: source={args.Source}, parameter={args.CommandParameter}.";
        };

        var activateProgrammatically = new CodexButton
        {
            Content = "TryActivate",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        activateProgrammatically.Click += (_, _) => linkBadge.TryActivate();

        var activateByKeyboard = new CodexButton
        {
            Content = "Keyboard activate",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        activateByKeyboard.Click += (_, _) => linkBadge.TryHandleActivationKey(Key.Enter);

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                statusBadge,
                rotateStatus,
                countBadge,
                increment,
                densityBadge,
                toggleSize,
                linkBadge,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        activateProgrammatically,
                        activateByKeyboard
                    }
                },
                new CodexBadge
                {
                    Content = "Command blocked",
                    Variant = CodexControlVariant.Outline,
                    IsInteractive = true,
                    Command = new SampleCommand(() => status.Text = "Blocked badge executed.", () => false)
                },
                new CodexButton
                {
                    Content = "Deploy",
                    IsEnabled = false,
                    TrailingIcon = new CodexBadge
                    {
                        Content = "locked",
                        Size = CodexControlSize.Small,
                        Variant = CodexControlVariant.Secondary
                    }
                }
            }
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
