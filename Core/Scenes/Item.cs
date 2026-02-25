using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class Item : Node3D, IActor<Player>, IInitializableActor, IItem
    {
        public ItemContext? Context { get; private set; }
        IActorContext? IActor.Context => Context;

        public Item Initialize(ItemContext context)
        {
            Context = context;
            return this;
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