using System.Linq;
using Godot;
using static Dose.Library.GeoPositioning;

namespace Dose.Library.Locations
{
    public class LocationDoorsProvider(Location location, int blockSize) : IMapCellProcessor
    {
        readonly static PackedScene _doorScene = GD.Load<PackedScene>("res://Scenes/Door.tscn");

        public Location Location { get; } = location;
        public int BlockSize { get; } = blockSize;
        
        public void Process(Map map, Vector2I position)
        {
            var cell = map.Get(position);
    
            foreach (var direction in AllDirections().Where(direction => cell.GetWall(direction)?.HasDoor == true))
            {
                AddDoor(map, position, direction);
            }
        }

        void AddDoor(Map map, Vector2I point, CardinalDirection direction)
        {
            var nextDirection = NextDirection(direction, GeoPositioning.ClockDirection.Clockwise);
            var pointDir = MoveTo(MoveTo(point, direction, 0.5f), nextDirection, -0.11275f);
    
            var door = (Node3D)_doorScene.Instantiate();

            door.Position = new Vector3(pointDir.X - map.HorizontalCells / 2, 0, pointDir.Y - map.VerticalCells / 2) * BlockSize;
            door.RotateY(DirectionToRadians(nextDirection));

            Location.AddChild(door);
        }

        
    }
}