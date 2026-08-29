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
    public partial class Npc : Actor<NpcContext>, IFollower<Player>, ITalkable, ISpeakable, INpc, IInitializableActor<NpcContext, NpcArgs>, ILookable, IDisappearable
    {
        NavigationAgent3D? _navigationAgent;
        Following<Player>? _following;
        Vector3? _lastNavigationTarget;
        float _followSpeed;
        uint _collisionLayerBeforeFollow;
        uint _collisionMaskBeforeFollow;
        bool _lockRotationBeforeFollow;
        bool _collisionDisabledForFollow;

        public IGameEvent<Npc>? GameEvent { get; private set; }
        public float EyeHeightProportion { get; private set; } = 0.9f;
        public ISpeaker? Speaker { get; private set; }
        public override NpcContext? Context { get ; protected set ; }
        public Node3D LookTarget => GetNode<Node3D>("LookTarget");
        float BillboardHeight => MeshInstance is { } mesh ? mesh.GetAabb().Size.Y * mesh.Scale.Y : 0f;
        public Vector3 TopLookTarget => GlobalPosition + new Vector3(0, BillboardHeight * EyeHeightProportion, 0);
        public Vector3 CenterLookTarget => GlobalPosition + new Vector3(0, BillboardHeight * .5f, 0);

        public override void _Ready()
        {
            _navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent");
        }


        public override void _Process(double delta)
        {
            if (GameEvent != null && GameEvent.ShouldTrigger(this))
            {
                GameEvent.OnTrigger(this);
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_following == null || _navigationAgent == null)
            {
                return;
            }

            var map = _navigationAgent.GetNavigationMap();

            if (NavigationServer3D.MapGetIterationId(map) == 0)
            {
                Stop();
                return;
            }

            if (_navigationAgent.IsNavigationFinished())
            {
                Stop();
                return;
            }

            var nextPosition = _navigationAgent.GetNextPathPosition();
            var direction = nextPosition - GlobalPosition;
            direction.Y = 0;

            if (direction.IsZeroApprox())
            {
                Stop();
                return;
            }

            direction = direction.Normalized();
            LinearVelocity = new Vector3(
                direction.X * _followSpeed,
                LinearVelocity.Y,
                direction.Z * _followSpeed
            );
        }

        public void Initialize(NpcContext context, NpcArgs args)
        {
            Context = context;
            GameEvent = args.GameEvent;
            MeshInstance = GetNode<MeshInstance3D>("Mesh");
            var scale = Vector3.One * context.Scale;
            MeshInstance.Scale *= scale;
            var collisionShape = GetNode<CollisionShape3D>("CollisionShape");
            collisionShape.Scale *= scale;
            collisionShape.Position *= context.Scale;
            var interactionShape = GetNode<CollisionShape3D>("InteractionArea/CollisionShape3D");
            interactionShape.Scale *= scale;
            interactionShape.Position *= context.Scale;
            LookTarget.Position *= context.Scale;

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

        public override IInteraction<Player> GetDefaultInteraction(Player interactor)
        {
            return new TalkInteraction<Player>(interactor);
        }

        public void Disappear()
        {
            QueueFree();
        }

        public void BeginFollow(Player target, float speed = 5f, float targetUpdateIntervalSeconds = 1f)
        {
            if (!_collisionDisabledForFollow)
            {
                _collisionLayerBeforeFollow = CollisionLayer;
                _collisionMaskBeforeFollow = CollisionMask;
                _lockRotationBeforeFollow = LockRotation;

                CollisionLayer = 0;
                CollisionMask = 0;
                LockRotation = true;
                AngularVelocity = Vector3.Zero;
                _collisionDisabledForFollow = true;
            }

            _following = new(this, target, speed, targetUpdateIntervalSeconds);
            _following.Start(this);
        }

        public void EndFollow()
        {
            _following?.Stop();
            _following = null;
            _lastNavigationTarget = null;

            if (_collisionDisabledForFollow)
            {
                CollisionLayer = _collisionLayerBeforeFollow;
                CollisionMask = _collisionMaskBeforeFollow;
                LockRotation = _lockRotationBeforeFollow;
                AngularVelocity = Vector3.Zero;
                _collisionDisabledForFollow = false;
            }
        }

        public void Follow(Player target, float speed)
        {
            if (_navigationAgent == null) return;

            _followSpeed = speed;

            if (_lastNavigationTarget?.IsEqualApprox(target.GlobalPosition) == true)
            {
                return;
            }

            _lastNavigationTarget = target.GlobalPosition;
            _navigationAgent.TargetPosition = target.GlobalPosition;
        }

        public void Stop()
        {
            LinearVelocity = new Vector3(0, LinearVelocity.Y, 0);
        }

        public bool HasReached(Player target)
        {
            return _navigationAgent == null || _navigationAgent.IsNavigationFinished();
        }
    }
}
