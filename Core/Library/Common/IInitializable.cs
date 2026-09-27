namespace Dose.Core.Library.Common
{
    public interface IInitializable<in TInitialization>
    {
        void Initialize(TInitialization initialization);
    }
}
