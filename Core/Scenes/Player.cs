using System;
using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.Control;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.Interactivity;
using Dose.Core.Library.Locations;
using Dose.Core.Library.Quests;
using Dose.Core.QuestArgs;
using Dose.Core.Quests;
using Godot;
using static Dose.Core.Library.GodotUtils;

namespace Dose.Core.Scenes
{
    public partial class Player : CharacterBody3D, ITalker, IPicker, IDoorOpener, IActionExecuter, IStairsUser
    {
        enum MoveState
        {
            Locked,
            Auto,
            Looking,
            Focused,
            Free
        }

        enum InteractionState
        {
            Curious,
            Talking,
            Locked,
            HoldingItem,
            QuestBookOpened
        }

        class AutoMoveParameters(Vector3 position, float speed)
        {
            public float Speed { get; } = speed;
            public Vector3 Position { get; } = position;
            public EventHandler? Completed { get; set; }
        }
       
        const float RAY_LENGTH = 2.3f;
        
        RayCast3D? _rayCast;
        float _cameraAngle = 0f;
        private float _mouseSensitivity = 0.1f;
        float _moveSpeed = 20f;
        AutoMoveParameters? _autoMoveParameters;
        Node3D? _head;
        Camera3D? _view;
        IActor<Player>? _currentTarget;
        IControlPlane? _controlPlane;
        MoveState _moveState = MoveState.Locked;
        InteractionState _interactionState;

        public DialogueRunner? DialogueRunner { get; private set; }
        public Inventory Inventory { get; } = [];
        public QuestBook QuestBook { get; } = [];
        public ItemContext? SelectedItem { get; protected set; }

        public override void _Ready()
        {
            _moveState = MoveState.Free;
            Input.MouseMode = Input.MouseModeEnum.Captured;
            _head = GetNode<Node3D>("Head");
            _view = GetNode<Camera3D>("Head/View");
            _rayCast = GetNode<RayCast3D>("Head/View/RayCast3D");
            _rayCast.TargetPosition = new Vector3(0, 0, -RAY_LENGTH);
            
            Inventory.ItemAdded += (sender, e) =>
            {
                QuestBook.TestCompleted<FindItemQuest, FindItemQuestArgs>(new FindItemQuestArgs 
                { 
                    ItemContext = e.ItemContext
                });
            };

            Inventory.ItemDropped += (sender, e) =>
            {
                if (!Inventory.Any())
                {
                    _interactionState = InteractionState.Curious;
                }  
            };
        }

        public override void _Process(double delta)
        {
            ProcessInteract();
            ProcessInventory();
            ProcessDialogue();
            ProcessQuestBook();
        }

        public override void _PhysicsProcess(double delta)
        {
            ProcessSeek();
            ProcessWalk();
            ProcessAutoMove((float)delta);
            ProcessFocus();
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseMotion motion)
            {
                ProcessLook(motion);
            }
        }

        public Player Initialize(IControlPlane controlPlane)
        {
            _controlPlane = controlPlane;

            return this;
        }

        public void OnDialogueBegin(DialogueRunner dialogueRunner)
        {
            DialogueRunner = dialogueRunner;
            
            if (dialogueRunner.Dialogue is Dialogue)
            {
                _moveState = MoveState.Focused;
                _interactionState = InteractionState.Talking;
            }
            
            UpdateDialogue();
        }

        public bool CanSeek()
        {
            return CanLook() && _moveState != MoveState.Focused;
        }

        public bool CanMove()
        {
            return _moveState == MoveState.Free;
        }

        public bool CanLook()
        {
            return
                _moveState == MoveState.Free || _moveState == MoveState.Looking || _moveState == MoveState.Focused &&
                Input.MouseMode == Input.MouseModeEnum.Captured;
        }

        public bool CanInteract()
        {
            return _interactionState != InteractionState.Locked && _interactionState != InteractionState.QuestBookOpened;
        }

        public bool CanSwitchItem()
        {
            return _interactionState == InteractionState.Curious;
        }

        public bool CanSelectItem()
        {
            return _interactionState == InteractionState.Curious || _interactionState == InteractionState.HoldingItem;
        }

        bool CanCloseQuestBook()
        {
            return _interactionState == InteractionState.QuestBookOpened;
        }
        
        bool CanOpenQuestBook()
        {
            return _interactionState == InteractionState.Curious || _interactionState == InteractionState.HoldingItem;
        }

