using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.ESpeak;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class Npc : Node3D, ITalkable, ISpeakable, INpc, IActor<Player>
    {
        MeshInstance3D? _mesh;

        public ISpeaker? Speaker { get; private set; }
        public NpcContext? Context { get; private set; }

        IActorContext? IActor.Context => Context;

        public Npc Initialize(NpcContext context, Player player)
        {
            Context = context;
            
            if (context.Voice == null)
            {
                GD.PushWarning("Voice not defined for NpcContext. Speaker won't be constructed.");
            }
            else
            {
                Speaker = new ESpeaker(context.Voice, player.ProgressDialogue);

                AddChild((Node)Speaker);
            }

            _mesh = GetNode<MeshInstance3D>("Mesh");

            UpdateTexture();

            return this;
        }

        public IDialogue? GetDialogue()
        {
            return Context?.Dialogue;
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
                stdMat.AlbedoTexture = Context.Texture;
            }
        }
    }
}
