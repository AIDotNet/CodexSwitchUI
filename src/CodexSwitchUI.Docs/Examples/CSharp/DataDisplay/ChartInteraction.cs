using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class ChartInteractionSample
{
    public static Control BuildChartInteractionPreview()
    {
        var status = Muted("Refresh data, toggle tooltip, change legend orientation, and switch indicator geometry.");
        var useBurstData = false;
        var tooltipOpen = true;
        var verticalLegend = false;
        var compact = false;
        var indicator = CodexChartIndicatorStyle.Dot;
        var version = 0;

        var chart = new CodexUsagePieChart
        {
            Width = 500,
            Height = 250,
            TotalLabel = "Tokens",
            TotalValue = "71.5K",
            ItemsSource = UsagePieChartItems()
        };
        var container = new CodexChartContainer
        {
            Width = 640,
            Title = "Interactive chart",
            Description = "Chart helpers keep Web-style config surfaces mounted while chart data changes.",
            Legend = ChartLegend(false),
            Tooltip = ChartTooltip("Current slice", true, indicator),
            Footer = Muted("Host state controls data, tooltip, legend, and density."),
            Content = chart
        };

        void RefreshContainer()
        {
            container.Size = compact ? CodexControlSize.Small : CodexControlSize.Medium;
            container.IsRefreshing = useBurstData;
            container.TransitionKey = $"chart-{++version}";
            container.Legend = ChartLegend(compact, verticalLegend ? Orientation.Vertical : Orientation.Horizontal, indicator);
            container.Tooltip = ChartTooltip(useBurstData ? "Burst dataset" : "Baseline dataset", tooltipOpen, indicator);
        }

        var refresh = Button("Refresh data", CodexControlVariant.Secondary);
        refresh.Click += (_, _) =>
        {
            useBurstData = !useBurstData;
            chart.ItemsSource = useBurstData ? UsagePieChartBurstItems() : UsagePieChartItems();
            chart.TotalValue = useBurstData ? "90.4K" : "71.5K";
            status.Text = useBurstData
                ? "Burst data loaded; container refresh bar and chart transition are active."
                : "Baseline data restored; chart content stayed mounted.";
            RefreshContainer();
        };

        var tooltip = Button("Toggle tooltip");
        tooltip.Click += (_, _) =>
        {
            tooltipOpen = !tooltipOpen;
            status.Text = tooltipOpen ? "Tooltip opened with the current indicator style." : "Tooltip closed without removing the chart body.";
            RefreshContainer();
        };

        var legend = Button("Toggle legend");
        legend.Click += (_, _) =>
        {
            verticalLegend = !verticalLegend;
            status.Text = verticalLegend ? "Legend switched to vertical config layout." : "Legend returned to horizontal config layout.";
            RefreshContainer();
        };

        var indicatorButton = Button("Indicator");
        indicatorButton.Click += (_, _) =>
        {
            indicator = indicator switch
            {
                CodexChartIndicatorStyle.Dot => CodexChartIndicatorStyle.Line,
                CodexChartIndicatorStyle.Line => CodexChartIndicatorStyle.Square,
                _ => CodexChartIndicatorStyle.Dot
            };
            status.Text = $"Indicator switched to {indicator}.";
            RefreshContainer();
        };

        var density = Button("Density");
        density.Click += (_, _) =>
        {
            compact = !compact;
            status.Text = compact ? "Compact chart container and legend density applied." : "Default chart density restored.";
            RefreshContainer();
        };

        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                status,
                container,
                Row(refresh, tooltip, legend, indicatorButton, density)
            }
        };
    }

    private static CodexChartLegend ChartLegend(
        bool compact,
        Orientation orientation = Orientation.Horizontal,
        CodexChartIndicatorStyle indicator = CodexChartIndicatorStyle.Dot)
    {
        return new CodexChartLegend
        {
            IsCompact = compact,
            Orientation = orientation,
            Items =
            {
                new CodexChartLegendItem { Content = "Desktop", Value = compact ? null : "7,324", IndicatorStyle = indicator },
                new CodexChartLegendItem { Content = "Mobile", Value = compact ? null : "7,250", IndicatorStyle = indicator }
            }
        };
    }

    private static CodexChartTooltipContent ChartTooltip(string label, bool isOpen, CodexChartIndicatorStyle indicator)
    {
        return new CodexChartTooltipContent
        {
            Label = label,
            IsOpen = isOpen,
            IndicatorStyle = indicator,
            Items =
            {
                new CodexChartTooltipItem { Content = "Desktop", Value = "7,324", IndicatorStyle = indicator },
                new CodexChartTooltipItem { Content = "Mobile", Value = "7,250", IndicatorStyle = indicator }
            }
        };
    }

    private static CodexUsagePieChartItem[] UsagePieChartItems()
    {
        return
        [
            new("gpt-5", 42.7, "60%", "42.7K tokens"),
            new("claude-sonnet", 18.3, "26%", "18.3K tokens"),
            new("o4-mini", 7.1, "10%", "7.1K tokens"),
            new("fallback", 3.4, "4%", "3.4K tokens")
        ];
    }

    private static CodexUsagePieChartItem[] UsagePieChartBurstItems()
    {
        return
        [
            new("gpt-5", 51.2, "57%", "51.2K tokens"),
            new("claude-sonnet", 21.9, "25%", "21.9K tokens"),
            new("gemini", 10.6, "12%", "10.6K tokens"),
            new("local", 5.7, "6%", "5.7K tokens")
        ];
    }

    private static CodexButton Button(string label, CodexControlVariant variant = CodexControlVariant.Ghost)
    {
        return new CodexButton { Content = label, Size = CodexControlSize.Small, Variant = variant };
    }

    private static StackPanel Row(params Control[] children)
    {
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        foreach (var child in children)
            row.Children.Add(child);
        return row;
    }

    private static CodexText Muted(string text)
    {
        return new CodexText { Role = CodexTextRole.Muted, Text = text };
    }
}
