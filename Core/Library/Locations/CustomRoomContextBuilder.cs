using System.Linq;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Locations;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class CustomRoomContextBuilder : IContextBuilder<CustomRoomContext?>
    {
        public CustomRoomContext? BuildContext(CustomRoomConfig config)
        {
            var scene = GD.Load<PackedScene>(config.ScenePath);
            var node = scene.Instantiate<Node3D>();

            if (node is not CustomRoom customRoom)
            {
                GD.PushError($"{nameof(config.ScenePath)} is not of type {nameof(CustomRoom)}");
                node.Free();
                return null;
            }

            var door = customRoom.SelectRandomPortal<IDoor>();

            return new CustomRoomContext
            {
                Door = door,
                Location = customRoom,
                RoomNumber = config.RoomNumber
            };
        }

        CustomRoomContext? IContextBuilder<CustomRoomContext?>.BuildContext(YamlConfig config)
        {
            return BuildContext((CustomRoomConfig)config);
        }
    }
}