using System.Linq;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Locations;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class CustomRoomContextBuilder : IContextBuilder<CustomRoomContext?>
    {
        public CustomRoomContext? BuildContext(CustomRoomConfig config)
        {
            var scene = GD.Load<PackedScene>(config.ScenePath);
            var node = scene.Instantiate();
            var door = (IDoor?)node.GetChildren().FirstOrDefault(n => n is IDoor);

            if (door is null)
            {
                GD.PushError($"Scene {scene.ResourceName} doesn't contain any door and will be inaccessible.");
                return null;
            }

            return new CustomRoomContext
            {
                RoomNumber = config.RoomNumber,
                Door = door
            };
        }

        CustomRoomContext? IContextBuilder<CustomRoomContext?>.BuildContext(YamlConfig config)
        {
            return BuildContext((CustomRoomConfig)config);
        }
    }
}