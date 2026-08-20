using System;
using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Randomness;
using Godot;
using static Dose.Core.Library.GeoPositioning;
using static Dose.Core.Library.Locations.MapCellWall;

namespace Dose.Core.Library.Locations
{
    public class RoomMapGenerator(
        int minSize,
        int maxSize,
        Probability probabilityToBuildDoor,
        Probability probabilityToSetWallOverlay,
        IEnumerable<WallOverlayTextureDefinition> wallOverlayTextures,
        Probability probabilityToSetFloorOverlay,
        IEnumerable<FloorOverlayTextureDefinition> floorOverlayTextures
    ) : IMapGenerator
    {
        const int MAX_CELLS_PER_LAMP_AXIS = 5;

        public int MinSize { get; } = minSize;
        public int MaxSize { get; } = maxSize;
        public Probability ProbabilityToBuildDoor { get; } = probabilityToBuildDoor;
        public Probability ProbabilityToSetWallOverlay { get; } = probabilityToSetWallOverlay;
        public IEnumerable<WallOverlayTextureDefinition> WallOverlayTextures { get; } = wallOverlayTextures;
        public Probability ProbabilityToSetFloorOverlay { get; } = probabilityToSetFloorOverlay;
        public IEnumerable<FloorOverlayTextureDefinition> FloorOverlayTextures { get; } = floorOverlayTextures;

        public Map GenerateMap()
        {
            var width = new Random().Next(MinSize, MaxSize);
            var height = new Random().Next(MinSize, MaxSize);
            var map = new Map(width, height);
            var startingPoint = new Vector2I();

            GenerateWall(map, startingPoint, CardinalDirection.East);

            new MapEnsurer(map, MapCellType.Doorway).EnsureAtLeastOne();

            FillEmptyCells(map, width, height);
            SetFloorOverlays(map);

            AddLamps(map, width, height);
    
            return map;
        }

        static void AddLamps(Map map, int width, int height)
        {
            var horizontalLampCount = Mathf.CeilToInt(width / (float)MAX_CELLS_PER_LAMP_AXIS);
            var verticalLampCount = Mathf.CeilToInt(height / (float)MAX_CELLS_PER_LAMP_AXIS);

            for (var xIndex = 0; xIndex < horizontalLampCount; xIndex++)
            {
                var x = GetLampCoordinate(width, horizontalLampCount, xIndex);

                for (var yIndex = 0; yIndex < verticalLampCount; yIndex++)
                {
                    var y = GetLampCoordinate(height, verticalLampCount, yIndex);
                    var position = new Vector2I(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
                    var offset = new Vector2(x - position.X, y - position.Y);

                    map.Set(position, map.Get(position).WithLamp(offset));
                }
            }
        }

        static float GetLampCoordinate(int cells, int lampCount, int lampIndex)
        {
            return (lampIndex + .5f) * cells / lampCount - .5f;
        }

        public void GenerateWall(Map map, Vector2I point, CardinalDirection direction)
        {
            var wallDirection = NextDirection(direction, ClockDirection.Counterclockwise);

            map.TryGet(point, out MapCell? existingCell);

            if (existingCell != null && existingCell.GetWall(wallDirection) != null)
                return;

            var shouldBuildDoor = new RandomDecider(ProbabilityToBuildDoor).Roll();
            var type = shouldBuildDoor ? MapCellType.Doorway : MapCellType.Solid;
            var overlay = PickWallOverlayTexture(type);
            var cell = (existingCell ?? new MapCell()).WithWall(
                wallDirection,
                new MapCellWall(
                    type,
                    overlay?.Texture,
                    overlay?.Scale ?? 1f,
                    overlay?.TextureRepeat ?? false
                )
            );

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

        WallOverlayTextureDefinition? PickWallOverlayTexture(MapCellType type)
        {
            if (type != MapCellType.Solid || !WallOverlayTextures.Any())
            {
                return null;
            }

            return new RandomDecider(ProbabilityToSetWallOverlay).Roll()
                ? new RandomSelector<WallOverlayTextureDefinition>(WallOverlayTextures).Pick()
                : null;
        }

        void SetFloorOverlays(Map map)
        {
            if (!FloorOverlayTextures.Any())
            {
                return;
            }

            foreach (var position in map.GetFilledPositions())
            {
                if (!new RandomDecider(ProbabilityToSetFloorOverlay).Roll())
                {
                    continue;
                }

                var overlay = new RandomSelector<FloorOverlayTextureDefinition>(FloorOverlayTextures).Pick();
                map.Set(position, map.Get(position).WithFloorOverlay(
                    overlay.Texture,
                    overlay.Scale,
                    overlay.TextureRepeat
                ));
            }
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
