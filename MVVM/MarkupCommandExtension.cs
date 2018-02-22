using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

namespace PatternHelper.MVVM
{
    [ComVisible(false)]
    public abstract class MarkupCommandExtension<TypeClass, TypeArgs> : MarkupExtension
        where TypeClass : class, new()
    {
        public IEventArgsConverter ParameterConverter { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (pvt != null)
            {
                switch (pvt.TargetProperty)
                {
                    case EventInfo evt:
                        return EventToCommand(evt.EventHandlerType);

                    case MethodInfo mvt:
                        return EventToCommand(mvt.GetParameters()[1].ParameterType);

                    case DependencyProperty cmd:
                        return new RelayCommand<TypeArgs>(
                            MarkupCommandExecute, MarkupCommandCanExecute, pvt.TargetObject);

                    default: break;
                }
            }

            return null;
        }

        protected virtual bool MarkupCommandCanExecute(TypeArgs o)
        {
            return true;
        }

        protected abstract void MarkupCommandExecute(TypeArgs o);

        #region EventToCommand
        private Delegate EventToCommand(Type dlgType)
        {
            if (dlgType == null) return null;

            var doAction = GetType().BaseType.GetMethod("DoAction", BindingFlags.NonPublic | BindingFlags.Instance);
            return doAction.CreateDelegate(dlgType, this);
        }

        private void DoAction(object sender, EventArgs e)
        {
            var cmdParams = ParameterConverter != null ?
                ParameterConverter.Convert(sender, e) : null;

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
