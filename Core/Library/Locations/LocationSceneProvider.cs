using System;
using System.Linq;
using Dose.Core.Library.Actors;
using Godot;
using static Dose.Core.Library.GeoPositioning;

namespace Dose.Core.Library.Locations
{
    public abstract class LocationSceneProvider<TInitializable, TContext, TArgs>(
        IBuildableLocation location,
        PackedScene scene,
        TArgs initializationArgs
    ) : LocationBuilder(location) where TInitializable : IInitializableActor<TContext, TArgs>
    {
        public new IBuildableLocation Location { get; } = location;
        public PackedScene Scene { get; } = scene;
        public TArgs InitializationArgs { get; } = initializationArgs;
        
        protected override void OnBuildCell(Vector2I position)
        {
            if (Location.Map == null)
            {
                throw new Exception("Location doesn't have a defined Map.");
            }

            var cell = Location.Map.Get(position);
    
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
                OnBuildWall(position, direction);
            }
        }

        protected abstract bool WallPredicate(MapCellWall wall);

        protected abstract TContext BuildContext();

        protected abstract Vector3 GetPosition(Vector2I point, CardinalDirection direction);

        protected abstract float GetRotation(Vector2I point, CardinalDirection direction);

        protected virtual Node3D? OnBuildWall(Vector2I point, CardinalDirection direction)
        {
            if (Location.Map == null)
            {
                GD.PushError($"{nameof(Location)} doesn't have a defined ${nameof(Location.Map)}.");
                return default;
            }

            var node = (Node3D)Scene.Instantiate();

            Location.OnBuild(node);

            node.Position = GetPosition(point, direction);
            node.RotateY(GetRotation(point, direction));

            if (node is not TInitializable initializable)
            {
                GD.PushWarning($"Instance of {Scene.ResourcePath} is not of type {nameof(TInitializable)}. The node will not be initialized.");
                return default;
            }

            initializable.Initialize(BuildContext(), InitializationArgs);

            return node;
        }
    }
}