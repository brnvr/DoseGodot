using Dose.Core.Library.Actors;
using Godot;
using static Dose.Core.Library.Locations.MapCellWall;

namespace Dose.Core.Library.Locations
{
    public class LocationStairsUpProvider<TInitializable, TArgs>(
        IBuildableLocation location,
        PackedScene stairsScene,
        int blockSize,
        Vector2 offset,
        TArgs initializationArgs
    ) : LocationStairsProvider<TInitializable, TArgs>(location, stairsScene, blockSize, offset, initializationArgs)
        where TInitializable : IInitializableActor<StairsContext, TArgs>
    {
        protected override bool WallPredicate(MapCellWall wall)
        {
            return wall.Type == MapCellType.Upstairs;
        }
    }
}