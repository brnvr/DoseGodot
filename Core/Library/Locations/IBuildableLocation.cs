using Godot;

namespace Dose.Core.Library.Locations
{
    public interface IBuildableLocation : ILocation
    {
        public void OnBuild(Node node);
    }
}