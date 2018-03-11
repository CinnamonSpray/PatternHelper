using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

namespace PatternHelper.MVVM.WPF.legacy
{
    [ComVisible(false)]
    public class TestCommandExtension<TCmdID> : MarkupExtension
    {
        private static RelayCommand<object> _RelayCmd;

        private IEventArgsConverter _EvtArgsCvt;
        public IEventArgsConverter EvtArgsCvt { set { _EvtArgsCvt = value; } }

        private static Dictionary<TCmdID, (Action<object>, Predicate<object>)> _Cmds;

        static TestCommandExtension()
        {
            _RelayCmd = new RelayCommand<object>(RelayExcute, RelayCanExcute);
            _Cmds = new Dictionary<TCmdID, (Action<object>, Predicate<object>)>();
        }

        public TestCommandExtension() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            var methodInfo = GetType().GetMethod("DoAction", BindingFlags.NonPublic | BindingFlags.Instance);

            return MatchCommand(pvt, methodInfo, this);
        }

        public static object MatchCommand(IProvideValueTarget pvt, MethodInfo methodInfo, object self)
        {
            if (pvt != null && methodInfo != null)
            {
                switch (pvt.TargetProperty)
                {
                    case EventInfo evt:
                        return EventToCommand(evt.EventHandlerType, methodInfo, self);

                    case MethodInfo mvt:
                        return EventToCommand(mvt.GetParameters()[1].ParameterType, methodInfo, self);

                    case DependencyProperty cmd when cmd.Name == "Command":
                        return _RelayCmd;

                    default: break;
                }
            }

            return null;
        }

        private static bool RelayCanExcute(object o)
        {
            return true;
        }

        private static void RelayExcute(object o)
        {

        }

        #region EventToCommand
        private static Delegate EventToCommand(Type dlgType, MethodInfo methodInfo, object self)
        {
            if (dlgType == null) return null;

            return methodInfo.CreateDelegate(dlgType, self);
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
    }
}
