using System;
using System.Windows.Input;

namespace PatternHelper.MVVM.WPF.legacy
{
    public class TestRelayCommand<T1> : ICommand
    {
        private Action<(T1, object)> _execute = null;
        private Predicate<(T1, object)> _canExecute = null;

        private T1 _cmdid = default(T1);

        public TestRelayCommand(Action<(T1, object)> executeMethod)
        {
            _execute = executeMethod;
        }

        public TestRelayCommand(Action<(T1, object)> execute, Predicate<(T1, object)> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        public TestRelayCommand(Action<(T1, object)> execute, Predicate<(T1, object)> canExecute, T1 cmdid)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;

            _cmdid = cmdid;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((_cmdid, parameter));
        }

        public void Execute(object parameter)
        {
            _execute((_cmdid, parameter));
        }
    }
}
