using Godot;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Dose.Core.Library.Locations
{
    public sealed class MapRenderQueue(MapRenderer renderer)
    {
        readonly ConcurrentDictionary<Guid, Task<Image>> _images = [];

        public Task<Image>? Queue(ILocation location)
        {
            if (location is not IBuildableLocation { Map: not null } buildable)
            {
                return null;
            }

            return _images.GetOrAdd(location.Id, _ => Task.Run(() => renderer.Render(buildable.Map)));
        }

        public Task<Image>? Get(ILocation location) =>
            _images.TryGetValue(location.Id, out var image) ? image : Queue(location);
    }
}
