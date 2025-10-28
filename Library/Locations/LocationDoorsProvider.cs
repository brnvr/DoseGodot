using System.Linq;
using Godot;
using static Dose.Library.GeoPositioning;

namespace Dose.Library.Locations
{
    public class LocationDoorsProvider(Location location, int blockSize) : LocationMapProcessor(location)
    {
        readonly static PackedScene _doorScene = GD.Load<PackedScene>("res://Scenes/Door.tscn");

        public int BlockSize { get; } = blockSize;
        
        protected override void ProcessCell(Vector2I position)
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
    
            var door = (Node3D)_doorScene.Instantiate();

            door.Position = new Vector3(pointDir.X - Location.Map.HorizontalCells / 2, 0, pointDir.Y - Location.Map.VerticalCells / 2) * BlockSize;
            door.RotateY(DirectionToRadians(nextDirection));

            Location.AddChild(door);
        }
    }
}