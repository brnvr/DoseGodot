using Godot;

namespace Dose.Core.Library.Locations
{
    public class LocationLampSceneFactory(int blockSize) : LocationBlockSceneFactory
    {
        public int BlockSize { get; } = blockSize;

        protected override Vector3 GetPosition(IBuildableLocation location, Vector2I point)
        {
             if (location.Map == null)
            {
                return Vector3.Zero;
            }

            return new Vector3(point.X - location.Map.CenterX, 0, point.Y - location.Map.CenterY) * BlockSize;
        }

        protected override float GetRotation(IBuildableLocation location, Vector2I point)
        {
            return 0;
        }

        protected override void InitializeNode(Node node)
        {
            return;
        }

        protected override bool Predicate(MapCell cell)
        {
            return cell.LampTexture != null;
        }
    }
}