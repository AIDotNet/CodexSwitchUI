using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;

namespace CodexSwitchUI.Controls;

public enum CodexFieldOrientation
{
    Vertical,
    Horizontal,
    Responsive
}

public enum CodexFieldLegendVariant
{
    Legend,
    Label
}

public class CodexField : CodexFrame
{
    private CodexLabel? _label;

    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<CodexField, string?>(nameof(Label));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexField, string?>(nameof(Description));

    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<CodexField, string?>(nameof(Message));

    public static readonly StyledProperty<CodexControlIntent> IntentProperty =
        AvaloniaProperty.Register<CodexField, CodexControlIntent>(nameof(Intent), CodexControlIntent.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexField, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<CodexFieldOrientation> OrientationProperty =
        AvaloniaProperty.Register<CodexField, CodexFieldOrientation>(nameof(Orientation), CodexFieldOrientation.Vertical);

    public static readonly StyledProperty<bool> IsRequiredProperty =
        AvaloniaProperty.Register<CodexField, bool>(nameof(IsRequired));

    public static readonly StyledProperty<bool> IsInvalidProperty =
        AvaloniaProperty.Register<CodexField, bool>(nameof(IsInvalid));

    public static readonly StyledProperty<bool> HasLabelProperty =
        AvaloniaProperty.Register<CodexField, bool>(nameof(HasLabel));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexField, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasMessageProperty =
        AvaloniaProperty.Register<CodexField, bool>(nameof(HasMessage));

    static CodexField()
    {
        LabelProperty.Changed.AddClassHandler<CodexField>((field, _) => field.SyncClasses());
        DescriptionProperty.Changed.AddClassHandler<CodexField>((field, _) => field.SyncClasses());
        MessageProperty.Changed.AddClassHandler<CodexField>((field, _) => field.SyncClasses());
        IntentProperty.Changed.AddClassHandler<CodexField>((field, _) => field.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexField>((field, _) => field.SyncClasses());
        OrientationProperty.Changed.AddClassHandler<CodexField>((field, _) => field.SyncClasses());
        IsRequiredProperty.Changed.AddClassHandler<CodexField>((field, _) => field.SyncClasses());
        IsInvalidProperty.Changed.AddClassHandler<CodexField>((field, _) => field.SyncClasses());
    }

    public CodexField()
    {
        SyncClasses();
    }

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
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

    public CodexFieldOrientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    public bool IsInvalid
    {
        get => GetValue(IsInvalidProperty);
        set => SetValue(IsInvalidProperty, value);
    }

    public bool HasLabel => GetValue(HasLabelProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasMessage => GetValue(HasMessageProperty);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _label = e.NameScope.Find<CodexLabel>("PART_Label");
        SyncLabelTarget();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            SyncLabelTarget();
        }
    }

    private void SyncLabelTarget()
    {
        if (_label is not null)
        {
            _label.Target = Content as IInputElement;
        }
    }

    private void SyncClasses()
    {
        var hasLabel = HasText(Label);
        var hasDescription = HasText(Description);
        var hasMessage = HasText(Message);

        SetValue(HasLabelProperty, hasLabel);
        SetValue(HasDescriptionProperty, hasDescription);
        SetValue(HasMessageProperty, hasMessage);

        CodexClassSync.SetIntent(Classes, Intent);
        CodexClassSync.SetSize(Classes, Size);
        SetOrientationClasses(Classes, Orientation);
        Classes.Set("field", true);
        Classes.Set("has-label", hasLabel);
        Classes.Set("has-description", hasDescription);
        Classes.Set("has-message", hasMessage);
        Classes.Set("required", IsRequired);
        Classes.Set("invalid", IsInvalid);
    }

    private static bool HasText(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    internal static void SetOrientationClasses(Classes classes, CodexFieldOrientation orientation)
    {
        classes.Set("orientation-vertical", orientation == CodexFieldOrientation.Vertical);
        classes.Set("orientation-horizontal", orientation == CodexFieldOrientation.Horizontal);
        classes.Set("orientation-responsive", orientation == CodexFieldOrientation.Responsive);
    }
}

public class CodexFieldGroup : ItemsControl
{
    public static readonly StyledProperty<CodexFieldOrientation> OrientationProperty =
        AvaloniaProperty.Register<CodexFieldGroup, CodexFieldOrientation>(nameof(Orientation), CodexFieldOrientation.Vertical);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexFieldGroup, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> HasItemsProperty =
        AvaloniaProperty.Register<CodexFieldGroup, bool>(nameof(HasItems));

    static CodexFieldGroup()
    {
        OrientationProperty.Changed.AddClassHandler<CodexFieldGroup>((group, _) => group.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexFieldGroup>((group, _) => group.SyncClasses());
    }

    public CodexFieldGroup()
    {
        Focusable = false;
        ItemsView.CollectionChanged += OnItemsChanged;
        SyncClasses();
    }

    public CodexFieldOrientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool HasItems => GetValue(HasItemsProperty);

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        SyncChild(container);
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncClasses();
    }

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        CodexField.SetOrientationClasses(Classes, Orientation);
        SetValue(HasItemsProperty, ItemsView.Count > 0);
        Classes.Set("field-group", true);
        Classes.Set("has-items", HasItems);
        Classes.Set("empty", !HasItems);

        foreach (var item in ItemsView.OfType<Control>())
        {
            SyncChild(item);
        }
    }

    private void SyncChild(Control control)
    {
        control.Classes.Set("field-group-item", true);

        if (control is CodexField field)
        {
            field.SetCurrentValue(CodexField.SizeProperty, Size);
        }
    }
}

public class CodexFieldSet : ItemsControl
{
    public static readonly StyledProperty<object?> LegendProperty =
        AvaloniaProperty.Register<CodexFieldSet, object?>(nameof(Legend));

