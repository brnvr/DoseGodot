using System.Linq;
using Dose.Core.ActorArgs;
using Dose.Core.Library.Actors;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class CustomRoom : Location
    {
        int _blockSize;
        [Export]
        ClockDirection _doorClockDirection = ClockDirection.Clockwise;

        public override LocationType? Type { get => LocationType.Room; }
        public override int BlockSize { get => _blockSize; }
        public override ClockDirection DoorClockDirection { get => _doorClockDirection; }

        public CustomRoom Initialize(int blockSize, LocationManager locationManager)
        {
            _blockSize = blockSize;

            foreach (var node in GetChildren())
            {
                if (node is not IInitializableActor<DoorContext, DoorArgs> door)
                {
                    continue;
                }

                door.Initialize(new DoorContext(), new DoorArgs
                {
                    Location = this,
                    OpenDirection = DoorClockDirection,
                    LocationManager = locationManager
                });
            }

            return this;
        }
    }
}
