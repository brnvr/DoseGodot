using System;
using System.Collections.Generic;
using Godot;

namespace Dose.Library.DataStructures
{
    public class Grid<T>(int horizontalCells, int verticalCells)
    {
        readonly T[,] _cells = new T[horizontalCells, verticalCells];
        readonly HashSet<Vector2I> _filledPositions = [];

        public int HorizontalCells { get; } = horizontalCells;
        public int VerticalCells { get; } = verticalCells;

        public void Fill(T value)
        {
            for (int i = 0; i < HorizontalCells; i++)
            {
                for (int j = 0; j < VerticalCells; j++)
                {
                    Set(new Vector2I(i, j), value);
                }
            }
        }

        public Vector2I[] GetFilledPositions()
        {
            return [.. _filledPositions];
        }

        public void Set(Vector2I position, T value)
        {
            _cells[position.X, position.Y] = value;

            if (value == null)
            {
                _filledPositions.Remove(position);

                return;
            }

            _filledPositions.Add(position);
        }

        public T Get(Vector2I position)
        {
            if (!InBounds(position))
            {
                throw new Exception($"Position out of bounds: {position.X}, {position.Y}");
            }

            return _cells[position.X, position.Y];
        }

        public bool TryGet(Vector2I position, out T value)
        {
            if (InBounds(position))
            {
                value = Get(position);
                return true;
            }

            value = default;
            return false;
        }

        public bool InBounds(Vector2I position)
        {
            return position.X >= 0 && position.X < HorizontalCells && position.Y >= 0 && position.Y < VerticalCells;
        }
    }
}