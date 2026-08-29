using Godot;

namespace Dose.Core.Library.Locations
{
    public class LocationLampFloorOverlayBuilder(
        IBuildableLocation location,
        Texture2D texture,
        int blockSize,
        string? meshInstanceName = null
    ) : LocationMeshBuilder(
        location,
        CreateMaterial(texture),
        false,
        meshInstanceName
    )
    {
        const float FLOOR_OFFSET = 0.005f;

        readonly QuadMesh _overlayMesh = new() { Size = Vector2.One };
        bool _hasOverlays;

        public int BlockSize { get; } = blockSize;

        protected override void OnBuildCell(Vector2I point)
        {
            if (Location.Map == null)
            {
                GD.PushWarning($"{nameof(Location.Map)} not defined. Skipping {nameof(OnBuildCell)}.");
                return;
            }

            var cell = Location.Map.Get(point);

            if (!cell.HasLamp)
            {
                return;
            }

            var position = new Vector3(
                (point.X + cell.LampOffset.X - Location.Map.CenterX) * BlockSize,
                FLOOR_OFFSET,
                (point.Y + cell.LampOffset.Y - Location.Map.CenterY) * BlockSize
            );
            var scale = new Vector3(BlockSize, BlockSize, BlockSize);
            var basis = new Basis(Vector3.Right, -Mathf.Pi / 2f).Scaled(scale);

            _surfaceTool.AppendFrom(_overlayMesh, 0, new Transform3D(basis, position));
            _hasOverlays = true;
        }

        protected override void OnFinish()
        {
            if (_hasOverlays)
            {
                base.OnFinish();
            }
        }

        static StandardMaterial3D CreateMaterial(Texture2D texture)
        {
            return new StandardMaterial3D
            {
                AlbedoTexture = texture,
                AlbedoColor = new Color(1f, 1f, 1f, 0.045f),
                Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
                TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
                CullMode = BaseMaterial3D.CullModeEnum.Disabled
            };
        }
    }
}
