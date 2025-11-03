using System;
using Dose.Library.Actors;
using Dose.Library.Common;
using Godot;

namespace Dose.Scripts
{
    public partial class Npc : Node3D, IActor<Player>
    {
        public IActorController? Controller { get; private set; }

        public Npc Initialize(NpcController controller)
        {
            Controller = controller;

            return this;
        }

        public void HandleInteraction(Player player)
        {
            GD.Print("lololo");
        }

    }
}
