using Godot;

namespace Dose.Core.Library.Locations
{
    public class LocationLampSceneFactory(Texture2D texture, int blockSize)
    : LocationBlockSceneFactory
    {
        public int BlockSize { get; } = blockSize;
        public Texture2D Texture { get; } = texture;

        protected override Vector3 GetPosition(IBuildableLocation location, Vector2I point)
        {
             if (location.Map == null)
            {
                return Vector3.Zero;
            }

            var offset = location.Map.Get(point).LampOffset;

            return new Vector3(
                point.X + offset.X - location.Map.CenterX,
                0,
                point.Y + offset.Y - location.Map.CenterY
            ) * BlockSize;
        }

        protected override float GetRotation(IBuildableLocation location, Vector2I point)
        {
            return 0;
        }

        protected override void InitializeNode(Node node)
        {
            var meshInstance = node.GetChild<MeshInstance3D>(0);

            var material = GodotUtils.CreateBillboardMaterial(Texture);

            meshInstance.SetSurfaceOverrideMaterial(0, material);
        }

        protected override bool Predicate(MapCell cell)
        {
            return cell.HasLamp;
        }
    }
}
