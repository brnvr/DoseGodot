using Dose.Core.Library.Locations;
using Dose.Core.Library.Randomness;
using Godot;
using System;
using System.Linq;
using static Dose.Core.Library.GodotUtils;


namespace Dose.Core.Scenes
{
    public partial class Location : Node3D, IBuildableLocation
    {
        public Map? Map { get; private set; }
        public int BlockSize { get; private set; }
        public IMapGenerator? MapGenerator { get; private set; }

        public void OnBuild(Node node)
        {
            AddChild(node);
        }

        public Location Initialize(IMapGenerator mapGenerator, int blockSize)
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

        public void CreateFlippedBack(string nodeName="WallMeshInstance")
        {
            var _meshInstance = GetNode<MeshInstance3D>(nodeName);

            if (_meshInstance == null)
            {
                GD.PushWarning($"{nodeName} node not found. Can't created flipped back.");
                return;
            }

            var inverted = new MeshInstance3D
            {
                Mesh = FlipNormals(_meshInstance.Mesh)
            };
            
            AddChild(inverted);
        }

        public IDoor SelectRandomDoor()
        {
            var doors = GetChildren().OfType<Door>();

            return new RandomSelector<Door>(doors).Pick();
        }

        public void Clear(int keepLevels)
        {
            Clear(keepLevels, null);
        }

        void Clear(int keepLevels, Location? previous)
        {
            var doors = GetChildren().OfType<Door>();

            foreach (var door in doors)
            {
                var otherLocation = door.LinkedDoor?.Location;
                
                if (otherLocation is Location _otherLocation && _otherLocation != previous)
                {
                    _otherLocation.Clear(keepLevels-1, this); 

                    if (keepLevels <= 0)
                    {
                        door.UnlinkDoor();
                        _otherLocation.QueueFree();
                    }
                }
            }
        }
    }

}