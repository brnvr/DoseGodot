using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Configuration.Actors
{
    public class NpcConfigConverter : YamlConfigConverter
    {
        public override NpcConfig FromDict(Dictionary<string, object?> dict)
        {
            var identifier = GetRequired<string>(dict, "identifier");
            return new NpcConfig
            {
                Identifier = identifier,
                DisplayName = GetOptional<string>(dict, "displayName") ?? identifier,
                Rarity = GetRequired<string>(dict, "rarity"),
                TexturePath = GetRequired<string>(dict, "texturePath"),
                Voice = GetRequired<string>(dict, "voice"),
                DialoguesDir = GetRequired<string>(dict, "dialoguesDir"),
                AvailableDialogues = GetRequired<List<object>>(dict, "availableDialogues").Select(o => (string)o)
            };
        }
    }
}