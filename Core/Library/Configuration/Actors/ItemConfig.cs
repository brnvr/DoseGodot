using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Configuration.Actors
{
    public class ItemConfig : YamlConfig
    {
        public required string DisplayName { get; init; }
        public required string TexturePath { get; init; }
        public required IEnumerable<ItemActionConfig> Execute { get; set; }
        public int? MaxQuantity { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            var dict = new Dictionary<string, object?>
            {
                { "displayName", DisplayName },
                { "texturePath", TexturePath },
                { "maxQuantity", MaxQuantity }
            };

            if (Execute.Any())
            {
                dict.Add("execute", Execute.Select(a => a.AsDict()));
            }

            return dict;
        }
    }
}