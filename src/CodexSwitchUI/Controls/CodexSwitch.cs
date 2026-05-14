using Avalonia;
using Avalonia.Controls.Primitives;

namespace CodexSwitchUI.Controls;

public class CodexSwitch : ToggleButton
{
    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexSwitch, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexSwitch, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexSwitch()
    {
        IntentProperty.Changed.AddClassHandler<CodexSwitch>((toggle, _) => toggle.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexSwitch>((toggle, _) => toggle.SyncClasses());
    }

    public CodexSwitch()
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