    public static readonly StyledProperty<object?> DescriptionProperty =
        AvaloniaProperty.Register<CodexFieldSet, object?>(nameof(Description));

    public static readonly StyledProperty<CodexFieldOrientation> OrientationProperty =
        AvaloniaProperty.Register<CodexFieldSet, CodexFieldOrientation>(nameof(Orientation), CodexFieldOrientation.Vertical);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexFieldSet, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> HasLegendProperty =
        AvaloniaProperty.Register<CodexFieldSet, bool>(nameof(HasLegend));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexFieldSet, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasItemsProperty =
        AvaloniaProperty.Register<CodexFieldSet, bool>(nameof(HasItems));

    static CodexFieldSet()
    {
        LegendProperty.Changed.AddClassHandler<CodexFieldSet>((fieldSet, _) => fieldSet.SyncClasses());
        DescriptionProperty.Changed.AddClassHandler<CodexFieldSet>((fieldSet, _) => fieldSet.SyncClasses());
        OrientationProperty.Changed.AddClassHandler<CodexFieldSet>((fieldSet, _) => fieldSet.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexFieldSet>((fieldSet, _) => fieldSet.SyncClasses());
    }

    public CodexFieldSet()
    {
        Focusable = false;
        ItemsView.CollectionChanged += OnItemsChanged;
        SyncClasses();
    }

    public object? Legend
    {
        get => GetValue(LegendProperty);
        set => SetValue(LegendProperty, value);
    }

    public object? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public CodexFieldOrientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool HasLegend => GetValue(HasLegendProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasItems => GetValue(HasItemsProperty);

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        SyncChild(container);
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncClasses();
    }

    private void SyncClasses()
    {
        var hasLegend = CodexFieldSlotState.HasValue(Legend);
        var hasDescription = CodexFieldSlotState.HasValue(Description);

        SetValue(HasLegendProperty, hasLegend);
        SetValue(HasDescriptionProperty, hasDescription);
        SetValue(HasItemsProperty, ItemsView.Count > 0);

        CodexClassSync.SetSize(Classes, Size);
        CodexField.SetOrientationClasses(Classes, Orientation);
        Classes.Set("field-set", true);
        Classes.Set("has-legend", hasLegend);
        Classes.Set("has-description", hasDescription);
        Classes.Set("has-items", HasItems);
        Classes.Set("empty", !HasItems);

        foreach (var item in ItemsView.OfType<Control>())
        {
            SyncChild(item);
        }
    }

    private void SyncChild(Control control)
    {
        control.Classes.Set("field-set-item", true);

        if (control is CodexField field)
        {
            field.SetCurrentValue(CodexField.SizeProperty, Size);
        }
    }
}

public class CodexFieldLegend : ContentControl
{
    public static readonly StyledProperty<CodexFieldLegendVariant> VariantProperty =
        AvaloniaProperty.Register<CodexFieldLegend, CodexFieldLegendVariant>(nameof(Variant), CodexFieldLegendVariant.Legend);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexFieldLegend, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexFieldLegend, bool>(nameof(HasContent));

