using Godot;

namespace Dose.Core.Library.Locations
{
    public record MapCellWall
    {
        public enum MapCellType
        {
            Solid,
            Doorway,
            Upstairs,
            Downstairs
        }

        public MapCellType Type { get; set; }

        public MapCellWall( MapCellType type)
        {
            Type = type;
        }
    }
}