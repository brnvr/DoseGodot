using System.Linq;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Control;
using Dose.Core.Library.DataStructures;
using Godot;
using static Dose.Core.Library.GodotUtils;

namespace Dose.Core.Scenes
{
    public partial class Player : BasePlayer
    {
        const float RAY_LENGTH = 2.3f;
        
        private RayCast3D? _rayCast;
        

        private float _cameraAngle = 0f;
        private float _mouseSensitivity = 0.1f;
        private float _moveSpeed = 20f;

        public override void _Ready()
        {
            Inventory = [];
            InventoryController = new InventoryController(Inventory);
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
            ProcessFocus();
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseMotion motion)
            {
                ProcessLook(motion);
            }
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
                InventoryController?.GoToPreviousItem();

            if (Input.IsActionJustPressed("InventoryNextItem"))
                InventoryController?.GoToNextItem();

            if (Input.IsActionJustPressed("InventorySelect"))
                SelectedItem = SelectedItem is null ? InventoryController?.CurrentItem : null;
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
    }
}