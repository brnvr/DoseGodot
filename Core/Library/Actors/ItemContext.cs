using System.Collections.Generic;
using Dose.Core.Library.Common;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Library.Actors
{
    public record ItemContext : IActorContext, IHasRarity
    {
        public required string Identifier { get; init; }
        public required string DisplayName { get; init; }
        public Rarity Rarity { get; init; }
        public Texture2D? Texture { get; init; }
        public int MaxQuantity { get; init; } = -1;
        public required IEnumerable<Executable> Execute { get; init; }
        public required IEnumerable<InteractionOperator> InteractWith { get; init; }
        public required bool KeepAfterUse { get; set; }
    }
}