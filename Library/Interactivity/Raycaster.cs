using Godot;

namespace Dose.Library.Interactivity
{
    public class Raycaster(Camera3D camera)
    {
        public Camera3D Camera { get; } = camera;

        public void Update()
        {
            if (Camera == null)
            {
                return;
            }

            var mousePos = Camera.GetViewport().GetMousePosition();
            var from = Camera.ProjectRayOrigin(mousePos);
            var to = from + Camera.ProjectRayNormal(mousePos) * 1000f;

            var spaceState = Camera.GetWorld3D().DirectSpaceState;

            var query = PhysicsRayQueryParameters3D.Create(from, to);
            var result = spaceState.IntersectRay(query);

            if (result.Count > 0)
            {
                var collider = result["collider"];
                GD.Print($"Hovering over: {collider}");
            }
        }
    }
}