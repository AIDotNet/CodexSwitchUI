using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CodexSwitchUI.Primitives;

public class CodexFocusRing : ContentControl
{
    public static readonly StyledProperty<IBrush?> RingBrushProperty =
        AvaloniaProperty.Register<CodexFocusRing, IBrush?>(nameof(RingBrush));

    public static readonly StyledProperty<Thickness> RingThicknessProperty =
        AvaloniaProperty.Register<CodexFocusRing, Thickness>(nameof(RingThickness), new Thickness(2));

    public static readonly StyledProperty<Thickness> RingOffsetProperty =
        AvaloniaProperty.Register<CodexFocusRing, Thickness>(nameof(RingOffset), new Thickness(2));

    public static readonly StyledProperty<bool> IsRingVisibleProperty =
        AvaloniaProperty.Register<CodexFocusRing, bool>(nameof(IsRingVisible), true);

    public IBrush? RingBrush
    {
        get => GetValue(RingBrushProperty);
        set => SetValue(RingBrushProperty, value);
    }

    public Thickness RingThickness
    {
        get => GetValue(RingThicknessProperty);
        set => SetValue(RingThicknessProperty, value);
    }

    public Thickness RingOffset
    {
        get => GetValue(RingOffsetProperty);
        set => SetValue(RingOffsetProperty, value);
    }

    public bool IsRingVisible
    {
        get => GetValue(IsRingVisibleProperty);
        set => SetValue(IsRingVisibleProperty, value);
    }
}
