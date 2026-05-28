using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public class CodexEmptyState : ContentControl
{
    private CodexButton? _actionButton;
    private CodexButton? _secondaryActionButton;
    private ICommand? _subscribedActionCommand;
    private ICommand? _subscribedSecondaryActionCommand;

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CodexEmptyState, object?>(nameof(Icon));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CodexEmptyState, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<CodexEmptyState, string?>(nameof(Description));

    public static readonly StyledProperty<object?> ActionProperty =
        AvaloniaProperty.Register<CodexEmptyState, object?>(nameof(Action));

    public static readonly StyledProperty<object?> SecondaryActionProperty =
        AvaloniaProperty.Register<CodexEmptyState, object?>(nameof(SecondaryAction));

    public static readonly StyledProperty<ICommand?> ActionCommandProperty =
        AvaloniaProperty.Register<CodexEmptyState, ICommand?>(nameof(ActionCommand));

    public static readonly StyledProperty<object?> ActionCommandParameterProperty =
        AvaloniaProperty.Register<CodexEmptyState, object?>(nameof(ActionCommandParameter));

    public static readonly StyledProperty<ICommand?> SecondaryActionCommandProperty =
        AvaloniaProperty.Register<CodexEmptyState, ICommand?>(nameof(SecondaryActionCommand));

    public static readonly StyledProperty<object?> SecondaryActionCommandParameterProperty =
        AvaloniaProperty.Register<CodexEmptyState, object?>(nameof(SecondaryActionCommandParameter));

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexEmptyState, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexEmptyState, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<CodexEmptyState, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<CodexEmptyState, bool>(nameof(HasIcon));

    public static readonly StyledProperty<bool> HasTitleProperty =
        AvaloniaProperty.Register<CodexEmptyState, bool>(nameof(HasTitle));

    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<CodexEmptyState, bool>(nameof(HasDescription));

    public static readonly StyledProperty<bool> HasHeaderProperty =
        AvaloniaProperty.Register<CodexEmptyState, bool>(nameof(HasHeader));

    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<CodexEmptyState, bool>(nameof(HasContent));

    public static readonly StyledProperty<bool> HasActionProperty =
        AvaloniaProperty.Register<CodexEmptyState, bool>(nameof(HasAction));

    public static readonly StyledProperty<bool> HasSecondaryActionProperty =
        AvaloniaProperty.Register<CodexEmptyState, bool>(nameof(HasSecondaryAction));

    public static readonly StyledProperty<bool> HasActionsProperty =
        AvaloniaProperty.Register<CodexEmptyState, bool>(nameof(HasActions));

    public static readonly StyledProperty<bool> CanExecuteActionProperty =
        AvaloniaProperty.Register<CodexEmptyState, bool>(nameof(CanExecuteAction), true);

    public static readonly StyledProperty<bool> CanExecuteSecondaryActionProperty =
        AvaloniaProperty.Register<CodexEmptyState, bool>(nameof(CanExecuteSecondaryAction), true);

    static CodexEmptyState()
    {
        IconProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, _) => emptyState.SyncSlots());
        TitleProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, _) => emptyState.SyncSlots());
        DescriptionProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, _) => emptyState.SyncSlots());
        ContentProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, _) => emptyState.SyncSlots());
        ActionProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, _) => emptyState.SyncSlots());
        SecondaryActionProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, _) => emptyState.SyncSlots());
        ActionCommandProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, args) => emptyState.OnActionCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        ActionCommandParameterProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, _) => emptyState.SyncActionStates());
        SecondaryActionCommandProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, args) => emptyState.OnSecondaryActionCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        SecondaryActionCommandParameterProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, _) => emptyState.SyncActionStates());
        VariantProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, _) => emptyState.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, _) => emptyState.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<CodexEmptyState>((emptyState, _) => emptyState.SyncActionStates());
    }

    public CodexEmptyState()
    {
        SyncSlots();
    }

    public event EventHandler<RoutedEventArgs>? ActionRequested;

    public event EventHandler<RoutedEventArgs>? SecondaryActionRequested;

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
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

    public object? Action
    {
        get => GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }

    public object? SecondaryAction
    {
        get => GetValue(SecondaryActionProperty);
        set => SetValue(SecondaryActionProperty, value);
    }

    public ICommand? ActionCommand
    {
        get => GetValue(ActionCommandProperty);
        set => SetValue(ActionCommandProperty, value);
    }

    public object? ActionCommandParameter
    {
        get => GetValue(ActionCommandParameterProperty);
        set => SetValue(ActionCommandParameterProperty, value);
    }

    public ICommand? SecondaryActionCommand
    {
        get => GetValue(SecondaryActionCommandProperty);
        set => SetValue(SecondaryActionCommandProperty, value);
    }

    public object? SecondaryActionCommandParameter
    {
        get => GetValue(SecondaryActionCommandParameterProperty);
        set => SetValue(SecondaryActionCommandParameterProperty, value);
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

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool HasIcon => GetValue(HasIconProperty);

    public bool HasTitle => GetValue(HasTitleProperty);

    public bool HasDescription => GetValue(HasDescriptionProperty);

    public bool HasHeader => GetValue(HasHeaderProperty);

    public bool HasContent => GetValue(HasContentProperty);

    public bool HasAction => GetValue(HasActionProperty);

    public bool HasSecondaryAction => GetValue(HasSecondaryActionProperty);

    public bool HasActions => GetValue(HasActionsProperty);

    public bool CanExecuteAction => GetValue(CanExecuteActionProperty);

    public bool CanExecuteSecondaryAction => GetValue(CanExecuteSecondaryActionProperty);

    public bool TryExecuteAction()
    {
        if (!CanExecuteAction)
        {
            return false;
        }

        ActionCommand?.Execute(ActionCommandParameter);
        ActionRequested?.Invoke(this, new RoutedEventArgs());
        return true;
    }

    public bool TryExecuteSecondaryAction()
    {
        if (!CanExecuteSecondaryAction)
        {
            return false;
        }

        SecondaryActionCommand?.Execute(SecondaryActionCommandParameter);
        SecondaryActionRequested?.Invoke(this, new RoutedEventArgs());
        return true;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_actionButton is not null)
        {
            _actionButton.Click -= OnActionClicked;
        }

        if (_secondaryActionButton is not null)
        {
            _secondaryActionButton.Click -= OnSecondaryActionClicked;
        }

        base.OnApplyTemplate(e);

        _actionButton = e.NameScope.Find<CodexButton>("PART_Action");
        _secondaryActionButton = e.NameScope.Find<CodexButton>("PART_SecondaryAction");

        if (_actionButton is not null)
        {
            _actionButton.Click += OnActionClicked;
        }

        if (_secondaryActionButton is not null)
        {
            _secondaryActionButton.Click += OnSecondaryActionClicked;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsEnabledProperty)
        {
            SyncActionStates();
        }
    }

    private void OnActionClicked(object? sender, RoutedEventArgs e)
    {
        TryExecuteAction();
    }

    private void OnSecondaryActionClicked(object? sender, RoutedEventArgs e)
    {
        TryExecuteSecondaryAction();
    }

    private void OnActionCommandChanged(ICommand? oldCommand, ICommand? newCommand)
    {
        if (ReferenceEquals(oldCommand, newCommand))
        {
            return;
        }

        if (_subscribedActionCommand is not null)
        {
            _subscribedActionCommand.CanExecuteChanged -= OnActionCanExecuteChanged;
        }

        _subscribedActionCommand = newCommand;

        if (_subscribedActionCommand is not null)
        {
            _subscribedActionCommand.CanExecuteChanged += OnActionCanExecuteChanged;
        }

        SyncActionStates();
    }

    private void OnSecondaryActionCommandChanged(ICommand? oldCommand, ICommand? newCommand)
    {
        if (ReferenceEquals(oldCommand, newCommand))
        {
            return;
        }

        if (_subscribedSecondaryActionCommand is not null)
        {
            _subscribedSecondaryActionCommand.CanExecuteChanged -= OnSecondaryActionCanExecuteChanged;
        }

        _subscribedSecondaryActionCommand = newCommand;

        if (_subscribedSecondaryActionCommand is not null)
        {
            _subscribedSecondaryActionCommand.CanExecuteChanged += OnSecondaryActionCanExecuteChanged;
        }

        SyncActionStates();
    }

    private void OnActionCanExecuteChanged(object? sender, EventArgs e)
    {
        SyncActionStates();
    }

    private void OnSecondaryActionCanExecuteChanged(object? sender, EventArgs e)
    {
        SyncActionStates();
    }

    private void SyncSlots()
    {
        var hasTitle = !string.IsNullOrWhiteSpace(Title);
        var hasDescription = !string.IsNullOrWhiteSpace(Description);
        var hasAction = HasValue(Action);
        var hasSecondaryAction = HasValue(SecondaryAction);

        SetValue(HasIconProperty, HasValue(Icon));
        SetValue(HasTitleProperty, hasTitle);
        SetValue(HasDescriptionProperty, hasDescription);
        SetValue(HasHeaderProperty, hasTitle || hasDescription);
        SetValue(HasContentProperty, HasValue(Content));
        SetValue(HasActionProperty, hasAction);
        SetValue(HasSecondaryActionProperty, hasSecondaryAction);
        SetValue(HasActionsProperty, hasAction || hasSecondaryAction);
        SyncActionStates();
    }

    private void SyncActionStates()
    {
        SetValue(CanExecuteActionProperty, IsEnabled && !IsLoading && HasAction && (ActionCommand?.CanExecute(ActionCommandParameter) ?? true));
        SetValue(CanExecuteSecondaryActionProperty, IsEnabled && !IsLoading && HasSecondaryAction && (SecondaryActionCommand?.CanExecute(SecondaryActionCommandParameter) ?? true));
        SyncClasses();
    }

    private void SyncClasses()
    {
        var actionCommandBlocked = ActionCommand is not null && HasAction && IsEnabled && !IsLoading && !CanExecuteAction;
        var secondaryActionCommandBlocked = SecondaryActionCommand is not null && HasSecondaryAction && IsEnabled && !IsLoading && !CanExecuteSecondaryAction;

        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        Classes.Set("loading", IsLoading);
        Classes.Set("has-icon", HasIcon);
        Classes.Set("has-title", HasTitle);
        Classes.Set("has-description", HasDescription);
        Classes.Set("has-header", HasHeader);
        Classes.Set("has-content", HasContent);
        Classes.Set("has-action", HasAction);
        Classes.Set("has-secondary-action", HasSecondaryAction);
        Classes.Set("has-actions", HasActions);
        Classes.Set("can-action", CanExecuteAction);
        Classes.Set("can-secondary-action", CanExecuteSecondaryAction);
        Classes.Set("action-command-blocked", actionCommandBlocked);
        Classes.Set("secondary-action-command-blocked", secondaryActionCommandBlocked);
        Classes.Set("command-blocked", actionCommandBlocked || secondaryActionCommandBlocked);
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
