using System;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class LocationSceneProvider(
        IBuildableLocation location,
        PackedScene scene,
        ILocationSceneFactory factory
    ) : LocationBuilder(location)
    {
        public new IBuildableLocation Location { get; } = location;
        public PackedScene Scene { get; } = scene;
        public ILocationSceneFactory Factory { get; } = factory;
        
        protected override void OnBuildCell(Vector2I position)
        {
            if (Location.Map == null)
            {
                throw new Exception("Location doesn't have a defined Map.");
            }

            Factory.Build(Location, Scene, position);  
        }

        // protected abstract bool WallPredicate(MapCellWall wall);

        // protected abstract Vector3 GetPosition(Vector2I point, CardinalDirection direction);

        // protected abstract float GetRotation(Vector2I point, CardinalDirection direction);

        // protected abstract void InitializeNode(Node node);

        // protected virtual Node3D? OnBuildWall(Vector2I point, CardinalDirection direction)
        // {
        //     if (Location.Map == null)
        //     {
        //         GD.PushError($"{nameof(Location)} doesn't have a defined ${nameof(Location.Map)}.");
        //         return default;
        //     }

        //     var node = (Node3D)Scene.Instantiate();

        //     Location.OnBuild(node);

        //     node.Position = GetPosition(point, direction);
        //     node.RotateY(GetRotation(point, direction));

        //     InitializeNode(node);

        //     return node;
        // }
    }
}