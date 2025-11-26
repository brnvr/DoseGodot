
using Godot;

namespace Dose.Core.Library.Actors
{
    public abstract record ItemContext : IActorContext
    {
        public string DisplayName { get; set; } = "";
        public Texture2D? Texture { get; set; }
    }
}