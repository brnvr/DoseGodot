using Dose.Core.Library.Actors;
using Godot;
using static Dose.Core.Library.Locations.MapCellWall;

namespace Dose.Core.Library.Locations
{
    public class LocationStairsDownProvider<TInitializable, TArgs>(
        IBuildableLocation location,
        PackedScene stairsScene,
        int blockSize,
        TArgs initializationArgs
    ) : LocationStairsProvider<TInitializable, TArgs>(location, stairsScene, blockSize, initializationArgs)
        where TInitializable : IInitializableActor<StairsContext, TArgs>
    {
        protected override bool WallPredicate(MapCellWall wall)
        {
            return wall.Type == MapCellType.Downstairs;
        }
    }
}