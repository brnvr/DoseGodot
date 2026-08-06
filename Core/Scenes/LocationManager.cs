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
using Dose.Core.Library.GameEvents;
using System.Linq;
using Dose.Core.Library;
namespace Dose.Core.Scenes
{

	public partial class LocationManager : Node, ILocationManager
	{
		const int BLOCK_SIZE = 4;
		const string ROOM_CATEGORY = "room";
		const string CORRIDOR_CATEGORY = "corridor";
		const string CORRIDOR_WITH_STAIRS_UP_ENSURED_CATEGORY = "corridorWithStairsUpEnsured";
		const string CORRIDOR_WITH_STAIRS_DOWN_ENSURED_CATEGORY = "corridorWithStairsDownEnsured";

		readonly Vector2 _doorOffset = new(-0.11275f, .5f);
		readonly Vector2 _stairsOffset = new(0, .5f);
		readonly static Probability _probabilityToSetNpcEvent = new(0.25);
		readonly static Probability _probabilityToSetLocationEvent = new (0.25);
		readonly static Texture2D _blackTexture = GD.Load<Texture2D>("res://Media/Textures/Environment/Common/black.png");
		readonly static Mesh _wallMesh = LoadMeshFromScene("res://Media/3DAssets/Environment/Wall.glb");
		readonly static Mesh _doorwayMesh = LoadMeshFromScene("res://Media/3DAssets/Environment/Doorway.glb");
		readonly static Mesh _floorMesh = LoadMeshFromScene("res://Media/3DAssets/Environment/Floor.glb");
		readonly static Mesh _ceilingMesh = LoadMeshFromScene("res://Media/3DAssets/Environment/Ceiling.glb");
		readonly static PackedScene _occluderScene = GD.Load<PackedScene>("res://Configuration/Scenes/LocationOccluder.tscn");
		readonly static PackedScene _doorScene = GD.Load<PackedScene>("res://Configuration/Scenes/Door.tscn");
		readonly static PackedScene _lampScene = GD.Load<PackedScene>("res://Configuration/Scenes/Lamp.tscn");
		readonly static PackedScene _buildableLocationScene = GD.Load<PackedScene>("res://Configuration/Scenes/BuildableLocation.tscn");
		readonly static PackedScene _stairsUpScene = GD.Load<PackedScene>("res://Configuration/Scenes/StairsUp.tscn");
		readonly static PackedScene _stairsDownScene = GD.Load<PackedScene>("res://Configuration/Scenes/StairsDown.tscn");

		ILocation? _currentLocation;
		ActorPool<ItemContext>? _itemPool;
		ActorPool<NpcContext>? _npcPool;
		WeightedPool<TextureDefinition>? _wallTexturePool;
		WeightedPool<TextureDefinition>? _lampTexturePool;
		WeightedPool<TextureDefinition>? _doorTexturePool;
		LocationQueue? _locationQueue;
		IEnumerable<IGameEvent<Npc>>? _npcEvents;
		IEnumerable<IGameEvent<Location>>? _locationEvents;

		public IControlPlane? ControlPlane { get; private set; }
		public Player? Player { get; private set; }
		

		public ILocation? CurrentLocation { get => _currentLocation; set { SetCurrentLocation(value); } }

		public override void _ExitTree()
        {
            CurrentLocation?.Clear(-1);
			_locationQueue?.Free();
        }

		public LocationManager Initialize(
			IControlPlane controlPlane,
			Player player,
			ActorPool<ItemContext> itemPool,
			ActorPool<NpcContext> npcPool,
			WeightedPool<TextureDefinition> wallTexturePool,
			WeightedPool<TextureDefinition> doorTexturePool,
			WeightedPool<TextureDefinition> lampTexturePool,
			IEnumerable<IGameEvent<Npc>> npcEvents,
			IEnumerable<IGameEvent<Location>> locationEvents
		)
        {
			Player = player;
			ControlPlane = controlPlane;
			_itemPool = itemPool;
			_npcPool = npcPool;
			_wallTexturePool = wallTexturePool;
			_doorTexturePool = doorTexturePool;
			_lampTexturePool = lampTexturePool;
			_npcEvents = npcEvents;
			_locationEvents = locationEvents;
			_locationQueue = new LocationQueue(2, new Dictionary<string, Func<ClockDirection, ILocation?>>
			{
				{ ROOM_CATEGORY, direction => CreateRoom(direction) },
				{ CORRIDOR_CATEGORY, direction => CreateCorridor(direction) },
				{ CORRIDOR_WITH_STAIRS_UP_ENSURED_CATEGORY, direction => CreateCorridor(direction, shouldEnsureStairsUp: true) },
				{ CORRIDOR_WITH_STAIRS_DOWN_ENSURED_CATEGORY, direction => CreateCorridor(direction, shouldEnsureStairsDown: true) }
			}).Initialize();

			CurrentLocation = InitializeLocation(_locationQueue.PopNextLocation(CORRIDOR_CATEGORY, ClockDirection.Clockwise));

            return this;
        }

