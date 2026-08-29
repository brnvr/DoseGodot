using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dose.Core.Library.Actors;
using static Dose.Core.Library.GeoPositioning;

namespace Dose.Core.Library.Locations
{
    public sealed class WorldMap(MapRenderQueue renderQueue, int cellSize = 8)
    {
        sealed record Entry(Map Map, Task<Image> Image, Vector2 Position, int Floor, int QuarterTurns);
        readonly Dictionary<Guid, Entry> _entries = [];
        Guid? _currentLocationId;

        public void Visit(ILocation location, ILocation? previousLocation = null)
        {
            if (location is not IBuildableLocation { Map: not null } buildable) return;

            var position = Vector2.Zero;
            var floor = 0;
            var quarterTurns = 0;

            if (previousLocation != null && _entries.TryGetValue(previousLocation.Id, out var previous))
            {
                position = previous.Position;
                floor = previous.Floor;
                var previousPortal = GetPortals(previousLocation).FirstOrDefault(p => p.Linked?.Location == location);
                var currentPortal = previousPortal?.Linked;

                if (previousPortal != null && currentPortal != null)
                {
                    var sourceDirection = Rotate(previousPortal.MapDirection, previous.QuarterTurns);
                    quarterTurns = FindRotation(currentPortal.MapDirection, CardinalOpposite(sourceDirection));
                    var sourcePortalPosition = previous.Position + Rotate(GetLocalPortalPosition(previous.Map, previousPortal), previous.QuarterTurns);
                    position = sourcePortalPosition - Rotate(GetLocalPortalPosition(buildable.Map, currentPortal), quarterTurns);
                }

                if (previousPortal is IStairs stairs)
                    floor += stairs.Direction == StairsDirection.Up ? 1 : -1;
            }

            if (!_entries.ContainsKey(location.Id) && renderQueue.Get(location) is Task<Image> rawImage)
            {
                var turns = quarterTurns;
                var rotatedImage = rawImage.ContinueWith(task =>
                {
                    var image = task.GetAwaiter().GetResult().Duplicate() as Image
                        ?? throw new InvalidOperationException("Could not duplicate the rendered map image.");
                    for (var turn = 0; turn < turns; turn++) image.Rotate90(ClockDirection.Clockwise);
                    return image;
                }, TaskScheduler.Default);
                _entries.Add(location.Id, new Entry(buildable.Map, rotatedImage, position, floor, quarterTurns));
            }

            _currentLocationId = location.Id;
        }

        public async Task<Image?> GetImageAsync(Guid locationId) =>
            _entries.TryGetValue(locationId, out var entry) ? await entry.Image.ConfigureAwait(false) : null;

        public Vector2 GetCurrentPlayerPixel(Vector2 localPosition)
        {
            var current = GetCurrentEntry();
            if (current == null) return Vector2.Zero;

            var width = current.Map.HorizontalCells * cellSize + 1;
            var height = current.Map.VerticalCells * cellSize + 1;
            var imagePosition = new Vector2(
                (localPosition.X + current.Map.CenterX) * cellSize + cellSize / 2f,
                (localPosition.Y + current.Map.CenterY) * cellSize + cellSize / 2f
            );

            for (var turn = 0; turn < current.QuarterTurns; turn++)
            {
                imagePosition = new Vector2(height - 1 - imagePosition.Y, imagePosition.X);
                (width, height) = (height, width);
            }

            return imagePosition;
        }

        Entry? GetCurrentEntry() => _currentLocationId is Guid id && _entries.TryGetValue(id, out var entry) ? entry : null;

        static IEnumerable<IPortal> GetPortals(ILocation location) =>
            location is Node node ? node.GetChildren().OfType<IPortal>() : [];

        static Vector2 GetLocalPortalPosition(Map map, IPortal portal)
        {
            var cellCenter = new Vector2(portal.MapPosition.X + 0.5f, portal.MapPosition.Y + 0.5f);
            return cellCenter + DirectionVector(portal.MapDirection) * 0.5f
                - new Vector2(map.HorizontalCells, map.VerticalCells) / 2f;
        }

        static int FindRotation(CardinalDirection from, CardinalDirection to)
        {
            for (var turns = 0; turns < 4; turns++)
                if (Rotate(from, turns) == to) return turns;
            return 0;
        }

        static CardinalDirection Rotate(CardinalDirection direction, int quarterTurns)
        {
            for (var turn = 0; turn < quarterTurns; turn++)
                direction = NextDirection(direction, ClockDirection.Clockwise);
            return direction;
        }

        static Vector2 Rotate(Vector2 value, int quarterTurns) => quarterTurns switch
        {
            1 => new Vector2(-value.Y, value.X),
            2 => -value,
            3 => new Vector2(value.Y, -value.X),
            _ => value
        };

        static Vector2 DirectionVector(CardinalDirection direction) => direction switch
        {
            CardinalDirection.North => Vector2.Up,
            CardinalDirection.South => Vector2.Down,
            CardinalDirection.East => Vector2.Right,
            _ => Vector2.Left
        };
    }
}
