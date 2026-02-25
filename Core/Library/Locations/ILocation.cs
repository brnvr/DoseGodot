using Godot;

namespace Dose.Core.Library.Locations
{
    public interface ILocation
    {
        public Map? Map { get; }
        IDoor SelectRandomDoor();
        void Clear(int keepLevels);
        public ClockDirection DoorsClockDirection { get; }
    }

    public interface ILocation<TLocation> : ILocation
    {
        TLocation Type { get; }
    }
}