using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

namespace CodexSwitchUI.Controls;

public enum CodexCheckBoxCheckedStateChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public sealed class CodexCheckBoxCheckedStateChangedEventArgs(
    bool? oldValue,
    bool? newValue,
    CodexCheckBoxCheckedStateChangeSource source = CodexCheckBoxCheckedStateChangeSource.Programmatic)
    : EventArgs
{
    public bool? OldValue { get; } = oldValue;

    public bool? NewValue { get; } = newValue;

    public CodexCheckBoxCheckedStateChangeSource Source { get; } = source;
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexCheckBox : CheckBox
{
    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexCheckBox, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexCheckBox, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    private CodexCheckBoxCheckedStateChangeSource? _pendingCheckedStateChangeSource;

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

    internal bool TryHandleActivationKey(Key key)
    {
        if (key is not Key.Space)
        {
            return false;
        }

        if (!IsEnabled)
        {
            return true;
        }

        _ = ToggleCheckedState(CodexCheckBoxCheckedStateChangeSource.Keyboard);
        return true;
    }

    internal bool SetCheckedState(bool? checkedState, CodexCheckBoxCheckedStateChangeSource source)
    {
        if (!IsEnabled || ToCheckedState(IsChecked) == checkedState)
        {
            return false;
        }

        RunWithCheckedStateChangeSource(source, () => IsChecked = checkedState);
        return true;
    }

    internal bool ToggleCheckedState(CodexCheckBoxCheckedStateChangeSource source)
    {
        return SetCheckedState(NextCheckedState(IsChecked, IsThreeState), source);
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
        if (IsEnabled
            && e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            _pendingCheckedStateChangeSource = CodexCheckBoxCheckedStateChangeSource.Pointer;
        }

        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        try
        {
            base.OnPointerReleased(e);
        }
        finally
        {
            _pendingCheckedStateChangeSource = null;
        }
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        _pendingCheckedStateChangeSource = null;
        base.OnPointerCaptureLost(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (TryHandleActivationKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
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

        var source = _pendingCheckedStateChangeSource ?? CodexCheckBoxCheckedStateChangeSource.Programmatic;
        CheckedStateChanged?.Invoke(this, new CodexCheckBoxCheckedStateChangedEventArgs(oldValue, newValue, source));
    }

    private void RunWithCheckedStateChangeSource(CodexCheckBoxCheckedStateChangeSource source, Action action)
    {
        var previousSource = _pendingCheckedStateChangeSource;
        _pendingCheckedStateChangeSource = source;
        try
        {
            action();
        }
        finally
        {
            _pendingCheckedStateChangeSource = previousSource;
        }
    }

    private static bool? ToCheckedState(object? value)
    {
        return value is bool checkedValue ? checkedValue : null;
    }

    private static bool? NextCheckedState(bool? value, bool isThreeState)
    {
        return value switch
        {
            true => isThreeState ? null : false,
            false => true,
            _ => false
        };
    }
}
