using Dose.Library.Actors;
using Godot;

namespace Dose.SceneScripts
{
    public partial class Npc : Node3D, IActor<Player>
    {
        public void HandleInteraction(Player player)
        {
            GD.Print("lololo");
        }

    }
}
