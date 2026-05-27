using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using CodexSwitchUI.Tokens;

namespace CodexSwitchUI.Controls;

public enum CodexChartIndicatorStyle
{
    Dot,
    Line,
    Square
}

public interface ICodexChartSeriesConfig
{
    string Key { get; }

    string Label { get; }

    IBrush? Color { get; }

    object? Icon { get; }
}

public sealed record CodexChartSeriesConfig(
    string Key,
    string Label,
    IBrush? Color = null,
    object? Icon = null) : ICodexChartSeriesConfig;

public class CodexChart : CodexChartContainer
{
}

public class CodexChartContainer : ContentControl
{
    private Control? _contentPresenter;
    private TranslateTransform? _contentTransform;
    private int _transitionVersion;

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CodexChartContainer, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexChartContainer, string?>(nameof(Description));

    public static readonly StyledProperty<object?> LegendProperty =
        AvaloniaProperty.Register<CodexChartContainer, object?>(nameof(Legend));

    public static readonly StyledProperty<object?> TooltipProperty =
        AvaloniaProperty.Register<CodexChartContainer, object?>(nameof(Tooltip));

    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<CodexChartContainer, object?>(nameof(Footer));

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexChartContainer, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsInteractiveProperty =
        AvaloniaProperty.Register<CodexChartContainer, bool>(nameof(IsInteractive));

    public static readonly StyledProperty<bool> IsRefreshingProperty =
        AvaloniaProperty.Register<CodexChartContainer, bool>(nameof(IsRefreshing));

    public static readonly StyledProperty<object?> TransitionKeyProperty =
        AvaloniaProperty.Register<CodexChartContainer, object?>(nameof(TransitionKey));

    public static readonly StyledProperty<double> TransitionOffsetProperty =
        AvaloniaProperty.Register<CodexChartContainer, double>(nameof(TransitionOffset), 7);

    public static readonly StyledProperty<bool> HasTitleProperty =
        AvaloniaProperty.Register<CodexChartContainer, bool>(nameof(HasTitle));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexChartContainer, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasHeaderProperty =
        AvaloniaProperty.Register<CodexChartContainer, bool>(nameof(HasHeader));

    public static readonly StyledProperty<bool> HasLegendProperty =
        AvaloniaProperty.Register<CodexChartContainer, bool>(nameof(HasLegend));

    public static readonly StyledProperty<bool> HasTooltipProperty =
        AvaloniaProperty.Register<CodexChartContainer, bool>(nameof(HasTooltip));

    public static readonly StyledProperty<bool> HasFooterProperty =
        AvaloniaProperty.Register<CodexChartContainer, bool>(nameof(HasFooter));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexChartContainer, bool>(nameof(HasContent));

    static CodexChartContainer()
    {
        TitleProperty.Changed.AddClassHandler<CodexChartContainer>((container, _) => container.SyncSlots());
        DescriptionProperty.Changed.AddClassHandler<CodexChartContainer>((container, _) => container.SyncSlots());
        LegendProperty.Changed.AddClassHandler<CodexChartContainer>((container, _) => container.SyncSlots());
        TooltipProperty.Changed.AddClassHandler<CodexChartContainer>((container, _) => container.SyncSlots());
        FooterProperty.Changed.AddClassHandler<CodexChartContainer>((container, _) => container.SyncSlots());
        ContentProperty.Changed.AddClassHandler<CodexChartContainer>((container, _) => container.SyncSlots());
        SizeProperty.Changed.AddClassHandler<CodexChartContainer>((container, _) => container.SyncClasses());
        IsInteractiveProperty.Changed.AddClassHandler<CodexChartContainer>((container, _) => container.SyncClasses());
        IsRefreshingProperty.Changed.AddClassHandler<CodexChartContainer>((container, _) => container.SyncClasses());
        TransitionKeyProperty.Changed.AddClassHandler<CodexChartContainer>((container, _) => container.StartContentTransition());
    }

