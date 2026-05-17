namespace Dose.Core.Library.Common
{
    public interface ILoader<T, TArgs>
    {
        T Load(TArgs loadArgs);
    }
}