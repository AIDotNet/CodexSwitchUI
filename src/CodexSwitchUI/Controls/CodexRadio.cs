using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

public class CodexRadio : RadioButton
{
    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexRadio, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexRadio, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexRadio()
    {
        IntentProperty.Changed.AddClassHandler<CodexRadio>((radio, _) => radio.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexRadio>((radio, _) => radio.SyncClasses());
    }

    public CodexRadio()
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
