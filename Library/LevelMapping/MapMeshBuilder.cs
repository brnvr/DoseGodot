using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Godot;
using static System.Math;
using static Dose.Library.GeoPositioning;
using static Dose.Library.DataStructures.Utils;

namespace Dose.Library.LevelMapping
{
    public class MapMeshBuilder(Map map, int blockSize, int maxThreads = 4)
    {
        readonly FrozenSet<CardinalDirection> _directions0 = new[] { CardinalDirection.South, CardinalDirection.West }.ToFrozenSet();
        readonly FrozenSet<CardinalDirection> _directions1 = new[] { CardinalDirection.North, CardinalDirection.East }.ToFrozenSet();

        public Map Map { get; } = map;
        public int BlockSize { get; } = blockSize;
        public int MaxThreads { get; set; } = maxThreads;

        public ArrayMesh Build()
        {
            var positions = Map.GetFilledPositions();
            var chunks = SplitEnumerable(positions, MaxThreads);
            var mesh = new ArrayMesh();

            BuildMesh(mesh);

            return mesh;
        }
        
        void BuildMesh(ArrayMesh mesh)
        {
            var allVertices = new List<Vector3>();
            var allIndices = new List<int>();

            foreach (var point in Map.GetFilledPositions())
            {
                var (cellVerts, cellIndices) = AddCellGeometry(point);
                int offset = allVertices.Count;
                allVertices.AddRange(cellVerts);
                allIndices.AddRange(cellIndices.Select(i => i + offset));
            }

            var normals = allVertices.Select(_ => Vector3.Forward).ToArray();

            var arrays = new Godot.Collections.Array();
            arrays.Resize((int)Mesh.ArrayType.Max);
            arrays[(int)Mesh.ArrayType.Vertex] = allVertices.ToArray();
            arrays[(int)Mesh.ArrayType.Normal] = normals;
            arrays[(int)Mesh.ArrayType.Index] = allIndices.ToArray();

            mesh.ClearSurfaces();
            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        }
        
        (List<Vector3> vertices, List<int> indices) AddCellGeometry(Vector2I point)
        {
            var cell = Map.Get(point) ?? throw new Exception($"Cell not defined at position: ({point.X}, {point.Y})");

            var vertices = new List<Vector3>();
            var indices = new List<int>();

            foreach (var direction in GetAllDirections())
            {
                var wall = cell.GetWall(direction);
                if (wall == null) continue;

                var dirFlag = _directions1.Contains(direction);

                var centerPosition = MoveToDirection(point, direction, 0.5f);
                var orientation = GetOrientationFromDirection(direction);
                var ortogonalOrientation = OrtogonalOrientation(orientation);
                var ortogonalDirections = GetAllDirections().Where(d => GetOrientationFromDirection(d) == ortogonalOrientation);

                var point0 = MoveToDirection(centerPosition, ortogonalDirections.First(), 0.5f) - new Vector2I(50, 50);
                var point1 = MoveToDirection(centerPosition, ortogonalDirections.Last(), 0.5f) - new Vector2I(50, 50);

                var wallStart = new Vector2(Mathf.Min(point0.X, point1.X), Mathf.Min(point0.Y, point1.Y));
                var wallEnd = new Vector2(Mathf.Max(point0.X, point1.X), Mathf.Max(point0.Y, point1.Y));

                var v0 = new Vector3(wallStart.X, 1, wallStart.Y);
                var v1 = new Vector3(wallEnd.X, 1, wallEnd.Y);
                var v2 = new Vector3(wallEnd.X, 0, wallEnd.Y);
                var v3 = new Vector3(wallStart.X, 0, wallStart.Y);

                int start = vertices.Count;

                vertices.AddRange([v0, v1, v2, v3]);

                if (dirFlag)
                    indices.AddRange([start + 0, start + 1, start + 2, start + 0, start + 2, start + 3]);
                else
                    indices.AddRange([start + 2, start + 1, start + 0, start + 3, start + 2, start + 0]);
            }

            return (vertices, indices);
        }
    }
}