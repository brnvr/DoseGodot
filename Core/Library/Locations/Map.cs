using Dose.Core.Library.DataStructures;

namespace Dose.Core.Library.Locations
{
    public class Map(int horizontalCells, int verticalCells) : Grid<MapCell>(horizontalCells, verticalCells);
}