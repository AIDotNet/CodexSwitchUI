using Avalonia.Controls;
using Avalonia.Layout;
using CodexSwitchUI.Controls;
using CodexSwitchUI.Primitives;

public static class MetricInteractionSample
{
    public static Control BuildMetricInteractionPreview()
    {
        var status = Muted("Refresh metric values from the host data model.");
        var version = 0;
        var tokens = new CodexStatCard
        {
            Label = "Tokens",
            Value = Text("42.7K", CodexTextRole.Subtitle),
            Detail = "+18% from yesterday",
            Icon = new CodexBadge { Content = "API", Variant = CodexControlVariant.Secondary }
        };
        var refresh = Button("Refresh metrics");
        refresh.Click += (_, _) =>
        {
            version++;
            tokens.Value = Text(version % 2 == 0 ? "42.7K" : "48.9K", CodexTextRole.Subtitle);
            tokens.Detail = version % 2 == 0 ? "+18% from yesterday" : "+31% after routing change";
            tokens.Icon = new CodexBadge
            {
                Content = version % 2 == 0 ? "API" : "LIVE",
                Variant = version % 2 == 0 ? CodexControlVariant.Secondary : CodexControlVariant.Success,
                IsStatusVisible = version % 2 != 0
            };
            status.Text = $"Metric refresh #{version}; value and icon slots updated.";
        };

        var latency = new CodexMetric
        {
            Label = "Latency",
            Value = Text("284ms", CodexTextRole.Subtitle),
            Detail = "p95 response time"
        };
        var toggleDetail = Button("Toggle detail");
        toggleDetail.Click += (_, _) =>
        {
            latency.Detail = string.IsNullOrWhiteSpace(latency.Detail) ? "p95 response time" : null;
            status.Text = string.IsNullOrWhiteSpace(latency.Detail)
                ? "Metric detail hidden; has-detail class cleared."
                : "Metric detail restored; has-detail class applied.";
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                status,
                tokens,
                refresh,
                latency,
                toggleDetail,
                new CodexStatCard
                {
                    Label = "Owner",
                    Value = Text("OpenAI", CodexTextRole.Subtitle),
                    Detail = "Primary route",
                    Icon = new CodexAvatar { Fallback = "OP", Size = CodexControlSize.Small, IsStatusVisible = true }
                },
                new CodexMetric
                {
                    Label = "Fallbacks",
                    Value = Text("7", CodexTextRole.Subtitle),
                    Detail = "Last hour"
                }
            }
        };
    }

    private static CodexButton Button(string label)
    {
        return new CodexButton
        {
            Content = label,
            Size = CodexControlSize.Small,
            Variant = CodexControlVariant.Secondary
        };
    }

    private static CodexText Muted(string text) => Text(text, CodexTextRole.Muted);

    private static CodexText Text(string text, CodexTextRole role)
    {
        return new CodexText { Role = role, Text = text };
    }
}
