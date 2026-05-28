using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;
using System.Windows.Input;

public static class ButtonInteractionSample
{
    public static Control BuildButtonInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Pointer release, Enter, and Space share the Button activation path."
        };
        var activationCount = 0;

        var save = new CodexButton
        {
            Content = "Save changes",
            LeadingIcon = "+",
            TrailingIcon = new CodexKbd { Content = "Enter", Size = CodexControlSize.Small }
        };
        save.Click += (_, _) =>
        {
            activationCount++;
            status.Text = $"Click: save activated {activationCount} time(s).";
        };

        var command = new SampleCommand(
            () => status.Text = "Command: CanExecute=true action ran.",
            () => true);

        var commandButton = new CodexButton
        {
            Content = "Command action",
            Variant = CodexControlVariant.Secondary,
            Command = command
        };

        var loading = new CodexButton
        {
            Content = "Saving",
            IsLoading = true,
            LoadingContent = "Saving changes"
        };
        loading.Click += (_, _) => status.Text = "This should not run while IsLoading=true.";

        var toggleLoading = new CodexButton
        {
            Content = "Toggle loading",
            Variant = CodexControlVariant.Ghost,
            Size = CodexControlSize.Small
        };
        toggleLoading.Click += (_, _) =>
        {
            loading.IsLoading = !loading.IsLoading;
            status.Text = loading.IsLoading
                ? "Loading enabled: click and command activation are suppressed."
                : "Loading cleared: the button can activate again.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        save,
                        commandButton,
                        loading,
                        toggleLoading
                    }
                },
                new CodexButton
                {
                    Content = "Command blocked",
                    Command = new SampleCommand(() => status.Text = "Blocked command ran unexpectedly.", () => false),
                    Variant = CodexControlVariant.Outline
                },
                new CodexButton
                {
                    Content = "Disabled destructive",
                    Variant = CodexControlVariant.Destructive,
                    IsEnabled = false
                }
            }
        };
    }

    private sealed class SampleCommand(Action execute, Func<bool> canExecute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => canExecute();

        public void Execute(object? parameter) => execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
