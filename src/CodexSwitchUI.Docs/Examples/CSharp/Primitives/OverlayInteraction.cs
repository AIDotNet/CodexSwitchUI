using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System.Windows.Input;

public static class OverlayInteractionSample
{
    public static Control BuildOverlayPrimitiveInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "Overlay is closed by default. Open it to test Escape and outside-pointer dismissal."
        };
        var dismiss = new CodexButton
        {
            Content = "Dismiss",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        var overlay = new CodexOverlay
        {
            IsOpen = false,
            CloseOnEscape = true,
            DismissOnOutsidePointer = true,
            IsScrimVisible = true,
            ScrimOpacity = 0.44,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            DismissCommand = new SampleCommand(() => status.Text = "DismissCommand fired and the overlay moved to closed state."),
            Content = new CodexCard
            {
                Width = 260,
                Title = "Dismissible layer",
                Description = "Buttons, Escape, and outside pointer converge through the overlay dismiss command.",
                Content = dismiss
            }
        };
        dismiss.Click += (_, _) => overlay.Dismiss();

        var reopen = new CodexButton
        {
            Content = "Open",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        reopen.Click += (_, _) =>
        {
            overlay.IsOpen = true;
            status.Text = "Overlay opened without rebuilding content.";
        };

        var toggleScrim = new CodexButton
        {
            Content = "Toggle scrim",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        toggleScrim.Click += (_, _) =>
        {
            overlay.IsScrimVisible = !overlay.IsScrimVisible;
            overlay.ScrimOpacity = overlay.IsScrimVisible ? 0.44 : 0;
            status.Text = overlay.IsScrimVisible
                ? "Scrim restored and opacity transitions through the overlay token."
                : "Scrim hidden while content remains mounted.";
        };

        var manualPolicy = new CodexButton
        {
            Content = "Manual policy",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        manualPolicy.Click += (_, _) =>
        {
            overlay.CloseOnEscape = !overlay.CloseOnEscape;
            overlay.DismissOnOutsidePointer = overlay.CloseOnEscape;
            status.Text = overlay.CloseOnEscape
                ? "Escape and outside pointer dismissal are enabled."
                : "Escape and outside pointer dismissal are disabled for manual workflows.";
        };

        var manualOverlay = new CodexOverlay
        {
            IsOpen = false,
            CloseOnEscape = false,
            DismissOnOutsidePointer = false,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Content = new CodexCard
            {
                Width = 230,
                Title = "Manual close",
                Description = "Close policy is owned by the host.",
                Content = new CodexButton { Content = "Apply", Size = CodexControlSize.Small }
            }
        };

        var noScrim = new CodexOverlay
        {
            IsOpen = false,
            IsScrimVisible = false,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Content = new CodexCard
            {
                Width = 220,
                Title = "Inline layer",
                Description = "Scrim slot removed.",
                Content = new CodexButton { Content = "Continue", Size = CodexControlSize.Small }
            }
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                new Grid { Height = 188, Children = { overlay } },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        reopen,
                        toggleScrim,
                        manualPolicy
                    }
                },
                new Grid { Height = 160, Children = { manualOverlay } },
                new Grid { Height = 150, Children = { noScrim } },
                new Grid
                {
                    Height = 150,
                    Children =
                    {
                        new CodexOverlay
                        {
                            IsOpen = false,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Content = new CodexCard
                            {
                                Width = 220,
                                Title = "Closed",
                                Description = "Opacity transitions to zero.",
                                Content = new CodexButton { Content = "Hidden", Size = CodexControlSize.Small }
                            }
                        }
                    }
                }
            }
        };
    }

    private sealed class SampleCommand(Action execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => execute();
    }
}
