using Dose.Core.Library.Common;
using Dose.Core.Library.Locations;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.Loaders
{
    public class CustomRoomLoader : ILoader<ILocation?, CustomRoomLoadArgs>
    {
        public ILocation? Load(CustomRoomLoadArgs loadArgs)
        {
            var node = loadArgs.Scene.Instantiate<Node>();

            if (node is not CustomRoom customRoom)
            {
                GD.PushError($"{loadArgs.Scene.ResourcePath} is not of type {nameof(CustomRoom)}.");
                node.Free();
                return null;
            }

            customRoom.Initialize(loadArgs.LocationManager);

            return (ILocation)node;
        }
    }
}