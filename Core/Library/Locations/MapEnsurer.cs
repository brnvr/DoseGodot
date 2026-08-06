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
            EnsureAtLeast(1);
        }

        public void EnsureAtLeast(int count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be at least one.");
            }

            var walls = AllDirections()
                .SelectMany(direction => Map.GetDefinedValues()
                    .Select(cell => cell.GetWall(direction)))
                .Where(wall => wall != null)
                .Cast<MapCellWall>()
                .ToList();

            while (walls.Count(wall => Types.Contains(wall.Type)) < count)
            {
                var solidWalls = walls
                    .Where(wall => wall.Type == MapCellType.Solid)
                    .ToList();

                if (solidWalls.Count == 0)
                {
                    GD.PushWarning($"Not enough solid walls to ensure {count} portal walls.");
                    return;
                }

                var wall = new RandomSelector<MapCellWall>(solidWalls).Pick();
                wall.Type = new RandomSelector<MapCellType>(Types).Pick();
            }
        }
    }
}
