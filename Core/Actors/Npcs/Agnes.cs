using Dose.Core.Library.Actors;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.ESpeak;
using Godot;

namespace Dose.Core.Actors.Npcs
{
    public record Agnes : NpcContext
    {
        public Agnes()
        {
            DisplayName = "Agnes";
            Texture = ResourceLoader.Load<Texture2D>("res://Media/Textures/Npcs/Agnes/Idle_000.png");
            Dialogue = new ShortDialogue("somedays i wish time would go slower");
            Voice = new Voice("mb-us2", 16000);
        }
    }
}