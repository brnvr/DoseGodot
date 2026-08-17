using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Numbers;

namespace Dose.Core.Library.Configuration.Actors
{
    public class ItemConfig : YamlConfig
    {
        public required string Identifier { get; init; }
        public required string DisplayName { get; init; }
        public required string TexturePath { get; init; }
        public required string EquipedTexturePath { get; init; }
        public required string Rarity { get; init; }
        public required string PackId { get; init; }
        public required IEnumerable<ItemActionConfig> Execute { get; init; }
        public int? MaxQuantity { get; init; }
        public required bool KeepAfterUse { get; init; }
        public ScaleFactor Scale { get; init; } = 1f;
        public ScaleFactor EquipedScale { get; init; } = 1f;

        public override Dictionary<string, object?> AsDict()
        {
            var dict = new Dictionary<string, object?>
            {
                { nameof(Identifier), Identifier },
                { nameof(DisplayName), DisplayName },
                { nameof(TexturePath), TexturePath },
                { nameof(EquipedTexturePath), EquipedTexturePath },
                { nameof(Rarity), Rarity },
                { nameof(PackId), PackId },
                { nameof(MaxQuantity), MaxQuantity },
                { nameof(KeepAfterUse), KeepAfterUse },
                { nameof(Scale), Scale.Value },
                { nameof(EquipedScale), EquipedScale.Value }
            };

            if (Execute.Any())
            {
                dict.Add(nameof(Execute), Execute.Select(a => a.AsDict()));
            }

            return dict;
        }
    }
}
