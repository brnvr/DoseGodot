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
            MeshInstance = GetNode<MeshInstance3D>("Mesh");
            var scale = Vector3.One * context.Scale;
            MeshInstance.Scale *= scale;
            var collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
            collisionShape.Scale *= scale;
            collisionShape.Position *= context.Scale;
            var interactionShape = GetNode<CollisionShape3D>("InteractionArea/CollisionShape3D");
            interactionShape.Scale *= scale;
            interactionShape.Position *= context.Scale;
            UpdateTexture();
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
