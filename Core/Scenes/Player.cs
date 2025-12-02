using System;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Control;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.Interactivity;
using Dose.Core.Library.Locations;
using Godot;
using static Dose.Core.Library.GodotUtils;

namespace Dose.Core.Scenes
{
    public partial class Player : CharacterBody3D, ITalker, IPicker, IDoorOpener
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
            Talking
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
        public ItemContext? SelectedItem { get; protected set; }

        public override void _Ready()
        {
            _moveState = MoveState.Free;
            Input.MouseMode = Input.MouseModeEnum.Captured;
            _head = GetNode<Node3D>("Head");
            _view = GetNode<Camera3D>("Head/View");
            _rayCast = GetNode<RayCast3D>("Head/View/RayCast3D");
            _rayCast.TargetPosition = new Vector3(0, 0, -RAY_LENGTH);
        }

        public override void _Process(double delta)
        {
            ProcessInteract();
            ProcessInventory();
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

        public bool CanMove()
        {
            return _moveState == MoveState.Free;
        }

        public bool CanLook()
        {
            return _moveState == MoveState.Free || _moveState == MoveState.Looking;
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
                _autoMoveParameters.Completed?.Invoke(_autoMoveParameters, new EventArgs());
                _autoMoveParameters = null;
                _moveState = MoveState.Free;

                return;
            }
            
            MoveAndSlide();
        }

        public void OnDoorOpened(IDoor door)
        {
            Transform3D? local = null;

            if (door is Door doorNode)
            {
                local = doorNode.GlobalTransform.AffineInverse() * GlobalTransform;
            }

            _controlPlane?.LocationController?.UpdateLocation(door);

            if (local != null && door.LinkedDoor is Door linkedDoorNode)
            {
                //var rot180 = new Basis(Vector3.Up, Mathf.Pi);
                // var _local = (Transform3D)local;
                // _local.Basis = rot180 * _local.Basis;
                // _local.Origin = rot180 * _local.Origin;
                GlobalTransform = linkedDoorNode.GlobalTransform * (Transform3D)local;

                var sign = door.OpenDirection == ClockDirection.Clockwise ? 1 : -1;
                var targetPosition = linkedDoorNode.GlobalPosition + linkedDoorNode.GlobalTransform.Basis.Z * 2f * sign;
                var parameters = new AutoMoveParameters(targetPosition, 5f);
                parameters.Completed += (sender, e) => linkedDoorNode.TryClose();

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

        void LookAt(Node3D node, double delta)
        {
            if (_head == null)
            {
                GD.PushWarning("View not defined. Skipping LookAt.");
                return;
            }

            Vector3 from = _head.GlobalPosition;
            Vector3 to = node.GlobalPosition + new Vector3(0, 2, 0);

            // Compute desired rotation
            Basis targetBasis = Basis.LookingAt(to - from, Vector3.Up);

            // Smooth rotation
            Basis currentBasis = _head.GlobalTransform.Basis;
            Basis newBasis = currentBasis.Slerp(targetBasis, (float)(delta * 5f));

            _head.GlobalTransform = new Transform3D(newBasis, from);
        }

        void ProcessInteract()
        {
            if (Input.IsActionJustPressed("Interact"))
            {
                Interact();
            }
        }

        void ProcessSeek()
        {
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
            if (Input.IsActionJustPressed("InventoryPreviousItem"))
                _controlPlane?.InventoryController?.GoToPreviousItem();

            if (Input.IsActionJustPressed("InventoryNextItem"))
                _controlPlane?.InventoryController?.GoToNextItem();

            if (Input.IsActionJustPressed("InventorySelect"))
                SelectedItem = SelectedItem is null ? _controlPlane?.InventoryController?.CurrentItem : null;
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
                GD.PushWarning("_currentTarget is null. Setting MoveState to Free.");

                _moveState = MoveState.Free;

                return;
            }

            if (_currentTarget is not Node3D node)
            {
                GD.PushWarning("_currentTarget is not a Node3D");

                _moveState = MoveState.Free;

                return;
            }

            LookAt(node, 0.01);
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
            }          
        }

        void AutoMoveTo(AutoMoveParameters parameters)
        {
            _autoMoveParameters = parameters;
            _moveState = MoveState.Auto;
        }
    }
}