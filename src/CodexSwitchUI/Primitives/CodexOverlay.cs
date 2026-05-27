using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using System.Windows.Input;

namespace CodexSwitchUI.Primitives;

public class CodexOverlay : ContentControl
{
    private ContentPresenter? _contentPresenter;

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CodexOverlay, bool>(nameof(IsOpen), true);

    public static readonly StyledProperty<IBrush?> ScrimBrushProperty =
        AvaloniaProperty.Register<CodexOverlay, IBrush?>(nameof(ScrimBrush));

    public static readonly StyledProperty<double> ScrimOpacityProperty =
        AvaloniaProperty.Register<CodexOverlay, double>(nameof(ScrimOpacity), 0.8);

    public static readonly StyledProperty<bool> IsScrimVisibleProperty =
        AvaloniaProperty.Register<CodexOverlay, bool>(nameof(IsScrimVisible), true);

    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<CodexOverlay, bool>(nameof(CloseOnEscape), true);

    public static readonly StyledProperty<bool> DismissOnOutsidePointerProperty =
        AvaloniaProperty.Register<CodexOverlay, bool>(nameof(DismissOnOutsidePointer), true);

    public static readonly StyledProperty<ICommand?> DismissCommandProperty =
        AvaloniaProperty.Register<CodexOverlay, ICommand?>(nameof(DismissCommand));

    static CodexOverlay()
    {
        IsOpenProperty.Changed.AddClassHandler<CodexOverlay>((overlay, _) => overlay.SyncOpenClasses());
        IsScrimVisibleProperty.Changed.AddClassHandler<CodexOverlay>((overlay, _) => overlay.SyncOpenClasses());
    }

    public CodexOverlay()
    {
        SyncOpenClasses();
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool CloseOnEscape
    {
        get => GetValue(CloseOnEscapeProperty);
        set => SetValue(CloseOnEscapeProperty, value);
    }

    public bool DismissOnOutsidePointer
    {
        get => GetValue(DismissOnOutsidePointerProperty);
        set => SetValue(DismissOnOutsidePointerProperty, value);
    }

    public ICommand? DismissCommand
    {
        get => GetValue(DismissCommandProperty);
        set => SetValue(DismissCommandProperty, value);
    }

    public IBrush? ScrimBrush
    {
        get => GetValue(ScrimBrushProperty);
        set => SetValue(ScrimBrushProperty, value);
    }

    public double ScrimOpacity
    {
        get => GetValue(ScrimOpacityProperty);
        set => SetValue(ScrimOpacityProperty, value);
    }

    public bool IsScrimVisible
    {
        get => GetValue(IsScrimVisibleProperty);
        set => SetValue(IsScrimVisibleProperty, value);
    }

    public bool Dismiss()
    {
        if (!IsOpen)
        {
            return false;
        }

        IsOpen = false;

        if (DismissCommand?.CanExecute(null) == true)
        {
            DismissCommand.Execute(null);
        }

        return true;
    }

    internal bool TryHandleDismissKey(Key key)
    {
        return key == Key.Escape && CloseOnEscape && Dismiss();
    }

    internal bool TryDismissFromOutsidePointer(Visual? source)
    {
        if (!DismissOnOutsidePointer || IsPointerInsideContent(source))
        {
            return false;
        }

        return Dismiss();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _contentPresenter = e.NameScope.Find<ContentPresenter>("PART_Content");
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (TryHandleDismissKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (TryDismissFromOutsidePointer(e.Source as Visual))
        {
            e.Handled = true;
            return;
        }

        base.OnPointerPressed(e);
    }

    private void SyncOpenClasses()
    {
        Classes.Set("is-open", IsOpen);
        Classes.Set("is-closed", !IsOpen);
        Classes.Set("has-scrim", IsScrimVisible);
    }

    private bool IsPointerInsideContent(Visual? source)
    {
        if (_contentPresenter is null || source is null)
        {
            return false;
        }

        for (var current = source; current is not null; current = current.GetVisualParent())
        {
            if (ReferenceEquals(current, _contentPresenter))
            {
                return true;
            }
        }

        return false;
    }
}
