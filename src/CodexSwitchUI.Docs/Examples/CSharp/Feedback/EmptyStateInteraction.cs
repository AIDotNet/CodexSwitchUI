using Avalonia.Controls;
using Avalonia.Interactivity;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;
using System.Windows.Input;

public static class EmptyStateInteractionSample
{
    public static Control BuildEmptyStateInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Primary and secondary actions route through the component."
        };
        var primaryCount = 0;
        var secondaryCount = 0;

        var emptyState = new CodexEmptyState
        {
            Title = "Action requested",
            Description = "ActionRequested and SecondaryActionRequested are raised from the same guarded action path.",
            Icon = "i",
            Action = "Refresh",
            SecondaryAction = "Clear filter",
            ActionCommand = new SampleCommand(() => primaryCount++),
            SecondaryActionCommand = new SampleCommand(() => secondaryCount++),
            MinHeight = 190
        };
        emptyState.ActionRequested += (_, _) =>
        {
            status.Text = $"ActionRequested: primary={primaryCount}, secondary={secondaryCount}.";
        };
        emptyState.SecondaryActionRequested += (_, _) =>
        {
            status.Text = $"SecondaryActionRequested: primary={primaryCount}, secondary={secondaryCount}.";
        };

        var runPrimary = new CodexButton
        {
            Content = "Run primary",
            Size = CodexControlSize.Small
        };
        runPrimary.Click += (_, _) =>
        {
            if (!emptyState.TryExecuteAction())
            {
                status.Text = "Primary action skipped by loading, disabled, or CanExecute=false.";
            }
        };

        var runSecondary = new CodexButton
        {
            Content = "Run secondary",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        runSecondary.Click += (_, _) =>
        {
            if (!emptyState.TryExecuteSecondaryAction())
            {
                status.Text = "Secondary action skipped by loading, disabled, or CanExecute=false.";
            }
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                emptyState,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { runPrimary, runSecondary }
                },
                new CodexEmptyState
                {
                    Title = "Loading gate",
                    Description = "IsLoading suppresses both action paths and lowers surface opacity.",
                    Icon = "...",
                    IsLoading = true,
                    Action = "Refreshing",
                    SecondaryAction = "Cancel",
                    MinHeight = 170
                },
                new CodexEmptyState
                {
                    Title = "Command blocked",
                    Description = "CanExecute=false disables host actions while the empty surface stays mounted.",
                    Icon = "?",
                    Variant = CodexControlVariant.Warning,
                    Action = "Inspect",
                    SecondaryAction = "Ignore",
                    ActionCommand = new SampleCommand(() => { }, () => false),
                    SecondaryActionCommand = new SampleCommand(() => { }, () => false),
                    MinHeight = 170
                }
            }
        };
    }

    private sealed class SampleCommand(Action execute, Func<bool>? canExecute = null) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
