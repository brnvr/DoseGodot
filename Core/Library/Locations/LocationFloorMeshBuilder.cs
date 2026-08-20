using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class LocationFloorMeshBuilder(
        IBuildableLocation location,
        Mesh floorMesh,
        StandardMaterial3D floorMaterial,
        int blockSize,
        string? meshInstanceName = null,
        uint collisionLayer = LocationCollisionLayer.Physics
    ) : LocationMeshBuilder(location, floorMaterial, true, meshInstanceName, collisionLayer)
    {
        const string OVERLAY_SHADER_CODE = """
            shader_type spatial;
            render_mode unshaded;

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

        readonly Dictionary<(Texture2D Texture, float Scale, bool TextureRepeat), SurfaceTool> _overlaySurfaceTools = [];
        readonly Shader _overlayShader = new() { Code = OVERLAY_SHADER_CODE };
        bool _hasPlainFloors;

        public int BlockSize { get; } = blockSize;
        public Mesh FloorMesh { get; } = floorMesh;

        protected override void OnBuildCell(Vector2I point)
        {
            if (Location.Map == null)
            {
                GD.PushWarning($"{nameof(Location.Map)} not defined. Skipping {nameof(OnBuildCell)}.");
                return;
            }

            var cell = Location.Map.Get(point);
            var position = new Vector3(
                (point.X - Location.Map.CenterX) * BlockSize,
                0,
                (point.Y - Location.Map.CenterY) * BlockSize
            );
            var scale = new Vector3(BlockSize, BlockSize, BlockSize);
            var transform = new Transform3D(Basis.Identity.Scaled(scale), position);

            if (cell.FloorOverlayTexture == null)
            {
                _surfaceTool.AppendFrom(FloorMesh, 0, transform);
                _hasPlainFloors = true;
                return;
            }

            GetOverlaySurfaceTool(
                cell.FloorOverlayTexture,
                cell.FloorOverlayTextureScale,
                cell.FloorOverlayTextureRepeat
            ).AppendFrom(FloorMesh, 0, transform);
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

            if (_hasPlainFloors)
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

            var meshInstance = new MeshInstance3D { Mesh = mesh };

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
            var material = new ShaderMaterial { Shader = _overlayShader };
            material.SetShaderParameter("base_texture", Material.AlbedoTexture);
            material.SetShaderParameter("overlay_texture", overlayTexture);
            material.SetShaderParameter("overlay_scale", overlayScale);
            material.SetShaderParameter("texture_repeat", textureRepeat);
            return material;
        }
    }
}
