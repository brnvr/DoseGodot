using Godot;

namespace Dose.Core.Library.Locations
{
    public record MapCellWall
    {
        public enum MapCellDoorType
        {
            None,
            Open,
            Locked
        }

        public Texture2DArray? TextureArray { get; }
        public MapCellDoorType DoorType { get; set; }

        public MapCellWall(Texture2DArray? textureArray, MapCellDoorType doorType)
        {
            TextureArray = textureArray;
            DoorType = doorType;
        }
    }
}