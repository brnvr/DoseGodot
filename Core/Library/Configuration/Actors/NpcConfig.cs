using System.Collections.Generic;
using Dose.Core.Library.Numbers;

namespace Dose.Core.Library.Configuration.Actors
{
    public class NpcConfig : YamlConfig
    {
        public required string Identifier { get; set; }
        public required string DisplayName { get; init; }
        public required string Rarity { get; set; }
        public required string PackId { get; init; }
        public required string TexturePath { get; init; }
        public required string DialoguesDir { get; set; }
        public required IEnumerable<string> AvailableDialogues { get; init; }
        public required string Voice { get; init; }
        public ScaleFactor Scale { get; init; } = 1f;

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { "identifier", Identifier },
                { "displayName", DisplayName },
                { "texturePath", TexturePath },
                { "rarity", Rarity },
                { "packId", PackId },
                { "dialoguesDir", DialoguesDir },
                { "availableDialogues", AvailableDialogues },
                { "Voice", Voice },
                { "scale", Scale.Value },
            };
        }
    }
}
