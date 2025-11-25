using Dose.Core.Library.DataStructures;
using Dose.Core.Library.ESpeak;
using Godot;

namespace Dose.Core.Library.Actors
{
    public abstract record NpcContext : IActorContext
    {
        public string DisplayName { get; set; } = "";
        public IDialogue? Dialogue { get; set; }
        public Texture2D? Texture { get; set; }
        public Voice? Voice { get; set; }
    }
}