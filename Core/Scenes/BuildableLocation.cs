using Dose.Core.Library.Locations;
using Godot;
using System;

namespace Dose.Core.Scenes
{
    

    public partial class BuildableLocation : Location, IBuildableLocation
    {
        LocationType? _type;
        int _blockSize;
        ClockDirection _doorsClockDirection = ClockDirection.Clockwise;

        public Map? Map { get; private set; }
        public override LocationType? Type { get => _type; }
        public override int BlockSize { get => _blockSize; }
        public IMapGenerator? MapGenerator { get; private set; }
        public override ClockDirection DoorsClockDirection { get => _doorsClockDirection; }

        public void OnBuild(Node node)
        {
            CallDeferred(Node.MethodName.AddChild, node);
        }

        public BuildableLocation Initialize(IMapGenerator mapGenerator, int blockSize, ClockDirection doorsClockDirection, LocationType type)
        {
            _type = type;
            MapGenerator = mapGenerator;
            _blockSize = blockSize;
            _doorsClockDirection = doorsClockDirection;
            
            if (Map != null)
            {
                throw new Exception("Location has already been initiated");
            }

            Map = MapGenerator.GenerateMap();

            return this;
        }
    }
}