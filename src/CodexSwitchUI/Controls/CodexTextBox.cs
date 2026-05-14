using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

public class CodexTextBox : TextBox
{
    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexTextBox, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexTextBox, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexTextBox()
    {
        IntentProperty.Changed.AddClassHandler<CodexTextBox>((textBox, _) => textBox.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexTextBox>((textBox, _) => textBox.SyncClasses());
        IsReadOnlyProperty.Changed.AddClassHandler<CodexTextBox>((textBox, _) => textBox.SyncClasses());
    }

    public CodexTextBox()
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
        Classes.Set("is-read-only", IsReadOnly);
    }
}
