using Godot;

namespace Dose.Library.Locations
{
    public interface IMapCellProcessor
    {
        void Process(Map map, Vector2I position);
    }
}