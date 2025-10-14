using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Dose.Library.Math;
using Dose.Library.Randomness;
using Godot;
using static Dose.Library.GeoPositioning;

namespace Dose.Library.LevelMapping
{
    public class CorridorMapGenerator
    (
        int horizontalCells,
        int verticalCells,
        Probability probabilityToContinueGenerating,
        Probability probabilityToBifurcate,
        Probability probabilityToBuildDoors
    ) : IMapGenerator
    {
        const int CORRIDOR_CHUNK_LENGTH_CELLS = 5;

        readonly FrozenSet<CardinalDirection> _directions0 = new[] { CardinalDirection.North, CardinalDirection.West }.ToFrozenSet();
        readonly FrozenSet<CardinalDirection> _directions1 = new[] { CardinalDirection.South, CardinalDirection.East }.ToFrozenSet();

        public int HorizontalCells { get; } = horizontalCells;
        public int VerticalCells { get; } = verticalCells;
        public Probability ProbabilityToContinueGenerating { get; } = probabilityToContinueGenerating;
        public Probability ProbabilityToBifurcate { get; } = probabilityToBifurcate;
        public Probability ProbabilityToBuildDoors { get; } = probabilityToBuildDoors;

        public Map GenerateMap()
        {
            var map = new Map(HorizontalCells, VerticalCells);
            var startingPoint = GetCenter();
            var selectedDirection = new RandomSelector<CardinalDirection>(GetAllDirections()).Pick();
            GenerateCorridor(map, startingPoint, selectedDirection);

            return map;
        }

        void GenerateCorridor(Map map, Vector2I point, CardinalDirection direction)
        {
            var buildDoorsDecider = new RandomDecider(ProbabilityToBuildDoors);
            var hasDoors0 = buildDoorsDecider.Roll();
            var hasDoors1 = buildDoorsDecider.Roll();

            GenerateCorridor(map, point, direction, hasDoors0, hasDoors1);
        }

        void GenerateCorridor(Map map, Vector2I point, CardinalDirection direction, bool hasDoors0, bool hasDoors1, int iteration = 0)
        {
            var baseCell = BuildCellWalls(new MapCell(), direction, hasDoors0, hasDoors1, iteration, GetAllDirections());

            map.Set(point, baseCell);
            RemoveIntersectingWalls(map, point, GetAllDirections());
            
            var nextPoint = MoveToDirection(point, direction);
            var isCellInBounds = map.InBounds(nextPoint);

            if (!isCellInBounds)
            {
                return;
            }

            var shouldContinueGenerating = (iteration % CORRIDOR_CHUNK_LENGTH_CELLS != 0) || new RandomDecider(ProbabilityToContinueGenerating).Roll();

            if (!shouldContinueGenerating)
            {
                Bifurcate(map, nextPoint, GetAllDirections().Except([direction]));
                return;
            }

            GenerateCorridor(map, nextPoint, direction, hasDoors0, hasDoors1, iteration + 1);
        }


        void Bifurcate(Map map, Vector2I point, IEnumerable<CardinalDirection> avaliableDirections)
        {
            var shouldBifurcate = new RandomDecider(ProbabilityToBifurcate).Roll();

            if (!shouldBifurcate)
            {
                return;
            }

            var nextDirection = ChooseNextDirection(map, point, avaliableDirections);

            if (nextDirection == null)
            {
                return;
            }

            var nextPoint = MoveToDirection(point, (CardinalDirection)nextDirection);
            var isCellInBounds = map.TryGet(nextPoint, out MapCell nextCell);

            if (isCellInBounds && nextCell == null)
            {
                GenerateCorridor(map, point, (CardinalDirection)nextDirection);
            }

            Bifurcate(map, point, avaliableDirections.Except([(CardinalDirection)nextDirection]));
        }

        static CardinalDirection? ChooseNextDirection(Map map, Vector2I point, IEnumerable<CardinalDirection> avaliableDirections)
        {
            if (!avaliableDirections.Any())
            {
                return null;
            }

            var selectedDirection = new RandomSelector<CardinalDirection>(avaliableDirections).Pick();

            var nextPoint = MoveToDirection(point, selectedDirection);

            var isCellInBounds = map.TryGet(nextPoint, out MapCell nextCell);

            if (isCellInBounds && nextCell == null)
            {
                return selectedDirection;
            }

            return ChooseNextDirection(map, point, avaliableDirections.Except([selectedDirection]));
        }

        MapCell BuildCellWalls(MapCell cell, CardinalDirection direction, bool hasDoors0, bool hasDoors1, int iteration, IEnumerable<CardinalDirection> wallDirections)
        {
            var wallDirection = wallDirections.First();
            var isBuildDoorsIteration = iteration == CORRIDOR_CHUNK_LENGTH_CELLS / 2;
            var shouldBuildDoors0 = hasDoors0 && isBuildDoorsIteration;
            var shouldBuildDoors1 = hasDoors1 && isBuildDoorsIteration;
            var buildOrientation = GetOrientationFromDirection(direction);
            var wallOrientation = GetOrientationFromDirection(wallDirection);
            var hasDoor = (buildOrientation != wallOrientation) && ((shouldBuildDoors0 && _directions0.Contains(wallDirection)) || (shouldBuildDoors1 && _directions1.Contains(wallDirection)));
            var wall = new MapCellWall(null, hasDoor);
            var newCell = cell.WithWall(wallDirection, wall);

            var remainingWallDirections = wallDirections.Except([wallDirection]);

            if (!remainingWallDirections.Any())
            {
                return newCell;
            }

            return BuildCellWalls(newCell, direction, hasDoors0, hasDoors1, iteration, remainingWallDirections);
        } 
  
        Vector2I GetCenter()
        {
            return new Vector2I(HorizontalCells / 2, VerticalCells / 2);
        }

        static void RemoveIntersectingWalls(Map map, Vector2I point, IEnumerable<CardinalDirection> directions)
        {
            var direction = directions.First();

            RemoveIntersectingWalls(map, point, direction);

            var remainingDirections = directions.Except([direction]);

            if (!remainingDirections.Any())
            {
                return;
            }

            RemoveIntersectingWalls(map, point, remainingDirections);
        }
        
        static void RemoveIntersectingWalls(Map map, Vector2I point, CardinalDirection direction)
        {
            var cell = map.Get(point) ?? throw new Exception($"Cell not found at position: ({point.X}, {point.Y})");
            var oppositeDirection = CardinalOpposite(direction);
            var neighborPoint = MoveToDirection(point, direction);
            var isNeighborCellInBounds = map.TryGet(neighborPoint, out MapCell neighborCell);
            
            if (isNeighborCellInBounds && neighborCell != null)
            {
                map.Set(point, cell.WithWall(direction, null));
                map.Set(neighborPoint, neighborCell.WithWall(oppositeDirection, null));
            }
        }
    }
}