using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Windows.Input;

namespace CodexSwitchUI.Controls;

public class CodexAlertDialog : CodexDialog
{
    public static readonly StyledProperty<object?> MediaProperty =
        AvaloniaProperty.Register<CodexAlertDialog, object?>(nameof(Media));

    public static readonly StyledProperty<object?> CancelContentProperty =
        AvaloniaProperty.Register<CodexAlertDialog, object?>(nameof(CancelContent), "Cancel");

    public static readonly StyledProperty<object?> ActionContentProperty =
        AvaloniaProperty.Register<CodexAlertDialog, object?>(nameof(ActionContent), "Continue");

    public static readonly StyledProperty<ICommand?> CancelCommandProperty =
        AvaloniaProperty.Register<CodexAlertDialog, ICommand?>(nameof(CancelCommand));

    public static readonly StyledProperty<ICommand?> ActionCommandProperty =
        AvaloniaProperty.Register<CodexAlertDialog, ICommand?>(nameof(ActionCommand));

    public static readonly StyledProperty<CodexControlVariant> ActionVariantProperty =
        AvaloniaProperty.Register<CodexAlertDialog, CodexControlVariant>(nameof(ActionVariant), CodexControlVariant.Default);

    public static readonly StyledProperty<CodexControlSize> SizeProperty =
        AvaloniaProperty.Register<CodexAlertDialog, CodexControlSize>(nameof(Size), CodexControlSize.Medium);

    public static readonly StyledProperty<bool> IsCancelLoadingProperty =
        AvaloniaProperty.Register<CodexAlertDialog, bool>(nameof(IsCancelLoading));

    public static readonly StyledProperty<bool> IsActionLoadingProperty =
        AvaloniaProperty.Register<CodexAlertDialog, bool>(nameof(IsActionLoading));

    public static readonly StyledProperty<bool> CloseOnCancelProperty =
        AvaloniaProperty.Register<CodexAlertDialog, bool>(nameof(CloseOnCancel), true);

    public static readonly StyledProperty<bool> CloseOnActionProperty =
        AvaloniaProperty.Register<CodexAlertDialog, bool>(nameof(CloseOnAction), true);

    public static readonly StyledProperty<bool> FocusCancelOnOpenProperty =
        AvaloniaProperty.Register<CodexAlertDialog, bool>(nameof(FocusCancelOnOpen), true);

    public static readonly StyledProperty<bool> HasMediaProperty =
        AvaloniaProperty.Register<CodexAlertDialog, bool>(nameof(HasMedia));

    public static readonly StyledProperty<bool> HasCancelContentProperty =
        AvaloniaProperty.Register<CodexAlertDialog, bool>(nameof(HasCancelContent));

    public static readonly StyledProperty<bool> HasActionContentProperty =
        AvaloniaProperty.Register<CodexAlertDialog, bool>(nameof(HasActionContent));

    private readonly AlertDialogPartCommand _cancelDialogCommand;
    private readonly AlertDialogPartCommand _actionDialogCommand;
    private CodexButton? _cancelButton;
    private bool _isCancelFocusQueued;

