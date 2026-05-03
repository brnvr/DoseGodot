using Dose.Core.Library.Actors;
using Godot;
using static Dose.Core.Library.GeoPositioning;

namespace Dose.Core.Library.Locations
{
    public abstract class LocationStairsProvider<TInitializable, TArgs>(
        IBuildableLocation location,
        PackedScene stairsScene,
        int blockSize,
        TArgs initializationArgs
    ) : LocationSceneProvider<TInitializable, StairsContext, TArgs>(location, stairsScene, initializationArgs)
        where TInitializable : IInitializableActor<StairsContext, TArgs>
    {
        public int BlockSize { get; } = blockSize;

        protected override StairsContext BuildContext()
        {
            return new StairsContext();
        }

        protected override Vector3 GetPosition(Vector2I point, CardinalDirection direction)
        {
            if (Location.Map == null)
            {
                return Vector3.Zero;
            }

            var nextDirection = NextDirection(direction, ClockDirection.Clockwise);

            var pointDir = MoveTo(point, direction, .5f);

            return new Vector3(pointDir.X - Location.Map.HorizontalCells / 2, 0, pointDir.Y - Location.Map.VerticalCells / 2) * BlockSize;
        }

        protected override float GetRotation(Vector2I point, CardinalDirection direction)
        {
            var nextDirection = NextDirection(direction, ClockDirection.Clockwise);
            return DirectionToRadians(nextDirection);
        }
    }
}