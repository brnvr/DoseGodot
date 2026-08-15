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
        const string OVERLAY_SHADER_CODE = """
            shader_type spatial;
            render_mode vertex_lighting;

            uniform sampler2D base_texture : source_color, filter_nearest;
            uniform sampler2D overlay_texture : source_color, filter_nearest, repeat_enable;
            uniform float overlay_scale = 1.0;
            uniform bool texture_repeat = false;

            void fragment() {
                vec4 base_color = texture(base_texture, UV);
                vec2 overlay_uv = (UV - vec2(0.5)) / overlay_scale + vec2(0.5);
                float overlay_mask = 1.0;

                if (!texture_repeat) {
                    overlay_mask = step(0.0, overlay_uv.x)
                        * step(overlay_uv.x, 1.0)
                        * step(0.0, overlay_uv.y)
                        * step(overlay_uv.y, 1.0);
                    overlay_uv = clamp(overlay_uv, vec2(0.0), vec2(1.0));
                }

                vec4 overlay_color = texture(overlay_texture, overlay_uv);
                ALBEDO = mix(base_color.rgb, overlay_color.rgb, overlay_color.a * overlay_mask);
            }
            """;

        readonly FrozenDictionary<MapCellType, Mesh?> _meshMap = new Dictionary<MapCellType, Mesh?>
        {
            { MapCellType.Solid, wallMesh },
            { MapCellType.Doorway, doorwayMesh }
        }.ToFrozenDictionary();
        readonly Dictionary<(Texture2D Texture, float Scale, bool TextureRepeat), SurfaceTool> _overlaySurfaceTools = [];
        readonly Shader _overlayShader = new() { Code = OVERLAY_SHADER_CODE };
        bool _hasPlainWalls;

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
            var wallPosition = new Vector3((centerPosition.X - Location.Map.CenterX) * BlockSize, 0, (centerPosition.Y - Location.Map.CenterY) * BlockSize);

            var scale = new Vector3(BlockSize, BlockSize, BlockSize);

            Basis rotation = direction switch
            {
                CardinalDirection.South => new Basis(Vector3.Up, Mathf.Pi).Scaled(scale),
                CardinalDirection.East => new Basis(Vector3.Up, -Mathf.Pi / 2f).Scaled(scale),
                CardinalDirection.West => new Basis(Vector3.Up, Mathf.Pi / 2f).Scaled(scale),
                _ => Basis.Identity.Scaled(scale)
            };

            var transform = new Transform3D(rotation, wallPosition);

            if (wall.OverlayTexture == null)
            {
                _surfaceTool.AppendFrom(mesh, 0, transform);
                _hasPlainWalls = true;
                return;
            }

            GetOverlaySurfaceTool(
                wall.OverlayTexture,
                wall.OverlayTextureScale,
                wall.OverlayTextureRepeat
            ).AppendFrom(mesh, 0, transform);
        }

        SurfaceTool GetOverlaySurfaceTool(Texture2D overlayTexture, float overlayScale, bool textureRepeat)
        {
            var overlay = (overlayTexture, overlayScale, textureRepeat);

            if (_overlaySurfaceTools.TryGetValue(overlay, out var surfaceTool))
            {
                return surfaceTool;
            }

            surfaceTool = new SurfaceTool();
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
            surfaceTool.SetSmoothGroup(uint.MaxValue);
            _overlaySurfaceTools.Add(overlay, surfaceTool);

            return surfaceTool;
        }

        protected override void OnFinish()
        {
            var mesh = new ArrayMesh();

            if (_hasPlainWalls)
            {
                _surfaceTool.Commit(mesh);
                mesh.SurfaceSetMaterial(mesh.GetSurfaceCount() - 1, Material);
            }

            foreach (var (overlay, surfaceTool) in _overlaySurfaceTools)
            {
                surfaceTool.Commit(mesh);
                mesh.SurfaceSetMaterial(
                    mesh.GetSurfaceCount() - 1,
                    CreateOverlayMaterial(overlay.Texture, overlay.Scale, overlay.TextureRepeat)
                );
            }

            var meshInstance = new MeshInstance3D
            {
                Mesh = mesh
            };

            if (MeshInstanceName != null)
            {
                meshInstance.Name = MeshInstanceName;
            }

            meshInstance.CreateTrimeshCollision();

            foreach (var body in meshInstance.GetChildren().OfType<StaticBody3D>())
            {
                body.CollisionLayer = CollisionLayer;
            }

            Location.OnBuild(meshInstance);
        }

        ShaderMaterial CreateOverlayMaterial(Texture2D overlayTexture, float overlayScale, bool textureRepeat)
        {
            var material = new ShaderMaterial
            {
                Shader = _overlayShader
            };

            material.SetShaderParameter("base_texture", Material.AlbedoTexture);
            material.SetShaderParameter("overlay_texture", overlayTexture);
            material.SetShaderParameter("overlay_scale", overlayScale);
            material.SetShaderParameter("texture_repeat", textureRepeat);

            return material;
        }
    }
}
