using Godot;

namespace Dose.Library.Locations
{
    public abstract class LocationMapProcessor(Location location)
    {
        public Location Location { get; } = location;

        public void Process()
        {
            foreach (var position in Location.Map.GetFilledPositions())
            {
                ProcessCell(position);
            }
        }

        protected abstract void ProcessCell(Vector2I position);
    }
}