using System.Collections.Generic;
using Dose.Core.Library.Common;
using Godot;

namespace Dose.Core.Library.Actors
{
    public record ItemContext : IActorContext
    {
        public required string DisplayName { get; init; }
        public Texture2D? Texture { get; init; }
        public int MaxQuantity { get; init; } = -1;
        public required IEnumerable<Executable> Execute { get; init; }
    }
}