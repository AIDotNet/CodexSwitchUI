using Avalonia;

namespace CodexSwitchUI.Controls;

public enum CodexSheetSide
{
    Right,
    Left,
    Top,
    Bottom
}

public class CodexSheet : CodexDialog
{
    public static readonly StyledProperty<CodexSheetSide> SideProperty =
        AvaloniaProperty.Register<CodexSheet, CodexSheetSide>(nameof(Side), CodexSheetSide.Right);

    static CodexSheet()
    {
        SideProperty.Changed.AddClassHandler<CodexSheet>((sheet, _) => sheet.SyncSideState());
    }

    public CodexSheet()
    {
        SyncSideState();
    }

    public CodexSheetSide Side
    {
        get => GetValue(SideProperty);
        set => SetValue(SideProperty, value);
    }

    private void SyncSideState()
    {
        Classes.Set("side-right", Side == CodexSheetSide.Right);
        Classes.Set("side-left", Side == CodexSheetSide.Left);
        Classes.Set("side-top", Side == CodexSheetSide.Top);
        Classes.Set("side-bottom", Side == CodexSheetSide.Bottom);
    }
}
