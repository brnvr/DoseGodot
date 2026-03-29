using Godot;

namespace Dose.Core.Library.Actors
{
    public record ItemContext : IActorContext
    {
        public string DisplayName { get; set; } = "";
        public Texture2D? Texture { get; set; }
        public int MaxQuantity { get; set; } = -1;
    }
}