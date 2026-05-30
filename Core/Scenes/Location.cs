using Dose.Core.Library.Locations;
using Dose.Core.Library.Randomness;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Dose.Core.Library.GodotUtils;


namespace Dose.Core.Scenes
{
    public abstract partial class Location : Node3D, ILocation
    {
        public enum LocationType
        {
            Room, Corridor
        }

        public abstract LocationType? Type { get; }

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

        public TPortal SelectRandomPortal<TPortal>(Func<TPortal, bool>? predicate = null) where TPortal : IPortal
        {
            var portals = GetChildren().OfType<TPortal>();

            if (predicate != null)
            {
                portals = portals.Where(predicate);
            }

            return new RandomSelector<TPortal>(portals).Pick();
        }

        public void Clear(int keepLevels)
        {
            Clear(keepLevels, null);
        }

        void Clear(int keepLevels, Location? previous)
        {
            var doors = GetChildren().OfType<Door>();
            var stairs = GetChildren().OfType<Stairs>();

            Clear(doors, keepLevels, previous);
            Clear(stairs, keepLevels, previous);
        }

        void Clear(IEnumerable<IPortal> portals, int keepLevels, Location? previous)
        {
            foreach (var portal in portals)
            {
                var otherLocation = portal.Linked?.Location;
                
                if (otherLocation is Location _otherLocation && _otherLocation != previous)
                {
                    _otherLocation.Clear(keepLevels-1, this); 

                    if (keepLevels <= 0)
                    {
                        portal.Unlink();
                        _otherLocation.QueueFree();
                    }
                }
            }
        }
    }

}