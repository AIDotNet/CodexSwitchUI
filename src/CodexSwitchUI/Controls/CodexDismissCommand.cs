using System.Windows.Input;

namespace CodexSwitchUI.Controls;

internal sealed class CodexDismissCommand(Func<bool> dismiss) : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        dismiss();
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