    static CodexFieldLegend()
    {
        VariantProperty.Changed.AddClassHandler<CodexFieldLegend>((legend, _) => legend.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexFieldLegend>((legend, _) => legend.SyncClasses());
        ContentProperty.Changed.AddClassHandler<CodexFieldLegend>((legend, _) => legend.SyncClasses());
    }

    public CodexFieldLegend()
    {
        SyncClasses();
    }

    public CodexFieldLegendVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool HasContent => GetValue(HasContentProperty);

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        SetValue(HasContentProperty, CodexFieldSlotState.HasValue(Content));
        Classes.Set("field-legend", true);
        Classes.Set("variant-legend", Variant == CodexFieldLegendVariant.Legend);
        Classes.Set("variant-label", Variant == CodexFieldLegendVariant.Label);
        Classes.Set("has-content", HasContent);
    }
}

public class CodexFieldContent : ContentControl
{
    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexFieldContent, bool>(nameof(HasContent));

    static CodexFieldContent()
    {
        ContentProperty.Changed.AddClassHandler<CodexFieldContent>((content, _) => content.SyncClasses());
    }

    public CodexFieldContent()
    {
        SyncClasses();
    }

    public bool HasContent => GetValue(HasContentProperty);

    private void SyncClasses()
    {
        SetValue(HasContentProperty, CodexFieldSlotState.HasValue(Content));
        Classes.Set("field-content", true);
        Classes.Set("has-content", HasContent);
    }
}

public class CodexFieldTitle : ContentControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexFieldTitle, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexFieldTitle, bool>(nameof(HasContent));

    static CodexFieldTitle()
    {
        SizeProperty.Changed.AddClassHandler<CodexFieldTitle>((title, _) => title.SyncClasses());
        ContentProperty.Changed.AddClassHandler<CodexFieldTitle>((title, _) => title.SyncClasses());
    }

    public CodexFieldTitle()
    {
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool HasContent => GetValue(HasContentProperty);

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        SetValue(HasContentProperty, CodexFieldSlotState.HasValue(Content));
        Classes.Set("field-title", true);
        Classes.Set("has-content", HasContent);
    }
}

public class CodexFieldDescription : ContentControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexFieldDescription, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexFieldDescription, bool>(nameof(HasContent));

    static CodexFieldDescription()
    {
        SizeProperty.Changed.AddClassHandler<CodexFieldDescription>((description, _) => description.SyncClasses());
        ContentProperty.Changed.AddClassHandler<CodexFieldDescription>((description, _) => description.SyncClasses());
    }

    public CodexFieldDescription()
    {
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool HasContent => GetValue(HasContentProperty);

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        SetValue(HasContentProperty, CodexFieldSlotState.HasValue(Content));
        Classes.Set("field-description", true);
        Classes.Set("has-content", HasContent);
    }
}

public class CodexFieldSeparator : ContentControl
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexFieldSeparator, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexFieldSeparator, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexFieldSeparator, bool>(nameof(HasContent));

    static CodexFieldSeparator()
    {
        OrientationProperty.Changed.AddClassHandler<CodexFieldSeparator>((separator, _) => separator.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexFieldSeparator>((separator, _) => separator.SyncClasses());
        ContentProperty.Changed.AddClassHandler<CodexFieldSeparator>((separator, _) => separator.SyncClasses());
    }

    public CodexFieldSeparator()
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

    public bool HasContent => GetValue(HasContentProperty);

    private void SyncClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        SetValue(HasContentProperty, CodexFieldSlotState.HasValue(Content));
        Classes.Set("field-separator", true);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("has-content", HasContent);
    }
}

public class CodexFieldError : ItemsControl
{
    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<CodexFieldError, string?>(nameof(Message));

    public static readonly StyledProperty<bool> HasMessageProperty =
        AvaloniaProperty.Register<CodexFieldError, bool>(nameof(HasMessage));

    public static readonly StyledProperty<bool> HasItemsProperty =
        AvaloniaProperty.Register<CodexFieldError, bool>(nameof(HasItems));

    static CodexFieldError()
    {
        MessageProperty.Changed.AddClassHandler<CodexFieldError>((error, _) => error.SyncClasses());
    }

    public CodexFieldError()
    {
        Focusable = false;
        ItemsView.CollectionChanged += OnItemsChanged;
        SyncClasses();
    }

    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public bool HasMessage => GetValue(HasMessageProperty);

    public bool HasItems => GetValue(HasItemsProperty);

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncClasses();
    }

    private void SyncClasses()
    {
        var hasMessage = !string.IsNullOrWhiteSpace(Message);
        SetValue(HasMessageProperty, hasMessage);
        SetValue(HasItemsProperty, ItemsView.Count > 0);
        Classes.Set("field-error", true);
        Classes.Set("has-message", hasMessage);
        Classes.Set("has-items", HasItems);
        Classes.Set("empty", !hasMessage && !HasItems);
    }
}

internal static class CodexFieldSlotState
{
    public static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
