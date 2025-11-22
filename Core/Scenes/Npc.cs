using System;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class Npc : Node3D, ITalkable, IActor<Player>
    {
        MeshInstance3D? _mesh;

        public IActorContext? Context { get; private set; }

        public Npc Initialize(NpcContext context)
        {
            Context = context;

            _mesh = GetNode<MeshInstance3D>("Mesh");

            UpdateTexture();

            return this;
        }

        public Dialogue? GetDialogue()
        {
            return ((NpcContext?)Context)?.Dialogue;
        }

        public void OnInteract(IInteraction<Player> interaction)
        {
            interaction.Perform(this);
        }

        public IInteraction<Player> GetDefaultInteraction(Player interactor)
        {
            return new TalkInteraction<Player>(interactor);
        }

        void UpdateTexture()
        {
            if (_mesh == null || Context == null)
            {
                GD.PushWarning("NPC not initialized. Skipping UpdateTexture");

                return;
            }

            var material = _mesh.GetActiveMaterial(0);

            if (material is StandardMaterial3D stdMat)
            {
                stdMat.AlbedoTexture = ((NpcContext)Context).Texture;
            }
        }
    }
}
