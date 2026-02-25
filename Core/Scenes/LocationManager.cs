using Dose.Core.Library.Locations;
using Godot;
using System.Collections.Generic;
using Dose.Core.Library.Numbers;
using static Dose.Core.Library.GodotUtils;
using Dose.Core.Library.Control;
using Dose.Core.Library.Randomness;
using Dose.Core.Library.DataStructures;
using System.Threading.Tasks;
using static Dose.Core.Library.GeoPositioning;
using System;
using System.Threading;
using System.Collections.Concurrent;
namespace Dose.Core.Scenes
{
	public partial class LocationManager : Node, ILocationManager
	{
		const int BLOCK_SIZE = 4;

		readonly static PackedScene _doorScene = GD.Load<PackedScene>("res://Configuration/Scenes/Door.tscn");
		readonly static PackedScene _locationScene = GD.Load<PackedScene>("res://Configuration/Scenes/Location.tscn");
		readonly static Mesh _wallMesh = LoadMeshFromScene("res://Media/3DAssets/Environment/Wall.glb");
		readonly static Mesh _wallWithOpeningMesh = LoadMeshFromScene("res://Media/3DAssets/Environment/WallWithOpening.glb");
		
		public IControlPlane? _controlPlane;
		ILocation? _currentLocation;
		LocationQueue? _locationQueue;

		public ILocation? CurrentLocation { get => _currentLocation; set { SetCurrentLocation(value); } }

		public override void _ExitTree()
        {
            CurrentLocation?.Clear(-1);
			_locationQueue?.Free();
        }

		public LocationManager Initialize(IControlPlane controlPlane)
        {
			_controlPlane = controlPlane;
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
				GD.PushError("LocationManager is not initialized. Can't go to next location.");
				return null;
			}

			if (_locationQueue == null)
			{
				GD.PushError("LocationManager is not initialized. Can't set next location.");
				return null;
			}

			CurrentLocation = _locationQueue.PopNextLocation(type, clockDirection);

			return CurrentLocation;
        }

		Location? CreateLocation(LocationType type, ClockDirection clockDirection)
		{
			if (_controlPlane == null)
            {
				GD.PushError("Can't create location because LocationManager has no defined IControlPlane.");
                return null;
            }

			IMapGenerator mapGenerator = type switch
			{
				LocationType.Room => GenerateRoomMap(),
				LocationType.Corridor => GenerateCorridorMap(),
				_ => throw new Exception($"Unimplemented ${nameof(LocationType)}: {type}")

			};

			var location = _locationScene.Instantiate<Location>().Initialize(mapGenerator, BLOCK_SIZE, clockDirection, type);

			var builders = new List<LocationBuilder> 
			{ 
				new LocationMeshBuilder(location, _wallMesh, _wallWithOpeningMesh, BLOCK_SIZE, "WallMeshInstance"),
				new LocationDoorsProvider(this, location, _doorScene, BLOCK_SIZE, _controlPlane)
			};

			foreach (var builder in builders)
			{
				builder.Build();
			}

			//location.CreateFlippedBack();

			return location;
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

		class LocationQueue(LocationManager locationManager, int maxLocationsPerType)
		{
			readonly Dictionary<(LocationType, ClockDirection), ConcurrentQueue<ILocation>> _nextLocations = [];

			public bool IsInitialized { get; private set; } = false;
			public int MaxLocationsPerType { get; } = maxLocationsPerType;
			public LocationManager LocationManager { get; } = locationManager;

			public void Initialize()
			{
				if (IsInitialized)
				{
					GD.PushError("LocationQueue has already been initialized.");
				}

				foreach (var type in Enum.GetValues<LocationType>())
				{
					foreach (var direction in Enum.GetValues<ClockDirection>())
					{
						_nextLocations[(type, direction)] = new ConcurrentQueue<ILocation>();

						while (_nextLocations.TryGetValue((type, direction), out ConcurrentQueue<ILocation>? value) && value.Count < MaxLocationsPerType)
						{
							var newLocation = LocationManager.CreateLocation(type, direction);

							if (newLocation == null)
							{
								GD.PushError($"Failed to create location of type {type} and direction {direction} during LocationQueue initialization.");
								continue;
							}

							_nextLocations[(type, direction)].Enqueue(newLocation);
						}
					}
				}

				IsInitialized = true;
			}

			public bool IsEmpty(LocationType type, ClockDirection direction)
			{
				return _nextLocations.TryGetValue((type, direction), out ConcurrentQueue<ILocation>? queue) && queue.IsEmpty;
			}

			public ILocation? PopNextLocation(LocationType type, ClockDirection direction)
			{
				var queue = _nextLocations[(type, direction)];

				if (queue.IsEmpty)
				{
					GD.Print($"No pre-generated locations of type {type} and direction {direction} are available. Generating one on the fly...");
					return LocationManager.CreateLocation(type, direction);
				}

				_ = Refill(type, direction);

				queue.TryDequeue(out var nextLocation);

				if (nextLocation == null)
				{
					GD.PushError($"Failed to dequeue next location of type {type} and direction {direction}.");
				}

				return nextLocation;
			}

			public void Free()
			{
				foreach (var queue in _nextLocations.Values)
				{
					while (queue.TryDequeue(out var location))
					{
						if (location is Node locationNode)
						{
							locationNode.QueueFree();
						}
					}
				}
			}

			async Task Refill(LocationType type, ClockDirection direction)
			{
				await Task.Run(async () =>
				{
					//await Task.Delay(5000);
					var newLocation = LocationManager.CreateLocation(type, direction);

					if (newLocation == null)
					{
						GD.PushError($"Failed to create location of type {type} and direction {direction}.");
						return;
					}

					_nextLocations[(type, direction)].Enqueue(newLocation);
				});
			}
		}
    }
}
