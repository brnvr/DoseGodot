using Dose.Core.Library.Locations;

namespace Dose.Core.Library.Interactivity
{
    public interface IDoorOpener
    {
        void OnDoorOpened(IDoor door);
    }
}