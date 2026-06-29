using Dose.Core.Library.DataStructures;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class Map(int horizontalCells, int verticalCells) : Grid<MapCell>(horizontalCells, verticalCells)
    {
        public Vector2I Center => new((int)CenterX, (int)CenterY);
        public float CenterX => HorizontalCells / 2;
        public float CenterY => VerticalCells / 2;

        public MapCell CenterCell => Get(new Vector2I((int)CenterX, (int)CenterY));
    }
}