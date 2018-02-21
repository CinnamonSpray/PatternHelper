using System;
using System.Globalization;
using System.Windows.Data;

namespace PatternHelper.MVVM
{
    public interface IEventArgsConverter
    {
        object Convert(object value, object parameter);
    }

    public abstract class EventArgsConverterExtension<T> : LazyMarkup<T>, IEventArgsConverter where T : new()
    {
        public abstract object Convert(object value, object parameter);
    }

    public abstract class ValuesConverterExtension<T> : LazyMarkup<T>, IValueConverter where T : new()
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }

    public abstract class MultiValueConverterExtension<T> : LazyMarkup<T>, IMultiValueConverter where T : new()
    {
        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);
        public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);
    }
}
