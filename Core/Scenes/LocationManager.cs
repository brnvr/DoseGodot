using Dose.Core.Library.Locations;
using Godot;
using System.Collections.Generic;
using Dose.Core.Library.Numbers;
using static Dose.Core.Library.GodotUtils;
using Dose.Core.Library.Control;
using Dose.Core.Library.Randomness;
using Dose.Core.Library.DataStructures;
using static Dose.Core.Library.GeoPositioning;
using System;
using Dose.Core.Library.Actors;
using Dose.Core.ActorArgs;
using Dose.Core.Library.Location;
namespace Dose.Core.Scenes
{
	public partial class LocationManager : Node, ILocationManager
	{
		const int BLOCK_SIZE = 4;
		const string ROOM_CATEGORY = "room";
		const string CORRIDOR_CATEGORY = "corridor";
		const string CORRIDOR_WITH_STAIRS_UP_ENSURED_CATEGORY = "corridorWithStairsUpEnsured";
		const string CORRIDOR_WITH_STAIRS_DOWN_ENSURED_CATEGORY = "corridorWithStairsDownEnsured";

		readonly static Mesh _wallMesh = LoadMeshFromScene("res://Media/3DAssets/Environment/Wall.glb");
		readonly static Mesh _doorwayMesh = LoadMeshFromScene("res://Media/3DAssets/Environment/Doorway.glb");
		readonly static PackedScene _doorScene = GD.Load<PackedScene>("res://Configuration/Scenes/Door.tscn");
		readonly static PackedScene _buildableLocationScne = GD.Load<PackedScene>("res://Configuration/Scenes/BuildableLocation.tscn");
		readonly static PackedScene _stairsUpScene = GD.Load<PackedScene>("res://Configuration/Scenes/StairsUp.tscn");
		readonly static PackedScene _stairsDownScene = GD.Load<PackedScene>("res://Configuration/Scenes/StairsDown.tscn");

		ILocation? _currentLocation;
		ActorPool<ItemContext>? _itemPool;
		ActorPool<NpcContext>? _npcPool;
		LocationQueue? _locationQueue;

		public IControlPlane? ControlPlane { get; private set; }
		public Player? Player { get; private set; }
		

		public ILocation? CurrentLocation { get => _currentLocation; set { SetCurrentLocation(value); } }

		public override void _ExitTree()
        {
            CurrentLocation?.Clear(-1);
			_locationQueue?.Free();
        }

		public LocationManager Initialize(IControlPlane controlPlane, Player player, ActorPool<ItemContext> itemPool, ActorPool<NpcContext> npcPool)
        {
			Player = player;
			ControlPlane = controlPlane;
			_itemPool = itemPool;
			_npcPool = npcPool;
			_locationQueue = new LocationQueue(2, new Dictionary<string, Func<ClockDirection, ILocation?>>
			{
				{ ROOM_CATEGORY, CreateRoom },
				{ CORRIDOR_CATEGORY, direction => CreateCorridor(direction) },
				{ CORRIDOR_WITH_STAIRS_UP_ENSURED_CATEGORY, direction => CreateCorridor(direction, shouldEnsureStairsUp: true) },
				{ CORRIDOR_WITH_STAIRS_DOWN_ENSURED_CATEGORY, direction => CreateCorridor(direction, shouldEnsureStairsDown: true) }
			}).Initialize();

			CurrentLocation = _locationQueue.PopNextLocation(CORRIDOR_CATEGORY, ClockDirection.Clockwise);

			// if (ControlPlane?.DoorNumberController != null)
			// {
			// 	foreach (var customRoomCtx in ControlPlane.DoorNumberController.CustomRoomPool.AvailableContexts)
			// 	{
			// 		if (customRoomCtx.Location is not CustomRoom customRoom)
			// 		{
			// 			GD.PushWarning($"{customRoomCtx.Location} is not of type {nameof(CustomRoom)} and will not be initialized.");
			// 			continue;
			// 		}

			// 		customRoom.Initialize(BLOCK_SIZE, this);
			// 	}
			// }

			//player.QuestBook.Add(new FindItemQuest(_itemPool.AvailableContexts.ToList()[0]));

            return this;
        }

		string GetNextLocationCategory(IPortal portal)
		{
			if (_currentLocation == null)
				return ROOM_CATEGORY;

			if (portal is IStairs stairs)
			{
				if (stairs.Direction == StairsDirection.Up)
					return CORRIDOR_WITH_STAIRS_DOWN_ENSURED_CATEGORY;

				if (stairs.Direction == StairsDirection.Down)
					return CORRIDOR_WITH_STAIRS_UP_ENSURED_CATEGORY;
			}
				

			if (_currentLocation is not BuildableLocation currentLocation)
				return ROOM_CATEGORY;

			if (currentLocation.Type == Location.LocationType.Corridor)
				return ROOM_CATEGORY;

			var categories = new WeightedList<string>
			{
				{ ROOM_CATEGORY, 3 },
				{ CORRIDOR_CATEGORY, 1 }
			};

			var selector = new RandomSelector<string>(categories);

			return selector.Pick();
		}

		public ILocation? BuildNextLocation(IPortal portal)
		{
			var category = GetNextLocationCategory(portal);
			var clockDirection = _currentLocation is IBuildableLocation buildable? ClockOpposite(buildable.DoorClockDirection) : ClockDirection.Clockwise;

			if (_locationQueue == null)
			{
				GD.PushError($"{nameof(LocationManager)} is not initialized. Can't set next location.");
				return null;
			}

			if (category == null) return null;

			CurrentLocation = _locationQueue.PopNextLocation(category, clockDirection);

			return CurrentLocation;
		}

