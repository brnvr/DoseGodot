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
                TexturePath = GetRequired<string>(dict, "texturePath"),
                Voice = GetRequired<string>(dict, "voice"),
                DialoguesDir = GetRequired<string>(dict, "dialoguesDir"),
                AvaliableDialogues = GetRequired<List<object>>(dict, "avaliableDialogues").Select(o => (string)o)
            };
        }
    }
}