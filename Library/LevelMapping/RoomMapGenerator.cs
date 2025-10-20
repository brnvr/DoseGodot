using System;
using Dose.Library.Math;
using Dose.Library.Randomness;
using Godot;
using static Dose.Library.GeoPositioning;

namespace Dose.Library.LevelMapping
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
            var direction = CardinalDirection.East;
            var startingPoint = new Vector2I();

            GenerateWall(map, startingPoint, direction);

            return map;
        }

        public void GenerateWall(Map map, Vector2I point, CardinalDirection direction)
        {
            var wallDirection = NextDirection(direction, GeoPositioning.ClockDirection.Counterclockwise);
            var shouldBuildDoor = new RandomDecider(ProbabilityToBuildDoor).Roll();
            var cell = new MapCell().WithWall(wallDirection, new MapCellWall(null, shouldBuildDoor));

            map.Set(point, cell);

            var nextPoint = MoveTo(point, direction);
            var isNextPointInBounds = map.TryGet(nextPoint, out MapCell nextCell);

            if (nextCell != null)
            {
                return;
            }

            var nextDirection = isNextPointInBounds ? direction : NextDirection(direction, GeoPositioning.ClockDirection.Clockwise);

            GenerateWall(map, nextPoint, nextDirection);
        }
    }
}