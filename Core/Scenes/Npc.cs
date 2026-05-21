using Dose.Core.ActorArgs;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.ESpeak;
using Dose.Core.Library.GameEvents;
using Dose.Core.Library.Interactivity;
using Dose.Core.Library.Randomness;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class Npc : RigidBody3D, ITalkable, ISpeakable, INpc, IActor<Player>, IInitializableActor<NpcContext, NpcArgs>, ILookable, IDisappearable
    {
        MeshInstance3D? _mesh;

        public IGameEvent<Npc>? GameEvent { get; private set; }
        public float EyeHeightProportion { get; private set; } = 0.9f;
        public ISpeaker? Speaker { get; private set; }
        public NpcContext? Context { get; private set; }
        public Node3D LookTarget => GetNode<Node3D>("LookTarget");
        public Vector3 TopLookTarget => GlobalPosition + new Vector3(0, _mesh?.GetAabb().Size.Y * EyeHeightProportion ?? 0, 0);
        public Vector3 CenterLookTarget => GlobalPosition + new Vector3(0, _mesh?.GetAabb().Size.Y * .5f ?? 0, 0);

        IActorContext? IActor.Context => Context;

        public override void _Ready()
        {
            LinearDamp = 15f;
        }

        public override void _Process(double delta)
        {
            if (GameEvent != null && GameEvent.ShouldTrigger(this))
            {
                GameEvent.OnTrigger(this);
            } 
        }

        public void Initialize(NpcContext context, NpcArgs args)
        {
            Context = context;
            GameEvent = args.GameEvent;
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
            if (Context == null)
            {
                return null;
            }

            var dialogueKey = new RandomSelector<string>(Context.AvailableDialogueKeys).Pick();
            return Context.DialogueMap[dialogueKey];
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

        public void Disappear()
        {
            QueueFree();
        }
    }
}
