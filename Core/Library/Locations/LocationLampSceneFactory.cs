using Godot;

namespace Dose.Core.Library.Locations
{
    public class LocationLampSceneFactory(Texture2D texture, int blockSize, Vector2 offset)
    : LocationBlockSceneFactory
    {
        public Vector2 Offset { get; } = offset;
        public int BlockSize { get; } = blockSize;
        public Texture2D Texture { get; } = texture;

        protected override Vector3 GetPosition(IBuildableLocation location, Vector2I point)
        {
             if (location.Map == null)
            {
                return Vector3.Zero;
            }

            return new Vector3(point.X + Offset.X - location.Map.CenterX, 0, point.Y + Offset.Y - location.Map.CenterY) * BlockSize;
        }

        protected override float GetRotation(IBuildableLocation location, Vector2I point)
        {
            return 0;
        }

        protected override void InitializeNode(Node node)
        {
            var meshInstance = node.GetChild<MeshInstance3D>(0);

            meshInstance.SetSurfaceOverrideMaterial(0, new StandardMaterial3D
            {
                AlbedoTexture = texture,
                BillboardMode = BaseMaterial3D.BillboardModeEnum.FixedY,
                Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
                TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            });
        }

        protected override bool Predicate(MapCell cell)
        {
            return cell.HasLamp;
        }
    }
}