		string GetNextLocationCategory(IPortal portal)
		{
			if (CurrentLocation == null)
				return ROOM_CATEGORY;

			if (portal is IStairs stairs)
			{
				if (stairs.Direction == StairsDirection.Up)
					return CORRIDOR_WITH_STAIRS_DOWN_ENSURED_CATEGORY;

				if (stairs.Direction == StairsDirection.Down)
					return CORRIDOR_WITH_STAIRS_UP_ENSURED_CATEGORY;
			}
				

			if (CurrentLocation is not BuildableLocation currentLocation)
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
			var clockDirection = CurrentLocation is IBuildableLocation buildable? ClockOpposite(buildable.DoorClockDirection) : ClockDirection.Clockwise;

			if (_locationQueue == null)
			{
				GD.PushError($"{nameof(LocationManager)} is not initialized. Can't set next location.");
				return null;
			}

			if (category == null) return null;

			var mustPreserveFrontier = CountReachableUnlinkedPortals() <= 1;
			var nextLocation = mustPreserveFrontier
				? CreateFrontierSafeLocation(category, clockDirection, portal)
				: _locationQueue.PopNextLocation(category, clockDirection);

			if (nextLocation == null)
			{
				return null;
			}

			if (mustPreserveFrontier && !HasCompatibleEntranceAndExit(nextLocation, portal))
			{
				GD.PushError("Failed to generate a location that preserves an unexplored portal.");
				return null;
			}

			var initializedLocation = InitializeLocation(nextLocation);

			if (initializedLocation is Node3D locationNode)
			{
				locationNode.Visible = false;
				AddChild(locationNode);
			}

			return initializedLocation;
		}

		ILocation? CreateFrontierSafeLocation(string category, ClockDirection clockDirection, IPortal incomingPortal)
		{
			var minimumDoorways = incomingPortal is IDoor ? 2 : 1;

			return category switch
			{
				ROOM_CATEGORY => CreateRoom(
					clockDirection,
					minimumDoorways: 2
				),
				CORRIDOR_CATEGORY => CreateCorridor(
					clockDirection,
					minimumDoorways: minimumDoorways,
					allowCustomRoomLinks: false
				),
				CORRIDOR_WITH_STAIRS_UP_ENSURED_CATEGORY => CreateCorridor(
					clockDirection,
					shouldEnsureStairsUp: true,
					minimumDoorways: minimumDoorways,
					allowCustomRoomLinks: false
				),
				CORRIDOR_WITH_STAIRS_DOWN_ENSURED_CATEGORY => CreateCorridor(
					clockDirection,
					shouldEnsureStairsDown: true,
					minimumDoorways: minimumDoorways,
					allowCustomRoomLinks: false
				),
				_ => null
			};
		}

		int CountReachableUnlinkedPortals()
		{
			if (CurrentLocation == null)
			{
				return 0;
			}

			Queue<ILocation> locations = new();
			HashSet<ILocation> visited = [];
			var frontierCount = 0;
			locations.Enqueue(CurrentLocation);

			while (locations.TryDequeue(out var location))
			{
				if (!visited.Add(location))
				{
					continue;
				}

				foreach (var locationPortal in GetPortals(location))
				{
					if (locationPortal.Linked == null)
					{
						frontierCount++;
					}
					else if (locationPortal.Linked.Location != null)
					{
						locations.Enqueue(locationPortal.Linked.Location);
					}
				}
			}

			return frontierCount;
		}

		static bool HasCompatibleEntranceAndExit(ILocation location, IPortal incomingPortal)
		{
			if (location is BuildableLocation { Map: not null } buildableLocation)
			{
				var portalTypes = buildableLocation.Map.GetFilledPositions()
					.SelectMany(position => AllDirections()
						.Select(direction => buildableLocation.Map.Get(position).GetWall(direction)?.Type))
					.Where(type => type == MapCellWall.MapCellType.Doorway ||
						type == MapCellWall.MapCellType.Upstairs ||
						type == MapCellWall.MapCellType.Downstairs)
					.Cast<MapCellWall.MapCellType>()
					.ToList();

				return portalTypes.Count >= 2 && portalTypes.Any(type =>
				{
					if (incomingPortal is not IStairs stairs)
					{
						return incomingPortal is IDoor && type == MapCellWall.MapCellType.Doorway;
					}

					return stairs.Direction switch
					{
						StairsDirection.Up => type == MapCellWall.MapCellType.Downstairs,
						StairsDirection.Down => type == MapCellWall.MapCellType.Upstairs,
						_ => false
					};
				});
			}

			var portals = GetPortals(location)
				.Where(portal => portal.Linked == null)
				.ToList();

			return portals.Count >= 2 && portals.Any(portal => AreCompatible(incomingPortal, portal));
		}

