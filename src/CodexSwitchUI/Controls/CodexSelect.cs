using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace CodexSwitchUI.Controls;

public class CodexSelect : ComboBox
{
    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexSelect, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexSelect, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    static CodexSelect()
    {
        IntentProperty.Changed.AddClassHandler<CodexSelect>((select, _) => select.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexSelect>((select, _) => select.SyncClasses());
        IsDropDownOpenProperty.Changed.AddClassHandler<CodexSelect>((select, _) => select.SyncPopupClasses());
    }

    public CodexSelect()
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

    private void SyncClasses()
    {
        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
    }

    private void SyncPopupClasses()
    {
        Classes.Set("popup-open", false);

        if (!IsDropDownOpen)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (IsDropDownOpen)
            {
                Classes.Set("popup-open", true);
            }
        });
    }
}
