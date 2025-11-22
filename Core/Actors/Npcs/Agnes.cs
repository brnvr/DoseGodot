using Dose.Core.Library.Actors;
using Dose.Core.Library.DataStructures;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.Actors.Npcs
{
    public record Agnes : NpcContext
    {
        public Agnes(Player player)
        {
            DisplayName = "Agnes";
            Texture = ResourceLoader.Load<Texture2D>("res://Media/Textures/Npcs/Agnes/Idle_000.png");
            Dialogue = Dialogue.Start()
                .WithNode(new DialogueSpeech("algum diálogo"))
                .WithNode(new DialogueSpeech("preu falar"))
                .WithNode(new DialogueSpeech("arguma coisa"));
        }
    }
}