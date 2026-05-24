using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Locations;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class CustomRoomContextBuilder : IObjectFromConfigBuilder<CustomRoomContext?>
    {
        public CustomRoomContext? Build(CustomRoomConfig config)
        {
            var scene = GD.Load<PackedScene>(config.ScenePath);

            return new CustomRoomContext
            {
                Scene = scene,
                RoomNumber = config.RoomNumber
            };
        }

        CustomRoomContext? IObjectFromConfigBuilder<CustomRoomContext?>.Build(YamlConfig config)
        {
            return Build((CustomRoomConfig)config);
        }
    }
}