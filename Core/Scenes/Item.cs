using System;
using Dose.Core.ActorArgs;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class Item : Actor<ItemContext>, IActor<Player>, IInitializableActor<ItemContext, ItemArgs>, IItem
    {
        public override ItemContext? Context { get; protected set; }
        IActorContext? IActor.Context => Context;
        Node3D? _pickupTarget;
        Inventory? _pickupInventory;

        [Export(PropertyHint.Range, "0.1,100,0.1,or_greater")]
        public float PickupSpeed { get; set; } = 12f;

        [Export(PropertyHint.Range, "0.01,2,0.01,or_greater")]
        public float PickupArrivalDistance { get; set; } = 0.15f;

        public override void _Ready()
        {
            base._Ready();
            SetPhysicsProcess(false);
        }

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

        public void BeginPickup(IPicker picker)
        {
            if (_pickupTarget != null || Context == null || picker.Inventory == null)
            {
                return;
            }

            _pickupTarget = picker.PickupTarget;
            _pickupInventory = picker.Inventory;

            CollisionLayer = 0;
            CollisionMask = 0;
            Freeze = true;
            LinearVelocity = Vector3.Zero;
            AngularVelocity = Vector3.Zero;

            var interactionArea = GetNode<Area3D>("InteractionArea");
            interactionArea.CollisionLayer = 0;
            interactionArea.CollisionMask = 0;
            interactionArea.Monitoring = false;
            interactionArea.Monitorable = false;

            var currentScene = GetTree().CurrentScene;
            if (currentScene != null && GetParent() != currentScene)
            {
                Reparent(currentScene, true);
            }

            SetPhysicsProcess(true);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_pickupTarget == null ||
                !IsInstanceValid(_pickupTarget) ||
                _pickupInventory == null)
            {
                SetPhysicsProcess(false);
                return;
            }

            var destination = _pickupTarget.GlobalPosition;
            GlobalPosition = GlobalPosition.MoveToward(
                destination,
                PickupSpeed * (float)delta
            );

            if (GlobalPosition.DistanceTo(destination) > PickupArrivalDistance)
            {
                return;
            }

            if (Context != null)
            {
                _pickupInventory.Add(Context);
            }

            Destroy();
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
