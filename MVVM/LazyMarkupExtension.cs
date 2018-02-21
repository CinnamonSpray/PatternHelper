using System;
using System.Windows.Markup;

namespace PatternHelper.MVVM
{
    public class LazyMarkup<T> : MarkupExtension where T : new()
    {
        private static Lazy<T> _converter = new Lazy<T>(() => new T());

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter.Value;
        }
    }
}
