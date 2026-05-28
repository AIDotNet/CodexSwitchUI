using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace CodexSwitchUI.Controls;

public enum CodexSwitchCheckedChangeSource
{
    Programmatic,
    Pointer,
    Keyboard
}

public sealed class CodexSwitchCheckedChangedEventArgs(
    bool oldValue,
    bool newValue,
    CodexSwitchCheckedChangeSource source = CodexSwitchCheckedChangeSource.Programmatic)
    : EventArgs
{
    public bool OldValue { get; } = oldValue;

    public bool NewValue { get; } = newValue;

    public CodexSwitchCheckedChangeSource Source { get; } = source;
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

    private CodexSwitchCheckedChangeSource? _pendingCheckedChangeSource;

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

    internal bool TryHandleActivationKey(Key key)
    {
        if (key is not (Key.Enter or Key.Space))
        {
            return false;
        }

        if (!IsEnabled)
        {
            return true;
        }

        _ = ToggleChecked(CodexSwitchCheckedChangeSource.Keyboard);
        return true;
    }

    internal bool SetChecked(bool isChecked, CodexSwitchCheckedChangeSource source)
    {
        if (!IsEnabled || ToCheckedValue(IsChecked) == isChecked)
        {
            return false;
        }

        RunWithCheckedChangeSource(source, () => IsChecked = isChecked);
        return true;
    }

    internal bool ToggleChecked(CodexSwitchCheckedChangeSource source)
    {
        return SetChecked(!ToCheckedValue(IsChecked), source);
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
            _pendingCheckedChangeSource = CodexSwitchCheckedChangeSource.Pointer;
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
            _pendingCheckedChangeSource = null;
        }
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        _pendingCheckedChangeSource = null;
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

        var source = _pendingCheckedChangeSource ?? CodexSwitchCheckedChangeSource.Programmatic;
        CheckedChanged?.Invoke(this, new CodexSwitchCheckedChangedEventArgs(oldValue, newValue, source));
    }

    private void RunWithCheckedChangeSource(CodexSwitchCheckedChangeSource source, Action action)
    {
        var previousSource = _pendingCheckedChangeSource;
        _pendingCheckedChangeSource = source;
        try
        {
            action();
        }
        finally
        {
            _pendingCheckedChangeSource = previousSource;
        }
    }

    private static bool ToCheckedValue(object? value)
    {
        return value is bool checkedValue && checkedValue;
    }
}
