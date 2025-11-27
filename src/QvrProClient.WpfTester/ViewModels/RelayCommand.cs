using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QvrProClient.WpfTester.ViewModels;

public class RelayCommand : ICommand
{
    private readonly Func<object?, bool>? _canExecute;
    private readonly Func<object?, Task>? _executeAsync;
    private readonly Action<object?>? _execute;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = _ => execute();
        if (canExecute is not null)
        {
            _canExecute = _ => canExecute();
        }
    }

    public RelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
    {
        _executeAsync = _ => executeAsync();
        if (canExecute is not null)
        {
            _canExecute = _ => canExecute();
        }
    }

    public RelayCommand(Func<object?, Task> executeAsync, Func<object?, bool>? canExecute = null)
    {
        _executeAsync = executeAsync;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public async void Execute(object? parameter)
    {
        if (_executeAsync is not null)
        {
            await _executeAsync(parameter).ConfigureAwait(false);
        }
        else
        {
            _execute?.Invoke(parameter);
        }
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
