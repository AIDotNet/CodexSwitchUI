using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System.Windows.Input;

public static class ItemInteractionSample
{
    public static Control BuildItemInteractionPreview()
    {
        var status = Muted("Activate a row or nested action.");
        var selectedCount = 0;
        var route = ItemRow(
            "Keyboard activation",
            "Enter, Space, and pointer release share the same command path.",
            "K",
            CodexControlVariant.Secondary,
            selected: true);
        route.ActivateCommandParameter = "route";
        route.Activated += (_, args) =>
        {
            selectedCount++;
            route.Footer = $"Activated {selectedCount} time(s), source={args.Source}.";
            status.Text = $"Activated: source={args.Source}, parameter={args.CommandParameter ?? "none"}.";
        };

        var nestedAction = new CodexButton
        {
            Content = "Configure",
            Size = CodexControlSize.Small
        };
        nestedAction.Click += (_, _) => status.Text = "Nested action clicked without replacing item layout.";

        var blocked = ItemRow(
            "Command blocked",
            "CanExecute=false removes row activation while media and copy stay mounted.",
            "B",
            CodexControlVariant.Warning);
        blocked.ActivateCommand = new SampleCommand(() => status.Text = "Blocked item command executed.", () => false);

        var activate = new CodexButton
        {
            Content = "Enter",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        activate.Click += (_, _) => route.TryHandleActivationKey(Key.Enter);

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                route,
                activate,
                ItemRow("Action slot", "Configure is a nested button.", "A", CodexControlVariant.Secondary, action: nestedAction),
                blocked,
                new CodexItemGroup
                {
                    Variant = CodexControlVariant.Secondary,
                    IsInset = true,
                    Items =
                    {
                        new CodexItem { Title = "Grouped row", Description = "Shared group chrome." },
                        new CodexItemSeparator(),
                        new CodexItem { Title = "Grouped row", Description = "Separator stays outside row activation." }
                    }
                }
            }
        };
    }

    private static CodexItem ItemRow(
        string title,
        string description,
        string initials,
        CodexControlVariant mediaVariant,
        bool selected = false,
        object? action = null)
    {
        return new CodexItem
        {
            Title = title,
            Description = description,
            Media = new CodexItemMedia { Content = initials, Variant = mediaVariant },
            Actions = action ?? new CodexButton { Content = "Open", Size = CodexControlSize.Small, Variant = CodexControlVariant.Secondary },
            IsInteractive = true,
            IsSelected = selected
        };
    }

    private static CodexText Muted(string text)
    {
        return new CodexText
        {
            Role = CodexTextRole.Muted,
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
