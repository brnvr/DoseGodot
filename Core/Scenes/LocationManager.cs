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
using Dose.Core.ActorPools;
using System.Linq;
namespace Dose.Core.Scenes
{
	public partial class LocationManager : Node, ILocationManager, ILocationCreator
	{
		const int BLOCK_SIZE = 4;

		readonly static ItemPool _itemPool = new();
		readonly static NpcPool _npcPool = new();
		readonly static PackedScene _doorScene = GD.Load<PackedScene>("res://Configuration/Scenes/Door.tscn");
		readonly static PackedScene _locationScene = GD.Load<PackedScene>("res://Configuration/Scenes/Location.tscn");
		readonly static Mesh _wallMesh = LoadMeshFromScene("res://Media/3DAssets/Environment/Wall.glb");
		readonly static Mesh _wallWithOpeningMesh = LoadMeshFromScene("res://Media/3DAssets/Environment/WallWithOpening.glb");
		
		ILocation? _currentLocation;
		LocationQueue? _locationQueue;

		public IControlPlane? ControlPlane { get; private set; }
		public Player? Player { get; private set; }
		

		public ILocation? CurrentLocation { get => _currentLocation; set { SetCurrentLocation(value); } }

		public override void _ExitTree()
        {
            CurrentLocation?.Clear(-1);
			_locationQueue?.Free();
        }

		public LocationManager Initialize(IControlPlane controlPlane, Player player)
        {
			Player = player;
			ControlPlane = controlPlane;
			_locationQueue = new LocationQueue(this, 2);

			_locationQueue.Initialize();

			CurrentLocation = _locationQueue.PopNextLocation(LocationType.Corridor, ClockDirection.Clockwise);

            return this;
        }

		LocationType GetNextLocationType()
		{
			if (_currentLocation == null)
				return LocationType.Room;

			if (_currentLocation is not Location currentLocation)
				return LocationType.Room;

			if (currentLocation.Type == LocationType.Corridor)
				return LocationType.Room;

			var locationsTypes = new WeightedList<LocationType> {
				{ LocationType.Room, 3 },
				{ LocationType.Corridor, 1 }
			};

			var selector = new RandomSelector<LocationType>(locationsTypes);

			return selector.Pick();
		}

		public ILocation? GoToNextLocation()
        {
			var type = GetNextLocationType();
			var clockDirection = _currentLocation is null ? ClockDirection.Clockwise : ClockOpposite(_currentLocation.DoorsClockDirection);
		
			if (_locationQueue == null)
			{
				GD.PushError($"{nameof(LocationManager)} is not initialized. Can't go to next location.");
				return null;
			}

			if (_locationQueue == null)
			{
				GD.PushError($"{nameof(LocationManager)} is not initialized. Can't set next location.");
				return null;
			}

			CurrentLocation = _locationQueue.PopNextLocation(type, clockDirection);

			return CurrentLocation;
        }

		Location? CreateLocation(LocationType type, ClockDirection clockDirection)
		{
			if (ControlPlane == null)
            {
				GD.PushError($"{nameof(LocationManager)} has no defined {nameof(IControlPlane)}.");
                return null;
            }

			IMapGenerator mapGenerator = type switch
			{
				LocationType.Room => GenerateRoomMap(),
				LocationType.Corridor => GenerateCorridorMap(),
				_ => throw new Exception($"Unimplemented ${nameof(LocationType)}: {type}")

			};

			var location = _locationScene.Instantiate<Location>().Initialize(mapGenerator, BLOCK_SIZE, clockDirection, type);

			BuildLocation(location);
			PopulateLocation(location);
		
			//location.CreateFlippedBack();

			return location;
		}

		void BuildLocation(Location location)
		{
			if (ControlPlane == null)
            {
				GD.PushError($"{nameof(LocationManager)} has no defined {nameof(IControlPlane)}.");
				return;
            }

			var builders = new List<LocationBuilder> 
			{ 
				new LocationMeshBuilder(location, _wallMesh, _wallWithOpeningMesh, BLOCK_SIZE, "WallMeshInstance"),
				new LocationDoorsProvider(this, location, _doorScene, BLOCK_SIZE, ControlPlane),
			};

			foreach (var builder in builders)
			{
				builder.Build();
			}
		}

		void PopulateLocation(Location location)
		{
			var itemSelector = new ActorPoolSelector<Item, ItemContext>(_itemPool);
			var npcSelector = new ActorPoolSelector<Npc, NpcContext>(_npcPool);

			var itemPopulator = new LocationPopulator<Item, ItemContext>(location, itemSelector, new Probability(0.5), BLOCK_SIZE);
			var npcPopulator = new LocationPopulator<Npc, NpcContext>(location, npcSelector, new Probability(0.5), BLOCK_SIZE);

			var populators = new List<LocationPopulator>
			{
				itemPopulator,
				npcPopulator
			};

			HashSet<Vector2I> populatedPositions = [];
			Dictionary<LocationPopulator, List<IInitializableActor>> generatedActors = [];

			foreach (var populator in populators)
			{
				var actors = populator.Populate(populatedPositions, out populatedPositions);
				generatedActors[populator] = actors;
			}

			if (Player == null)
			{
				GD.Print($"{nameof(LocationManager)} has no defined {nameof(Player)}. Can't initialize NPC speakers.");
				return;
			}

			foreach (Npc npc in generatedActors[npcPopulator].Cast<Npc>())
			{
				npc.InitializeSpeaker(Player.ProgressDialogue);
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

        static CorridorMapGenerator GenerateCorridorMap()
        {
            return new CorridorMapGenerator(
				horizontalCells: 500,
				verticalCells: 500,
				probabilityToExtendCorridor: new Probability(0.7),
				probabilityToBifurcate: new Probability(0.25),
				probabilityToBuildDoors: new Probability(0.5)
			);
        }

        ILocation? ILocationCreator.CreateLocation(LocationType type, ClockDirection doorsClockDirection)
        {
            return CreateLocation(type, doorsClockDirection);
        }

    }
}
