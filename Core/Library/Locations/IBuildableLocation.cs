using Godot;

namespace Dose.Core.Library.Locations
{
    public interface IBuildableLocation : ILocation
    {
        public Map? Map { get; }
        public void OnBuild(Node node);
        ClockDirection DoorClockDirection { get; }
    }
}