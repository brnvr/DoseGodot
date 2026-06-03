using Godot;

namespace Dose.Core.Library.Actors
{
    public class StairsContext : IActorContext
    {
        public string Identifier => "stairs";
        public string DisplayName => $"{Direction.ToString().ToLower()}stairs";
        public int MaxQuantity => -1;
        public StairsDirection Direction { get; init; }
        public Texture2D? Texture { get; }
    }
}