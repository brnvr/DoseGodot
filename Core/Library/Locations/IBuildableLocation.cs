using Godot;

namespace Dose.Core.Library.Locations
{
    public interface IBuildableLocation : ILocation
    {
        public Map? Map { get; }
        int BlockSize { get; }
        public void OnBuild(Node node);
        ClockDirection DoorClockDirection { get; }
    }
}
