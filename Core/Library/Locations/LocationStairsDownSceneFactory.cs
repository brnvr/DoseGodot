using Godot;
using static Dose.Core.Library.Locations.MapCellWall;

namespace Dose.Core.Library.Locations
{
    public class LocationStairsDownSceneFactory<TInitializationArgs>
    (
        TInitializationArgs initializationArgs,
        int blockSize,
        Vector2 offset
    ) : LocationStairsSceneFactory<TInitializationArgs>(initializationArgs, blockSize, offset)
    {
         protected override bool WallPredicate(MapCellWall wall)
        {
            return wall.Type == MapCellType.Downstairs;
        }
    }
}