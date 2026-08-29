using System;
using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Packs
{
    public class PackConfigConverter : YamlConfigConverter
    {
        public override PackConfig FromDict(Dictionary<string, object?> dict)
        {
            var id = GetRequired<string>(dict, nameof(PackConfig.Id));

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Pack id cannot be empty.");
            }

            var parentId = GetOptional<string>(dict, nameof(PackConfig.ParentId));

            return new PackConfig
            {
                Id = id,
                ParentId = string.IsNullOrWhiteSpace(parentId) ? null : parentId,
                Rarity = GetRequired<string>(dict, nameof(PackConfig.Rarity))
            };
        }
    }
}
