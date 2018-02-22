using System;
using System.Windows;
using System.Windows.Input;

namespace PatternHelper.MVVM
{
    public class RelayCommand<T> : ICommand
    {
        private Action<T> _execute = null;
        private Predicate<T> _canExecute = null;

        private object TargetObject = null;

        public RelayCommand(Action<T> executeMethod)
        {
            _execute = executeMethod;
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute, object targetobject)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;

            TargetObject = targetobject;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)DefaultParameter(parameter));
        }

        public void Execute(object parameter)
        {
            _execute((T)DefaultParameter(parameter));
        }

        private object DefaultParameter(object original)
        {
            if (original == null && TargetObject is FrameworkElement)
                return (TargetObject as FrameworkElement).DataContext;

            else return original;    
        }
    }
}
