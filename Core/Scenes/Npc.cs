using System;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.ESpeak;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class Npc : Node3D, ITalkable, ISpeakable, INpc, IActor<Player>, IInitializableActor
    {
        MeshInstance3D? _mesh;

        public ISpeaker? Speaker { get; private set; }
        public NpcContext? Context { get; private set; }

        IActorContext? IActor.Context => Context;

       IActor IInitializableActor.Initialize(IActorContext context)
        {
            if (context is not NpcContext npcContext)
            {
                GD.PushError($"{nameof(Npc)}.{nameof(IInitializableActor.Initialize)} called with non-{nameof(NpcContext)}.");
                return this;
            }

            InitializeContext(npcContext);

            return this;
        }
    
        public Npc InitializeContext(NpcContext context)
        {
            Context = context;

            _mesh = GetNode<MeshInstance3D>("Mesh");

            UpdateTexture();

            return this;
        }

        public Npc InitializeSpeaker(Action onDialogueComplete)
        {
            if (Context == null)
            {
                GD.PushWarning($"{nameof(Npc)}.{nameof(InitializeSpeaker)} called before {nameof(InitializeContext)}. {nameof(Speaker)} won't be constructed.");
                return this;
            }

            if (Context.Voice == null)
            {
                GD.PushWarning($"{nameof(Npc)}.{nameof(InitializeSpeaker)} called with null voice in {nameof(NpcContext)}. {nameof(Speaker)} won't be constructed.");
                return this;
            }

            Speaker = new ESpeaker(Context.Voice, onDialogueComplete);

            AddChild((Node)Speaker);

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
                GD.PushWarning($"{nameof(Npc)} not initialized. Skipping {nameof(UpdateTexture)}.");

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
