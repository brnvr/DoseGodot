using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Actors
{
    public class NpcConfig : YamlConfig
    {
        public required string Identifier { get; set; }
        public required string DisplayName { get; init; }
        public required string TexturePath { get; init; }
        public required string DialoguesDir { get; set; }
        public required IEnumerable<string> AvailableDialogues { get; init; }
        public required string Voice { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { "identifier", Identifier },
                { "displayName", DisplayName },
                { "texturePath", TexturePath },
                { "dialoguesDir", DialoguesDir },
                { "availableDialogues", AvailableDialogues },
                { "Voice", Voice },
            };
        }
    }
}