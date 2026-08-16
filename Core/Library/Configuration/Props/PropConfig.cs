using System.Collections.Generic;
using Dose.Core.Library.Numbers;

namespace Dose.Core.Library.Configuration.Props
{
    public class PropConfig : YamlConfig
    {
        public string? TexturePath { get; init; }
        public string? ModelPath { get; init; }
        public required string Rarity { get; init; }
        public required string PackId { get; init; }
        public ScaleFactor Scale { get; init; } = 1f;
        public NormalizedValue MinDistanceToWall { get; init; } = 0f;
        public NormalizedValue MaxDistanceToWall { get; init; } = 1f;
        public NormalizedValue MinDistanceFromCorner { get; init; } = 0f;
        public NormalizedValue MaxDistanceFromCorner { get; init; } = 1f;
        public bool IsBillboard => TexturePath != null;

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { nameof(TexturePath), TexturePath },
                { nameof(ModelPath), ModelPath },
                { nameof(Rarity), Rarity },
                { nameof(PackId), PackId },
                { nameof(Scale), Scale.Value },
                { nameof(MinDistanceToWall), MinDistanceToWall.Value },
                { nameof(MaxDistanceToWall), MaxDistanceToWall.Value },
                { nameof(MinDistanceFromCorner), MinDistanceFromCorner.Value },
                { nameof(MaxDistanceFromCorner), MaxDistanceFromCorner.Value }
            };
        }
    }
}
