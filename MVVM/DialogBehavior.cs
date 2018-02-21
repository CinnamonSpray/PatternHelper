using System.Windows;

namespace PatternHelper.MVVM
{
    public abstract class DialogBehavior<T1, T2> where T1 : new() 
    {
        public static readonly DependencyProperty ShowProperty =
            DependencyProperty.RegisterAttached("Show", typeof(bool), 
                typeof(DialogBehavior<T1, T2>), new UIPropertyMetadata(false, OnOpenFlag));

        public static object GetShow(DependencyObject obj)
        {
            return obj.GetValue(ShowProperty);
        }

        public static void SetShow(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowProperty, value);
        }

        private static void OnOpenFlag(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!((bool)e.NewValue)) return;

            var behavior = e.Property.OwnerType.GetMethod("OnDialog");

            behavior.Invoke(new T1(), new object[] { (T2)(((FrameworkElement)obj).DataContext) });
        }

        public abstract void OnDialog(T2 context);
    }
}
