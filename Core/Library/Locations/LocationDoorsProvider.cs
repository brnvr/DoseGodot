using System;
using System.Linq;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Control;
using Godot;
using static Dose.Core.Library.GeoPositioning;
using static Dose.Core.Library.Locations.MapCellWall;

namespace Dose.Core.Library.Locations
{
    public class LocationDoorsProvider(
        ILocationManager locationManager, IBuildableLocation location, PackedScene doorScene, int blockSize, IControlPlane controlPlane, GeoPositioning.ClockDirection clockDirection
    ) : LocationBuilder(location)
    {
        public int BlockSize { get; } = blockSize;
        public PackedScene DoorScene { get; } = doorScene;
        public IControlPlane ControlPlane { get; } = controlPlane;
        public ILocationManager LocationManager { get; } = locationManager;
        public GeoPositioning.ClockDirection ClockDirection { get; } = clockDirection;
        
        protected override void OnBuildCell(Vector2I position)
        {
            if (Location.Map == null)
            {
                throw new Exception("Location doesn't have a defined Map.");
            }

            var cell = Location.Map.Get(position);
    
            foreach (var direction in AllDirections().Where(direction => cell.GetWall(direction) != null && cell.GetWall(direction)?.DoorType != MapCellDoorType.None))
            {
                AddDoor(position, direction);
            }
        }

        void AddDoor(Vector2I point, CardinalDirection direction)
        {
            if (Location.Map == null)
            {
                throw new Exception("Location doesn't have a defined Map.");
            }

            var nextDirection = NextDirection(direction, ClockDirection);
            var pointDir = MoveTo(MoveTo(point, direction, 0.5f), nextDirection, -0.11275f);

            var position = new Vector3(pointDir.X - Location.Map.HorizontalCells / 2, 0, pointDir.Y - Location.Map.VerticalCells / 2) * BlockSize;
            var rotation = DirectionToRadians(nextDirection);

            var door = (Node3D)DoorScene.Instantiate();

            Location.OnBuild(door);

            door.Position = position;
            door.RotateY(rotation);

            if (door is IDoor initializableDoor)
            {
                var context = new DoorContext
                {
                    Number = ControlPlane.DoorNumberController?.PopRandomNumber()
                };

                initializableDoor.Initialize(context, LocationManager, Location);
            }
        }
    }
}