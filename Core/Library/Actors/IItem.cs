using Dose.Core.Library.Interactivity;

namespace Dose.Core.Library.Actors
{
    public interface IItem
    {
        void Destroy();
        void BeginPickup(IPicker picker);
        ItemContext? Context { get; }
    }
}
