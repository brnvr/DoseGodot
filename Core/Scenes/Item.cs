using System;
using Dose.Core.ActorArgs;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class Item : RigidBody3D, IActor<Player>, IInitializableActor<ItemContext, ItemArgs>, IItem
    {
        public ItemContext? Context { get; private set; }
        IActorContext? IActor.Context => Context;

        public override void _Ready()
        {
            LinearDamp = 15f;
        }

        public void Initialize(ItemContext context, ItemArgs args)
        {
            Context = context;
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

        public IActor Initialize(IActorContext context)
        {
            if (context is ItemContext itemContext)
            {
                return Initialize(itemContext);
            }
            else
            {
                GD.PushError($"Invalid context type for {nameof(Item)} initialization: {context?.GetType()}. Expected {typeof(ItemContext)}.");
                return this;
            }
        }

    }
}