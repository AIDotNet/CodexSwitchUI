using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class PinnedTableInteractionSample
{
    public static Control BuildPinnedTableInteractionPreview()
    {
        var table = CreatePinnedTable(isCompact: false, isLoading: false, transitionKey: "providers-live");
        var status = Muted("Drag the middle body horizontally; the header follows the same offset.");
        var refreshVersion = 0;

        var refresh = new CodexButton
        {
            Content = "Refresh data",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        refresh.Click += (_, _) =>
        {
            table.IsLoading = !table.IsLoading;
            table.TransitionKey = $"providers-refresh-{++refreshVersion}";
            status.Text = table.IsLoading
                ? "Loading class is active; pinned regions keep their synchronized layout."
                : "TransitionKey changed; pinned regions run the tokenized refresh motion.";
        };

        var density = new CodexButton
        {
            Content = "Toggle density",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        density.Click += (_, _) =>
        {
            table.IsCompact = !table.IsCompact;
            table.Height = table.IsCompact ? 180 : 220;
            table.TransitionKey = $"providers-density-{++refreshVersion}";
            status.Text = table.IsCompact
                ? "Compact density preserves start, middle, and end template alignment."
                : "Default density restored with the same pinned scroll contract.";
        };

        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                status,
                table,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { refresh, density }
                }
            }
        };
    }

    private static CodexPinnedTable CreatePinnedTable(bool isCompact, bool isLoading, object transitionKey)
    {
        return new CodexPinnedTable
        {
            Width = 650,
            Height = isCompact ? 180 : 220,
            StartColumnWidth = new GridLength(150),
            EndColumnWidth = new GridLength(132),
            MiddleMinWidth = 620,
            IsCompact = isCompact,
            IsLoading = isLoading,
            TransitionKey = transitionKey,
            ItemsSource = PinnedTableRows(),
            StartHeader = new CodexTableHead { Content = Muted("Provider") },
            MiddleHeader = PinnedTableHeader(),
            EndHeader = new CodexTableHead
            {
                Content = Muted("Status"),
                Alignment = CodexTableCellAlignment.Right
            },
            StartCellTemplate = new FuncDataTemplate<PinnedProviderRow>((row, _) => new CodexTableRow
            {
                IsSelected = row?.IsActive == true,
                Content = new CodexTableCell
                {
                    Content = Text(row?.Name ?? string.Empty, CodexTextRole.Body)
                }
            }),
            MiddleCellTemplate = new FuncDataTemplate<PinnedProviderRow>((row, _) => new CodexTableRow
            {
                IsSelected = row?.IsActive == true,
                Content = PinnedTableMiddleRow(row)
            }),
            EndCellTemplate = new FuncDataTemplate<PinnedProviderRow>((row, _) => new CodexTableRow
            {
                IsSelected = row?.IsActive == true,
                Content = new CodexTableCell
                {
                    Alignment = CodexTableCellAlignment.Right,
                    Content = new CodexBadge
                    {
                        Content = row?.Status ?? string.Empty,
                        Variant = row?.IsActive == true ? CodexControlVariant.Success : CodexControlVariant.Secondary,
                        HorizontalAlignment = HorizontalAlignment.Right
                    }
                }
            })
        };
    }

    private static Control PinnedTableHeader()
    {
        return PinnedTableGrid(
            TableHead("Model", 0),
            TableHead("Tokens", 1, CodexTableCellAlignment.Right),
            TableHead("Cost", 2, CodexTableCellAlignment.Right),
            TableHead("Fallback", 3));
    }

    private static Control PinnedTableMiddleRow(PinnedProviderRow? row)
    {
        return PinnedTableGrid(
            TableCell(row?.Model ?? string.Empty, 0),
            TableCell(row?.Tokens ?? string.Empty, 1, CodexTableCellAlignment.Right),
            TableCell(row?.Cost ?? string.Empty, 2, CodexTableCellAlignment.Right),
            TableCell(row?.Fallback ?? string.Empty, 3));
    }

    private static Grid PinnedTableGrid(params Control[] cells)
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(1.5, GridUnitType.Star)),
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

    private static PinnedProviderRow[] PinnedTableRows()
    {
        return
        [
            new("OpenAI", "gpt-5", "42.7K", "$0.84", "Claude", "Active", true),
            new("Claude", "claude-sonnet", "18.3K", "$0.41", "OpenAI", "Ready", false),
            new("Local proxy", "qwen-coder", "7.1K", "$0.03", "OpenAI", "Ready", false),
            new("Archive", "o4-mini", "3.4K", "$0.04", "Claude", "Paused", false)
        ];
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

    private sealed record PinnedProviderRow(
        string Name,
        string Model,
        string Tokens,
        string Cost,
        string Fallback,
        string Status,
        bool IsActive);
}
