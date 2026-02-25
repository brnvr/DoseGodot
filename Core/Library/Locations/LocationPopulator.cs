using System.Collections.Generic;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Randomness;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class LocationPopulator<TActor, TActorContext>(
        IBuildableLocation location,
        LocationPopulatorSelector<TActor, TActorContext> selector,
        Probability probabilityPopulate,
        int blockSize
    ) : LocationBuilder(location) where TActor : class, IInitializableActor, new() where TActorContext : IActorContext
    {
        public LocationPopulatorSelector<TActor, TActorContext> Selector { get; } = selector;
        public Probability ProbabilityPopulate { get; } = probabilityPopulate;
        public int BlockSize { get; } = blockSize;

        protected override void OnBuildCell(Vector2I position)
        {
            if (Selector.AvaliableContexts.Count == 0) return;

            var shouldPopulate = new RandomDecider(ProbabilityPopulate).Roll();

            if (!shouldPopulate) return;

            var nextActor = Selector.Select();

            if (nextActor == null) return;
    
            if (nextActor is Node nextActorNode)
            {
                Location.OnBuild(nextActorNode);
            }

            if (Location?.Map == null)
            {
                return;
            }

            if (nextActor is Node3D nextActorNode3D)
            {
                nextActorNode3D.Position = new Vector3(position.X - Location.Map.HorizontalCells / 2, 0, position.Y - Location.Map.VerticalCells / 2) * BlockSize;
            }
            //_allActorContexts.Remove(nextActor);
        }
    }
}