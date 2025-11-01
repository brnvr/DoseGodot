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

        public enum ClockDirection
        {
            Clockwise,
            Counterclockwise
        }

        public static ICollection<CardinalDirection> AllDirections()
        {
            return
            [
                CardinalDirection.East,
                CardinalDirection.West,
                CardinalDirection.North,
                CardinalDirection.South
            ];
        }

        public static ICollection<Orientation> AllOrientations()
        {
            return [
                Orientation.Horizontal,
                Orientation.Vertical
            ];
        }

        public static Orientation OrtogonalOrientation(Orientation orientation)
        {
            return orientation switch
            {
                Orientation.Horizontal => Orientation.Vertical,
                Orientation.Vertical => Orientation.Horizontal,
                _ => throw new Exception("Invalid orientation."),
            };
        }
        
        public static Orientation OrientationFromDirection(CardinalDirection direction)
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

        public static Vector2I MoveTo(Vector2I point, CardinalDirection direction, int distance = 1)
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

        public static Vector2 MoveTo(Vector2 point, CardinalDirection direction, float distance)
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

        public static CardinalDirection NextDirection(CardinalDirection cardinalDirection, ClockDirection clockDirection)
        {
            return cardinalDirection switch
            {
                CardinalDirection.North => clockDirection == ClockDirection.Clockwise ? CardinalDirection.East : CardinalDirection.West,
                CardinalDirection.South => clockDirection == ClockDirection.Clockwise ? CardinalDirection.West : CardinalDirection.East,
                CardinalDirection.East => clockDirection == ClockDirection.Clockwise ? CardinalDirection.South : CardinalDirection.North,
                CardinalDirection.West => clockDirection == ClockDirection.Clockwise ? CardinalDirection.North : CardinalDirection.South,
                _ => throw new Exception("Invalid direction."),
            };
        }

        public static float DirectionToRadians(CardinalDirection direction)
        {
            return direction switch
            {
                CardinalDirection.North => (float)Math.PI / 2,
                CardinalDirection.South => (float)Math.PI * 3 / 2,
                CardinalDirection.East => 0,
                CardinalDirection.West => (float)Math.PI,
                _ => throw new Exception("Invalid direction."),
            };
        }
    }
}