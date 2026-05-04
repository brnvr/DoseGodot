using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Randomness;
using Godot;
using static Dose.Core.Library.GeoPositioning;
using static Dose.Core.Library.Locations.MapCellWall;

namespace Dose.Core.Library.Locations
{
    public class CorridorMapGenerator
    (
        int horizontalCells,
        int verticalCells,
        Probability probabilityToExtendCorridor,
        Probability probabilityToBifurcate,
        Probability probabilityToBuildDoors,
        Probability probabilityToBuildStairs
    ) : IMapGenerator
    {
        const int CORRIDOR_CHUNK_LENGTH_CELLS = 5;

        readonly FrozenSet<CardinalDirection> _directions0 = new[] { CardinalDirection.North, CardinalDirection.West }.ToFrozenSet();
        readonly FrozenSet<CardinalDirection> _directions1 = new[] { CardinalDirection.South, CardinalDirection.East }.ToFrozenSet();

        public int HorizontalCells { get; } = horizontalCells;
        public int VerticalCells { get; } = verticalCells;
        public bool ShouldEnsureStairsUp { get; init; } = false;
        public bool ShouldEnsureStairsDown { get; init; } = false;
        public Probability ProbabilityToExtendCorridor { get; } = probabilityToExtendCorridor;
        public Probability ProbabilityToBifurcate { get; } = probabilityToBifurcate;
        public Probability ProbabilityToBuildDoors { get; } = probabilityToBuildDoors;
        public Probability ProbabilityToBuildStairs { get; } = probabilityToBuildStairs;

        public Map GenerateMap()
        {
            var map = new Map(HorizontalCells, VerticalCells);
            var startingPoint = GetCenter();
            var selectedDirection = new RandomSelector<CardinalDirection>(AllDirections()).Pick();
            GenerateCorridor(map, startingPoint, selectedDirection);

            new MapEnsurer(map, MapCellType.Doorway).EnsureAtLeastOne();

            if (ShouldEnsureStairsUp)
            {
                new MapEnsurer(map, MapCellType.Upstairs).EnsureAtLeastOne();
            }

            if (ShouldEnsureStairsDown)
            {
                new MapEnsurer(map, MapCellType.Downstairs).EnsureAtLeastOne();
            }

            return map;
        }

        static void GenerateIntersection(Map map, Vector2I point)
        {
            var baseCell = BuildCellWalls(new MapCell(), AllDirections());

            map.Set(point, baseCell);
            RemoveIntersectingWalls(map, point, AllDirections());
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
            var baseCell = BuildCellWalls(new MapCell(), direction, hasDoors0, hasDoors1, iteration, AllDirections());

            map.Set(point, baseCell);
            RemoveIntersectingWalls(map, point, AllDirections());
            
            var nextPoint = MoveTo(point, direction);
            var isCellInBounds = map.InBounds(nextPoint);

            if (!isCellInBounds)
            {
                return;
            }

            var isEndOfChunk = iteration == CORRIDOR_CHUNK_LENGTH_CELLS-1;

            if (isEndOfChunk)
            {
                var shouldExtendCorridor = new RandomDecider(ProbabilityToExtendCorridor).Roll();

                if (shouldExtendCorridor)
                {

                    if (Bifurcate(map, nextPoint, [direction]))
                    {
                        GenerateIntersection(map, nextPoint);
                    }
                }

                var shouldBifurcate = new RandomDecider(ProbabilityToBifurcate).Roll();

                if (shouldBifurcate)
                {
                    if (Bifurcate(map, nextPoint, AllDirections().Except([direction])))
                    {
                        GenerateIntersection(map, nextPoint);
                    }
                }

                return;
            }

            GenerateCorridor(map, nextPoint, direction, hasDoors0, hasDoors1, iteration + 1);
        }


        bool Bifurcate(Map map, Vector2I point, IEnumerable<CardinalDirection> availableDirections)
        {
            var nextDirection = ChooseNextDirection(map, point, availableDirections);

            if (nextDirection == null)
            {
                return false;
            }

            var nextPoint = MoveTo(point, (CardinalDirection)nextDirection);
            var isCellInBounds = map.TryGet(nextPoint, out MapCell? nextCell);

            var canBifurcate = isCellInBounds && nextCell == null;

            if (canBifurcate)
            {
                GenerateCorridor(map, nextPoint, (CardinalDirection)nextDirection);
            }

            var shouldBifurcate = new RandomDecider(ProbabilityToBifurcate).Roll();

            if (shouldBifurcate)
            {
                var successful = Bifurcate(map, point, availableDirections.Except([(CardinalDirection)nextDirection]));

                return canBifurcate || successful;
            }

            return canBifurcate;
        }

        static CardinalDirection? ChooseNextDirection(Map map, Vector2I point, IEnumerable<CardinalDirection> availableDirections)
        {
            if (!availableDirections.Any())
            {
                return null;
            }

            var selectedDirection = new RandomSelector<CardinalDirection>(availableDirections).Pick();

            var nextPoint = MoveTo(point, selectedDirection);

            var isCellInBounds = map.TryGet(nextPoint, out MapCell? nextCell);

            if (isCellInBounds && nextCell == null)
            {
                return selectedDirection;
            }

            return ChooseNextDirection(map, point, availableDirections.Except([selectedDirection]));
        }

        static MapCell BuildCellWalls(MapCell cell, IEnumerable<CardinalDirection> wallDirections)
        {
            var wallDirection = wallDirections.First();
            var wall = new MapCellWall(null, MapCellType.Solid);
            var newCell = cell.WithWall(wallDirection, wall);

            var remainingWallDirections = wallDirections.Except([wallDirection]);

            if (!remainingWallDirections.Any())
            {
                return newCell;
            }

            return BuildCellWalls(newCell, remainingWallDirections);
        }

        MapCellType DecideMapCellType(bool hasDoor)
        {
            if (hasDoor)
            {
                return MapCellType.Doorway;
            }

            if (new RandomDecider(ProbabilityToBuildStairs).Roll())
            {
                return new RandomSelector<MapCellType>([MapCellType.Upstairs, MapCellType.Downstairs]).Pick();
            }

            return MapCellType.Solid;
        }

        MapCell BuildCellWalls(MapCell cell, CardinalDirection direction, bool hasDoors0, bool hasDoors1, int iteration, IEnumerable<CardinalDirection> wallDirections)
        {
            var wallDirection = wallDirections.First();
            var isBuildDoorsIteration = iteration == CORRIDOR_CHUNK_LENGTH_CELLS / 2;
            var shouldBuildDoors0 = hasDoors0 && isBuildDoorsIteration;
            var shouldBuildDoors1 = hasDoors1 && isBuildDoorsIteration;
            var buildOrientation = OrientationFromDirection(direction);
            var wallOrientation = OrientationFromDirection(wallDirection);
            var hasDoor = (buildOrientation != wallOrientation) && ((shouldBuildDoors0 && _directions0.Contains(wallDirection)) || (shouldBuildDoors1 && _directions1.Contains(wallDirection)));
            var wall = new MapCellWall(null, DecideMapCellType(hasDoor));
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
            var neighborPoint = MoveTo(point, direction);
            var isNeighborCellInBounds = map.TryGet(neighborPoint, out MapCell? neighborCell);
            
            if (isNeighborCellInBounds && neighborCell != null)
            {
                map.Set(point, cell.WithWall(direction, null));
                map.Set(neighborPoint, neighborCell.WithWall(oppositeDirection, null));
            }
        }
    }
}