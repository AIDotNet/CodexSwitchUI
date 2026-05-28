using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;
using System.Windows.Input;

public static class SegmentedControlInteractionSample
{
    public static Control BuildSegmentedControlInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "ValueChanged reports old value, new value, and source metadata."
        };
        var segmented = new CodexSegmentedControl
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            SelectedValue = "code",
            Content = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new CodexSegmentedButton { Content = "Preview", Value = "preview" },
                    new CodexSegmentedButton { Content = "Code", Value = "code", IsSelected = true },
                    new CodexSegmentedButton { Content = "Events", Value = "events" }
                }
            }
        };
        segmented.ValueChanged += (_, args) =>
        {
            status.Text = $"ValueChanged: {args.OldValue ?? "none"} -> {args.NewValue ?? "none"} (source={args.Source}).";
        };

        var selectPreview = new CodexButton
        {
            Content = "Select preview",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        selectPreview.Click += (_, _) => segmented.SelectedValue = "preview";

        var selectEvents = new CodexButton
        {
            Content = "Select events",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        selectEvents.Click += (_, _) => segmented.SelectedValue = "events";

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                segmented,
                status,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { selectPreview, selectEvents }
                },
                new CodexSegmentedControl
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new CodexSegmentedButton { Content = "Light", Value = "light" },
                            new CodexSegmentedButton { Content = "Dark", Value = "dark", IsSelected = true },
                            new CodexSegmentedButton { Content = "System", Value = "system", IsEnabled = false }
                        }
                    }
                },
                new CodexSegmentedControl
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new CodexSegmentedButton { Content = "Auto", Value = "auto", IsSelected = true },
                            new CodexSegmentedButton
                            {
                                Content = "Manual",
                                Value = "manual",
                                Command = new SampleCommand(() => status.Text = "Command-backed segment executed.", () => false)
                            },
                            new CodexSegmentedButton { Content = "Off", Value = "off", IsEnabled = false }
                        }
                    }
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
