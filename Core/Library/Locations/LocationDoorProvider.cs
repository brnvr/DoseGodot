using Dose.Core.Library.Actors;
using Dose.Core.Library.Control;
using Godot;
using static Dose.Core.Library.GeoPositioning;
using static Dose.Core.Library.Locations.MapCellWall;

namespace Dose.Core.Library.Locations
{
    public class LocationDoorProvider<TInitializable, TArgs>(
        IBuildableLocation location,
        PackedScene doorScene,
        int blockSize,
        Vector2 offset,
        TArgs initializationArgs,
        DoorNumberController? doorNumberController
    ) : LocationSceneProvider<TInitializable, DoorContext, TArgs>(location, doorScene, initializationArgs)
        where TInitializable : IInitializableActor<DoorContext, TArgs>
    {
        public Vector2 Offset { get; } = offset;
        public int BlockSize { get; } = blockSize;
        public DoorNumberController? DoorNumberController { get; } = doorNumberController;

        protected override DoorContext BuildContext()
        {
            return new DoorContext
            {
                Number = DoorNumberController?.PopRandomNumber()
            };
        }

        protected override Vector3 GetPosition(Vector2I point, CardinalDirection direction)
        {
            if (Location.Map == null)
            {
                return Vector3.Zero;
            }

            var nextDirection = NextDirection(direction, Location.DoorsClockDirection);
            var pointDir = MoveTo(MoveTo(point, direction, Offset.Y), nextDirection, Offset.X);

            return new Vector3(pointDir.X - Location.Map.HorizontalCells / 2, 0, pointDir.Y - Location.Map.VerticalCells / 2) * BlockSize;
        }

        protected override float GetRotation(Vector2I point, CardinalDirection direction)
        {
            var nextDirection = NextDirection(direction, Location.DoorsClockDirection);

            return DirectionToRadians(nextDirection);
        }

        protected override bool WallPredicate(MapCellWall wall)
        {
            return wall.Type == MapCellType.Doorway;
        }
    }
}