using Godot;

namespace Dose.Core.Library.Locations
{
    public interface ILocationSceneFactory
    {
        void Build(IBuildableLocation location, PackedScene scene, Vector2I position);
    }
}