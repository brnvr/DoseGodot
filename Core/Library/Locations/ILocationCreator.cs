using Godot;

namespace Dose.Core.Library.Locations
{
    public interface ILocationCreator
    {
        ILocation? CreateLocation(LocationType type, ClockDirection doorsClockDirection);
    }
}