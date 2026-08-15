using Godot;

namespace Dose.Core.Library.Locations
{
    public record MapCellWall
    {
        public enum MapCellType
        {
            Solid,
            Doorway,
            Upstairs,
            Downstairs
        }

        public MapCellType Type { get; set; }
        public Texture2D? OverlayTexture { get; set; }
        public float OverlayTextureScale { get; set; }
        public bool OverlayTextureRepeat { get; set; }

        public MapCellWall(
            MapCellType type,
            Texture2D? overlayTexture = null,
            float overlayTextureScale = 1f,
            bool overlayTextureRepeat = false
        )
        {
            Type = type;
            OverlayTexture = overlayTexture;
            OverlayTextureScale = overlayTextureScale;
            OverlayTextureRepeat = overlayTextureRepeat;
        }
    }
}
