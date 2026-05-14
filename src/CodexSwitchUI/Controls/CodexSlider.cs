using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

public class CodexSlider : Slider
{
    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexSlider, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexSlider, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexSlider()
    {
        IntentProperty.Changed.AddClassHandler<CodexSlider>((slider, _) => slider.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexSlider>((slider, _) => slider.SyncClasses());
    }

    public CodexSlider()
    {
        SyncClasses();
    }

    public CodexControlIntent Intent
    {
        get => GetValue(IntentProperty);
        set => SetValue(IntentProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
    }
}
