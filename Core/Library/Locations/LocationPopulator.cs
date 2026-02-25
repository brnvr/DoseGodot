using System;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Randomness;
using Godot;

namespace Dose.Core.Library.Locations
{
    public abstract class LocationPopulator(
        IBuildableLocation location,
        Probability probabilityPopulate,
        int blockSize
    )
    {
        public IBuildableLocation Location { get; } = location;
        public Probability ProbabilityPopulate { get; } = probabilityPopulate;
        public int BlockSize { get; } = blockSize;

        public void Populate()
        {
            if (Location?.Map == null)
            {
                return;
            }
            var decider = new RandomDecider(ProbabilityPopulate);

            while (decider.Roll())
            {
                var position = new Vector2I(
                    new Random().Next(Location.Map.HorizontalCells),
                    new Random().Next(Location.Map.VerticalCells)
                );

                PopulateCell(position);
            }
        }

        protected abstract void PopulateCell(Vector2I position);
    }

    public class LocationPopulator<TActor, TActorContext>(
        IBuildableLocation location,
        LocationPopulatorSelector<TActor, TActorContext> selector,
        Probability probabilityPopulate,
        int blockSize
    ) : LocationPopulator(location, probabilityPopulate, blockSize) where TActor : class, IInitializableActor, new() where TActorContext : IActorContext
    {
        public LocationPopulatorSelector<TActor, TActorContext> Selector { get; } = selector;

        protected override void PopulateCell(Vector2I position)
        {
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
        }
    }
}