        public void ProgressDialogue()
        {
            if (DialogueRunner == null)
            {
                GD.PushWarning("Dialogue dialogueRunner is null. Can't progress dialogue.");
                _moveState = MoveState.Free;
                _interactionState = InteractionState.Curious;
                _controlPlane?.DialogueController?.OnDialogueEnd();

                return;
            }

            if (!DialogueRunner.GoToNextNode())
            {
                DialogueRunner = null;
                _moveState = MoveState.Free;
                _interactionState = InteractionState.Curious;
                _controlPlane?.DialogueController?.OnDialogueEnd();

                UpdateTarget();

                if (_currentTarget is ISpeakable speakable)
                {
                    speakable.Speaker?.Stop();
                }

                return;
            }

            UpdateDialogue();
        }

        public void ProcessAutoMove(float delta)
        {
            if (_moveState != MoveState.Auto) return;
            if (_autoMoveParameters == null) return;

            var distance = _autoMoveParameters.Position - GlobalPosition;

            Velocity = distance.Normalized() * _autoMoveParameters.Speed;

            if (distance.Length() <= _autoMoveParameters.Speed * delta)
            {
                var completedCallback = _autoMoveParameters.Completed;

                _moveState = MoveState.Free;
                _autoMoveParameters = null;
                completedCallback?.Invoke(_autoMoveParameters, new EventArgs());

                return;
            }
            
            MoveAndSlide();

            for (int i = 0; i < GetSlideCollisionCount(); i++)
            {
                KinematicCollision3D collision = GetSlideCollision(i);

                if (collision.GetCollider() is RigidBody3D body)
                {
                    body.LinearDamp = 15f;
                    body.ApplyCentralImpulse(-collision.GetNormal() * 20f);
                }
            }
        }

        public void OnDoorOpened(IDoor door)
        {
            Transform3D? local = null;

            if (door is Door doorNode)
            {
                local = doorNode.GlobalTransform.AffineInverse() * GlobalTransform;
            }

            _controlPlane?.LocationController?.UpdateLocation(door);

            if (local != null && door.Linked is Door linkedDoorNode)
            {
                GlobalTransform = linkedDoorNode.GlobalTransform * (Transform3D)local;

                var targetPosition = linkedDoorNode.GetHingeCenter();
                var parameters = new AutoMoveParameters(targetPosition, 5f);
                parameters.Completed += (sender, e) =>
                {
                    //door.OpenDirection = ClockOpposite(door.OpenDirection);
                    var sign = door.OpenDirection == ClockDirection.Clockwise ? 1 : -1;
                    var parameters = new AutoMoveParameters(targetPosition + linkedDoorNode.GlobalTransform.Basis.Z * 2f * sign, 5f);

                    parameters.Completed += (sender, e) => linkedDoorNode.TryClose();

                    AutoMoveTo(parameters);
                };

                AutoMoveTo(parameters);

                linkedDoorNode.TryOpen();
            }
        }


        void UpdateDialogue()
        {
            if (DialogueRunner == null) return;

            _controlPlane?.DialogueController?.OnDialogueChange(DialogueRunner.GetCurrentNode());
        }

        void UpdateTarget()
        {
            _controlPlane?.InteractivityController?.OnTargetChange(_currentTarget);
        }

        void LookAt(Vector3 to, double delta)
        {
            if (_head == null || _view == null)
            {
                GD.PushWarning("View not defined. Skipping LookAt.");
                return;
            }

            Vector3 from = _head.GlobalPosition;

            // --- YAW ONLY (this preserves your original correct behavior) ---
            Vector3 flatDir = to - from;
            flatDir.Y = 0;

            Basis targetBasis = Basis.LookingAt(flatDir, Vector3.Up);

            Basis currentBasis = _head.GlobalTransform.Basis;
            currentBasis = currentBasis.Orthonormalized();
            targetBasis = targetBasis.Orthonormalized();

            Basis newBasis = currentBasis.Slerp(targetBasis, (float)(delta * 5f));
            _head.GlobalTransform = new Transform3D(newBasis, from);

            // --- PITCH (separate, clean) ---
            Vector3 dir = (to - from).Normalized();
            float targetPitch = Mathf.Asin(dir.Y);

            float newPitch = Mathf.LerpAngle(_view.Rotation.X, targetPitch, (float)(delta * 5f));
            newPitch = Mathf.Clamp(newPitch, Mathf.DegToRad(-90), Mathf.DegToRad(90));

            _view.Rotation = new Vector3(newPitch, 0, 0);

            // keep mouse system in sync
            _cameraAngle = Mathf.RadToDeg(newPitch);
        }