    public CodexChartContainer()
    {
        SyncSlots();
        SyncClasses();
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public object? Legend
    {
        get => GetValue(LegendProperty);
        set => SetValue(LegendProperty, value);
    }

    public object? Tooltip
    {
        get => GetValue(TooltipProperty);
        set => SetValue(TooltipProperty, value);
    }

    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool IsInteractive
    {
        get => GetValue(IsInteractiveProperty);
        set => SetValue(IsInteractiveProperty, value);
    }

    public bool IsRefreshing
    {
        get => GetValue(IsRefreshingProperty);
        set => SetValue(IsRefreshingProperty, value);
    }

    public object? TransitionKey
    {
        get => GetValue(TransitionKeyProperty);
        set => SetValue(TransitionKeyProperty, value);
    }

    public double TransitionOffset
    {
        get => GetValue(TransitionOffsetProperty);
        set => SetValue(TransitionOffsetProperty, value);
    }

    public bool HasTitle => GetValue(HasTitleProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasHeader => GetValue(HasHeaderProperty);

    public bool HasLegend => GetValue(HasLegendProperty);

    public bool HasTooltip => GetValue(HasTooltipProperty);

    public bool HasFooter => GetValue(HasFooterProperty);

    public bool HasContent => GetValue(HasContentProperty);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _contentPresenter = e.NameScope.Find<Control>("PART_ChartContent");
        if (_contentPresenter is null)
            return;

        _contentTransform = _contentPresenter.RenderTransform as TranslateTransform;
        if (_contentTransform is null)
        {
            _contentTransform = new TranslateTransform();
            _contentPresenter.RenderTransform = _contentTransform;
        }

        ApplyContentTransitionResources();
    }

    private void SyncSlots()
    {
        var hasTitle = HasValue(Title);
        var hasDescription = HasValue(Description);

        SetValue(HasTitleProperty, hasTitle);
        SetValue(HasDescriptionProperty, hasDescription);
        SetValue(HasHeaderProperty, hasTitle || hasDescription);
        SetValue(HasLegendProperty, HasValue(Legend));
        SetValue(HasTooltipProperty, HasValue(Tooltip));
        SetValue(HasFooterProperty, HasValue(Footer));
        SetValue(HasContentProperty, HasValue(Content));

        SyncClasses();
    }

    private void SyncClasses()
    {
        Classes.Set("chart-container", true);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("interactive", IsInteractive);
        Classes.Set("refreshing", IsRefreshing);
        Classes.Set("has-header", HasHeader);
        Classes.Set("has-legend", HasLegend);
        Classes.Set("has-tooltip", HasTooltip);
        Classes.Set("has-footer", HasFooter);
    }

    private void StartContentTransition()
    {
        if (_contentPresenter is null)
            return;

        var duration = ApplyContentTransitionResources();
        if (duration <= TimeSpan.Zero)
        {
            _contentPresenter.Opacity = 1;
            if (_contentTransform is not null)
                _contentTransform.Y = 0;

            return;
        }

        var version = ++_transitionVersion;
        _contentPresenter.Opacity = 0.72;
        if (_contentTransform is not null)
            _contentTransform.Y = Math.Max(0, TransitionOffset);

        Dispatcher.UIThread.Post(() =>
        {
            if (version != _transitionVersion || _contentPresenter is null)
                return;

            _contentPresenter.Opacity = 1;
            if (_contentTransform is not null)
                _contentTransform.Y = 0;
        }, DispatcherPriority.Render);
    }

    private TimeSpan ApplyContentTransitionResources()
    {
        if (_contentPresenter is null)
            return TimeSpan.Zero;

        var duration = CodexMotion.ResolveDefaultDuration(_contentPresenter);
        var easing = CodexMotion.ResolveEaseOut(_contentPresenter);
        CodexMotion.ApplyOpacityTransition(_contentPresenter, duration, easing);

        if (_contentTransform is not null)
        {
            CodexMotion.ApplyTranslateYTransition(_contentTransform, duration, easing);
        }

        return duration;
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}

public class CodexChartLegend : ItemsControl
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexChartLegend, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexChartLegend, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<CodexChartLegend, bool>(nameof(IsCompact));

    static CodexChartLegend()
    {
        OrientationProperty.Changed.AddClassHandler<CodexChartLegend>((legend, _) => legend.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexChartLegend>((legend, _) => legend.SyncClasses());
        IsCompactProperty.Changed.AddClassHandler<CodexChartLegend>((legend, _) => legend.SyncClasses());
    }

    public CodexChartLegend()
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

    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("chart-legend", true);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("compact", IsCompact);
        CodexClassSync.SetSize(Classes, Size);
    }
}

public class CodexChartLegendItem : ContentControl
{
    public static readonly StyledProperty<IBrush?> IndicatorBrushProperty =
        AvaloniaProperty.Register<CodexChartLegendItem, IBrush?>(nameof(IndicatorBrush));

