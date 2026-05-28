using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class DataTableInteractionSample
{
    public static Control BuildDataTableInteractionPreview()
    {
        var payments = DataTableRows();
        var status = Muted("Use the controls to filter, sort, hide amount, select rows, and move pages.");
        var tableHost = new StackPanel { Spacing = 12 };
        var failedOnly = false;
        var sortDescending = false;
        var showAmount = true;
        var page = 1;
        var version = 0;

        void RenderTable()
        {
            var visibleRows = DataTableVisibleRows(payments, failedOnly, sortDescending);
            tableHost.Children.Clear();
            tableHost.Children.Add(DataTableToolbar(failedOnly ? "failed" : string.Empty, columnsOpen: !showAmount, showAmount));
            tableHost.Children.Add(visibleRows.Length == 0
                ? CreateDataTableEmpty(showAmount, $"payments-empty-{version}")
                : CreateDataTable(
                    visibleRows,
                    showAmount,
                    $"payments-interaction-{version}",
                    isCompact: failedOnly,
                    onRowActivated: payment =>
                    {
                        payment.IsSelected = !payment.IsSelected;
                        version++;
                        status.Text = $"{payment.Email} selection toggled; {DataTableSelectedCount(payments)} of {payments.Length} row(s) selected.";
                        RenderTable();
                    },
                    amountHeader: sortDescending ? "Amount desc" : "Amount asc"));
            tableHost.Children.Add(DataTableFooter($"{DataTableSelectedCount(payments)} of {payments.Length} row(s) selected.", page, 3));
        }

        var filter = new CodexButton
        {
            Content = "Filter failed",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        filter.Click += (_, _) =>
        {
            failedOnly = !failedOnly;
            page = 1;
            version++;
            status.Text = failedOnly ? "Filter applied: failed payments only." : "Filter cleared: all payments visible.";
            RenderTable();
        };

        var sort = new CodexButton
        {
            Content = "Sort amount",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        sort.Click += (_, _) =>
        {
            sortDescending = !sortDescending;
            version++;
            status.Text = sortDescending ? "Amount sorted descending." : "Amount sorted ascending.";
            RenderTable();
        };

        var columns = new CodexButton
        {
            Content = "Toggle amount",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        columns.Click += (_, _) =>
        {
            showAmount = !showAmount;
            version++;
            status.Text = showAmount ? "Amount column is visible." : "Amount column hidden through visibility state.";
            RenderTable();
        };

        var next = new CodexButton
        {
            Content = "Next page",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Ghost
        };
        next.Click += (_, _) =>
        {
            page = page == 3 ? 1 : page + 1;
            version++;
            status.Text = $"Page changed to {page}; toolbar and selection state stayed mounted.";
            RenderTable();
        };

        RenderTable();

        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                status,
                tableHost,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { filter, sort, columns, next }
                }
            }
        };
    }

    private static DataTablePayment[] DataTableRows()
    {
        return
        [
            new("728ed52f", "success", "ken99@example.com", 316m, true),
            new("489e1d42", "processing", "abe45@example.com", 242m),
            new("f8e8c9d1", "success", "silas22@example.com", 874m),
            new("5b7a9c31", "failed", "carmella@example.com", 721m),
            new("9c1f4a76", "pending", "monserrat44@example.com", 837m)
        ];
    }

    private static DataTablePayment[] DataTableVisibleRows(DataTablePayment[] rows, bool failedOnly, bool sortDescending)
    {
        var visibleRows = new List<DataTablePayment>();
        foreach (var row in rows)
        {
            if (!failedOnly || row.Status == "failed")
                visibleRows.Add(row);
        }

        visibleRows.Sort((left, right) => sortDescending
            ? right.Amount.CompareTo(left.Amount)
            : left.Amount.CompareTo(right.Amount));

        return visibleRows.ToArray();
    }

    private static int DataTableSelectedCount(DataTablePayment[] rows)
    {
        var count = 0;
        foreach (var row in rows)
        {
            if (row.IsSelected)
                count++;
        }

        return count;
    }

    private static Control DataTableToolbar(string filterText, bool columnsOpen, bool showAmount)
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };
        grid.Children.Add(new CodexTextBox
        {
            Width = 260,
            PlaceholderText = "Filter emails...",
            Text = filterText
        });

        var columns = new CodexDropdownButton
        {
            Content = "Columns",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Outline,
            IsOpen = columnsOpen,
            Align = CodexDropdownAlign.End,
            DropDownContent = new StackPanel
            {
                Width = 184,
                Spacing = 8,
                Children =
                {
                    new CodexCheckBox { Content = "Status", IsChecked = true },
                    new CodexCheckBox { Content = "Email", IsChecked = true },
                    new CodexCheckBox { Content = "Amount", IsChecked = showAmount },
                    new CodexCheckBox { Content = "Actions", IsChecked = true }
                }
            }
        };
        Grid.SetColumn(columns, 1);
        grid.Children.Add(columns);
        return grid;
    }

    private static CodexTable CreateDataTable(
        DataTablePayment[] rows,
        bool showAmount,
        object transitionKey,
        bool isCompact = false,
        Action<DataTablePayment>? onRowActivated = null,
        string amountHeader = "Amount")
    {
        var body = new CodexTableBody();
        foreach (var row in rows)
        {
            body.Items.Add(DataTablePaymentRow(row, showAmount, onRowActivated));
        }

        return new CodexTable
        {
            IsStriped = true,
            IsHoverable = true,
            IsCompact = isCompact,
            TransitionKey = transitionKey,
            Content = new StackPanel
            {
                Spacing = 0,
                Children =
                {
                    DataTableHeaderRow(showAmount, amountHeader),
                    body,
                    new CodexTableCaption { Content = "Payments table composed from CodexTable, input, dropdown, checkbox, and pagination primitives." }
                }
            }
        };
    }

    private static CodexTable CreateDataTableEmpty(bool showAmount, object transitionKey)
    {
        return new CodexTable
        {
            IsStriped = true,
            IsHoverable = false,
            TransitionKey = transitionKey,
            Content = new StackPanel
            {
                Spacing = 0,
                Children =
                {
                    DataTableHeaderRow(showAmount, "Amount"),
                    new CodexTableRow
                    {
                        Content = new CodexTableCell
                        {
                            Content = Muted("No results. Clear the filter or refresh the payment source.")
                        }
                    },
                    new CodexTableCaption { Content = "Empty results keep the table scaffold mounted." }
                }
            }
        };
    }

    private static Control DataTableFooter(string summary, int page, int pageCount)
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };
        grid.Children.Add(Muted(summary));

        var pagination = new CodexPagination
        {
            Page = page,
            PageCount = pageCount,
            BoundaryCount = 1,
            SiblingCount = 1,
            IsCompact = true,
            ShowFirstLast = false,
            Size = CodexControlSize.Small
        };
        Grid.SetColumn(pagination, 1);
        grid.Children.Add(pagination);
        return grid;
    }

    private static Control DataTableHeaderRow(bool showAmount, string amountHeader)
    {
        return new CodexTableHeader
        {
            Content = DataTableGrid(
                showAmount,
                DataTableHead(new CodexCheckBox { IsChecked = false }, 0, CodexTableCellAlignment.Center),
                DataTableHead("Status", 1),
                DataTableHead("Email", 2),
                DataTableHead(amountHeader, 3, CodexTableCellAlignment.Right, showAmount),
                DataTableHead(string.Empty, 4))
        };
    }

    private static Control DataTablePaymentRow(DataTablePayment payment, bool showAmount, Action<DataTablePayment>? onActivated)
    {
        var row = new CodexTableRow
        {
            IsSelected = payment.IsSelected,
            Content = DataTableGrid(
                showAmount,
                DataTableCell(new CodexCheckBox { IsChecked = payment.IsSelected }, 0, CodexTableCellAlignment.Center),
                DataTableCell(DataTableStatusBadge(payment.Status), 1),
                DataTableCell(Text(payment.Email, CodexTextRole.Body), 2),
                DataTableCell(Text($"${payment.Amount:0.00}", CodexTextRole.Body), 3, CodexTableCellAlignment.Right, showAmount),
                DataTableCell(new CodexDropdownButton
                {
                    Content = "...",
                    Size = CodexControlSize.Small,
                    Variant = CodexControlVariant.Ghost,
                    Align = CodexDropdownAlign.End,
                    DropDownContent = ActionMenu("Copy payment ID", "View customer", "View details")
                }, 4, CodexTableCellAlignment.Right))
        };

        if (onActivated is not null)
        {
            row.PointerReleased += (_, args) =>
            {
                if (args.GetCurrentPoint(row).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonReleased)
                {
                    return;
                }

                onActivated(payment);
                args.Handled = true;
            };
        }

        return row;
    }

    private static Control DataTableStatusBadge(string status)
    {
        var variant = status switch
        {
            "success" => CodexControlVariant.Success,
            "failed" => CodexControlVariant.Destructive,
            "processing" => CodexControlVariant.Warning,
            _ => CodexControlVariant.Secondary
        };

        return new CodexBadge
        {
            Content = status,
            Variant = variant,
            HorizontalAlignment = HorizontalAlignment.Left
        };
    }

    private static StackPanel ActionMenu(params string[] labels)
    {
        var menu = new StackPanel
        {
            Width = 170,
            Spacing = 6
        };

        foreach (var label in labels)
        {
            menu.Children.Add(new CodexButton
            {
                Content = label,
                Variant = CodexControlVariant.Ghost,
                HorizontalAlignment = HorizontalAlignment.Stretch
            });
        }

        return menu;
    }

    private static Grid DataTableGrid(bool showAmount, params Control[] cells)
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(42)),
                new ColumnDefinition(new GridLength(1.05, GridUnitType.Star)),
                new ColumnDefinition(new GridLength(2.1, GridUnitType.Star)),
                new ColumnDefinition(showAmount ? GridLength.Star : new GridLength(0)),
                new ColumnDefinition(new GridLength(86))
            }
        };

        foreach (var cell in cells)
        {
            grid.Children.Add(cell);
        }

        return grid;
    }

    private static Control DataTableHead(string text, int column, CodexTableCellAlignment alignment = CodexTableCellAlignment.Left, bool isVisible = true)
    {
        return DataTableHead(Text(text, CodexTextRole.Muted), column, alignment, isVisible);
    }

    private static Control DataTableHead(Control content, int column, CodexTableCellAlignment alignment = CodexTableCellAlignment.Left, bool isVisible = true)
    {
        var head = new CodexTableHead
        {
            Content = content,
            Alignment = alignment,
            IsVisible = isVisible
        };
        Grid.SetColumn(head, column);
        return head;
    }

    private static Control DataTableCell(Control content, int column, CodexTableCellAlignment alignment = CodexTableCellAlignment.Left, bool isVisible = true)
    {
        var cell = new CodexTableCell
        {
            Content = content,
            Alignment = alignment,
            IsVisible = isVisible
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

    private sealed class DataTablePayment(string id, string status, string email, decimal amount, bool isSelected = false)
    {
        public string Id { get; } = id;
        public string Status { get; } = status;
        public string Email { get; } = email;
        public decimal Amount { get; } = amount;
        public bool IsSelected { get; set; } = isSelected;
    }
}
