using Dose.Library.DataStructures;

namespace Dose.Library.LevelMapping
{
    public class Map(int horizontalCells, int verticalCells) : Grid<MapCell>(horizontalCells, verticalCells);
}