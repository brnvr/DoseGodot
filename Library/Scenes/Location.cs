using Dose.Library.Doors;
using Dose.Library.Locations;
using Godot;
using System;

namespace Dose.Library.Scenes
{
    public partial class Location : Node3D, IBuildableLocation
    {
        public Map? Map { get; private set; }
        public int BlockSize { get; private set; }
        public IMapGenerator? MapGenerator { get; private set; }

        public void HandleBuild(Node node)
        {
            AddChild(node);
        }

        public Location Init(IMapGenerator mapGenerator, int blockSize)
        {
            MapGenerator = mapGenerator;
            BlockSize = blockSize;
            
            if (Map != null)
            {
                throw new Exception("Location has already been initiated");
            }

            Map = MapGenerator.GenerateMap();

            return this;
        }
    }

}