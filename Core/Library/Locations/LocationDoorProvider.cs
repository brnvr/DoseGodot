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
        DoorNumberController? doorNumberController,
        CustomRoomController? customRoomController
    ) : LocationSceneProvider<TInitializable, DoorContext, TArgs>(location, doorScene, initializationArgs)
        where TInitializable : IInitializableActor<DoorContext, TArgs>
    {
        public Vector2 Offset { get; } = offset;
        public int BlockSize { get; } = blockSize;
        public DoorNumberController? DoorNumberController { get; } = doorNumberController;
        public CustomRoomController? CustomRoomController { get; } = customRoomController;

        protected override DoorContext BuildContext()
        {
            return new DoorContext
            {
                Number = DoorNumberController?.PullNumber(237)
            };
        }

        protected override Vector3 GetPosition(Vector2I point, CardinalDirection direction)
        {
            if (Location.Map == null)
            {
                return Vector3.Zero;
            }

            var nextDirection = NextDirection(direction, Location.DoorClockDirection);
            var pointDir = MoveTo(MoveTo(point, direction, Offset.Y), nextDirection, Offset.X);

            return new Vector3(pointDir.X - Location.Map.HorizontalCells / 2, 0, pointDir.Y - Location.Map.VerticalCells / 2) * BlockSize;
        }

        protected override float GetRotation(Vector2I point, CardinalDirection direction)
        {
            var nextDirection = NextDirection(direction, Location.DoorClockDirection);

            return DirectionToRadians(nextDirection);
        }

        protected override bool WallPredicate(MapCellWall wall)
        {
            return wall.Type == MapCellType.Doorway;
        }

        // protected ILocationOccluder? BuildOccluder(Vector2I point, CardinalDirection direction)
        // {
        //     if (OccluderFactory == null) return null;

        //     var occluder = OccluderFactory.Build(blockSize);

        //     node.Position = GetPosition(point, direction);
        //     node.RotateY(GetRotation(point, direction));
        // }

        protected override Node3D? OnBuildWall(Vector2I point, CardinalDirection direction)
        {
            var node = base.OnBuildWall(point, direction);

            if (DoorNumberController == null)
            {
                return node;
            }

            if (node is not IDoor door)
            {
                GD.PushWarning($"{nameof(doorScene)} doesn't implement {nameof(IDoor)}. Skipping door linking...");
                return node;
            }
            
            if (CustomRoomController == null)
            {
                return node;
            }

            if (door.Context?.Number == null || !CustomRoomController.TryLoadCustomRoom((int)door.Context.Number, out ICustomRoom? customRoom))
            {
                return node;
            }

            if (customRoom is null)
            {
                GD.PushWarning($"{nameof(customRoom)} was not loaded correctly and will not be accessible.");
                return node;
            }

            var linkedDoor = customRoom.SelectRandomPortal<IDoor>();
            
            door.Link(linkedDoor);

            return node;
        }
    }

    public class LocationDoorProvider<TInitializable, TArgs, TLocationOccluder>(
        IBuildableLocation location,
        PackedScene doorScene,
        LocationOccluderFactory<TLocationOccluder> occluderFactory,
        int blockSize,
        Vector2 offset,
        TArgs initializationArgs,
        DoorNumberController? doorNumberController,
        CustomRoomController? customRoomController
    ) : LocationDoorProvider<TInitializable, TArgs>(location, doorScene, blockSize, offset, initializationArgs, doorNumberController, customRoomController)
        where TInitializable : IInitializableActor<DoorContext, TArgs>
        where TLocationOccluder : Node3D, ILocationOccluder
    {
        public LocationOccluderFactory<TLocationOccluder> OccluderFactory { get; } = occluderFactory;

        protected TLocationOccluder? BuildOccluder(Vector2I point, CardinalDirection direction)
        {
            if (Location.Map == null) return null;
        
            var occluder = OccluderFactory.Build(BlockSize);

            if (occluder == null) return null;

            var pointDir = MoveTo(point, direction, 0.5f);

            occluder.Position = new Vector3(pointDir.X - Location.Map.HorizontalCells / 2, 0, pointDir.Y - Location.Map.VerticalCells / 2) * BlockSize;
            occluder.RotateY(DirectionToRadians(NextDirection(direction, ClockDirection.Counterclockwise)));

            return occluder;
        }

        protected override Node3D? OnBuildWall(Vector2I point, CardinalDirection direction)
        {
            var node = base.OnBuildWall(point, direction);

            if (node is not ILocationOccluderAttachable receptor)
            {
                GD.PushWarning($"{nameof(node)} does not implement {nameof(ILocationOccluderAttachable)}. Skipping occluder association...");
                return node;
            }

            var occluder = BuildOccluder(point, direction);

            if (occluder == null)
            {
                GD.PushWarning($"{nameof(occluder)} is null. Skipping occluder association...");
                return node;
            }

            receptor.AttachLocationOccluder(occluder);

            Location.OnBuild(occluder);

            return node;
        }
    }
}