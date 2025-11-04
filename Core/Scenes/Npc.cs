using System;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Godot;

namespace Dose.Core.Scenes
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