    static CodexAlertDialog()
    {
        MediaProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) => dialog.SyncAlertDialogClasses());
        CancelContentProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) => dialog.SyncAlertDialogClasses());
        ActionContentProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) => dialog.SyncAlertDialogClasses());
        CancelCommandProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) => dialog.RaisePartCommandStateChanged());
        ActionCommandProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) => dialog.RaisePartCommandStateChanged());
        ActionVariantProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) => dialog.SyncAlertDialogClasses());
        SizeProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) => dialog.SyncAlertDialogClasses());
        IsCancelLoadingProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) =>
        {
            dialog.SyncAlertDialogClasses();
            dialog.RaisePartCommandStateChanged();
        });
        IsActionLoadingProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) =>
        {
            dialog.SyncAlertDialogClasses();
            dialog.RaisePartCommandStateChanged();
        });
        CloseOnCancelProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) => dialog.SyncAlertDialogClasses());
        CloseOnActionProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) => dialog.SyncAlertDialogClasses());
        DismissOnOutsidePointerProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) => dialog.SyncAlertDialogClasses());
        IsModalProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) =>
        {
            if (!dialog.IsModal)
            {
                dialog.IsModal = true;
                return;
            }

            dialog.SyncAlertDialogClasses();
        });
        FocusCancelOnOpenProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) =>
        {
            dialog.SyncAlertDialogClasses();
            dialog.QueueCancelFocus();
        });
        IsOpenProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) =>
        {
            dialog.SyncAlertDialogClasses();
            dialog.RaisePartCommandStateChanged();
            dialog.QueueCancelFocus();
        });
        InputElement.IsEnabledProperty.Changed.AddClassHandler<CodexAlertDialog>((dialog, _) => dialog.RaisePartCommandStateChanged());
    }

    public CodexAlertDialog()
    {
        _cancelDialogCommand = new AlertDialogPartCommand(this, static dialog => dialog.CanCancel(), static dialog => dialog.Cancel());
        _actionDialogCommand = new AlertDialogPartCommand(this, static dialog => dialog.CanAction(), static dialog => dialog.Confirm());
        IsCloseVisible = false;
        IsModal = true;
        DismissOnOutsidePointer = false;
        SyncAlertDialogClasses();
    }

    public object? Media
    {
        get => GetValue(MediaProperty);
        set => SetValue(MediaProperty, value);
    }

    public object? CancelContent
    {
        get => GetValue(CancelContentProperty);
        set => SetValue(CancelContentProperty, value);
    }

    public object? ActionContent
    {
        get => GetValue(ActionContentProperty);
        set => SetValue(ActionContentProperty, value);
    }

    public ICommand? CancelCommand
    {
        get => GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    public ICommand? ActionCommand
    {
        get => GetValue(ActionCommandProperty);
        set => SetValue(ActionCommandProperty, value);
    }

    public CodexControlVariant ActionVariant
    {
        get => GetValue(ActionVariantProperty);
        set => SetValue(ActionVariantProperty, value);
    }

    public CodexControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public bool IsCancelLoading
    {
        get => GetValue(IsCancelLoadingProperty);
        set => SetValue(IsCancelLoadingProperty, value);
    }

    public bool IsActionLoading
    {
        get => GetValue(IsActionLoadingProperty);
        set => SetValue(IsActionLoadingProperty, value);
    }

    public bool CloseOnCancel
    {
        get => GetValue(CloseOnCancelProperty);
        set => SetValue(CloseOnCancelProperty, value);
    }

    public bool CloseOnAction
    {
        get => GetValue(CloseOnActionProperty);
        set => SetValue(CloseOnActionProperty, value);
    }

    public bool FocusCancelOnOpen
    {
        get => GetValue(FocusCancelOnOpenProperty);
        set => SetValue(FocusCancelOnOpenProperty, value);
    }

    public bool HasMedia => GetValue(HasMediaProperty);

    public bool HasCancelContent => GetValue(HasCancelContentProperty);

    public bool HasActionContent => GetValue(HasActionContentProperty);

    public ICommand CancelDialogCommand => _cancelDialogCommand;

    public ICommand ActionDialogCommand => _actionDialogCommand;

    public bool CanCancel()
    {
        return IsOpen
            && IsEnabled
            && !IsCancelLoading
            && (CancelCommand?.CanExecute(null) ?? true);
    }

    public bool CanAction()
    {
        return IsOpen
            && IsEnabled
            && !IsActionLoading
            && (ActionCommand?.CanExecute(null) ?? true);
    }

    public bool Cancel()
    {
        if (!CanCancel())
        {
            return false;
        }

        CancelCommand?.Execute(null);
        return CloseOnCancel ? Dismiss() : true;
    }

    public bool Confirm()
    {
        if (!CanAction())
        {
            return false;
        }

        ActionCommand?.Execute(null);
        return CloseOnAction ? Dismiss() : true;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _cancelButton = e.NameScope.Find<CodexButton>("PART_Cancel");
        QueueCancelFocus();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        QueueCancelFocus();
    }

    private void SyncAlertDialogClasses()
    {
        CodexClassSync.SetSize(Classes, Size);
        SetValue(HasMediaProperty, HasValue(Media));
        SetValue(HasCancelContentProperty, HasValue(CancelContent));
        SetValue(HasActionContentProperty, HasValue(ActionContent));
        Classes.Set("alert-dialog", true);
        Classes.Set("modal", IsModal);
        Classes.Set("non-modal", !IsModal);
        Classes.Set("response-required", !DismissOnOutsidePointer);
        Classes.Set("outside-dismissable", DismissOnOutsidePointer);
        Classes.Set("focus-cancel", FocusCancelOnOpen);
        Classes.Set("has-media", HasMedia);
        Classes.Set("has-cancel", HasCancelContent);
        Classes.Set("has-alert-action", HasActionContent);
        Classes.Set("cancel-loading", IsCancelLoading);
        Classes.Set("action-loading", IsActionLoading);
        Classes.Set("loading", IsCancelLoading || IsActionLoading);
        Classes.Set("close-on-cancel", CloseOnCancel);
        Classes.Set("close-on-action", CloseOnAction);
        Classes.Set("action-destructive", ActionVariant == CodexControlVariant.Destructive);
    }

    private void RaisePartCommandStateChanged()
    {
        _cancelDialogCommand?.RaiseCanExecuteChanged();
        _actionDialogCommand?.RaiseCanExecuteChanged();
    }

    private void QueueCancelFocus()
    {
        if (!IsOpen || !FocusCancelOnOpen || _cancelButton is null || _isCancelFocusQueued || !this.IsAttachedToVisualTree())
        {
            return;
        }

        _isCancelFocusQueued = true;
        Dispatcher.UIThread.Post(() =>
        {
            _isCancelFocusQueued = false;
            if (IsOpen && FocusCancelOnOpen && _cancelButton is { Focusable: true, IsEffectivelyEnabled: true, IsEffectivelyVisible: true })
            {
                _cancelButton.Focus(NavigationMethod.Tab, KeyModifiers.None);
            }
        }, DispatcherPriority.Loaded);
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }

    private sealed class AlertDialogPartCommand(
        CodexAlertDialog dialog,
        Func<CodexAlertDialog, bool> canExecute,
        Func<CodexAlertDialog, bool> execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecute(dialog);
        }

        public void Execute(object? parameter)
        {
            execute(dialog);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
