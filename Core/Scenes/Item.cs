using System;
using Dose.Core.ActorArgs;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class Item : Actor<ItemContext>, IActor<Player>, IInitializableActor<ItemContext, ItemArgs>, IItem
    {
        public override ItemContext? Context { get; protected set; }
        IActorContext? IActor.Context => Context;

        public void Initialize(ItemContext context, ItemArgs args)
        {
            Context = context;
        }

        public void Destroy()
        {
            QueueFree();
        }

        public override IInteraction<Player> GetDefaultInteraction(Player interactor)
        {
            return new PickInteraction<Player>(interactor);
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