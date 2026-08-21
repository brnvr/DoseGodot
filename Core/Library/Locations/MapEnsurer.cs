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

        public void EnsureAtLeastOne(
            Func<Vector2I, CardinalDirection, MapCellWall, bool> candidatePredicate
        )
        {
            EnsureAtLeast(1, candidatePredicate);
        }

        public void EnsureAtLeast(
            int count,
            Func<Vector2I, CardinalDirection, MapCellWall, bool>? candidatePredicate = null
        )
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be at least one.");
            }

            var walls = Map.GetFilledPositions()
                .SelectMany(position => AllDirections()
                    .Select(direction => (
                        Position: position,
                        Direction: direction,
                        Wall: Map.Get(position).GetWall(direction)
                    )))
                .Where(candidate => candidate.Wall != null)
                .Select(candidate => (
                    candidate.Position,
                    candidate.Direction,
                    Wall: candidate.Wall!
                ))
                .ToList();

            while (walls.Count(candidate => Types.Contains(candidate.Wall.Type)) < count)
            {
                var solidWalls = walls
                    .Where(candidate =>
                        candidate.Wall.Type == MapCellType.Solid &&
                        (candidatePredicate == null || candidatePredicate(
                            candidate.Position,
                            candidate.Direction,
                            candidate.Wall
                        )))
                    .ToList();

                if (solidWalls.Count == 0)
                {
                    GD.PushWarning($"Not enough solid walls to ensure {count} portal walls.");
                    return;
                }

                var wall = new RandomSelector<(Vector2I Position, CardinalDirection Direction, MapCellWall Wall)>(solidWalls).Pick().Wall;
                wall.Type = new RandomSelector<MapCellType>(Types).Pick();
                wall.OverlayTexture = null;
                wall.OverlayTextureScale = 1f;
                wall.OverlayTextureRepeat = false;
            }
        }
    }
}
