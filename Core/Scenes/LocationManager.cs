using Dose.Core.Library.Locations;
using Godot;
using System.Collections.Generic;
using Dose.Core.Library.Numbers;
using static Dose.Core.Library.GodotUtils;
using Dose.Core.Library.Control;
using Dose.Core.Library.Randomness;
using Dose.Core.Library.DataStructures;

namespace Dose.Core.Scenes
{
	public partial class LocationManager : Node, ILocationManager
	{
		public enum LocationType
        {
			Room,
            Corridor
        }

		const int BLOCK_SIZE = 4;

		readonly static PackedScene _doorScene = GD.Load<PackedScene>("res://Configuration/Scenes/Door.tscn");
		readonly static PackedScene _locationScene = GD.Load<PackedScene>("res://Configuration/Scenes/Location.tscn");
		readonly static Mesh _wallMesh = LoadMeshFromScene("res://Media/3DAssets/Environment/Wall.glb");
		readonly static Mesh _wallWithOpeningMesh = LoadMeshFromScene("res://Media/3DAssets/Environment/WallWithOpening.glb");
		
		IControlPlane? _controlPlane;
		ILocation? _currentLocation;
		LocationType? _currentLocationType;
		ClockDirection _nextDoorClockDirection = ClockDirection.Clockwise;

		public ILocation? CurrentLocation { get => _currentLocation; set { SetCurrentLocation(value); } }

		public override void _ExitTree()
        {
            CurrentLocation?.Clear(-1);
        }

		public LocationManager Initialize(IControlPlane controlPlane)
        {
			_controlPlane = controlPlane;

			var location = CreateLocation(LocationType.Corridor);

			if (location != null)
            {
                SetCurrentLocation(location);
            }

            return this;
        }

		public Location? CreateLocation(LocationType type)
        {
			_currentLocationType = type;

			if (_controlPlane == null)
            {
				GD.PushError("Can't create location because LocationManager has no defined IControlPlane.");
                return null;
            }

            IMapGenerator mapGenerator = type switch
            {
                LocationType.Room => GenerateRoomMap(),
                LocationType.Corridor => GenerateCorridorMap(),
                _ => throw new System.Exception($"Unimplemented ${nameof(LocationType)}: {type}")

            };

			var location = _locationScene.Instantiate<Location>().Initialize(mapGenerator, BLOCK_SIZE);

			var builders = new List<LocationBuilder> 
			{ 
				new LocationMeshBuilder(location, _wallMesh, _wallWithOpeningMesh, BLOCK_SIZE, "WallMeshInstance"),
				new LocationDoorsProvider(this, location, _doorScene, BLOCK_SIZE, _controlPlane, _nextDoorClockDirection)
			 };

			foreach (var builder in builders)
			{
				builder.Build();
			}

			location.CreateFlippedBack();

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

			_nextDoorClockDirection = (
				_nextDoorClockDirection == ClockDirection.Clockwise ? ClockDirection.Counterclockwise : ClockDirection.Clockwise
			);
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

        public ILocation? CreateLocation()
        {
			if (_currentLocation == null || _currentLocationType == null)
				return null;

			if (_currentLocationType is LocationType.Corridor)
            	return CreateLocation(LocationType.Room);

			var locationsTypes = new WeightedList<LocationType> {
				{ LocationType.Room, 3 },
				{ LocationType.Corridor, 1 }
			};

			var selector = new RandomSelector<LocationType>(locationsTypes);

			return CreateLocation(selector.Pick());
        }
    }
}
