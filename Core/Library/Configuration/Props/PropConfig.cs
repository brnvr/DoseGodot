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
                { "texturePath", TexturePath },
                { "modelPath", ModelPath },
                { "rarity", Rarity },
                { "packId", PackId },
                { "scale", Scale.Value },
                { "minDistanceToWall", MinDistanceToWall.Value },
                { "maxDistanceToWall", MaxDistanceToWall.Value },
                { "minDistanceFromCorner", MinDistanceFromCorner.Value },
                { "maxDistanceFromCorner", MaxDistanceFromCorner.Value }
            };
        }
    }
}
