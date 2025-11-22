using Dose.Core.Library.DataStructures;
using Godot;

namespace Dose.Core.Library.Actors
{
    public abstract record NpcContext : IActorContext
    {
        public string DisplayName { get; set; } = "";
        public Dialogue? Dialogue { get; set; }
        public Texture2D? Texture { get; set; }
    }
}