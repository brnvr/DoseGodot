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
                GD.PushWarning($"{nameof(Npc)} not initialized. Skipping {nameof(UpdateTexture)}.");

                return;
            }

            MeshInstance.SetSurfaceOverrideMaterial(0, new StandardMaterial3D
            {
                AlbedoTexture = Context.Texture,
                BillboardMode = BaseMaterial3D.BillboardModeEnum.FixedY,
                Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
                TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            });
        }
    }
}