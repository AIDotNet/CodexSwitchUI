using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

namespace CodexSwitchUI.Controls;

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexButton : Button
{
    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexButton, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexButton, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexButton, bool>(nameof(IsLoading));

    public static readonly StyledProperty<object?> LoadingContentProperty =
        AvaloniaProperty.Register<CodexButton, object?>(nameof(LoadingContent));

    public static readonly StyledProperty<object?> LeadingIconProperty =
        AvaloniaProperty.Register<CodexButton, object?>(nameof(LeadingIcon));

    public static readonly StyledProperty<object?> TrailingIconProperty =
        AvaloniaProperty.Register<CodexButton, object?>(nameof(TrailingIcon));

    static CodexButton()
    {
        VariantProperty.Changed.AddClassHandler<CodexButton>((button, _) => button.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexButton>((button, _) => button.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexButton>((button, _) => button.SyncClasses());
        LoadingContentProperty.Changed.AddClassHandler<CodexButton>((button, _) => button.SyncClasses());
        LeadingIconProperty.Changed.AddClassHandler<CodexButton>((button, _) => button.SyncClasses());
        TrailingIconProperty.Changed.AddClassHandler<CodexButton>((button, _) => button.SyncClasses());
    }

    public CodexButton()
    {
        SyncClasses();
    }

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public object? LoadingContent
    {
        get => GetValue(LoadingContentProperty);
        set => SetValue(LoadingContentProperty, value);
    }

    public object? LeadingIcon
    {
        get => GetValue(LeadingIconProperty);
        set => SetValue(LeadingIconProperty, value);
    }

    public object? TrailingIcon
    {
        get => GetValue(TrailingIconProperty);
        set => SetValue(TrailingIconProperty, value);
    }

    protected override void OnClick()
    {
        if (IsLoading)
        {
            return;
        }

        base.OnClick();
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
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("is-loading", IsLoading);
        Classes.Set("has-loading-content", LoadingContent is not null);
        Classes.Set("has-leading-icon", LeadingIcon is not null);
        Classes.Set("has-trailing-icon", TrailingIcon is not null);
    }
}
