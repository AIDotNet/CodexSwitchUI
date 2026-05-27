using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace CodexSwitchUI.Controls;

public class CodexLabel : Label
{
    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexLabel, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexLabel, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsRequiredProperty =
        AvaloniaProperty.Register<CodexLabel, bool>(nameof(IsRequired));

    static CodexLabel()
    {
        IntentProperty.Changed.AddClassHandler<CodexLabel>((label, _) => label.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexLabel>((label, _) => label.SyncClasses());
        IsRequiredProperty.Changed.AddClassHandler<CodexLabel>((label, _) => label.SyncClasses());
    }

    public CodexLabel()
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

    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TargetProperty || change.Property == IsEnabledProperty)
        {
            SyncClasses();
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        SyncClasses();
        base.OnPointerPressed(e);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("label", true);
        Classes.Set("required", IsRequired);
        Classes.Set("has-target", Target is not null);
        Classes.Set("target-disabled", Target is InputElement { IsEnabled: false });
    }
}
