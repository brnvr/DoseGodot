using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Actors
{
    public class ItemConfigConverter : YamlConfigConverter
    {
        public override ItemConfig FromDict(Dictionary<string, object?> dict)
        {
            return new ItemConfig
            {
                DisplayName = GetRequired<string>(dict, "displayName"),
                TexturePath = GetRequired<string>(dict, "texturePath"),
                MaxQuantity = GetOptional<int?>(dict, "maxQuantity")
            };
        }
    }
}