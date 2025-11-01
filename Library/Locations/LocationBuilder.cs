using Godot;

namespace Dose.Library.Locations
{
    public abstract class LocationBuilder(IBuildableLocation location)
    {
        public IBuildableLocation Location { get; } = location;

        public void Build()
        {
            foreach (var position in Location.Map.GetFilledPositions())
            {
                BuildCell(position);
            }
        }

        protected abstract void BuildCell(Vector2I position);
    }
}