    public static readonly StyledProperty<CodexChartIndicatorStyle> IndicatorStyleProperty =
        AvaloniaProperty.Register<CodexChartLegendItem, CodexChartIndicatorStyle>(nameof(IndicatorStyle), CodexChartIndicatorStyle.Dot);

    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CodexChartLegendItem, string?>(nameof(Value));

    public static readonly StyledProperty<bool> HasValueProperty =
        AvaloniaProperty.Register<CodexChartLegendItem, bool>(nameof(HasValue));

    static CodexChartLegendItem()
    {
        IndicatorStyleProperty.Changed.AddClassHandler<CodexChartLegendItem>((item, _) => item.SyncClasses());
        ValueProperty.Changed.AddClassHandler<CodexChartLegendItem>((item, _) => item.SyncClasses());
    }

    public CodexChartLegendItem()
    {
        SyncClasses();
    }

    public IBrush? IndicatorBrush
    {
        get => GetValue(IndicatorBrushProperty);
        set => SetValue(IndicatorBrushProperty, value);
    }

    public CodexChartIndicatorStyle IndicatorStyle
    {
        get => GetValue(IndicatorStyleProperty);
        set => SetValue(IndicatorStyleProperty, value);
    }

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public bool HasValue => GetValue(HasValueProperty);

    private void SyncClasses()
    {
        Classes.Set("chart-legend-item", true);
        Classes.Set("has-value", !string.IsNullOrWhiteSpace(Value));
        Classes.Set("indicator-dot", IndicatorStyle == CodexChartIndicatorStyle.Dot);
        Classes.Set("indicator-line", IndicatorStyle == CodexChartIndicatorStyle.Line);
        Classes.Set("indicator-square", IndicatorStyle == CodexChartIndicatorStyle.Square);
        SetValue(HasValueProperty, !string.IsNullOrWhiteSpace(Value));
    }
}

public class CodexChartTooltipContent : ItemsControl
{
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<CodexChartTooltipContent, string?>(nameof(Label));

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexChartTooltipContent, bool>(nameof(IsOpen), true);

    public static readonly StyledProperty<bool> HideLabelProperty =
        AvaloniaProperty.Register<CodexChartTooltipContent, bool>(nameof(HideLabel));

    public static readonly StyledProperty<bool> HideIndicatorProperty =
        AvaloniaProperty.Register<CodexChartTooltipContent, bool>(nameof(HideIndicator));

    public static readonly StyledProperty<CodexChartIndicatorStyle> IndicatorStyleProperty =
        AvaloniaProperty.Register<CodexChartTooltipContent, CodexChartIndicatorStyle>(nameof(IndicatorStyle), CodexChartIndicatorStyle.Dot);

    public static readonly StyledProperty<bool> HasLabelProperty =
        AvaloniaProperty.Register<CodexChartTooltipContent, bool>(nameof(HasLabel));

