using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CodexSwitchUI.Controls;

public enum CodexDirectionMode
{
    LeftToRight,
    RightToLeft
}

public sealed class CodexDirectionChangedEventArgs(CodexDirectionMode oldDirection, CodexDirectionMode newDirection, FlowDirection flowDirection)
    : EventArgs
{
    public CodexDirectionMode OldDirection { get; } = oldDirection;

    public CodexDirectionMode NewDirection { get; } = newDirection;

    public FlowDirection FlowDirection { get; } = flowDirection;
}

public class CodexDirection : ContentControl
{
    public static readonly StyledProperty<CodexDirectionMode> DirectionProperty =
        AvaloniaProperty.Register<CodexDirection, CodexDirectionMode>(nameof(Direction), CodexDirectionMode.LeftToRight);

    static CodexDirection()
    {
        DirectionProperty.Changed.AddClassHandler<CodexDirection>((direction, args) => direction.OnDirectionChanged(args));
    }

    public CodexDirection()
    {
        SyncDirection();
    }

    public event EventHandler<CodexDirectionChangedEventArgs>? DirectionChanged;

    public CodexDirectionMode Direction
    {
        get => GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    public bool IsRightToLeft => Direction == CodexDirectionMode.RightToLeft;

    private void OnDirectionChanged(AvaloniaPropertyChangedEventArgs args)
    {
        var oldDirection = args.OldValue is CodexDirectionMode oldValue
            ? oldValue
            : CodexDirectionMode.LeftToRight;
        var flowDirection = SyncDirection();

        DirectionChanged?.Invoke(this, new CodexDirectionChangedEventArgs(oldDirection, Direction, flowDirection));
    }

    private FlowDirection SyncDirection()
    {
        var flowDirection = IsRightToLeft
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;

        FlowDirection = flowDirection;
        Classes.Set("direction", true);
        Classes.Set("direction-ltr", !IsRightToLeft);
        Classes.Set("direction-rtl", IsRightToLeft);

        return flowDirection;
    }
}