		BuildableLocation? CreateRoom(ClockDirection clockDirection)
		{
			if (ControlPlane == null)
            {
				GD.PushError($"{nameof(LocationManager)} has no defined {nameof(IControlPlane)}.");
                return null;
            }

			IMapGenerator mapGenerator = GenerateRoomMap();

			var location = _buildableLocationScne.Instantiate<BuildableLocation>().Initialize(mapGenerator, BLOCK_SIZE, clockDirection, Location.LocationType.Room);

			BuildLocation(location);
			PopulateLocation(location);
		
			//location.CreateFlippedBack();

			return location;
		}

		BuildableLocation? CreateCorridor(ClockDirection clockDirection, bool shouldEnsureStairsUp = false, bool shouldEnsureStairsDown = false)
		{
			if (ControlPlane == null)
            {
				GD.PushError($"{nameof(LocationManager)} has no defined {nameof(IControlPlane)}.");
                return null;
            }

			IMapGenerator mapGenerator = GenerateCorridorMap(shouldEnsureStairsUp, shouldEnsureStairsDown);

			var location = _buildableLocationScne.Instantiate<BuildableLocation>().Initialize(mapGenerator, BLOCK_SIZE, clockDirection, Location.LocationType.Corridor);

			BuildLocation(location);
			PopulateLocation(location);
		
			//location.CreateFlippedBack();

			return location;
		}

		void BuildLocation(BuildableLocation location)
		{
			if (ControlPlane == null)
            {
				GD.PushError($"{nameof(LocationManager)} has no defined {nameof(IControlPlane)}.");
				return;
            }

			var doorNumberController = location.Type == Location.LocationType.Corridor ? ControlPlane.DoorNumberController : null;

			var doorArgs = new DoorArgs
			{
				Location = location,
				LocationManager = this,
				OpenDirection = ClockOpposite(location.DoorClockDirection)
			};

			var stairsArgs = new StairsArgs
			{
				Location = location
			};

			var doorOffset = new Vector2(-0.11275f, .5f);
			var stairsOffset = new Vector2(0, .5f);

			var builders = new List<LocationBuilder> 
			{ 
				new LocationMeshBuilder(location, _wallMesh, _doorwayMesh, BLOCK_SIZE, "WallMeshInstance"),
				new LocationDoorProvider<Door, DoorArgs>(location, _doorScene, BLOCK_SIZE, doorOffset, doorArgs, doorNumberController, ControlPlane.CustomRoomController),
				new LocationStairsUpProvider<Stairs, StairsArgs>(location, _stairsUpScene, BLOCK_SIZE, stairsOffset, stairsArgs),
				new LocationStairsDownProvider<Stairs, StairsArgs>(location, _stairsDownScene, BLOCK_SIZE, stairsOffset, stairsArgs),
			};

			foreach (var builder in builders)
			{
				builder.Build();
			}
		}

		void PopulateLocation(BuildableLocation location)
		{
			if (_itemPool == null)
			{
				GD.PushWarning($"{nameof(_itemPool)} is null");
				return;
			}

			if (_npcPool == null)
			{
				GD.PushWarning($"{nameof(_npcPool)} is null");
				return;
			}

			var itemSelector = new ActorPoolSelector<Item, ItemContext, ItemArgs>(_itemPool);
			var npcSelector = new ActorPoolSelector<Npc, NpcContext, NpcArgs>(_npcPool);

			var itemArgs = new ItemArgs();
			var npcArgs = new NpcArgs
			{
				Player = Player
			};

			var itemPopulator = new LocationPopulator<Item, ItemContext, ItemArgs>(
				location,
				itemSelector,
				new Probability(0.5),
				itemArgs,
				BLOCK_SIZE
			);

			var npcPopulator = new LocationPopulator<Npc, NpcContext, NpcArgs>(
				location,
				npcSelector,
				new Probability(0.5),
				npcArgs,
				BLOCK_SIZE
			);

			var populators = new List<LocationPopulator>
			{
				itemPopulator,
				npcPopulator
			};

			HashSet<Vector2I> populatedPositions = [];

			foreach (var populator in populators)
			{
				populator.Populate(populatedPositions, out populatedPositions);
			}
		}

		void SetCurrentLocation(ILocation? location)
        {
			if (_currentLocation is Node currentLocationNode)
            {
				RemoveChild(currentLocationNode);
            }

			if (location is Node locationNode)
            {
				AddChild(locationNode);
            }

			_currentLocation = location;
        }

		static RoomMapGenerator GenerateRoomMap()
        {
            return new RoomMapGenerator(
				minSize: 5,
				maxSize: 10,
				probabilityToBuildDoor: new Probability(0.1)
			);
        }

        static CorridorMapGenerator GenerateCorridorMap(bool shouldEnsureStairsUp, bool shouldEnsureStairsDown)
        {
            return new CorridorMapGenerator(
				horizontalCells: 500,
				verticalCells: 500,
				probabilityToExtendCorridor: new Probability(0.5),
				probabilityToBifurcate: new Probability(0.15),
				probabilityToBuildDoors: new Probability(0.4),
				probabilityToBuildStairs: new Probability(0.1)
			)
			{
				ShouldEnsureStairsUp = shouldEnsureStairsUp,
				ShouldEnsureStairsDown = shouldEnsureStairsDown
			};
        }

        public bool PortalPredicate(IPortal portal, IPortal linkedPortal)
        {
            if (portal is not IStairs stairs)
			{
				return true;
			}

			if (linkedPortal is not IStairs linkedStairs)
			{
				return true;
			}

			return stairs.Direction != linkedStairs.Direction;
        }
    }
}
