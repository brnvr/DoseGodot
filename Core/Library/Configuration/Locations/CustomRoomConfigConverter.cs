using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Locations
{
    public class CustomRoomConfigConverter : YamlConfigConverter
    {
        public override CustomRoomConfig FromDict(Dictionary<string, object?> dict)
        {
            return new CustomRoomConfig
            {
                ScenePath = GetRequired<string>(dict, nameof(CustomRoomConfig.ScenePath)),
                RoomNumber = int.Parse(GetRequired<string>(dict, nameof(CustomRoomConfig.RoomNumber)))
            };
        }
    }
}
