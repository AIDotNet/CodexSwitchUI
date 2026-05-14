using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace CodexSwitchUI.Controls;

public enum CodexTableCellAlignment
{
    Left,
    Center,
    Right
}

public class CodexTable : ItemsControl
{
    public static readonly StyledProperty<bool> IsHoverableProperty =
        AvaloniaProperty.Register<CodexTable, bool>(nameof(IsHoverable), true);

    public static readonly StyledProperty<bool> IsStripedProperty =
        AvaloniaProperty.Register<CodexTable, bool>(nameof(IsStriped));

    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<CodexTable, bool>(nameof(IsCompact));

    static CodexTable()
    {
        IsHoverableProperty.Changed.AddClassHandler<CodexTable>((table, _) => table.SyncClasses());
        IsStripedProperty.Changed.AddClassHandler<CodexTable>((table, _) => table.SyncClasses());
        IsCompactProperty.Changed.AddClassHandler<CodexTable>((table, _) => table.SyncClasses());
    }

    public CodexTable()
    {
        SyncClasses();
    }

    public bool IsHoverable
    {
        get => GetValue(IsHoverableProperty);
        set => SetValue(IsHoverableProperty, value);
    }

    public bool IsStriped
    {
        get => GetValue(IsStripedProperty);
        set => SetValue(IsStripedProperty, value);
    }

    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("hoverable", IsHoverable);
        Classes.Set("striped", IsStriped);
        Classes.Set("compact", IsCompact);
    }
}

public class CodexTableHeader : ItemsControl
{
}

public class CodexTableBody : ItemsControl
{
}

public class CodexTableFooter : ItemsControl
{
}

public class CodexTableRow : ItemsControl
{
    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<CodexTableRow, bool>(nameof(IsSelected));

    static CodexTableRow()
    {
        IsSelectedProperty.Changed.AddClassHandler<CodexTableRow>((row, _) => row.SyncClasses());
    }

    public CodexTableRow()
    {
        SyncClasses();
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("selected", IsSelected);
    }
}

public class CodexTableHead : ContentControl
{
    public static readonly StyledProperty<CodexTableCellAlignment> AlignmentProperty =
        AvaloniaProperty.Register<CodexTableHead, CodexTableCellAlignment>(nameof(Alignment));

    static CodexTableHead()
    {
        AlignmentProperty.Changed.AddClassHandler<CodexTableHead>((head, _) => head.SyncClasses());
    }

    public CodexTableHead()
    {
        SyncClasses();
    }

    public CodexTableCellAlignment Alignment
    {
        get => GetValue(AlignmentProperty);
        set => SetValue(AlignmentProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("align-left", Alignment == CodexTableCellAlignment.Left);
        Classes.Set("align-center", Alignment == CodexTableCellAlignment.Center);
        Classes.Set("align-right", Alignment == CodexTableCellAlignment.Right);
        HorizontalContentAlignment = Alignment switch
        {
            CodexTableCellAlignment.Center => HorizontalAlignment.Center,
            CodexTableCellAlignment.Right => HorizontalAlignment.Right,
            _ => HorizontalAlignment.Left
        };
    }
}

public class CodexTableCell : ContentControl
{
    public static readonly StyledProperty<CodexTableCellAlignment> AlignmentProperty =
        AvaloniaProperty.Register<CodexTableCell, CodexTableCellAlignment>(nameof(Alignment));

    static CodexTableCell()
    {
        AlignmentProperty.Changed.AddClassHandler<CodexTableCell>((cell, _) => cell.SyncClasses());
    }

    public CodexTableCell()
    {
        SyncClasses();
    }

    public CodexTableCellAlignment Alignment
    {
        get => GetValue(AlignmentProperty);
        set => SetValue(AlignmentProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("align-left", Alignment == CodexTableCellAlignment.Left);
        Classes.Set("align-center", Alignment == CodexTableCellAlignment.Center);
        Classes.Set("align-right", Alignment == CodexTableCellAlignment.Right);
        HorizontalContentAlignment = Alignment switch
        {
            CodexTableCellAlignment.Center => HorizontalAlignment.Center,
            CodexTableCellAlignment.Right => HorizontalAlignment.Right,
            _ => HorizontalAlignment.Left
        };
    }
}

public class CodexTableCaption : ContentControl
{
}
