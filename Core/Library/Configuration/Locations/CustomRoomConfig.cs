using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Locations
{
    public class CustomRoomConfig : YamlConfig
    {
        public required int RoomNumber { get; init; }
        public required string ScenePath { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { "scenePath", ScenePath },
                { "roomNumber", RoomNumber }
            };
        }
    }
}