using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

namespace PatternHelper.MVVM.WPF
{
    [ComVisible(false)]
    public abstract class MarkupCommandExtension<ArgsType> : MarkupExtension
    {
        private ProvideValues _PVS = null;

        private IEventArgsConverter _EvtArgsCvt = null;
        public IEventArgsConverter EvtArgsCvt { set { _EvtArgsCvt = value; } }

        public MarkupCommandExtension() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
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

                    case DependencyProperty _:
                        return new RelayCommand<object>(RelayExcute, RelayCanExcute);

                    default: break;
                }
            }

            return null;
        }

        protected virtual bool MarkupCommandCanExecute(ArgsType o)
        {
            return true;
        }

        protected abstract void MarkupCommandExecute(ArgsType o);

        private bool RelayCanExcute(object o)
        {
            return MarkupCommandCanExecute((ArgsType)DefaultParameter(o));
        }

        private void RelayExcute(object o)
        {
            MarkupCommandExecute((ArgsType)DefaultParameter(o));
        }

        private object DefaultParameter(object original)
        {
            if (original != null) return original;

            return (_PVS.TargetObject as FrameworkElement).DataContext ?? null;
        }

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

            if (RelayCanExcute(cmdParams))
            {
                RelayExcute(cmdParams);
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
