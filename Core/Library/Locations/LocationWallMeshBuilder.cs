using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static Dose.Core.Library.GeoPositioning;
using static Dose.Core.Library.Locations.MapCellWall;

namespace Dose.Core.Library.Locations
{
    public class LocationWallMeshBuilder(
        IBuildableLocation location,
        Mesh wallMesh,
        Mesh doorwayMesh,
        StandardMaterial3D wallMaterial,
        int blockSize,
        string? meshInstanceName = null
) : LocationMeshBuilder(location, wallMaterial, true, meshInstanceName)
    {
        readonly FrozenDictionary<MapCellType, Mesh?> _meshMap = new Dictionary<MapCellType, Mesh?>
        {
            { MapCellType.Solid, wallMesh },
            { MapCellType.Doorway, doorwayMesh }
        }.ToFrozenDictionary();

        public int BlockSize { get; } = blockSize;

        protected override void OnBuildCell(Vector2I position)
        {
            AppendCell(position, AllDirections());
        }

        void AppendCell(Vector2I point, IEnumerable<CardinalDirection> directions)
        {
            var direction = directions.First();

            AppendWall(point, direction);

            var remainingDirections = directions.Except([direction]);

            if (remainingDirections.Any())
            {
                AppendCell(point, remainingDirections);
            }
        }

        void AppendWall(Vector2I point, CardinalDirection direction)
        {
            var cell = Location.Map?.Get(point) ?? throw new Exception($"Cell not defined at position: ({point.X}, {point.Y})");
            var wall = cell.GetWall(direction);

            if (wall == null)
            {
                return;
            }

            var mesh = _meshMap.GetValueOrDefault(wall.Type);

            if (mesh == null)
            {
                return;
            }

            var centerPosition = MoveTo(point, direction, 0.5f);
            var wallPosition = new Vector3((centerPosition.X - Location.Map.HorizontalCells / 2) * BlockSize, 0, (centerPosition.Y - Location.Map.VerticalCells / 2) * BlockSize);

            var scale = new Vector3(BlockSize, BlockSize, BlockSize);

            Basis rotation = direction switch
            {
                CardinalDirection.South => new Basis(Vector3.Up, Mathf.Pi).Scaled(scale),
                CardinalDirection.East => new Basis(Vector3.Up, -Mathf.Pi / 2f).Scaled(scale),
                CardinalDirection.West => new Basis(Vector3.Up, Mathf.Pi / 2f).Scaled(scale),
                _ => Basis.Identity.Scaled(scale)
            };

            var transform = new Transform3D(rotation, wallPosition);

            _surfaceTool.AppendFrom(mesh, 0, transform);
        }
    }
}