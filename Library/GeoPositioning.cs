using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Godot;

namespace Dose.Library
{
    public static class GeoPositioning
    {
        public enum CardinalDirection
        {
            East,
            West,
            North,
            South
        }

        public enum Orientation
        {
            Horizontal,
            Vertical
        }

        public static ICollection<CardinalDirection> GetAllDirections()
        {
            return
            [
                CardinalDirection.East,
                CardinalDirection.West,
                CardinalDirection.North,
                CardinalDirection.South
            ];
        }

        public static ICollection<Orientation> GetAllOrientation()
        {
            return [
                Orientation.Horizontal,
                Orientation.Vertical
            ];
        }
        
        public static Orientation GetOrientationFromDirection(CardinalDirection direction)
        {
            return direction switch
            {
                CardinalDirection.North => Orientation.Vertical,
                CardinalDirection.South => Orientation.Vertical,
                CardinalDirection.East => Orientation.Horizontal,
                CardinalDirection.West => Orientation.Horizontal,
                _ => throw new Exception("Invalid direction."),
            };
        }

        public static Vector2I MoveToDirection(Vector2I point, CardinalDirection direction, int distance = 1)
        {
            return direction switch
            {
                CardinalDirection.North => point - new Vector2I(0, distance),
                CardinalDirection.South => point + new Vector2I(0, distance),
                CardinalDirection.East => point + new Vector2I(distance, 0),
                CardinalDirection.West => point - new Vector2I(distance, 0),
                _ => throw new Exception("Invalid direction."),
            };
        }

        public static Vector2 MoveToDirection(Vector2 point, CardinalDirection direction, float distance)
        {
            return direction switch
            {
                CardinalDirection.North => point - new Vector2(0, distance),
                CardinalDirection.South => point + new Vector2(0, distance),
                CardinalDirection.East => point + new Vector2(distance, 0),
                CardinalDirection.West => point - new Vector2(distance, 0),
                _ => throw new Exception("Invalid direction."),
            };
        }

        public static CardinalDirection CardinalOpposite(CardinalDirection direction)
        {
            return direction switch
            {
                CardinalDirection.North => CardinalDirection.South,
                CardinalDirection.South => CardinalDirection.North,
                CardinalDirection.East => CardinalDirection.West,
                CardinalDirection.West => CardinalDirection.East,
                _ => throw new Exception("Invalid direction."),
            };
        }
    }
}