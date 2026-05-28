using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace CodexSwitchUI.Controls;

public sealed class CodexSliderValueChangingEventArgs(double oldValue, double newValue)
    : EventArgs
{
    public double OldValue { get; } = oldValue;

    public double NewValue { get; } = newValue;

    public IReadOnlyList<double> OldValues { get; } = [oldValue];

    public IReadOnlyList<double> NewValues { get; } = [newValue];
}

public sealed class CodexSliderValueCommittedEventArgs(double oldValue, double newValue, string source)
    : EventArgs
{
    public double OldValue { get; } = oldValue;

    public double NewValue { get; } = newValue;

    public IReadOnlyList<double> OldValues { get; } = [oldValue];

    public IReadOnlyList<double> NewValues { get; } = [newValue];

    public string Source { get; } = source;
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexSlider : Slider
{
    private double _lastCommittedValue;
    private bool _hasCommittedValue;
    private bool _isPointerChanging;

    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexSlider, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexSlider, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexSlider()
    {
        IntentProperty.Changed.AddClassHandler<CodexSlider>((slider, _) => slider.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexSlider>((slider, _) => slider.SyncClasses());
        MinimumProperty.Changed.AddClassHandler<CodexSlider>((slider, _) => slider.SyncValueClasses());
        MaximumProperty.Changed.AddClassHandler<CodexSlider>((slider, _) => slider.SyncValueClasses());
        ValueProperty.Changed.AddClassHandler<CodexSlider>((slider, args) => slider.OnSliderValueChanged(args));
    }

    public CodexSlider()
    {
        SyncClasses();
        RememberCommittedValue();
    }

    public event EventHandler<CodexSliderValueChangingEventArgs>? ValueChanging;

    public event EventHandler<CodexSliderValueCommittedEventArgs>? ValueCommitted;

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
        CommitPointerValue();
        CommitValue("focus");
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        if (!TryBeginPointerChange(updateKind))
        {
            return;
        }

        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        base.OnPointerReleased(e);
        TryCommitPointerValue(updateKind);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (IsSliderNavigationKey(e.Key))
        {
            CommitValue("keyboard");
        }
    }

    public bool CommitValue()
    {
        return CommitValue("programmatic");
    }

    internal bool TryBeginPointerChange(PointerUpdateKind updateKind)
    {
        if (updateKind != PointerUpdateKind.LeftButtonPressed || !IsEnabled)
        {
            return false;
        }

        _isPointerChanging = true;
        Classes.Set("dragging", true);
        return true;
    }

    internal bool TryCommitPointerValue(PointerUpdateKind updateKind)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased)
        {
            return false;
        }

        return CommitPointerValue();
    }

    private void SyncClasses()
    {
        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("slider", true);
        SyncValueClasses();
    }

    private void SyncValueClasses()
    {
        Classes.Set("has-value", Value > Minimum);
        Classes.Set("at-min", AreClose(Value, Minimum));
        Classes.Set("at-max", AreClose(Value, Maximum));
    }

    private void OnSliderValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        var oldValue = args.OldValue is double oldDouble ? oldDouble : Value;
        var newValue = args.NewValue is double newDouble ? newDouble : Value;

        SyncValueClasses();

        if (AreClose(oldValue, newValue))
        {
            return;
        }

        if (ValueChanging is null && ValueCommitted is null && !_isPointerChanging)
        {
            _lastCommittedValue = newValue;
            _hasCommittedValue = true;
            return;
        }

        if (!_hasCommittedValue)
        {
            _lastCommittedValue = oldValue;
            _hasCommittedValue = true;
        }

        ValueChanging?.Invoke(this, new CodexSliderValueChangingEventArgs(oldValue, newValue));
    }

    private bool CommitPointerValue()
    {
        if (!_isPointerChanging)
        {
            return false;
        }

        _isPointerChanging = false;
        Classes.Set("dragging", false);
        return CommitValue("pointer");
    }

    private bool CommitValue(string source)
    {
        if (!_hasCommittedValue)
        {
            RememberCommittedValue();
            return false;
        }

        var oldValue = _lastCommittedValue;
        var newValue = Value;

        if (AreClose(oldValue, newValue))
        {
            return false;
        }

        _lastCommittedValue = newValue;
        ValueCommitted?.Invoke(this, new CodexSliderValueCommittedEventArgs(oldValue, newValue, source));
        return true;
    }

    private void RememberCommittedValue()
    {
        _lastCommittedValue = Value;
        _hasCommittedValue = true;
    }

    private static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) < 0.000_001;
    }

    private static bool IsSliderNavigationKey(Key key)
    {
        return key is Key.Left
            or Key.Right
            or Key.Up
            or Key.Down
            or Key.PageUp
            or Key.PageDown
            or Key.Home
            or Key.End;
    }
}
