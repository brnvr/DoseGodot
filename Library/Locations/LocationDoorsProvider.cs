using System.Linq;
using Godot;
using static Dose.Library.GeoPositioning;

namespace Dose.Library.Locations
{
    public class LocationDoorsProvider(IBuildableLocation location, PackedScene doorScene, int blockSize) : LocationBuilder(location)
    {
        public int BlockSize { get; } = blockSize;
        public PackedScene DoorScene { get; } = doorScene;
        
        protected override void BuildCell(Vector2I position)
        {
            var cell = Location.Map.Get(position);
    
            foreach (var direction in AllDirections().Where(direction => cell.GetWall(direction)?.HasDoor == true))
            {
                AddDoor(position, direction);
            }
        }

        void AddDoor(Vector2I point, CardinalDirection direction)
        {
            var nextDirection = NextDirection(direction, GeoPositioning.ClockDirection.Clockwise);
            var pointDir = MoveTo(MoveTo(point, direction, 0.5f), nextDirection, -0.11275f);

            var position = new Vector3(pointDir.X - Location.Map.HorizontalCells / 2, 0, pointDir.Y - Location.Map.VerticalCells / 2) * BlockSize;
            var rotation = DirectionToRadians(nextDirection);

            var door = (Node3D)DoorScene.Instantiate();

            Location.HandleBuild(door);

            door.Position = position;
            door.RotateY(rotation);
        }
    }
}