using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static Dose.Library.GodotUtils;
using static Dose.Library.GeoPositioning;

namespace Dose.Library.Locations
{
    public class LocationMeshBuildera(Map map, int blockSize)
    {
        readonly static Mesh _wallMesh = LoadMeshFromScene("res://Scenes/Models/Wall.glb");
        readonly static Mesh _wallWithOpeningMesh = LoadMeshFromScene("res://Scenes/Models/WallWithOpening.glb");

        public Map Map { get; } = map;
        public int BlockSize { get; } = blockSize;

        public ArrayMesh Build()
        {
            var st = new SurfaceTool();
            st.Begin(Mesh.PrimitiveType.Triangles);

            foreach (var point in Map.GetFilledPositions())
            {
                AppendCell(st, point, AllDirections());
            }

            st.Index();
            return st.Commit();
        }
        

        void AppendCell(SurfaceTool st, Vector2I point, IEnumerable<CardinalDirection> directions)
        {
            var direction = directions.First();

            AppendWall(st, point, direction);

            var remainingDirections = directions.Except([direction]);

            if (remainingDirections.Any())
            {
                AppendCell(st, point, remainingDirections);
            }
        }

        void AppendWall(SurfaceTool st, Vector2I point, CardinalDirection direction)
        {
            var cell = Map.Get(point) ?? throw new Exception($"Cell not defined at position: ({point.X}, {point.Y})");
            var wall = cell.GetWall(direction);

            if (wall == null)
            {
                return;
            }

            var mesh = wall.HasDoor ? _wallWithOpeningMesh : _wallMesh;

            var centerPosition = MoveTo(point, direction, 0.5f);
            var wallPosition = new Vector3((centerPosition.X - Map.HorizontalCells / 2) * BlockSize, 0, (centerPosition.Y - Map.VerticalCells / 2) * BlockSize);

            var scale = new Vector3(BlockSize, BlockSize, BlockSize);

            Basis rotation = direction switch
            {
                CardinalDirection.South => new Basis(Vector3.Up, Mathf.Pi).Scaled(scale),
                CardinalDirection.East => new Basis(Vector3.Up, -Mathf.Pi / 2f).Scaled(scale),
                CardinalDirection.West => new Basis(Vector3.Up, Mathf.Pi / 2f).Scaled(scale),
                _ => Basis.Identity.Scaled(scale)
            };

            var transform = new Transform3D(rotation, wallPosition);

            st.AppendFrom(mesh, 0, transform);
        }
    }
}