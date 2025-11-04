using Godot;

namespace Dose.Core.Library.Locations
{
    public interface IBuildableLocation : ILocation
    {
        public void HandleBuild(Node node);
    }
}