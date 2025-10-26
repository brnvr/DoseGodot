using Dose.Library.DataStructures;

namespace Dose.Library.Locations
{
    public class Map(int horizontalCells, int verticalCells) : Grid<MapCell>(horizontalCells, verticalCells);
}