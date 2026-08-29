using Godot;
using static Dose.Core.Library.GeoPositioning;

namespace Dose.Core.Library.Locations
{
    public sealed class MapRenderer(int cellSize = 8)
    {
        static readonly Color FloorColor = new("34323d");
        static readonly Color WallColor = new("d8d2c4");
        static readonly Color DoorColor = new("a88652");
        static readonly Color StairsColor = new("7d91ad");

        public int CellSize { get; } = cellSize;

        public Image Render(Map map)
        {
            var image = Image.CreateEmpty(
                map.HorizontalCells * CellSize + 1,
                map.VerticalCells * CellSize + 1,
                false,
                Image.Format.Rgba8
            );
            image.Fill(Colors.Transparent);

            foreach (var position in map.GetFilledPositions())
            {
                var origin = position * CellSize;
                image.FillRect(new Rect2I(origin, Vector2I.One * CellSize), FloorColor);

                foreach (var direction in AllDirections())
                {
                    var wall = map.Get(position).GetWall(direction);
                    if (wall == null)
                    {
                        continue;
                    }

                    DrawWall(image, origin, direction, WallColor);

                    if (wall.Type != MapCellWall.MapCellType.Solid)
                    {
                        DrawOpening(image, origin, direction, wall.Type);
                    }
                }
            }

            return image;
        }

        void DrawWall(Image image, Vector2I origin, CardinalDirection direction, Color color)
        {
            var rect = direction switch
            {
                CardinalDirection.North => new Rect2I(origin, new Vector2I(CellSize + 1, 1)),
                CardinalDirection.South => new Rect2I(origin + new Vector2I(0, CellSize), new Vector2I(CellSize + 1, 1)),
                CardinalDirection.West => new Rect2I(origin, new Vector2I(1, CellSize + 1)),
                _ => new Rect2I(origin + new Vector2I(CellSize, 0), new Vector2I(1, CellSize + 1))
            };
            image.FillRect(rect, color);
        }

        void DrawOpening(Image image, Vector2I origin, CardinalDirection direction, MapCellWall.MapCellType type)
        {
            var color = type == MapCellWall.MapCellType.Doorway ? DoorColor : StairsColor;
            var inset = CellSize / 3;
            var length = CellSize - inset * 2 + 1;
            var rect = direction switch
            {
                CardinalDirection.North => new Rect2I(origin + new Vector2I(inset, 0), new Vector2I(length, 1)),
                CardinalDirection.South => new Rect2I(origin + new Vector2I(inset, CellSize), new Vector2I(length, 1)),
                CardinalDirection.West => new Rect2I(origin + new Vector2I(0, inset), new Vector2I(1, length)),
                _ => new Rect2I(origin + new Vector2I(CellSize, inset), new Vector2I(1, length))
            };
            image.FillRect(rect, color);
        }
    }
}
