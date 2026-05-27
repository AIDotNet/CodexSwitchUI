using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace CodexSwitchUI.Controls;

public sealed class CodexSwitchCheckedChangedEventArgs(bool oldValue, bool newValue)
    : EventArgs
{
    public bool OldValue { get; } = oldValue;

    public bool NewValue { get; } = newValue;
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexSwitch : ToggleButton
{
    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexSwitch, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexSwitch, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexSwitch, bool>(nameof(HasContent));

    static CodexSwitch()
    {
        IntentProperty.Changed.AddClassHandler<CodexSwitch>((toggle, _) => toggle.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexSwitch>((toggle, _) => toggle.SyncClasses());
        ContentControl.ContentProperty.Changed.AddClassHandler<CodexSwitch>((toggle, _) => toggle.SyncContentState());
        IsCheckedProperty.Changed.AddClassHandler<CodexSwitch>((toggle, args) => toggle.OnCheckedChanged(args));
    }

    public CodexSwitch()
    {
        SyncClasses();
        SyncContentState();
    }

    public event EventHandler<CodexSwitchCheckedChangedEventArgs>? CheckedChanged;

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

    public bool HasContent => GetValue(HasContentProperty);

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

    private void SyncContentState()
    {
        SetValue(HasContentProperty, Content is string text ? !string.IsNullOrWhiteSpace(text) : Content is not null);
    }

    private void OnCheckedChanged(AvaloniaPropertyChangedEventArgs args)
    {
        var oldValue = ToCheckedValue(args.OldValue);
        var newValue = ToCheckedValue(args.NewValue);
        if (oldValue == newValue)
        {
            return;
        }

        CheckedChanged?.Invoke(this, new CodexSwitchCheckedChangedEventArgs(oldValue, newValue));
    }

    private static bool ToCheckedValue(object? value)
    {
        return value is bool checkedValue && checkedValue;
    }
}
