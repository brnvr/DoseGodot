using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.Location;
using Godot;

namespace Dose.Core.Library.Locations
{
    public interface IDoor : IPortal
    {
        DoorContext? Context { get; }
        ClockDirection OpenDirection { get; }
    }
}