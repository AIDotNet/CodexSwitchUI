using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CodexSwitchUI.Primitives;

public class CodexOverlay : ContentControl
{
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexOverlay, bool>(nameof(IsOpen), true);

    public static readonly StyledProperty<IBrush?> ScrimBrushProperty =
        AvaloniaProperty.Register<CodexOverlay, IBrush?>(nameof(ScrimBrush));

    public static readonly StyledProperty<double> ScrimOpacityProperty =
        AvaloniaProperty.Register<CodexOverlay, double>(nameof(ScrimOpacity), 0.8);

    public static readonly StyledProperty<bool> IsScrimVisibleProperty =
        AvaloniaProperty.Register<CodexOverlay, bool>(nameof(IsScrimVisible), true);

    static CodexOverlay()
    {
        IsOpenProperty.Changed.AddClassHandler<CodexOverlay>((overlay, _) => overlay.SyncOpenClasses());
        IsScrimVisibleProperty.Changed.AddClassHandler<CodexOverlay>((overlay, _) => overlay.SyncOpenClasses());
    }

    public CodexOverlay()
    {
        SyncOpenClasses();
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public IBrush? ScrimBrush
    {
        get => GetValue(ScrimBrushProperty);
        set => SetValue(ScrimBrushProperty, value);
    }

    public double ScrimOpacity
    {
        get => GetValue(ScrimOpacityProperty);
        set => SetValue(ScrimOpacityProperty, value);
    }

    public bool IsScrimVisible
    {
        get => GetValue(IsScrimVisibleProperty);
        set => SetValue(IsScrimVisibleProperty, value);
    }

    private void SyncOpenClasses()
    {
        Classes.Set("is-open", IsOpen);
        Classes.Set("is-closed", !IsOpen);
        Classes.Set("has-scrim", IsScrimVisible);
    }
}
