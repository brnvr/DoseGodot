using Godot;

public partial class Raycaster : Node3D
{
    private Camera3D _camera;

    public override void _Ready()
    {
        _camera = GetViewport().GetCamera3D();
    }

    public override void _Process(double delta)
    {
        if (_camera == null) return;

        var mousePos = GetViewport().GetMousePosition();
        var from = _camera.ProjectRayOrigin(mousePos);
        var to = from + _camera.ProjectRayNormal(mousePos) * 1000f;

        var spaceState = GetWorld3D().DirectSpaceState;

        var query = PhysicsRayQueryParameters3D.Create(from, to);
        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            var collider = result["collider"];
            GD.Print($"Hovering over: {collider}");
        }
    }
}