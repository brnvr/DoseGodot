using System;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Randomness;
using Godot;
using static Dose.Core.Library.GeoPositioning;

namespace Dose.Core.Library.Locations
{
    public class RoomMapGenerator(int minSize, int maxSize, Probability probabilityToBuildDoor) : IMapGenerator
    {
        public int MinSize { get; } = minSize;
        public int MaxSize { get; } = maxSize;
        public Probability ProbabilityToBuildDoor { get; } = probabilityToBuildDoor;

        public Map GenerateMap()
        {
            var size = new Random().Next(MinSize, MaxSize);
            var map = new Map(size, size);
            var startingPoint = new Vector2I();

            GenerateWall(map, startingPoint, CardinalDirection.East);

            return map;
        }

        public void GenerateWall(Map map, Vector2I point, CardinalDirection direction)
        {
            var wallDirection = NextDirection(direction, GeoPositioning.ClockDirection.Counterclockwise);

            map.TryGet(point, out MapCell? existingCell);

            if (existingCell != null && existingCell.GetWall(wallDirection) != null)
                return;

            var shouldBuildDoor = new RandomDecider(ProbabilityToBuildDoor).Roll();
            var cell = (existingCell ?? new MapCell()).WithWall(wallDirection, new MapCellWall(null, shouldBuildDoor));

            map.Set(point, cell);

            var nextPoint = MoveTo(point, direction);
            var isNextPointInBounds = map.TryGet(nextPoint, out MapCell? nextCell);

            if (isNextPointInBounds)
            {
                GenerateWall(map, nextPoint, direction);

                return;
            }

            var nextDirection = NextDirection(direction, GeoPositioning.ClockDirection.Clockwise);

            GenerateWall(map, point, nextDirection);
        }
    }
}