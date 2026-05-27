using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

namespace CodexSwitchUI.Controls;

[PseudoClasses(CodexFocusVisible.PseudoClass)]
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

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, CodexFocusVisible.FromFocusChange(e));
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        base.OnPointerPressed(e);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("is-read-only", IsReadOnly);
    }
}
