namespace Dose.Core.Library.Actors
{
    public interface IItem
    {
        void Destroy();
        ItemContext? Context { get; }
    }
}