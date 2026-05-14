using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Controls;

public class CodexProgress : ProgressBar
{
    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexProgress, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexProgress, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexProgress()
    {
        VariantProperty.Changed.AddClassHandler<CodexProgress>((progress, _) => progress.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexProgress>((progress, _) => progress.SyncClasses());
        IsIndeterminateProperty.Changed.AddClassHandler<CodexProgress>((progress, _) => progress.SyncClasses());
    }

    public CodexProgress()
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

    private void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("indeterminate", IsIndeterminate);
    }
}
