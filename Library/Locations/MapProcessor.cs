namespace Dose.Library.Locations
{
    public class MapProcessor(Map map, IMapCellProcessor cellProcessor)
    {
        Map Map { get; } = map;
        IMapCellProcessor CellProcessor { get; } = cellProcessor;

        public void Process()
        {
            foreach (var position in Map.GetFilledPositions())
            {
                CellProcessor.Process(Map, position);
            }
        }
    }
}