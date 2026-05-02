using System.Linq;
using Godot;
using static Dose.Core.Library.GeoPositioning;
using static Dose.Core.Library.Locations.MapCellWall;

namespace Dose.Core.Library.Locations
{
    public class MapDoorEnsurer(Map map)
    {
        public Map Map { get; } = map;

        public void EnsureAtLeastOne()
        {
            var allCells = Map.GetDefinedValues();

            var wallWithDoor = AllDirections()
                .SelectMany(dir => allCells
                    .Select(cell => cell.GetWall(dir))
                    .Where(w => w?.Type == MapCellType.Doorway))
                .FirstOrDefault();

            if (wallWithDoor == null)
            {
                var wall = AllDirections()
                    .Where(dir => allCells.First().GetWall(dir) != null)
                    .Select(dir => allCells.First().GetWall(dir))
                    .FirstOrDefault();
                    
                if (wall == null)
                {
                    GD.PushWarning("No walls found. Can't ensure a door a minimum door number of 1.");
                    return;
                }
                
                wall.Type = MapCellType.Doorway;
            }
            else
            {
                // TODO: Update ensurer logic
                // if (!allCells.Any(cell => AllDirections().Any(dir => cell.GetWall(dir)?.DoorType != MapCellDoorType.Locked)))
                // {
                //     wallWithDoor.DoorType = MapCellDoorType.Open;
                // }
            }
        }
    }
}