        void ProcessDialogue()
        {
            DialogueRunner?.GetCurrentNode().Process();
        }

        void ProcessInteract()
        {
            if (!CanInteract()) return;

            if (Input.IsActionJustPressed("Interact"))
            {
                Interact();
            }
        }

        void ProcessQuestBook()
        {
            if (_interactionState == InteractionState.QuestBookOpened)
            {
                if (Input.IsActionJustPressed("ToggleQuestBook") || Input.IsActionJustPressed("Interact") || Input.IsActionJustPressed("InventorySelect"))
                {
                    CloseQuestBook();
                }

                return;
            }

            if (Input.IsActionJustPressed("ToggleQuestBook"))
            {
                OpenQuestBook();
            }     
        }

        void OpenQuestBook()
        {
            if (!CanOpenQuestBook()) return;

            UnselectItem();
            _interactionState = InteractionState.QuestBookOpened;
            _controlPlane?.QuestBookController?.OnQuestBookOpened();
        }

        void CloseQuestBook()
        {
            if (!CanCloseQuestBook()) return;

            _interactionState = InteractionState.Curious;
            _controlPlane?.QuestBookController?.OnQuestBookClosed();  
        }

        void ProcessSeek()
        {
            if (!CanSeek()) return;

            var previousTarget = _currentTarget;
            _currentTarget = null;

            if (_rayCast == null) return;

            if (_rayCast.IsColliding())
            {
                var collider = _rayCast.GetCollider();

                if (collider is Node node)
                {
                    if (TryGetParentNode(node, n => n is IActor<Player>, out Node? _interactable))
                    {
                        _currentTarget = (IActor<Player>?)_interactable;
                    }
                }
            }

            if (_currentTarget != previousTarget)
            {
                UpdateTarget();
            }
        }

        void ProcessLook(InputEventMouseMotion motion)
        {
            if (!CanLook()) return;
            if (_head == null) return;
            if (_view == null) return;

            _head.RotateY(Mathf.DegToRad(-motion.Relative.X * _mouseSensitivity));
                float change = -motion.Relative.Y * _mouseSensitivity;

                if ((change + _cameraAngle) < 90f && (change + _cameraAngle) > -90f)
                {
                    _view.RotateX(Mathf.DegToRad(change));
                    _cameraAngle += change;
                }
        }

        void ProcessInventory()
        {
            if (CanSwitchItem())
            {
                if (Input.IsActionJustPressed("InventoryPreviousItem"))
                    _controlPlane?.InventoryController?.GoToPreviousItem();

                if (Input.IsActionJustPressed("InventoryNextItem"))
                    _controlPlane?.InventoryController?.GoToNextItem();      
            }

            if (CanSelectItem() && Input.IsActionJustPressed("InventorySelect"))
            {
                if (_interactionState == InteractionState.HoldingItem)
                {
                    UnselectItem();
                }
                else if (_controlPlane?.InventoryController?.CurrentItem != null)
                {
                    SelectItem();
                }
            }
        }

        void UnselectItem()
        {
            SelectedItem = null;
            _interactionState = InteractionState.Curious;
            _controlPlane?.InventoryController?.OnActiveItemUnselected();
        }

        void SelectItem()
        {
            SelectedItem = _controlPlane?.InventoryController?.CurrentItem;
            _interactionState = InteractionState.HoldingItem;
            _controlPlane?.InventoryController?.OnActiveItemSet();
        }

        void ProcessWalk()
        {
            if (!CanMove()) return;
            if (_view == null) return;
            
            Vector3 direction = Vector3.Zero;
            Vector3 forward = -_view.GlobalTransform.Basis.Z;
            Vector3 right = _view.GlobalTransform.Basis.X;

            forward.Y = 0;
            right.Y = 0;
            forward = forward.Normalized();
            right = right.Normalized();

            if (Input.IsActionPressed("MoveForward"))
                direction += forward;
            if (Input.IsActionPressed("MoveBackward"))
                direction -= forward;
            if (Input.IsActionPressed("MoveLeft"))
                direction -= right;
            if (Input.IsActionPressed("MoveRight"))
                direction += right;

            Velocity = direction.Normalized() * _moveSpeed;
            MoveAndSlide();
        }

