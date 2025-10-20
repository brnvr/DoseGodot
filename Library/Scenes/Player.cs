using Godot;

public partial class Player : CharacterBody3D
{
	private Node3D _head;
	private Camera3D _view;

	private float _cameraAngle = 0F;
	private float _mouseSensitivity = 0.1F;
	private float _moveSpeed = 20F;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_head = GetNode<Node3D>("Head");
		_view = GetNode<Camera3D>("Head/View");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("ui_cancel"))
			Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	public override void _PhysicsProcess(double delta)
	{
		Walk();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventMouseMotion motion) return;

		_head.RotateY(Mathf.DegToRad(-motion.Relative.X * _mouseSensitivity));
		float change = -motion.Relative.Y * _mouseSensitivity;

		if (!((change + _cameraAngle) < 90F) || !((change + _cameraAngle) > -90F)) return;
		
		_view.RotateX(Mathf.DegToRad(change));
		_cameraAngle += change;
	}

	private void Walk()
{
	Vector3 direction = Vector3.Zero;

	// Get the camera's forward and right vectors
	Vector3 forward = -_view.GlobalTransform.Basis.Z;
	Vector3 right = _view.GlobalTransform.Basis.X;

	// Flatten vectors to ignore vertical tilt
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
