using Avalonia.Controls;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;
using System;
using System.Linq;

public static class AccordionInteractionSample
{
    public static Control BuildAccordionInteractionPreview()
    {
        var status = new CodexText
        {
            Role = CodexTextRole.Muted,
            Text = "ValueChanged reports open values, changed item, and source metadata."
        };
        var routing = new CodexAccordionItem
        {
            Value = "routing",
            Header = "Routing",
            IsOpen = true,
            Content = new CodexText { Role = CodexTextRole.Muted, Text = "Initially open single-mode item." }
        };
        var billing = new CodexAccordionItem
        {
            Value = "billing",
            Header = "Billing",
            Content = new CodexText { Role = CodexTextRole.Muted, Text = "Programmatic open closes Routing in single mode." }
        };
        var audit = new CodexAccordionItem
        {
            Value = "audit",
            Header = "Audit",
            IsEnabled = false,
            Content = new CodexText { Role = CodexTextRole.Muted, Text = "Disabled triggers remain skipped." }
        };
        var accordion = new CodexAccordion
        {
            IsCollapsible = true,
            Items = { routing, billing, audit }
        };
        accordion.ValueChanged += (_, args) =>
        {
            var values = args.NewValues.Count == 0 ? "none" : string.Join(", ", args.NewValues);
            status.Text = $"Open values: {values}; changed={args.ChangedValue ?? "none"}; source={args.Source}.";
        };

        var openBilling = new CodexButton
        {
            Content = "Open billing",
            Size = CodexControlSize.Small
        };
        openBilling.Click += (_, _) => billing.IsOpen = true;

        var collapseAll = new CodexButton
        {
            Content = "Collapse all",
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
        collapseAll.Click += (_, _) =>
        {
            foreach (var item in accordion.Items.OfType<CodexAccordionItem>())
            {
                item.IsOpen = false;
            }
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                accordion,
                status,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children = { openBilling, collapseAll }
                },
                new CodexAccordion
                {
                    Type = CodexAccordionType.Multiple,
                    Size = CodexControlSize.Small,
                    AnimationDuration = TimeSpan.Zero,
                    Items =
                    {
                        new CodexAccordionItem
                        {
                            Value = "multi-routes",
                            Header = "Multiple routes",
                            IsOpen = true,
                            Content = new CodexText { Role = CodexTextRole.Muted, Text = "Independent toggle remains open." }
                        },
                        new CodexAccordionItem
                        {
                            Value = "multi-limits",
                            Header = "Multiple limits",
                            IsOpen = true,
                            Content = new CodexText { Role = CodexTextRole.Muted, Text = "The second item can stay open at the same time." }
                        }
                    }
                }
            }
        };
    }
}
