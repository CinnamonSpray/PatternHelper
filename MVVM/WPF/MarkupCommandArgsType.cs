
namespace PatternHelper.MVVM.WPF
{
    public interface ITargetContext<T>
    {
        T DataContext { get; }
    }
}
