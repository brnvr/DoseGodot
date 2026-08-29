using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Packs
{
    public class PackConfig : YamlConfig
    {
        public required string Id { get; init; }
        public string? ParentId { get; init; }
        public required string Rarity { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { nameof(Id), Id },
                { nameof(ParentId), ParentId },
                { nameof(Rarity), Rarity }
            };
        }
    }
}
