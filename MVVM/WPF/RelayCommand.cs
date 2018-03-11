using System;
using System.Windows.Input;

namespace PatternHelper.MVVM.WPF
{
    public class RelayCommand<T> : ICommand
    {
        private Action<T> _execute = null;
        private Predicate<T> _canExecute = null;

        public RelayCommand(Action<T> executeMethod)
        {
            _execute = executeMethod;
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}
