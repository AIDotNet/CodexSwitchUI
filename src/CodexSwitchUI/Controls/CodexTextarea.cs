using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace CodexSwitchUI.Controls;

public class CodexTextarea : CodexTextBox
{
    static CodexTextarea()
    {
        MinLinesProperty.Changed.AddClassHandler<CodexTextarea>((textarea, _) => textarea.SyncTextareaClasses());
    }

    public CodexTextarea()
    {
        AcceptsReturn = true;
        MinLines = 3;
        TextWrapping = TextWrapping.Wrap;
        ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Disabled);
        ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Auto);
        SyncTextareaClasses();
    }

    private void SyncTextareaClasses()
    {
        Classes.Set("textarea", true);
        Classes.Set("textarea-tall", MinLines >= 5);
    }
}
