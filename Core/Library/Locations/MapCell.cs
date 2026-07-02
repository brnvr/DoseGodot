using System;
using Godot;
using static Dose.Core.Library.GeoPositioning;

namespace Dose.Core.Library.Locations
{
    public record MapCell
    {
        public bool HasLamp { get; private set; }
        public MapCellWall? EastWall { get; private set; }
        public MapCellWall? WestWall { get; private set; }
        public MapCellWall? NorthWall { get; private set; }
        public MapCellWall? SouthWall { get; private set; }

        public MapCell() { }
        
        public MapCell(MapCellWall eastWall, MapCellWall westWall, MapCellWall northWall, MapCellWall southWall, bool hasLamp)
        {
            HasLamp = hasLamp;
            EastWall = eastWall;
            WestWall = westWall;
            NorthWall = northWall;
            SouthWall = southWall;
        }

        public MapCellWall? GetWall(CardinalDirection direction)
        {
            return direction switch
            {
                CardinalDirection.North => NorthWall,
                CardinalDirection.South => SouthWall,
                CardinalDirection.East => EastWall,
                CardinalDirection.West => WestWall,
                _ => throw new Exception("Invalid direction."),
            };
        }

        public MapCell WithWall(CardinalDirection direction, MapCellWall? wall) =>
        direction switch
        {
            CardinalDirection.North => this with { NorthWall = wall },
            CardinalDirection.South => this with { SouthWall = wall },
            CardinalDirection.East  => this with { EastWall  = wall },
            CardinalDirection.West  => this with { WestWall  = wall },
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
        
        public MapCell WithLamp() => this with { HasLamp = true };
    }
}