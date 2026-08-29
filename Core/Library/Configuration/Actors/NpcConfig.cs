using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Configuration.Actors
{
    public class NpcConfig : YamlConfig
    {
        public required string Identifier { get; set; }
        public required string DisplayName { get; init; }
        public required string Rarity { get; set; }
        public required string PackId { get; init; }
        public LocationRestriction LocationRestriction { get; init; } = LocationRestriction.AnyLocation;
        public required string TexturePath { get; init; }
        public required string DialoguesDir { get; set; }
        public required IEnumerable<string> AvailableDialogues { get; init; }
        public required string Voice { get; init; }
        public required IEnumerable<NpcItemInteractionConfig> InteractWith { get; init; }
        public ScaleFactor Scale { get; init; } = 1f;

        public override Dictionary<string, object?> AsDict()
        {
            var dict = new Dictionary<string, object?>
            {
                { nameof(Identifier), Identifier },
                { nameof(DisplayName), DisplayName },
                { nameof(TexturePath), TexturePath },
                { nameof(Rarity), Rarity },
                { nameof(PackId), PackId },
                { nameof(LocationRestriction), LocationRestriction.ToString() },
                { nameof(DialoguesDir), DialoguesDir },
                { nameof(AvailableDialogues), AvailableDialogues },
                { nameof(Voice), Voice },
                { nameof(Scale), Scale.Value },
            };

            if (InteractWith.Any())
            {
                dict.Add(nameof(InteractWith), InteractWith.Select(interaction => interaction.AsDict()));
            }

            return dict;
        }
    }
}