		static bool AreCompatible(IPortal source, IPortal candidate)
		{
			if (source is IStairs sourceStairs)
			{
				return candidate is IStairs candidateStairs &&
					sourceStairs.Direction != candidateStairs.Direction;
			}

			return source is IDoor && candidate is IDoor;
		}

		static IEnumerable<IPortal> GetPortals(ILocation location)
		{
			return location is Node locationNode
				? locationNode.GetChildren().OfType<IPortal>()
				: [];
		}

		BuildableLocation? CreateRoom(
			ClockDirection clockDirection,
			int minimumDoorways = 1
		)
		{
			if (ControlPlane == null)
            {
				GD.PushError($"{nameof(LocationManager)} has no defined {nameof(IControlPlane)}.");
                return null;
            }

			IMapGenerator mapGenerator = GenerateRoomMap();

			var location = _buildableLocationScene.Instantiate<BuildableLocation>().Initialize(
				mapGenerator,
				BLOCK_SIZE,
				clockDirection,
				Location.LocationType.Room,
				gameEvent: PickRandom(_locationEvents, _probabilityToSetLocationEvent)
			);

			if (location.Map != null)
			{
				new MapEnsurer(location.Map, MapCellWall.MapCellType.Doorway).EnsureAtLeast(minimumDoorways);
			}

			BuildLocation(location);
			PopulateLocation(location);
		
			//location.CreateFlippedBack();

			return location;
		}

		BuildableLocation? CreateCorridor(
			ClockDirection clockDirection,
			bool shouldEnsureStairsUp = false,
			bool shouldEnsureStairsDown = false,
			int minimumDoorways = 1,
			bool allowCustomRoomLinks = true
		)
		{
			if (ControlPlane == null)
            {
				GD.PushError($"{nameof(LocationManager)} has no defined {nameof(IControlPlane)}.");
                return null;
            }

			IMapGenerator mapGenerator = GenerateCorridorMap(shouldEnsureStairsUp, shouldEnsureStairsDown);

			var location = _buildableLocationScene.Instantiate<BuildableLocation>().Initialize(
				mapGenerator,
				BLOCK_SIZE,
				clockDirection,
				Location.LocationType.Corridor,
				gameEvent: PickRandom(_locationEvents, _probabilityToSetLocationEvent)
			);

			if (location.Map != null)
			{
				new MapEnsurer(location.Map, MapCellWall.MapCellType.Doorway).EnsureAtLeast(minimumDoorways);
			}

			BuildLocation(location, allowCustomRoomLinks);
			PopulateLocation(location);
		
			//location.CreateFlippedBack();

			return location;
		}

		static Vector2 GetLampOffset(BuildableLocation location)
		{
			if (location.Type == Location.LocationType.Corridor || location.Map == null)
			{
				return Vector2.Zero;
			}

			var xOffset = location.Map.HorizontalCells % 2 == 0 ? -0.5f : 0f;
			var yOffset = location.Map.VerticalCells % 2 == 0 ? -0.5f : 0f;

			return new Vector2(xOffset, yOffset);
		}

