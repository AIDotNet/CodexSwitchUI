using Avalonia;
using Avalonia.Controls;

namespace CodexSwitchUI.Primitives;

public class CodexText : TextBlock
{
    public static readonly StyledProperty<CodexTextRole> RoleProperty =
        AvaloniaProperty.Register<CodexText, CodexTextRole>(nameof(Role), CodexTextRole.Body);

    static CodexText()
    {
        RoleProperty.Changed.AddClassHandler<CodexText>((text, _) => text.SyncClasses());
    }

    public CodexText()
    {
        SyncClasses();
    }

    public CodexTextRole Role
    {
        get => GetValue(RoleProperty);
        set => SetValue(RoleProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("role-title", Role == CodexTextRole.Title);
        Classes.Set("role-subtitle", Role == CodexTextRole.Subtitle);
        Classes.Set("role-body", Role == CodexTextRole.Body);
        Classes.Set("role-muted", Role == CodexTextRole.Muted);
        Classes.Set("role-code", Role == CodexTextRole.Code);
    }
}
