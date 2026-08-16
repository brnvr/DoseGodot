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
        public required IEnumerable<ItemInteractionConfig> InteractWith { get; init; }
        public required bool KeepAfterUse { get; init; }
        public ScaleFactor Scale { get; init; } = 1f;

        public override Dictionary<string, object?> AsDict()
        {
            var dict = new Dictionary<string, object?>
            {
                { "identifier", Identifier },
                { "displayName", DisplayName },
                { "texturePath", TexturePath },
                { "equipedTexturePath", EquipedTexturePath },
                { "rarity", Rarity },
                { "packId", PackId },
                { "maxQuantity", MaxQuantity },
                { "keepAfterUse", KeepAfterUse },
                { "scale", Scale.Value }
            };

            if (Execute.Any())
            {
                dict.Add("execute", Execute.Select(a => a.AsDict()));
            }

            if (InteractWith.Any())
            {
                dict.Add("interactWith", Execute.Select(i => i.AsDict()));
            }
            

            return dict;
        }
    }
}
