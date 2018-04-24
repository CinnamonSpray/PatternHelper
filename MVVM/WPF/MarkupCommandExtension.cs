using System;
using System.Collections.Generic;
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
                var context = _PVS?.TargetObject as FrameworkElement;

                if (context != null) return (T1)context.DataContext;

                return default(T1);
            }

            set
            {
                var context = _PVS?.TargetObject as FrameworkElement;

                if (context != null) context.DataContext = value;
            }
        }

        private IEventArgsConverter _EvtArgsCvt = null;
        public IEventArgsConverter EvtArgsCvt { set { _EvtArgsCvt = value; } }

        private object _Cmd = null;
        private static Dictionary<string, MarkupCommandExtension<T1, T2>> _Dic;

        static MarkupCommandExtension()
        {
            _Dic = new Dictionary<string, MarkupCommandExtension<T1, T2>>();
        }

        public MarkupCommandExtension()
        {
            if (!(_Dic.TryGetValue(GetType().Name, out _)))
                _Dic.Add(GetType().Name, this);
        }

        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            var pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (pvt != null)
            {
                if (_Dic.TryGetValue(GetType().Name, out MarkupCommandExtension<T1, T2> self))
                {
                    self._PVS = new ProvideValues(pvt.TargetObject, pvt.TargetProperty);

                    if (self._Cmd == null)
                    {
                        switch (_PVS.TargetProperty)
                        {
                            case EventInfo evt:
                                self._Cmd =  EventToCommand(evt.EventHandlerType);
                                break;

                            case MethodInfo mvt:
                                self._Cmd =  EventToCommand(mvt.GetParameters()[1].ParameterType);
                                break;

                            case DependencyProperty cmd when cmd.Name == "Command":
                                self._Cmd =  new RelayCommand<T2>(MarkupCommandExecute, MarkupCommandCanExecute);
                                break;

                            default: break;
                        }
                    }

                    return self._Cmd;
                }
                /*
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
                */
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
