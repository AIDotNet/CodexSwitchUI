using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public sealed class CodexItemActivatedEventArgs(object? commandParameter) : RoutedEventArgs
{
    public object? CommandParameter { get; } = commandParameter;
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexItem : ContentControl
{
    private ICommand? _subscribedActivateCommand;

    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<CodexItem, object?>(nameof(Header));

    public static readonly StyledProperty<object?> MediaProperty =
        AvaloniaProperty.Register<CodexItem, object?>(nameof(Media));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CodexItem, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexItem, string?>(nameof(Description));

    public static readonly StyledProperty<object?> ActionsProperty =
        AvaloniaProperty.Register<CodexItem, object?>(nameof(Actions));

    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<CodexItem, object?>(nameof(Footer));

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexItem, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexItem, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsInteractiveProperty =
        AvaloniaProperty.Register<CodexItem, bool>(nameof(IsInteractive));

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<CodexItem, bool>(nameof(IsSelected));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexItem, bool>(nameof(IsLoading));

    public static readonly StyledProperty<ICommand?> ActivateCommandProperty =
        AvaloniaProperty.Register<CodexItem, ICommand?>(nameof(ActivateCommand));

    public static readonly StyledProperty<object?> ActivateCommandParameterProperty =
        AvaloniaProperty.Register<CodexItem, object?>(nameof(ActivateCommandParameter));

    public static readonly StyledProperty<bool> HasHeaderProperty =
        AvaloniaProperty.Register<CodexItem, bool>(nameof(HasHeader));

    public static readonly StyledProperty<bool> HasMediaProperty =
        AvaloniaProperty.Register<CodexItem, bool>(nameof(HasMedia));

    public static readonly StyledProperty<bool> HasTitleProperty =
        AvaloniaProperty.Register<CodexItem, bool>(nameof(HasTitle));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexItem, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexItem, bool>(nameof(HasContent));

    public static readonly StyledProperty<bool> HasActionsProperty =
        AvaloniaProperty.Register<CodexItem, bool>(nameof(HasActions));

    public static readonly StyledProperty<bool> HasFooterProperty =
        AvaloniaProperty.Register<CodexItem, bool>(nameof(HasFooter));

    public static readonly StyledProperty<bool> CanActivateProperty =
        AvaloniaProperty.Register<CodexItem, bool>(nameof(CanActivate), true);

    static CodexItem()
    {
        HeaderProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncSlots());
        MediaProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncSlots());
        TitleProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncSlots());
        DescriptionProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncSlots());
        ContentProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncSlots());
        ActionsProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncSlots());
        FooterProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncSlots());
        VariantProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncClasses());
        IsInteractiveProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncClasses());
        IsSelectedProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncActivation());
        ActivateCommandProperty.Changed.AddClassHandler<CodexItem>((item, args) => item.OnActivateCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        ActivateCommandParameterProperty.Changed.AddClassHandler<CodexItem>((item, _) => item.SyncActivation());
    }

    public CodexItem()
    {
        SyncSlots();
        SyncActivation();
    }

    public event EventHandler<CodexItemActivatedEventArgs>? Activated;

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object? Media
    {
        get => GetValue(MediaProperty);
        set => SetValue(MediaProperty, value);
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

    public object? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
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

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public ICommand? ActivateCommand
    {
        get => GetValue(ActivateCommandProperty);
        set => SetValue(ActivateCommandProperty, value);
    }

    public object? ActivateCommandParameter
    {
        get => GetValue(ActivateCommandParameterProperty);
        set => SetValue(ActivateCommandParameterProperty, value);
    }

    public bool HasHeader => GetValue(HasHeaderProperty);

    public bool HasMedia => GetValue(HasMediaProperty);

    public bool HasTitle => GetValue(HasTitleProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasContent => GetValue(HasContentProperty);

    public bool HasActions => GetValue(HasActionsProperty);

    public bool HasFooter => GetValue(HasFooterProperty);

    public bool CanActivate => GetValue(CanActivateProperty);

    public bool TryActivate()
    {
        if (!IsInteractive || !CanActivate)
        {
            return false;
        }

        ActivateCommand?.Execute(ActivateCommandParameter);
        Activated?.Invoke(this, new CodexItemActivatedEventArgs(ActivateCommandParameter));
        return true;
    }

    public bool TryHandleActivationKey(Key key)
    {
        if (key is not (Key.Enter or Key.Space))
        {
            return false;
        }

        return TryActivate();
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, CodexFocusVisible.FromFocusChange(e));
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PseudoClasses.Set(CodexFocusVisible.PseudoClass, false);
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (e.Handled || ShouldIgnoreActivation(e.Source))
        {
            return;
        }

        if (TryActivate())
        {
            e.Handled = true;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (TryHandleActivationKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsEnabledProperty)
        {
            SyncActivation();
        }
    }

    private void OnActivateCommandChanged(ICommand? oldCommand, ICommand? newCommand)
    {
        if (ReferenceEquals(oldCommand, newCommand))
        {
            return;
        }

        if (_subscribedActivateCommand is not null)
        {
            _subscribedActivateCommand.CanExecuteChanged -= OnActivateCommandCanExecuteChanged;
        }

        _subscribedActivateCommand = newCommand;

        if (_subscribedActivateCommand is not null)
        {
            _subscribedActivateCommand.CanExecuteChanged += OnActivateCommandCanExecuteChanged;
        }

        SyncActivation();
    }

    private void OnActivateCommandCanExecuteChanged(object? sender, EventArgs e)
    {
        SyncActivation();
    }

    private void SyncSlots()
    {
        SetValue(HasHeaderProperty, Header is not null);
        SetValue(HasMediaProperty, Media is not null);
        SetValue(HasTitleProperty, HasText(Title));
        SetValue(HasDescriptionProperty, HasText(Description));
        SetValue(HasContentProperty, Content is not null);
        SetValue(HasActionsProperty, Actions is not null);
        SetValue(HasFooterProperty, Footer is not null);
        SyncClasses();
    }

    private void SyncActivation()
    {
        var canExecute = !IsLoading
            && IsEnabled
            && (ActivateCommand?.CanExecute(ActivateCommandParameter) ?? true);
        SetValue(CanActivateProperty, canExecute);
        SyncClasses();
    }

    private void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("item", true);
        Classes.Set("interactive", IsInteractive);
        Classes.Set("selected", IsSelected);
        Classes.Set("loading", IsLoading);
        Classes.Set("can-activate", CanActivate);
        Classes.Set("has-header", HasHeader);
        Classes.Set("has-media", HasMedia);
        Classes.Set("has-title", HasTitle);
        Classes.Set("has-description", HasDescription);
        Classes.Set("has-content", HasContent);
        Classes.Set("has-actions", HasActions);
        Classes.Set("has-footer", HasFooter);
        Focusable = IsInteractive && IsEnabled;
    }

    private static bool HasText(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    private static bool ShouldIgnoreActivation(object? source)
    {
        return source is Button or ToggleButton or TextBox or ComboBox or Slider or MenuItem;
    }
}

public class CodexItemGroup : ItemsControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexItemGroup, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexItemGroup, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<bool> IsInsetProperty =
        AvaloniaProperty.Register<CodexItemGroup, bool>(nameof(IsInset));

    static CodexItemGroup()
    {
        SizeProperty.Changed.AddClassHandler<CodexItemGroup>((group, _) => group.SyncClasses());
        VariantProperty.Changed.AddClassHandler<CodexItemGroup>((group, _) => group.SyncClasses());
        IsInsetProperty.Changed.AddClassHandler<CodexItemGroup>((group, _) => group.SyncClasses());
    }

    public CodexItemGroup()
    {
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public bool IsInset
    {
        get => GetValue(IsInsetProperty);
        set => SetValue(IsInsetProperty, value);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("item-group", true);
        Classes.Set("inset", IsInset);
    }
}

public class CodexItemMedia : ContentControl
{
    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexItemMedia, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexItemMedia, CodexControlVariant>(nameof(Variant), CodexControlVariant.Secondary);

    public static readonly StyledProperty<bool> IsImageProperty =
        AvaloniaProperty.Register<CodexItemMedia, bool>(nameof(IsImage));

    static CodexItemMedia()
    {
        SizeProperty.Changed.AddClassHandler<CodexItemMedia>((media, _) => media.SyncClasses());
        VariantProperty.Changed.AddClassHandler<CodexItemMedia>((media, _) => media.SyncClasses());
        IsImageProperty.Changed.AddClassHandler<CodexItemMedia>((media, _) => media.SyncClasses());
    }

    public CodexItemMedia()
    {
        SyncClasses();
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public CodexControlVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public bool IsImage
    {
        get => GetValue(IsImageProperty);
        set => SetValue(IsImageProperty, value);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("item-media", true);
        Classes.Set("image", IsImage);
    }
}

public class CodexItemHeader : ContentControl
{
}

public class CodexItemContent : ContentControl
{
}

public class CodexItemTitle : ContentControl
{
}

public class CodexItemDescription : ContentControl
{
}

public class CodexItemActions : ContentControl
{
}

public class CodexItemFooter : ContentControl
{
}

public class CodexItemSeparator : TemplatedControl
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CodexItemSeparator, Orientation>(nameof(Orientation), Orientation.Horizontal);

    static CodexItemSeparator()
    {
        OrientationProperty.Changed.AddClassHandler<CodexItemSeparator>((separator, _) => separator.SyncClasses());
    }

    public CodexItemSeparator()
    {
        SyncClasses();
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("item-separator", true);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
    }
}
