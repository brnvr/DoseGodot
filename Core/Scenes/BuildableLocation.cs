using Dose.Core.Library.GameEvents;
using Dose.Core.Library.Locations;
using Godot;
using System;

namespace Dose.Core.Scenes
{
    public partial class BuildableLocation : Location, IBuildableLocation
    {
        LocationType? _type;
        
        public IGameEvent<Location>? GameEvent;
        public Map? Map { get; private set; }
        public override LocationType? Type { get => _type; }
        public int BlockSize { get; private set; }
        public IMapGenerator? MapGenerator { get; private set; }
        public ClockDirection DoorClockDirection { get; private set; }

        public void OnBuild(Node node)
        {
            CallDeferred(Node.MethodName.AddChild, node);
        }

        public override void _Process(double delta)
        {
            if (GameEvent != null && GameEvent.ShouldTrigger(this))
            {
                GameEvent.OnTrigger(this);
            }
        }

        public BuildableLocation Initialize(
            IMapGenerator mapGenerator,
            int blockSize,
            ClockDirection doorClockDirection,
            LocationType type,
            IGameEvent<Location>? gameEvent
        )
        {
            _type = type;
            GameEvent = gameEvent;
            MapGenerator = mapGenerator;
            BlockSize = blockSize;
            DoorClockDirection = doorClockDirection;
            
            if (Map != null)
            {
                throw new Exception("Location has already been initiated");
            }

            Map = MapGenerator.GenerateMap();

            return this;
        }
    }
}