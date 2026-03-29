using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Actors
{
    public class ItemConfig : YamlConfig
    {
        public required string DisplayName { get; init; }
        public required string TexturePath { get; init; }
        public int? MaxQuantity { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { "displayName", DisplayName },
                { "texturePath", TexturePath },
                { "maxQuantity", MaxQuantity }
            };
        }
    }
}