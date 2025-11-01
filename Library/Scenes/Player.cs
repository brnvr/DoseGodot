using Dose.Library.Interactivity;
using Godot;
using static Dose.Library.GodotUtils;

namespace Dose.Library.Scenes
{
    public partial class Player : CharacterBody3D
    {
        const float RAY_LENGTH = 2.3f;

        private Node3D? _head;
        private Camera3D? _view;
        private RayCast3D? _rayCast;
        private IInteractable<Player>? _currentTarget;

        private float _cameraAngle = 0f;
        private float _mouseSensitivity = 0.1f;
        private float _moveSpeed = 20f;

        public override void _Ready()
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            _head = GetNode<Node3D>("Head");
            _view = GetNode<Camera3D>("Head/View");
            _rayCast = GetNode<RayCast3D>("Head/View/RayCast3D");
            _rayCast.TargetPosition = new Vector3(0, 0, -RAY_LENGTH);
        }

        public override void _Process(double delta)
        {
            if (Input.IsActionPressed("ui_cancel"))
                Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        public override void _PhysicsProcess(double delta)
        {
            Seek();
            Walk();
        }

        public override void _Input(InputEvent @event)
        {
            if (_head == null) return;
            if (_view == null) return;

            // Camera rotation
            if (@event is InputEventMouseMotion motion)
            {
                _head.RotateY(Mathf.DegToRad(-motion.Relative.X * _mouseSensitivity));
                float change = -motion.Relative.Y * _mouseSensitivity;

                if ((change + _cameraAngle) < 90f && (change + _cameraAngle) > -90f)
                {
                    _view.RotateX(Mathf.DegToRad(change));
                    _cameraAngle += change;
                }
            }

            // Interact on left click
            if (@event is InputEventMouseButton mouseButtonEvent)
            {
                if (mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed)
                {
                    _currentTarget?.HandleInteraction(this);
                }
            }
        }

        
        private void Seek()
        {
            _currentTarget = null; // reset each frame
            if (_rayCast == null) return;

            if (_rayCast.IsColliding())
            {
                var collider = _rayCast.GetCollider();
                if (collider is Node node)
                {
                    if (TryGetParentNode(node, n => n is IInteractable<Player>, out Node _interactable))
                    {
                        _currentTarget = (IInteractable<Player>)_interactable;
                    }
                }
            }
        }

        private void Walk()
        {
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
    }
}