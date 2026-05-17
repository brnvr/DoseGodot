using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class LocationLoadRegistry
    {
        readonly List<ILocation> _registry = [];

        public void Add(ILocation location)
        {
            _registry.Add(location);
        }

        public void Clear(int keepLevels = -1)
        {
            foreach (var location in _registry)
            {
                location.Clear(keepLevels);
                
                if (location is Node node)
                {
                    node.QueueFree();
                }
            }
        }
    }
}