using System;
using System.Windows.Input;

namespace PatternHelper.MVVM.WPF
{
    public class RelayCommand<T1> : ICommand
    {
        private Action<T1> _execute = null;
        private Predicate<T1> _canExecute = null;

        public RelayCommand(Action<T1> executeMethod)
        {
            _execute = executeMethod;
        }

        public RelayCommand(Action<T1> execute, Predicate<T1> canExecute)
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
            return _canExecute == null ? true : _canExecute((T1)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T1)parameter);
        }
    }
}
