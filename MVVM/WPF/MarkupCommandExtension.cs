using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

namespace PatternHelper.MVVM.WPF
{
    [ComVisible(false)]
    public abstract class MarkupCommandExtension<T1, T2> : MarkupExtension
    {
        private ProvideValues _PVS = null;
        protected T1 DataContext
        {
            get
            {
                var context = _PVS.TargetObject as FrameworkElement;

                if (context != null) return (T1)context.DataContext;

                return default(T1);
            }

            set
            {
                var context = _PVS.TargetObject as FrameworkElement;

                if (context != null) context.DataContext = value;
            }
        }

        private IEventArgsConverter _EvtArgsCvt = null;
        public IEventArgsConverter EvtArgsCvt { set { _EvtArgsCvt = value; } }

        public MarkupCommandExtension() { }

        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            var pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (pvt != null)
            {
                _PVS = new ProvideValues(pvt.TargetObject, pvt.TargetProperty);

                switch (_PVS.TargetProperty)
                {
                    case EventInfo evt:
                        return EventToCommand(evt.EventHandlerType);

                    case MethodInfo mvt:
                        return EventToCommand(mvt.GetParameters()[1].ParameterType);

                    case DependencyProperty cmd when cmd.Name == "Command":
                        return new RelayCommand<T2>(MarkupCommandExecute, MarkupCommandCanExecute);

                    default: break;
                }
            }

            return null;
        }

        protected virtual bool MarkupCommandCanExecute(T2 args)
        {
            return true;
        }

        protected abstract void MarkupCommandExecute(T2 args);

        #region EventToCommand
        private Delegate EventToCommand(Type dlgType)
        {
            if (dlgType == null) return null;

            var doAction = GetType().BaseType.GetMethod("DoAction", BindingFlags.NonPublic | BindingFlags.Instance);
            return doAction.CreateDelegate(dlgType, this);
        }

        private void DoAction(object sender, EventArgs e)
        {
            var cmdParams = _EvtArgsCvt != null ? _EvtArgsCvt.Convert(sender, e) : null;

            if (MarkupCommandCanExecute((T2)cmdParams))
            {
                MarkupCommandExecute((T2)cmdParams);
            }
        }
        #endregion

        #region Private FieldType
        private class ProvideValues : IProvideValueTarget
        {
            public object TargetObject { get; private set; }

            public object TargetProperty { get; private set; }

            public ProvideValues(object tobj, object tproperty)
            {
                TargetObject = tobj;
                TargetProperty = tproperty;
            }
        }
        #endregion
    }
}
