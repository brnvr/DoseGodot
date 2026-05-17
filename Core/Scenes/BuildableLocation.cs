using Dose.Core.Library.Locations;
using Godot;
using System;

namespace Dose.Core.Scenes
{
    

    public partial class BuildableLocation : Location, IBuildableLocation
    {
        LocationType? _type;
        ClockDirection _doorClockDirection = ClockDirection.Clockwise;

        public Map? Map { get; private set; }
        public override LocationType? Type { get => _type; }
        public int BlockSize { get; private set; }
        public IMapGenerator? MapGenerator { get; private set; }
        public override ClockDirection DoorClockDirection { get => _doorClockDirection; }

        public void OnBuild(Node node)
        {
            CallDeferred(Node.MethodName.AddChild, node);
        }

        public BuildableLocation Initialize(IMapGenerator mapGenerator, int blockSize, ClockDirection doorClockDirection, LocationType type)
        {
            _type = type;
            MapGenerator = mapGenerator;
            BlockSize = blockSize;
            _doorClockDirection = doorClockDirection;
            
            if (Map != null)
            {
                throw new Exception("Location has already been initiated");
            }

            Map = MapGenerator.GenerateMap();

            return this;
        }
    }
}