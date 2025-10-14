using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Godot;
using static System.Math;
using static Dose.Library.GeoPositioning;

namespace Dose.Library.LevelMapping
{
    public class MapMeshBuilder(Map map, int blockSize)
    {
        readonly FrozenSet<CardinalDirection> _directions0 = new[] { CardinalDirection.South, CardinalDirection.West }.ToFrozenSet();
        readonly FrozenSet<CardinalDirection> _directions1 = new[] { CardinalDirection.North, CardinalDirection.East }.ToFrozenSet();

        public Map Map { get; } = map;
        public int BlockSize { get; } = blockSize;

        public ArrayMesh Build()
        {
            var startingPoint = Map.GetFilledPositions().First();
            var buildDirection = GetStartingDirection(startingPoint);
            var wallDirections = GetWallDirections(GetOrientationFromDirection(buildDirection));

            return BuildWalls(new ArrayMesh(), startingPoint, startingPoint, buildDirection, wallDirections);
        }

        CardinalDirection GetStartingDirection(Vector2I startingPoint)
        {
            var cell = Map.Get(startingPoint) ?? throw new Exception($"No cell defined at position: ({startingPoint.X}, {startingPoint.Y})");

            return GetAllDirections().Where(direction => cell.GetWall(direction) == null).First();
        }

        static IEnumerable<CardinalDirection> GetWallDirections(GeoPositioning.Orientation orientation)
        {
            return GetAllDirections().Where(direction => GetOrientationFromDirection(direction) != orientation);
        }

        void AddWallToMesh(ArrayMesh mesh, Vector2 startingPoint, Vector2  endingPoint, CardinalDirection wallDirection)
        {
            var dirFlag = _directions1.Contains(wallDirection);

            var point0 = new Vector2(Min(startingPoint.X, endingPoint.X), Min(startingPoint.Y, endingPoint.Y));
            var point1 = new Vector2(Max(startingPoint.X, endingPoint.X), Max(startingPoint.Y, endingPoint.Y));
        
            var wallStart = MoveToDirection(point0, wallDirection, 0.5f) - new Vector2I(250, 250);
            var wallEnd = MoveToDirection(point1, wallDirection, 0.5f) - new Vector2I(250, 250);

            var v0 = new Vector3(wallStart.X, 1, wallStart.Y);
            var v1 = new Vector3(wallEnd.X, 1, wallEnd.Y);
            var v2 = new Vector3(wallEnd.X, 0, wallEnd.Y);
            var v3 = new Vector3(wallStart.X, 0, wallStart.Y);

            Vector3[] vertices;
            int[] indices;

            if (dirFlag)
            {
                vertices = [v0, v1, v2, v3];
                indices =
                [
                    0, 1, 2,
                    0, 2, 3
                ];
            }
            else
            {
                vertices = [v0, v1, v2, v3];
                indices =
                [
                    2, 1, 0,
                    3, 2, 0
                ];
            }

            var normals = new Vector3[]
            {
                Vector3.Forward, Vector3.Forward, Vector3.Forward, Vector3.Forward
            };

            var arrays = new Godot.Collections.Array();
            arrays.Resize((int)Mesh.ArrayType.Max);
            arrays[(int)Mesh.ArrayType.Vertex] = vertices;
            arrays[(int)Mesh.ArrayType.Normal] = normals;
            arrays[(int)Mesh.ArrayType.Index] = indices;

            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        }

        ArrayMesh BuildWall(ArrayMesh mesh, Vector2I startingPoint, Vector2I endingPoint, CardinalDirection buildDirection, CardinalDirection wallDirection)
        {
            var endingCell = Map.Get(endingPoint) ?? throw new Exception($"No cell defined at position: ({endingPoint.X}, {endingPoint.Y})");
            var wall = endingCell.GetWall(wallDirection);

            var _mesh = wall == null ? CreateBifurcation(mesh, MoveToDirection(endingPoint, wallDirection), wallDirection) : mesh;

            var nextPoint = MoveToDirection(endingPoint, buildDirection);
            var isNextCellInBounds = Map.TryGet(nextPoint, out MapCell nextCell);

            if (!isNextCellInBounds || nextCell == null)
            {
                AddWallToMesh(_mesh, MoveToDirection(startingPoint, buildDirection, -0.5f), MoveToDirection(endingPoint, buildDirection, -0.5f), wallDirection);
                return _mesh;
            }

            return BuildWall(_mesh, startingPoint, nextPoint, buildDirection, wallDirection);
        }

        
        ArrayMesh CreateBifurcation(ArrayMesh mesh, Vector2I point, CardinalDirection buildDirection)
        {
            var newWallDirections = GetWallDirections(GetOrientationFromDirection(buildDirection));

            return BuildWalls(mesh, point, point, buildDirection, newWallDirections);
        }

        ArrayMesh BuildWalls(ArrayMesh mesh, Vector2I startingPoint, Vector2I endingPoint, CardinalDirection buildDirection, IEnumerable<CardinalDirection> wallDirections)
        {
            var wallDirection = wallDirections.First();

            mesh = BuildWall(mesh, startingPoint, endingPoint, buildDirection, wallDirection);
            
            var remainingWallDirections = wallDirections.Except([wallDirection]);

            if (!remainingWallDirections.Any())
            {
                return mesh;
            }

            return BuildWalls(mesh, startingPoint, endingPoint, buildDirection, remainingWallDirections);
        }
    }
}