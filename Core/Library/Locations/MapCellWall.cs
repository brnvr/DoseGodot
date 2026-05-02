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

        public Texture2DArray? TextureArray { get; }
        public MapCellType Type { get; set; }

        public MapCellWall(Texture2DArray? textureArray, MapCellType type)
        {
            Type = type;
            TextureArray = textureArray;
        }
    }
}