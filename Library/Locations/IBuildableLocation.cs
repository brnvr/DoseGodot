using Godot;

namespace Dose.Library.Locations
{
    public interface IBuildableLocation : ILocation
    {
        public void HandleBuild(Node node);
    }
}