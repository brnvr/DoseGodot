using Dose.Core.Library.Actors;

namespace Dose.Core.Library.Locations
{
    public interface IDoor
    {
        ILocation? OtherLocation { get; set; }
        void Initialize(DoorContext context, ILocationManager locationManager, ILocation otherLocation);
    }
}