        void ProcessFocus()
        {
            if (_moveState != MoveState.Focused) return;

            if (_currentTarget == null)
            {
                GD.PushWarning($"{nameof(_currentTarget)} is null. Setting MoveState to Free.");

                _moveState = MoveState.Free;

                return;
            }

            if (_currentTarget is not ILookable lookable)
            {
                GD.PushWarning($"{nameof(_currentTarget)} is not an ${nameof(ILookable)}");

                _moveState = MoveState.Free;

                return;
            }

            var lookAtPoint = GetLookableLookAtPoint(lookable);

            if (lookAtPoint == null)
            {
                return;
            }

            LookAt((Vector3)lookAtPoint, 0.01);
        }

        Vector3? GetLookableLookAtPoint(ILookable lookable, float refDist = 3) {
            if (lookable is not Node3D lookableNode)
            {
                GD.PushWarning($"{nameof(lookable)} is not a {nameof(Node3D)}. Can't execute {nameof(GetLookableLookAtPoint)}.");
                return null;
            }

            var dist = GlobalPosition.DistanceTo(lookableNode.GlobalPosition);

            Vector3 prop = (1 - (dist / refDist)) * (lookable.TopLookTarget - lookable.CenterLookTarget);

            return lookable.CenterLookTarget + prop;
        }

        void Interact()
        {
            switch (_interactionState)
            {
                case InteractionState.Curious:
                    _currentTarget?.OnInteract(_currentTarget.GetDefaultInteraction(this));
                    break;

                case InteractionState.Talking:
                    ProgressDialogue();
                    break;

                case InteractionState.HoldingItem:
                    if (_currentTarget != null) UseItemWith(_currentTarget);
                    break;
            }          
        }

        void UseItemWith(IActor<Player> actor)
        {
            var item = _controlPlane?.InventoryController?.CurrentItem;

            if (item == null)
            {
                GD.PushWarning($"Player is not holding any item. Skipping {nameof(UseItemWith)}.");
                return;
            }

            if (actor.Context == null)
            {
                GD.PushWarning($"Actor context is undefined. Skipping {nameof(UseItemWith)}.");
                return;
            }

            foreach (var op in item.InteractWith)
            {
                if (op.TryInteract(item, actor.Context))
                {
                    GD.Print($"Interacted with actor \"{actor.Context.Identifier}\" using item \"{item.Identifier}\".");
                }
            }
        }

        public bool ExecuteAction(IActionPerformer actionPerformer, Dictionary<string, object?> args)
        {
            if (_currentTarget is null)
            {
                GD.PushWarning($"{nameof(_currentTarget)} is null. Skipping {ExecuteAction}...");
                return true;
            }

            return actionPerformer.Perform(_currentTarget, args);
        }

        void AutoMoveTo(AutoMoveParameters parameters)
        {
            _autoMoveParameters = parameters;
            _moveState = MoveState.Auto;
        }

        public void UseStairs(IStairs stairs)
        {
            _moveState = MoveState.Locked;
            _interactionState = InteractionState.Locked;

            _ = FadeTransition.Instance?.Fade(() =>
            {
                _controlPlane?.LocationController?.UpdateLocation(stairs);

                if (stairs is Stairs stairsNode && stairsNode.Linked is Stairs linked)
                {
                    var targetTransform = linked.GlobalTransform;

                    // "South" → usually forward → -Z
                    Vector3 offset = targetTransform.Basis.Z * 2f;

                    // Keep same height or align with stairs
                    Vector3 newPosition = targetTransform.Origin + offset;

                    GlobalTransform = new Transform3D(
                        targetTransform.Basis, // match rotation of stairs
                        newPosition
                    );

                    if (_head != null)
                    {
                        var localBasis = linked.GlobalTransform.Basis.Inverse() * linked.GlobalTransform.Basis;
                        _head.Rotation = (localBasis * Basis.FromEuler(new Vector3(0, Mathf.Pi, 0))).GetEuler();
                    }

                    _moveState = MoveState.Free;
                    _interactionState = InteractionState.Curious;
                }
            }, 2);
        }
    }
}