    static CodexChartTooltipContent()
    {
        LabelProperty.Changed.AddClassHandler<CodexChartTooltipContent>((tooltip, _) => tooltip.SyncClasses());
        IsOpenProperty.Changed.AddClassHandler<CodexChartTooltipContent>((tooltip, _) => tooltip.SyncClasses());
        HideLabelProperty.Changed.AddClassHandler<CodexChartTooltipContent>((tooltip, _) => tooltip.SyncClasses());
        HideIndicatorProperty.Changed.AddClassHandler<CodexChartTooltipContent>((tooltip, _) => tooltip.SyncClasses());
        IndicatorStyleProperty.Changed.AddClassHandler<CodexChartTooltipContent>((tooltip, _) => tooltip.SyncClasses());
    }

    public CodexChartTooltipContent()
    {
        SyncClasses();
    }

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool HideLabel
    {
        get => GetValue(HideLabelProperty);
        set => SetValue(HideLabelProperty, value);
    }

    public bool HideIndicator
    {
        get => GetValue(HideIndicatorProperty);
        set => SetValue(HideIndicatorProperty, value);
    }

    public CodexChartIndicatorStyle IndicatorStyle
    {
        get => GetValue(IndicatorStyleProperty);
        set => SetValue(IndicatorStyleProperty, value);
    }

    public bool HasLabel => GetValue(HasLabelProperty);

    private void SyncClasses()
    {
        var hasLabel = !HideLabel && !string.IsNullOrWhiteSpace(Label);
        SetValue(HasLabelProperty, hasLabel);
        Classes.Set("chart-tooltip", true);
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("has-label", hasLabel);
        Classes.Set("hide-indicator", HideIndicator);
        Classes.Set("indicator-dot", IndicatorStyle == CodexChartIndicatorStyle.Dot);
        Classes.Set("indicator-line", IndicatorStyle == CodexChartIndicatorStyle.Line);
        Classes.Set("indicator-square", IndicatorStyle == CodexChartIndicatorStyle.Square);
    }
}

public class CodexChartTooltipItem : ContentControl
{
    public static readonly StyledProperty<IBrush?> IndicatorBrushProperty =
        AvaloniaProperty.Register<CodexChartTooltipItem, IBrush?>(nameof(IndicatorBrush));

    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CodexChartTooltipItem, string?>(nameof(Value));

    public static readonly StyledProperty<CodexChartIndicatorStyle> IndicatorStyleProperty =
        AvaloniaProperty.Register<CodexChartTooltipItem, CodexChartIndicatorStyle>(nameof(IndicatorStyle), CodexChartIndicatorStyle.Dot);

    public static readonly StyledProperty<bool> HasValueProperty =
        AvaloniaProperty.Register<CodexChartTooltipItem, bool>(nameof(HasValue));

    static CodexChartTooltipItem()
    {
        ValueProperty.Changed.AddClassHandler<CodexChartTooltipItem>((item, _) => item.SyncClasses());
        IndicatorStyleProperty.Changed.AddClassHandler<CodexChartTooltipItem>((item, _) => item.SyncClasses());
    }

    public CodexChartTooltipItem()
    {
        SyncClasses();
    }

    public IBrush? IndicatorBrush
    {
        get => GetValue(IndicatorBrushProperty);
        set => SetValue(IndicatorBrushProperty, value);
    }

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public CodexChartIndicatorStyle IndicatorStyle
    {
        get => GetValue(IndicatorStyleProperty);
        set => SetValue(IndicatorStyleProperty, value);
    }

    public bool HasValue => GetValue(HasValueProperty);

    private void SyncClasses()
    {
        var hasValue = !string.IsNullOrWhiteSpace(Value);
        SetValue(HasValueProperty, hasValue);
        Classes.Set("chart-tooltip-item", true);
        Classes.Set("has-value", hasValue);
        Classes.Set("indicator-dot", IndicatorStyle == CodexChartIndicatorStyle.Dot);
        Classes.Set("indicator-line", IndicatorStyle == CodexChartIndicatorStyle.Line);
        Classes.Set("indicator-square", IndicatorStyle == CodexChartIndicatorStyle.Square);
    }
}
