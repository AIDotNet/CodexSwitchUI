using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public enum CodexBadgeActivationSource
{
    Programmatic,
    Pointer,
    Keyboard
}

[PseudoClasses(CodexFocusVisible.PseudoClass)]
public class CodexBadge : CodexFrame
{
    private ICommand? _subscribedCommand;

    public static readonly StyledProperty<CodexControlVariant> VariantProperty =
        AvaloniaProperty.Register<CodexBadge, CodexControlVariant>(nameof(Variant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexBadge, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<CodexControlVariant> StatusVariantProperty =
        AvaloniaProperty.Register<CodexBadge, CodexControlVariant>(nameof(StatusVariant), CodexControlVariant.Success);

    public static readonly StyledProperty<bool> IsStatusVisibleProperty =
        AvaloniaProperty.Register<CodexBadge, bool>(nameof(IsStatusVisible));

    public static readonly StyledProperty<bool> IsInteractiveProperty =
        AvaloniaProperty.Register<CodexBadge, bool>(nameof(IsInteractive));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<CodexBadge, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<CodexBadge, object?>(nameof(CommandParameter));

    static CodexBadge()
    {
        VariantProperty.Changed.AddClassHandler<CodexBadge>((badge, _) => badge.SyncClasses());
        SizeProperty.Changed.AddClassHandler<CodexBadge>((badge, _) => badge.SyncClasses());
        StatusVariantProperty.Changed.AddClassHandler<CodexBadge>((badge, _) => badge.SyncClasses());
        IsStatusVisibleProperty.Changed.AddClassHandler<CodexBadge>((badge, _) => badge.SyncClasses());
        IsInteractiveProperty.Changed.AddClassHandler<CodexBadge>((badge, _) => badge.SyncClasses());
        CommandProperty.Changed.AddClassHandler<CodexBadge>((badge, args) => badge.OnCommandChanged(args.OldValue as ICommand, args.NewValue as ICommand));
        CommandParameterProperty.Changed.AddClassHandler<CodexBadge>((badge, _) => badge.SyncClasses());
        IsEnabledProperty.Changed.AddClassHandler<CodexBadge>((badge, _) => badge.SyncClasses());
    }

    public CodexBadge()
    {
        SyncClasses();
    }

    public event EventHandler<CodexBadgeActivatedEventArgs>? Activated;

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

    public CodexControlVariant StatusVariant
    {
        get => GetValue(StatusVariantProperty);
        set => SetValue(StatusVariantProperty, value);
    }

    public bool IsStatusVisible
    {
        get => GetValue(IsStatusVisibleProperty);
        set => SetValue(IsStatusVisibleProperty, value);
    }

    public bool IsInteractive
    {
        get => GetValue(IsInteractiveProperty);
        set => SetValue(IsInteractiveProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool CanActivate => IsEnabled
                               && (IsInteractive || Command is not null)
                               && (Command?.CanExecute(CommandParameter) ?? true);

    public bool TryActivate()
    {
        return TryActivate(CodexBadgeActivationSource.Programmatic);
    }

    internal bool TryActivate(CodexBadgeActivationSource source)
    {
        if (!CanActivate)
        {
            return false;
        }

        Command?.Execute(CommandParameter);
        Activated?.Invoke(this, new CodexBadgeActivatedEventArgs(CommandParameter, source));
        return true;
    }

    internal bool TryHandlePointerActivation(PointerUpdateKind updateKind)
    {
        if (updateKind != PointerUpdateKind.LeftButtonReleased)
        {
            return false;
        }

        return TryActivate(CodexBadgeActivationSource.Pointer);
    }

    public bool TryHandleActivationKey(Key key)
    {
        if (key is not (Key.Enter or Key.Space))
        {
            return false;
        }

        return TryActivate(CodexBadgeActivationSource.Keyboard);
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

        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        if (CanActivate && updateKind == PointerUpdateKind.LeftButtonPressed)
        {
            Focus(NavigationMethod.Pointer, KeyModifiers.None);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        var updateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        if (TryHandlePointerActivation(updateKind))
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

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
            _subscribedCommand = null;
        }

        base.OnDetachedFromVisualTree(e);
    }

    private void SyncClasses()
    {
        CodexClassSync.SetVariant(Classes, Variant);
        CodexClassSync.SetSize(Classes, Size);
        SetStatusVariantClasses();
        var hasActivation = IsInteractive || Command is not null;
        Classes.Set("status-visible", IsStatusVisible);
        Classes.Set("interactive", hasActivation);
        Classes.Set("can-activate", CanActivate);
        Classes.Set("command-blocked", hasActivation && !CanActivate);
        SetCurrentValue(FocusableProperty, hasActivation && IsEnabled);
    }

    private void OnCommandChanged(ICommand? oldCommand, ICommand? newCommand)
    {
        if (ReferenceEquals(oldCommand, newCommand))
        {
            return;
        }

        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
        }

        _subscribedCommand = newCommand;

        if (_subscribedCommand is not null)
        {
            _subscribedCommand.CanExecuteChanged += OnCommandCanExecuteChanged;
        }

        SyncClasses();
    }

    private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
    {
        SyncClasses();
    }

    private void SetStatusVariantClasses()
    {
        Classes.Set("status-default", StatusVariant == CodexControlVariant.Default);
        Classes.Set("status-secondary", StatusVariant == CodexControlVariant.Secondary);
        Classes.Set("status-destructive", StatusVariant == CodexControlVariant.Destructive);
        Classes.Set("status-outline", StatusVariant == CodexControlVariant.Outline);
        Classes.Set("status-ghost", StatusVariant == CodexControlVariant.Ghost);
        Classes.Set("status-link", StatusVariant == CodexControlVariant.Link);
        Classes.Set("status-success", StatusVariant == CodexControlVariant.Success);
        Classes.Set("status-warning", StatusVariant == CodexControlVariant.Warning);
    }
}

public sealed class CodexBadgeActivatedEventArgs(
    object? commandParameter,
    CodexBadgeActivationSource source = CodexBadgeActivationSource.Programmatic) : EventArgs
{
    public object? CommandParameter { get; } = commandParameter;

    public CodexBadgeActivationSource Source { get; } = source;
}
