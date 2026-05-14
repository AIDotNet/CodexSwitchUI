using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace CodexSwitchUI.Controls;

public class CodexSeparator : TemplatedControl
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexSeparator, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexSeparator, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexSeparator()
    {
        OrientationProperty.Changed.AddClassHandler<CodexSeparator>((separator, _) => separator.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexSeparator>((separator, _) => separator.SyncClasses());
    }

    public CodexSeparator()
    {
        SyncClasses();
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        CodexClassSync.SetSize(Classes, Size);
    }
}
