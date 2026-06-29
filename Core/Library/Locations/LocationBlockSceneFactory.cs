using Godot;

namespace Dose.Core.Library.Locations
{
    public abstract class LocationBlockSceneFactory : ILocationSceneFactory
    {
        public void Build(IBuildableLocation location, PackedScene scene, Vector2I position)
        {
            if (location.Map == null)
            {
                GD.PushWarning($"Location {location} has null map. Skipping block building...");
                return;
            }

            if (!Predicate(location.Map.Get(position)))
            {
                return;
            }

            var node = (Node3D)scene.Instantiate();

            InitializeNode(node);
            location.OnBuild(node);
            
            node.Position = GetPosition(location, position);
            node.RotateY(GetRotation(location, position));
        }

        protected abstract bool Predicate(MapCell cell);

        protected abstract void InitializeNode(Node node);

        protected abstract Vector3 GetPosition(IBuildableLocation location, Vector2I point);

        protected abstract float GetRotation(IBuildableLocation location, Vector2I point);
    }
}