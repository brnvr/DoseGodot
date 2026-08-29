using Dose.Core.Library;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Scenes
{
    public abstract partial class Actor<TContext> : RigidBody3D, IActor<Player> where TContext : IActorContext
    {
        protected MeshInstance3D? MeshInstance { get; set; }

        public abstract TContext? Context { get; protected set; }

        IActorContext? IActor.Context => Context;

        public abstract IInteraction<Player> GetDefaultInteraction(Player interactor);

        public override void _Ready()
        {
            LinearDamp = 15f;
        }
    
        public void OnInteract(IInteraction<Player> interaction)
        {
            interaction.Perform(this);
        }

        protected void UpdateTexture()
        {
            if (MeshInstance == null || Context == null)
            {
                GD.PushWarning($"{GetType().Name} not initialized. Skipping {nameof(UpdateTexture)}.");

                return;
            }

            var material = GodotUtils.CreateBillboardMaterial(Context.Texture);

            MeshInstance.SetSurfaceOverrideMaterial(0, material);
        }
    }
}
