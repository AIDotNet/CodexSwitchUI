using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class TableInteractionSample
{
    public static Control BuildTableInteractionPreview()
    {
        var status = Muted("Select a row to move the selected class and restart table content motion.");
        var version = 0;
        var gpt = TableDataRow("gpt-5", "Responses", "42.7K", "Primary", true);
        var claude = TableDataRow("claude-sonnet", "Claude", "18.3K", "Fallback", false);
        var mini = TableDataRow("o4-mini", "Chat", "7.1K", "Disabled", false, isEnabled: false);
        var rows = new[] { gpt, claude, mini };
        var table = new CodexTable
        {
            IsStriped = true,
            IsHoverable = true,
            TransitionKey = "providers-initial",
            Content = new StackPanel
            {
                Spacing = 0,
                Children =
                {
                    TableHeaderRow(),
                    new CodexTableBody { Items = { gpt, claude, mini } },
                    new CodexTableCaption { Content = "Pointer row selection updates selected state without rebuilding table chrome." }
                }
            }
        };

        void SelectRow(CodexTableRow row, string model)
        {
            if (!row.IsEnabled)
            {
                status.Text = "Disabled rows ignore pointer selection.";
                return;
            }

            foreach (var item in rows)
            {
                item.IsSelected = ReferenceEquals(item, row);
            }

            table.TransitionKey = $"{model}-{++version}";
            status.Text = $"{model} selected; sibling rows cleared and content transition restarted.";
        }

        void SelectRowFromPointer(PointerReleasedEventArgs args, CodexTableRow row, string model)
        {
            if (args.GetCurrentPoint(row).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonReleased)
            {
                return;
            }

            SelectRow(row, model);
            args.Handled = true;
        }

        gpt.PointerReleased += (_, args) => SelectRowFromPointer(args, gpt, "gpt-5");
        claude.PointerReleased += (_, args) => SelectRowFromPointer(args, claude, "claude-sonnet");
        mini.PointerReleased += (_, args) => SelectRowFromPointer(args, mini, "o4-mini");

        var density = new CodexButton
        {
            Content = "Toggle density",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        density.Click += (_, _) =>
        {
            table.IsCompact = !table.IsCompact;
            table.TransitionKey = $"density-{++version}";
            status.Text = table.IsCompact
                ? "Compact density applied; row selection state stayed intact."
                : "Default density restored; row selection state stayed intact.";
        };

        var hover = new CodexButton
        {
            Content = "Toggle hover",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        hover.Click += (_, _) =>
        {
            table.IsHoverable = !table.IsHoverable;
            status.Text = table.IsHoverable
                ? "Hover feedback enabled for selectable rows."
                : "Hover feedback disabled while selection state remains visible.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                table,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { density, hover }
                },
                new CodexTable
                {
                    IsCompact = true,
                    IsStriped = true,
                    TransitionKey = "refreshing",
                    Content = new StackPanel
                    {
                        Spacing = 0,
                        Children =
                        {
                            TableHeaderRow(),
                            TableDataRow("gpt-5", "Responses", "43.2K", "Refreshing", true),
                            TableDataRow("claude-sonnet", "Claude", "18.8K", "Queued", false)
                        }
                    }
                }
            }
        };
    }

    private static Control TableHeaderRow()
    {
        return new CodexTableHeader
        {
            Content = TableGrid(
                TableHead("Model", 0),
                TableHead("Protocol", 1),
                TableHead("Tokens", 2, CodexTableCellAlignment.Right),
                TableHead("State", 3))
        };
    }

    private static CodexTableRow TableDataRow(string model, string protocol, string tokens, string state, bool isSelected, bool isEnabled = true)
    {
        return new CodexTableRow
        {
            IsSelected = isSelected,
            IsEnabled = isEnabled,
            Content = TableGrid(
                TableCell(model, 0),
                TableCell(protocol, 1),
                TableCell(tokens, 2, CodexTableCellAlignment.Right),
                TableCell(state, 3))
        };
    }

    private static Grid TableGrid(params Control[] cells)
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(1.4, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            }
        };

        foreach (var cell in cells)
        {
            grid.Children.Add(cell);
        }

        return grid;
    }

    private static Control TableHead(string text, int column, CodexTableCellAlignment alignment = CodexTableCellAlignment.Left)
    {
        var head = new CodexTableHead
        {
            Content = Muted(text),
            Alignment = alignment
        };
        Grid.SetColumn(head, column);
        return head;
    }

    private static Control TableCell(string text, int column, CodexTableCellAlignment alignment = CodexTableCellAlignment.Left)
    {
        var cell = new CodexTableCell
        {
            Content = Text(text, CodexTextRole.Body),
            Alignment = alignment
        };
        Grid.SetColumn(cell, column);
        return cell;
    }

    private static CodexText Muted(string text)
    {
        return Text(text, CodexTextRole.Muted);
    }

    private static CodexText Text(string text, CodexTextRole role)
    {
        return new CodexText
        {
            Role = role,
            Text = text
        };
    }
}
