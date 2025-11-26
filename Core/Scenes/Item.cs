using Dose.Core.Actors.Items;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Scenes;
using Godot;
using System;

namespace Dose.Core.Scenes
{
    public partial class Item : Node3D, IActor<Player>, IItem
    {
        public ItemContext? Context { get; private set; }
        IActorContext? IActor.Context => Context;

        public override void _Ready()
        {
            Context = new CurrantSyrup();
        }


        public void Destroy()
        {
            QueueFree();
        }

        public IInteraction<Player> GetDefaultInteraction(Player interactor)
        {
            return new PickInteraction<Player>(interactor);
        }

        public void OnInteract(IInteraction<Player> interaction)
        {
            interaction.Perform(this);
        }
    }
}