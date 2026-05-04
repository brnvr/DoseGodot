using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Randomness;
using Godot;
using static Dose.Core.Library.GeoPositioning;
using static Dose.Core.Library.Locations.MapCellWall;

namespace Dose.Core.Library.Locations
{
    public class MapEnsurer(Map map, params MapCellType[] types)
    {
        public Map Map { get; } = map;
        public List<MapCellType> Types { get; } = [.. types];

        public void EnsureAtLeastOne()
        {
            var allCells = Map.GetDefinedValues();

            var wallOfType = AllDirections()
                .SelectMany(dir => allCells
                    .Select(cell => cell.GetWall(dir))
                    .Where(w => w is not null && Types.Contains(w.Type)))
                .FirstOrDefault();

            if (wallOfType == null)
            {
                var wall = AllDirections()
                    .Where(dir => allCells.First().GetWall(dir)?.Type == MapCellType.Solid)
                    .Select(dir => allCells.First().GetWall(dir))
                    .FirstOrDefault();
                    
                if (wall == null)
                {
                    GD.PushWarning("No walls found. Can't ensure a wall type.");
                    return;
                }
                
                wall.Type = new RandomSelector<MapCellType>(Types).Pick();
            }
            else
            {
                // TODO: Update ensurer logic
                // if (!allCells.Any(cell => AllDirections().Any(dir => cell.GetWall(dir)?.DoorType != MapCellDoorType.Locked)))
                // {
                //     wallOfType.DoorType = MapCellDoorType.Open;
                // }
            }
        }
    }
}