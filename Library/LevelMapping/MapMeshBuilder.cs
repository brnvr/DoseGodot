using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static Dose.Library.GeoPositioning;

namespace Dose.Library.LevelMapping
{
    public class MapMeshBuilder(Map map, int blockSize)
    {
        readonly FrozenSet<CardinalDirection> _invertNormalsDirections = new[] { CardinalDirection.North, CardinalDirection.East }.ToFrozenSet();

        public Map Map { get; } = map;
        public int BlockSize { get; } = blockSize;

        public ArrayMesh Build()
        {
            var st = new SurfaceTool();
            st.Begin(Mesh.PrimitiveType.Triangles);

            foreach (var point in Map.GetFilledPositions())
            {
                AddCellGeometry(st, point, GetAllDirections());
            }  

            st.Index();
            return st.Commit();
        }

        void AddCellGeometry(SurfaceTool st, Vector2I point, IEnumerable<CardinalDirection> directions)
        {
            var direction = directions.First();

            BuildWallGeometry(st, point, direction);

            var remainingDirections = directions.Except([direction]);

            if (remainingDirections.Any())
            {
                AddCellGeometry(st, point, remainingDirections);
            }   
        }

        void BuildWallGeometry(SurfaceTool st, Vector2I point, CardinalDirection direction)
        {
            var cell = Map.Get(point) ?? throw new Exception($"Cell not defined at position: ({point.X}, {point.Y})");

            var wall = cell.GetWall(direction);

            if (wall == null)
            {
                return;
            }

            var centerPosition = MoveToDirection(point, direction, 0.5f);
            var orientation = GetOrientationFromDirection(direction);
            var ortogonalOrientation = OrtogonalOrientation(orientation);
            var ortogonalDirections = GetAllDirections().Where(d => GetOrientationFromDirection(d) == ortogonalOrientation);

            var point0 = MoveToDirection(centerPosition, ortogonalDirections.First(), 0.5f) - new Vector2I(50, 50);
            var point1 = MoveToDirection(centerPosition, ortogonalDirections.Last(), 0.5f) - new Vector2I(50, 50);

            var wallStart = new Vector2(Mathf.Min(point0.X, point1.X), Mathf.Min(point0.Y, point1.Y));
            var wallEnd = new Vector2(Mathf.Max(point0.X, point1.X), Mathf.Max(point0.Y, point1.Y));

            var v0 = new Vector3(wallStart.X * BlockSize, BlockSize, wallStart.Y * BlockSize);
            var v1 = new Vector3(wallEnd.X * BlockSize, BlockSize, wallEnd.Y * BlockSize);
            var v2 = new Vector3(wallEnd.X * BlockSize, 0, wallEnd.Y * BlockSize);
            var v3 = new Vector3(wallStart.X * BlockSize, 0, wallStart.Y * BlockSize);

            bool invert = _invertNormalsDirections.Contains(direction);

            int[] indices = invert ? [0, 1, 2, 0, 2, 3] : [2, 1, 0, 3, 2, 0];
            Vector3[] vertices = [v0, v1, v2, v3];

            AddQuad(st, vertices, indices);
        }

        static void AddQuad(SurfaceTool st, Vector3[] vertices, int[] indices)
            {
            for (int i = 0; i < indices.Length; i += 3)
            {
                var i0 = indices[i];
                var i1 = indices[i + 1];
                var i2 = indices[i + 2];

                var normal = (vertices[i1] - vertices[i0]).Cross(vertices[i2] - vertices[i0]).Normalized();

                st.SetNormal(normal); st.AddVertex(vertices[i0]);
                st.SetNormal(normal); st.AddVertex(vertices[i1]);
                st.SetNormal(normal); st.AddVertex(vertices[i2]);
            }
        }
    }
}
