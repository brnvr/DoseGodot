using System.Collections.Generic;
using Dose.Core.Library.Common;
using Godot;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Actors
{
    public record ItemContext : IActorContext, IPackResource
    {
        public required string Identifier { get; init; }
        public required string DisplayName { get; init; }
        public string Description { get; init; } = "";
        public Rarity Rarity { get; init; }
        public required string PackId { get; init; }
        public LocationRestriction LocationRestriction { get; init; } = LocationRestriction.AnyLocation;
        public Texture2D? Texture { get; init; }
        public Texture2D? EquipedTexture { get; init; }
        public int MaxQuantity { get; init; } = -1;
        public required IEnumerable<Executable> Execute { get; init; }
        public required bool KeepAfterUse { get; set; }
        public ScaleFactor Scale { get; init; } = 1f;
        public ScaleFactor EquipedScale { get; init; } = 1f;
    }
}
