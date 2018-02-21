using System;
using System.Windows.Input;

namespace PatternHelper.MVVM
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

        public bool CanExecute(object o)
        {
            return _canExecute == null ? true : _canExecute((T)o);
        }

        public void Execute(object o)
        {
            _execute((T)o);
        }
    }
}
