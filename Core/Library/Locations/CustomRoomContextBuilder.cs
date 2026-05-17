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

            return new CustomRoomContext
            {
                Scene = scene,
                RoomNumber = config.RoomNumber
            };
        }

        CustomRoomContext? IContextBuilder<CustomRoomContext?>.BuildContext(YamlConfig config)
        {
            return BuildContext((CustomRoomConfig)config);
        }
    }
}