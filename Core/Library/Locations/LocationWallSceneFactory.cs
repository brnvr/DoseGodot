using System.Linq;
using Godot;
using static Dose.Core.Library.GeoPositioning;

namespace Dose.Core.Library.Locations
{
    public abstract class LocationWallSceneFactory : ILocationSceneFactory
    {
        public void Build(IBuildableLocation location, PackedScene scene, Vector2I position)
        {
            if (location.Map == null)
            {
                GD.PushWarning($"Location {location} has null map. Skipping block building...");
                return;
            }

            var cell = location.Map.Get(position);
    
            foreach (var direction in AllDirections().Where(direction =>
            {
                var wall = cell.GetWall(direction);

                if (wall == null)
                {
                    return false;
                }
                
                return WallPredicate(wall);
            }))
            {
                OnBuildWall(location, scene, position, direction);
            }
        }

        protected virtual Node3D? OnBuildWall(IBuildableLocation location, PackedScene scene, Vector2I point, CardinalDirection direction)
        {
            var node = (Node3D)scene.Instantiate();

            location.OnBuild(node);

            if (node is IPortal portal)
            {
                portal.MapPosition = point;
                portal.MapDirection = direction;
            }

            node.Position = GetPosition(location, point, direction);
            node.RotateY(GetRotation(location, point, direction));

            InitializeNode(node);

            return node;
        }

        protected abstract bool WallPredicate(MapCellWall wall);

        protected abstract void InitializeNode(Node node);

        protected abstract Vector3 GetPosition(IBuildableLocation location, Vector2I point, CardinalDirection direction);

        protected abstract float GetRotation(IBuildableLocation location, Vector2I point, CardinalDirection direction);
    }
}
