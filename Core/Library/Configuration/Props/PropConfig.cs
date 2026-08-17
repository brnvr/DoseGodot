using System.Collections.Generic;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Configuration.Props
{
    public class PropConfig : YamlConfig
    {
        public string? TexturePath { get; init; }
        public string? ModelPath { get; init; }
        public required string Rarity { get; init; }
        public required string PackId { get; init; }
        public LocationRestriction LocationRestriction { get; init; } = LocationRestriction.AnyLocation;
        public ScaleFactor Scale { get; init; } = 1f;
        public float YOffset { get; init; }
        public SurfaceOffsetConfig? SurfaceOffset { get; init; }
        public NormalizedValue MinDistanceFromWall { get; init; } = 0f;
        public NormalizedValue MaxDistanceFromWall { get; init; } = 1f;
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
                { nameof(LocationRestriction), LocationRestriction.ToString() },
                { nameof(Scale), Scale.Value },
                { nameof(YOffset), YOffset },
                { nameof(SurfaceOffset), SurfaceOffset?.AsDict() },
                { nameof(MinDistanceFromWall), MinDistanceFromWall.Value },
                { nameof(MaxDistanceFromWall), MaxDistanceFromWall.Value },
                { nameof(MinDistanceFromCorner), MinDistanceFromCorner.Value },
                { nameof(MaxDistanceFromCorner), MaxDistanceFromCorner.Value }
            };
        }
    }
}
