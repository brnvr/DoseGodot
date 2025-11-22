using System.Linq;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
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
            _moveState = MoveState.Free;
            //Inventory = new Inventory();
            Input.MouseMode = Input.MouseModeEnum.Captured;
            _head = GetNode<Node3D>("Head");
            _view = GetNode<Camera3D>("Head/View");
            _rayCast = GetNode<RayCast3D>("Head/View/RayCast3D");
            _rayCast.TargetPosition = new Vector3(0, 0, -RAY_LENGTH);
        }

        public override void _Process(double delta)
        {
            if (Input.IsActionPressed("ui_cancel"))
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
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

            var shouldInteract = @event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed;

            if (shouldInteract)
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

            if (Input.IsActionPressed("ui_up"))
                direction += forward;
            if (Input.IsActionPressed("ui_down"))
                direction -= forward;
            if (Input.IsActionPressed("ui_left"))
                direction -= right;
            if (Input.IsActionPressed("ui_right"))
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