using System.Collections.Generic;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Configuration.Locations
{
    public class TextureConfig : YamlConfig
    {
        public required string Rarity { get; init; }
        public required string PackId { get; init; }
        public LocationRestriction LocationRestriction { get; init; } = LocationRestriction.AnyLocation;
        public required string TexturePath { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { nameof(TexturePath), TexturePath },
                { nameof(Rarity), Rarity.ToString() },
                { nameof(PackId), PackId },
                { nameof(LocationRestriction), LocationRestriction.ToString() }
            };
        }
    }
}
