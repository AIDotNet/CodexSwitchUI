using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

namespace CodexSwitchUI.Controls;

public sealed class CodexCheckBoxCheckedStateChangedEventArgs(bool? oldValue, bool? newValue)
    : EventArgs
{
    public bool? OldValue { get; } = oldValue;

    public bool? NewValue { get; } = newValue;
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
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
        IsCheckedProperty.Changed.AddClassHandler<CodexCheckBox>((checkBox, args) => checkBox.OnCheckedStateChanged(args));
    }

    public CodexCheckBox()
    {
        SyncClasses();
    }

    public event EventHandler<CodexCheckBoxCheckedStateChangedEventArgs>? CheckedStateChanged;

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
        Classes.Set("state-checked", IsChecked == true);
        Classes.Set("state-unchecked", IsChecked == false);
        Classes.Set("state-indeterminate", IsChecked is null);
    }

    private void OnCheckedStateChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SyncClasses();

        var oldValue = ToCheckedState(args.OldValue);
        var newValue = ToCheckedState(args.NewValue);
        if (oldValue == newValue)
        {
            return;
        }

        CheckedStateChanged?.Invoke(this, new CodexCheckBoxCheckedStateChangedEventArgs(oldValue, newValue));
    }

    private static bool? ToCheckedState(object? value)
    {
        return value is bool checkedValue ? checkedValue : null;
    }
}
