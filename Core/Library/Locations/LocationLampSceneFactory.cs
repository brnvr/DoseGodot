using Godot;
using Dose.Core.Library.Numbers;

namespace Dose.Core.Library.Locations
{
    public class LocationLampSceneFactory(Texture2D texture, ScaleFactor scale, int blockSize)
    : LocationBlockSceneFactory
    {
        public int BlockSize { get; } = blockSize;
        public Texture2D Texture { get; } = texture;
        public ScaleFactor Scale { get; } = scale;

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
            ScaleFromTop(meshInstance);
        }

        void ScaleFromTop(MeshInstance3D meshInstance)
        {
            var meshTop = meshInstance.GetAabb().End.Y;
            var topY = meshInstance.Position.Y + meshTop * meshInstance.Scale.Y;
            var scaled = meshInstance.Scale * Scale.Value;
            var position = meshInstance.Position;
            position.Y = topY - meshTop * scaled.Y;
            meshInstance.Scale = scaled;
            meshInstance.Position = position;
        }

        protected override bool Predicate(MapCell cell)
        {
            return cell.HasLamp;
        }
    }
}
