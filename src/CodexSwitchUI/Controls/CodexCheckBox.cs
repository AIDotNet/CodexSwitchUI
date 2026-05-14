using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

public class CodexCheckBox : CheckBox
{
    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexCheckBox, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexCheckBox, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexCheckBox()
    {
        IntentProperty.Changed.AddClassHandler<CodexCheckBox>((checkBox, _) => checkBox.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexCheckBox>((checkBox, _) => checkBox.SyncClasses());
    }

    public CodexCheckBox()
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
