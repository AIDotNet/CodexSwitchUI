using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace CodexSwitchUI.Controls;

public class CodexTabs : TabControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexTabs, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexTabs, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<CodexTabsVariant> VariantProperty =
        AvaloniaProperty.Register<CodexTabs, CodexTabsVariant>(nameof(Variant), CodexTabsVariant.Default);

    static CodexTabs()
    {
        SizeProperty.Changed.AddClassHandler<CodexTabs>((tabs, _) => tabs.SyncClasses());
        OrientationProperty.Changed.AddClassHandler<CodexTabs>((tabs, _) => tabs.SyncClasses());
        VariantProperty.Changed.AddClassHandler<CodexTabs>((tabs, _) => tabs.SyncClasses());
    }

    public CodexTabs()
    {
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public CodexTabsVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("variant-default", Variant == CodexTabsVariant.Default);
        Classes.Set("variant-line", Variant == CodexTabsVariant.Line);
    }
}
