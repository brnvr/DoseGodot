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

            AddCentralLamp(map, width, height);
    
            return map;
        }

        static void AddCentralLamp(Map map, int width, int height)
        {
            var center = new Vector2((width - 1) / 2f, (height - 1) / 2f);
            var position = new Vector2I(
                Mathf.FloorToInt(center.X),
                Mathf.FloorToInt(center.Y)
            );
            var offset = center - new Vector2(position.X, position.Y);

            map.Set(position, map.Get(position).WithLamp(offset));
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
