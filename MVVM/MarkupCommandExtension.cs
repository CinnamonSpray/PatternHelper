using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace PatternHelper.MVVM
{
    [ComVisible(false)]
    public abstract class MarkupCommandExtension<TypeClass, TypeArgs> : MarkupExtension, ICommand
        where TypeClass : class, new()
    {
        public IEventArgsConverter ParameterConverter { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (pvt != null)
            {
                // EventToCommand
                var evt = pvt.TargetProperty as EventInfo;
                if (evt != null)
                {
                    return EventToCommand(evt.EventHandlerType);
                }

                var mvt = pvt.TargetProperty as MethodInfo;
                if (mvt != null)
                {
                    return EventToCommand(mvt.GetParameters()[1].ParameterType);
                }

                // ICommand
                var cmd = pvt.TargetProperty as DependencyProperty;
                if (cmd != null)
                {
                    // return new Lazy<T>(() => new T()).Value;
                    return new TypeClass();
                }
            }

            return null;
        }

        protected virtual bool MarkupCommandCanExecute(TypeArgs o)
        {
            return true;
        }

        protected abstract void MarkupCommandExecute(TypeArgs o);

        #region ICommand
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            MarkupCommandExecute((TypeArgs)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return MarkupCommandCanExecute((TypeArgs)parameter);
        }
        #endregion

        #region EventToCommand
        private Delegate EventToCommand(Type dlgType)
        {
            if (dlgType == null) return null;

            var doAction = GetType().BaseType.GetMethod("DoAction", BindingFlags.NonPublic | BindingFlags.Instance);
            return doAction.CreateDelegate(dlgType, this);
        }

        private void DoAction(object sender, EventArgs e)
        {
            var cmdParams = ParameterConverter != null ? ParameterConverter.Convert(sender, e) : e;

            if (MarkupCommandCanExecute((TypeArgs)cmdParams))
            {
                MarkupCommandExecute((TypeArgs)cmdParams);
            }
        }
        #endregion

        public MarkupCommandExtension() { }

        public MarkupCommandExtension(IEventArgsConverter parameterConverter)
        {
            ParameterConverter = parameterConverter;
        }
    }
}
