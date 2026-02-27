using System;
using Dose.Core.ActorArgs;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.ESpeak;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class Npc : Node3D, ITalkable, ISpeakable, INpc, IActor<Player>, IInitializableActor<NpcContext, NpcArgs>
    {
        MeshInstance3D? _mesh;

        public ISpeaker? Speaker { get; private set; }
        public NpcContext? Context { get; private set; }

        IActorContext? IActor.Context => Context;
    
        public void Initialize(NpcContext context, NpcArgs args)
        {
            Context = context;

            _mesh = GetNode<MeshInstance3D>("Mesh");

            UpdateTexture();

            if (Context.Voice == null)
            {
                GD.Print($"No voice assigned to NPC {Context.DisplayName}. Skipping speaker initialization.");
                return;
            }

            if (args.Player == null)
            {
                GD.Print($"No {nameof(args.Player)} provided in NpcArgs for NPC {Context.DisplayName}. Skipping speaker initialization.");
                return;
            }

            Speaker = new ESpeaker(Context.Voice, args.Player.ProgressDialogue);
            
            AddChild((Node)Speaker);
        }

        public override void _ExitTree()
        {
            if (Speaker?.IsSpeaking == true)
            {
                Speaker?.Stop();
            }

            base._ExitTree();

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
