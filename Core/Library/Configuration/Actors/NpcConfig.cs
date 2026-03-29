using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Actors
{
    public class NpcConfig : YamlConfig
    {
        public required string DisplayName { get; init; }
        public required string TexturePath { get; init; }
        public required string DialoguesDir { get; set; }
        public required string CurrentDialogue { get; init; }
        public required string Voice { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { "displayName", DisplayName },
                { "texturePath", TexturePath },
                { "dialoguesDir", DialoguesDir },
                { "currentDialogue", CurrentDialogue },
                { "Voice", Voice },
            };
        }
    }
}