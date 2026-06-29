using Dose.Core.Library.Actors;
using Dose.Core.Library.Control;
using Godot;
using static Dose.Core.Library.GeoPositioning;
using static Dose.Core.Library.Locations.MapCellWall;

namespace Dose.Core.Library.Locations
{
    public class LocationDoorSceneFactory<TInitializationArgs>
    (
        DoorNumberController? doorNumberController,
        CustomRoomController? customRoomController,
        TInitializationArgs initializationArgs,
        int blockSize,
        Vector2 offset,
        bool shoudPullNumber
        ) : LocationWallSceneFactory
    {
        public int BlockSize { get; } = blockSize;
        public Vector2 Offset { get; } = offset;
        public bool ShouldPullNumber { get; } = shoudPullNumber;
        TInitializationArgs InitializationArgs { get; } = initializationArgs;
        public DoorNumberController? DoorNumberController = doorNumberController;
        public CustomRoomController? CustomRoomController = customRoomController;

        protected DoorContext BuildContext()
        {
            return new DoorContext
            {
                Number = ShouldPullNumber ? DoorNumberController?.PullNumber(237) : null
            };
        }

        protected override Vector3 GetPosition(IBuildableLocation location, Vector2I point, CardinalDirection direction)
        {
            if (location.Map == null)
            {
                return Vector3.Zero;
            }

            var nextDirection = NextDirection(direction, location.DoorClockDirection);
            var pointDir = MoveTo(MoveTo(point, direction, Offset.Y), nextDirection, Offset.X);

            return new Vector3(pointDir.X - location.Map.CenterX, 0, pointDir.Y - location.Map.CenterY) * BlockSize;
        }

        protected override float GetRotation(IBuildableLocation location, Vector2I point, CardinalDirection direction)
        {
            var nextDirection = NextDirection(direction, location.DoorClockDirection);

            return DirectionToRadians(nextDirection);
        }

        protected override bool WallPredicate(MapCellWall wall)
        {
            return wall.Type == MapCellType.Doorway;
        }

        protected override Node3D? OnBuildWall(IBuildableLocation location, PackedScene scene, Vector2I point, CardinalDirection direction)
        {
            var node = base.OnBuildWall(location, scene, point, direction);

            if (DoorNumberController == null)
            {
                return node;
            }

            if (node is not IDoor door)
            {
                GD.PushWarning($"{nameof(scene)} doesn't implement {nameof(IDoor)}. Skipping door linking...");
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

        protected override void InitializeNode(Node node)
        {
            if (node is not IInitializableActor<DoorContext, TInitializationArgs> initializable)
            {
                GD.PushWarning($"Instance of {node.Name} is not of type {nameof(IInitializableActor<DoorContext, TInitializationArgs>)}. The node will not be initialized.");
                return;
            }

            initializable.Initialize(BuildContext(), InitializationArgs);
        }
    }

    public class LocationDoorSceneFactory<TInitializationArgs, TLocationOccluder>
    (
        DoorNumberController? doorNumberController,
        CustomRoomController? customRoomController,
        TInitializationArgs initializationArgs,
        LocationOccluderFactory<TLocationOccluder> occluderFactory,
        int blockSize,
        Vector2 offset,
        bool shoudPullNumber
        ) : LocationDoorSceneFactory<TInitializationArgs>(doorNumberController, customRoomController, initializationArgs, blockSize, offset, shoudPullNumber)
        where TLocationOccluder : Node3D, ILocationOccluder
        {
            public LocationOccluderFactory<TLocationOccluder> OccluderFactory { get; } = occluderFactory;

            protected TLocationOccluder? BuildOccluder(IBuildableLocation location, ClockDirection openDirection)
            {
                if (location.Map == null) return null;
            
                var occluder = OccluderFactory.Build(BlockSize);

                if (occluder == null) return null;

                //var pointDir = MoveTo(point, direction, 0.5f);

                occluder.Position = new Vector3(-Offset.X * BlockSize, 0, 0);
                occluder.RotateY(openDirection == ClockDirection.Counterclockwise ? Mathf.DegToRad(180) : 0);

                return occluder;
            }

            protected override Node3D? OnBuildWall(IBuildableLocation location, PackedScene scene, Vector2I point, CardinalDirection direction)
            {
                var node = base.OnBuildWall(location, scene, point, direction);

                if (node is not IDoor door) return null;

                var occluder = BuildOccluder(location, door.OpenDirection);

                if (occluder == null)
                {
                    GD.PushWarning($"{nameof(occluder)} is null. Skipping occluder association...");
                    return node;
                }

                node.CallDeferred(Node.MethodName.AddChild, occluder);

                return node;
            }
            
        }
    }