using Dose.Core.Library.Common;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class CustomRoomLoader<T>() where T : Node, ICustomRoom
    {
        public T Load(PackedScene scene, ILocationManager locationManager)
        {
            var node = scene.Instantiate<T>();

            node.Initialize(locationManager);

            return node;
        }
    }
}