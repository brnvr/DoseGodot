using Godot;

namespace Dose.Library.LevelMapping
{
    public record MapCellWall
    {
        public Texture2DArray TextureArray { get; }
        public bool HasDoor { get; }

        public MapCellWall(Texture2DArray textureArray, bool hasDoor)
        {
            TextureArray = textureArray;
            HasDoor = hasDoor;
        }
    }
}