		void BuildLocation(BuildableLocation location, bool allowCustomRoomLinks = true)
		{
			if (ControlPlane == null)
            {
				GD.PushError($"{nameof(LocationManager)} has no defined {nameof(IControlPlane)}.");
				return;
            }

			var doorNumberController = location.Type == Location.LocationType.Corridor ? ControlPlane.DoorNumberController : null;
			
			var doorTexture = PickRandomTexture(_doorTexturePool?.GetWeightedList());

			if (doorTexture is null)
			{
				GD.PushError("Door texture could not be selected.");
				return;
			}

			var doorArgs = new DoorArgs
			{
				Material = CreatePlainMaterial(doorTexture),
				Location = location,
				LocationManager = this,
				OpenDirection = ClockOpposite(location.DoorClockDirection),
			};

			var stairsArgs = new StairsArgs
			{
				Location = location
			};

			var wallTexture = PickRandomTexture(_wallTexturePool?.GetWeightedList());

			if (wallTexture is null)
			{
				GD.PushError("Wall texture could not be selected.");
				return;
			}

			var lampTexture = PickRandomTexture(_lampTexturePool?.GetWeightedList());

			if (lampTexture is null)
			{
				GD.PushError("Lamp texture could not be selected.");
				return;
			}
			
			var occluderFactory = new LocationOccluderFactory<LocationOccluder>
			(
				_occluderScene,
				_wallMesh,
				_doorwayMesh,
				_floorMesh,
				_ceilingMesh,
				CreatePlainMaterial(_blackTexture)
			);

			var shouldPullNumber = allowCustomRoomLinks && location.Type == Location.LocationType.Corridor;

			var doorContext = new DoorContext
            {
                Number = shouldPullNumber ? ControlPlane.DoorNumberController?.PullNumber(237) : null
            };

			var builders = new List<LocationBuilder> 
			{ 
				new LocationWallMeshBuilder(location, _wallMesh, _doorwayMesh, CreatePlainMaterial(wallTexture), BLOCK_SIZE, "WallMeshInstance"),
				new LocationBaseMeshBuilder(
					location,
					_floorMesh,
					CreatePlainMaterial(_blackTexture),
					BLOCK_SIZE,
					true,
					"FloorMeshInstance",
					LocationCollisionLayer.NavigationSource
				),
				new LocationBaseMeshBuilder(location, _ceilingMesh, CreatePlainMaterial(_blackTexture), BLOCK_SIZE, false, "CeilingMeshInstance"),
				new LocationSceneProvider
				(
					location,
					_doorScene,
					new LocationDoorSceneFactory<DoorArgs, LocationOccluder>
					(
						ControlPlane.DoorNumberController,
						ControlPlane.CustomRoomController,
						doorArgs,
						occluderFactory,
						BLOCK_SIZE,
						_doorOffset,
						shouldPullNumber
					)
				),
				new LocationSceneProvider(location, _stairsUpScene, new LocationStairsUpSceneFactory<StairsArgs>
				(
					stairsArgs,
					BLOCK_SIZE,
					_stairsOffset
				)),
				new LocationSceneProvider(location, _stairsDownScene, new LocationStairsDownSceneFactory<StairsArgs>
				(
					stairsArgs,
					BLOCK_SIZE,
					_stairsOffset
				)),
				new LocationSceneProvider(location, _lampScene, new LocationLampSceneFactory
				(
					lampTexture,
					BLOCK_SIZE,
					GetLampOffset(location)
				))
			};

			foreach (var builder in builders)
			{
				builder.Build();
			}
		}

		static Texture2D? PickRandomTexture(IEnumerable<TextureDefinition>? values)
		{
			var textureDefinition = PickRandom(values);

			if (textureDefinition == null)
			{
				return null;
			}

			return GD.Load<Texture2D>(textureDefinition.TexturePath);
		}

		static T? PickRandom<T>(IEnumerable<T>? values)
		{
			if (values == null || !values.Any())
			{
				return default;
			}

			var selector = new RandomSelector<T>(values);

			return selector.Pick();
		}

        static T? PickRandom<T>(IEnumerable<T>? values, Probability probability)
		{
			if (values == null || !values.Any())
			{
				return default;
			}

			var shouldSelectEvent = new RandomDecider(probability).Roll();

			if (!shouldSelectEvent)
			{
				return default;
			}

			return PickRandom(values);
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
				Player = Player,
				GameEvent = PickRandom(_npcEvents, _probabilityToSetNpcEvent)
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

			var populatedPositions = GetReservedActorPositions(location);

			foreach (var populator in populators)
			{
				populator.Populate(populatedPositions, out populatedPositions);
			}
		}

		static HashSet<Vector2I> GetReservedActorPositions(BuildableLocation location)
		{
			if (location.Map == null)
			{
				return [];
			}

			return
            [
                .. location.Map.GetFilledPositions()
					.Where(position => AllDirections().Any(direction =>
					{
						var wallType = location.Map.Get(position).GetWall(direction)?.Type;

						return wallType == MapCellWall.MapCellType.Doorway ||
							wallType == MapCellWall.MapCellType.Upstairs ||
							wallType == MapCellWall.MapCellType.Downstairs;
					}))
,
                location.Map.Center,
            ];
		}

		void SetCurrentLocation(ILocation? location)
        {
			if (_currentLocation is Node currentLocationNode && currentLocationNode.GetParent() == this)
            {
                RemoveChild(currentLocationNode);
            }

			if (location is Node locationNode && locationNode.GetParent() != this)
            {
                AddChild(locationNode);
            }

			if (location is Node3D locationNode3D)
			{
				locationNode3D.Visible = true;
			}

			_currentLocation = location;
        }

		static ILocation? InitializeLocation(ILocation? location)
		{
			if (location is not Location locationNode) return location;

			if (locationNode.NavigationRegion == null)
			{
				Callable.From(() => { locationNode.InitializeNavigationRegion(); }).CallDeferred();
			}

			return location;
		}

		static RoomMapGenerator GenerateRoomMap()
        {
            return new RoomMapGenerator(
				minSize: 2,
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
    }
}
