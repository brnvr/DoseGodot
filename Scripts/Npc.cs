using System;
using Dose.Interactions;
using Dose.Library.Actors;
using Dose.Library.Common;
using Godot;

namespace Dose.Scripts
{
    public partial class Npc : Node3D, IActor<Player>
    {
        public IActorContext? Context { get; private set; }

        public Npc Initialize(NpcContext context)
        {
            Context = context;

            return this;
        }

        public void HandleInteraction(IInteraction<Player> interaction)
        {
            GD.Print("lololo");
        }

        public IInteraction<Player> GetDefaultInteraction(Player interactor)
        {
            return new ToggleInteraction<Player>(interactor);
        }

    }
}
