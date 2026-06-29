using Dose.Core.Library.Actors;
using Godot;
using static Dose.Core.Library.GeoPositioning;

namespace Dose.Core.Library.Locations
{
    public abstract class LocationStairsSceneFactory<TInitializationArgs>
    (
        TInitializationArgs initializationArgs,
        int blockSize,
        Vector2 offset
        ) : LocationWallSceneFactory
    {
        public int BlockSize { get; } = blockSize;
        public Vector2 Offset { get; } = offset;
        TInitializationArgs InitializationArgs { get; } = initializationArgs;

        protected StairsContext BuildContext()
        {
            return new StairsContext();
        }

        protected override Vector3 GetPosition(IBuildableLocation location, Vector2I point, CardinalDirection direction)
        {
            if (location.Map == null)
            {
                return Vector3.Zero;
            }

            var nextDirection = NextDirection(direction, ClockDirection.Clockwise);

            var pointDir = MoveTo(MoveTo(point, direction, Offset.Y), nextDirection, Offset.X);

            return new Vector3(pointDir.X - location.Map.CenterX, 0, pointDir.Y - location.Map.CenterY) * BlockSize;
        }

        protected override float GetRotation(IBuildableLocation location, Vector2I point, CardinalDirection direction)
        {
            var nextDirection = NextDirection(direction, ClockDirection.Clockwise);
            return DirectionToRadians(nextDirection);
        }

        protected override void InitializeNode(Node node)
        {
            if (node is not IInitializableActor<StairsContext, TInitializationArgs> initializable)
            {
                GD.PushWarning($"Instance of {node.Name} is not of type {nameof(IInitializableActor<StairsContext, TInitializationArgs>)}. The node will not be initialized.");
                return;
            }

            initializable.Initialize(BuildContext(), InitializationArgs);
        }
    }
}