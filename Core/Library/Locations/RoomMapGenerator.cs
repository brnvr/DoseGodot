using System;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Randomness;
using Godot;
using static Dose.Core.Library.GeoPositioning;
using static Dose.Core.Library.Locations.MapCellWall;

namespace Dose.Core.Library.Locations
{
    public class RoomMapGenerator(int minSize, int maxSize, Probability probabilityToBuildDoor) : IMapGenerator
    {
        public int MinSize { get; } = minSize;
        public int MaxSize { get; } = maxSize;
        public Probability ProbabilityToBuildDoor { get; } = probabilityToBuildDoor;

        public Map GenerateMap()
        {
            var width = new Random().Next(MinSize, MaxSize);
            var height = new Random().Next(MinSize, MaxSize);
            var map = new Map(width, height);
            var startingPoint = new Vector2I();

            GenerateWall(map, startingPoint, CardinalDirection.East);

            new MapEnsurer(map, MapCellType.Doorway).EnsureAtLeastOne();

            FillEmptyCells(map, width, height);

            return map;
        }

        public void GenerateWall(Map map, Vector2I point, CardinalDirection direction)
        {
            var wallDirection = NextDirection(direction, ClockDirection.Counterclockwise);

            map.TryGet(point, out MapCell? existingCell);

            if (existingCell != null && existingCell.GetWall(wallDirection) != null)
                return;

            var shouldBuildDoor = new RandomDecider(ProbabilityToBuildDoor).Roll();
            var type = shouldBuildDoor ? MapCellType.Doorway : MapCellType.Solid;
            var cell = (existingCell ?? new MapCell()).WithWall(wallDirection, new MapCellWall(null, type));

            map.Set(point, cell);

            var nextPoint = MoveTo(point, direction);
            var isNextPointInBounds = map.TryGet(nextPoint, out MapCell? nextCell);

            if (isNextPointInBounds)
            {
                GenerateWall(map, nextPoint, direction);

                return;
            }

            var nextDirection = NextDirection(direction, ClockDirection.Clockwise);

            GenerateWall(map, point, nextDirection);
        }

        private static void FillEmptyCells(Map map, int width, int height)
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var point = new Vector2I(x, y);

                    if (!map.TryGet(point, out MapCell? cell) || cell == null)
                    {
                        map.Set(point, new MapCell());
                    }
                }
            }
        }
    }
}