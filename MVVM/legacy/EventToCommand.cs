using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace PatternHelper.MVVM
{
    [MarkupExtensionReturnType(typeof(EventHandler))]
    public sealed class EventToCommandExtension : MarkupExtension
    {
        // private IRootObjectProvider Root { get; set; }

        public string Command { get; set; }

        public IEventArgsConverter CommandParameter { get; set; }

        public EventToCommandExtension() { }

        public EventToCommandExtension(string command)
        {
            Command = command;
        }

        public EventToCommandExtension(string command, IEventArgsConverter commandParameter)
        {
            Command = command;
            CommandParameter = commandParameter;
        }

        public override object ProvideValue(IServiceProvider sp)
        {
            // Root = sp.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

            var pvt = sp.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (pvt != null)
            {
                Type dlgType = null;

                var evt = pvt.TargetProperty as EventInfo;
                if (evt != null)
                {
                    dlgType = evt.EventHandlerType;
                }
            
                var mi = pvt.TargetProperty as MethodInfo;
                if (mi != null)
                {
                    dlgType = mi.GetParameters()[1].ParameterType;
                }
                
                if (dlgType != null)
                {
                    var doAction = GetType().GetMethod("DoAction", BindingFlags.NonPublic | BindingFlags.Instance);
                    return doAction.CreateDelegate(dlgType, this);
                }
            }
            return null;
        }

        private void DoAction(object sender, EventArgs e)
        {
            ICommand command = null;
            var dc = (sender as FrameworkElement).DataContext; ;
            
            // dc = (Root.RootObject as FrameworkElement).DataContext;

            if (Command != null)
            {
                command = (ICommand)ParsePropertyPath(dc, Command);
            }

            var cmdParams = CommandParameter != null ? CommandParameter.Convert(sender, e) : e;

            if (command != null && command.CanExecute(cmdParams))
            {
                command.Execute(cmdParams);
            }
        }

        private static object ParsePropertyPath(object target, string path)
        {
            var props = path.Split('.');
            foreach (var prop in props)
            {
                target = target.GetType().GetProperty(prop).GetValue(target);
            }
            return target;
        }
    }
}
