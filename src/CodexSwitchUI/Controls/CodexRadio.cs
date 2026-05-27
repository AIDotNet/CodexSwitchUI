using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

namespace CodexSwitchUI.Controls;

[PseudoClasses(CodexFocusVisible.PseudoClass)]
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
    }
}
