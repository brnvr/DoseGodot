using System;
using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Randomness;
using Godot;

namespace Dose.Core.Library.Locations
{
    public abstract class LocationPopulator(
        ILocation location,
        Probability probabilityPopulate,
        int blockSize
    )
    {
        public ILocation Location { get; } = location;
        public Probability ProbabilityPopulate { get; } = probabilityPopulate;
        public int BlockSize { get; } = blockSize;

        public List<IInitializableActor> Populate(HashSet<Vector2I> populatedPositions, out HashSet<Vector2I> populatedPositionsOut)
        {
            populatedPositionsOut = [.. populatedPositions];
            List<IInitializableActor> actors = [];

            if (Location?.Map == null)
            {
                GD.PushWarning("Location.Map is null. Cannot populate location.");
                return actors;
            }

            var avaliablePositions = Location.Map.GetFilledPositions().Except(populatedPositions).ToList();
            var decider = new RandomDecider(ProbabilityPopulate);

            while (decider.Roll())
            {
                var positionSelector = new RandomSelector<Vector2I>(avaliablePositions);
                var position = positionSelector.Pick();

                populatedPositionsOut.Add(position);
                avaliablePositions.Remove(position);

                var actor = PopulateCell(position);
                if (actor == null)
                {
                    return actors;
                }

                actors.Add(actor);

                if (avaliablePositions.Count == 0)
                {
                    return actors;
                }
            }

            return actors;
        }

        protected abstract IInitializableActor? PopulateCell(Vector2I position);
    }

    public class LocationPopulator<TActor, TActorContext>(
        IBuildableLocation location,
        ActorPoolSelector<TActor, TActorContext> selector,
        Probability probabilityPopulate,
        int blockSize
    ) : LocationPopulator(location, probabilityPopulate, blockSize) where TActor : class, IInitializableActor, new() where TActorContext : IActorContext
    {
        public new IBuildableLocation Location { get; } = location;
        public ActorPoolSelector<TActor, TActorContext> Selector { get; } = selector;

        protected override IInitializableActor? PopulateCell(Vector2I position)
        {
            var nextActor = Selector.Select();

            if (nextActor == null) return null;
    
            if (nextActor is Node nextActorNode)
            {
                //Location.OnBuild(nextActorNode);
                if (Location is Node locationNode)
                {
                    locationNode.AddChild(nextActorNode);
                }
            }

            if (Location?.Map == null)
            {
                GD.PushWarning("Location.Map is null. Cannot populate cell.");

                return nextActor;
            }

            if (nextActor is Node3D nextActorNode3D)
            {
                nextActorNode3D.Position = new Vector3(
                    position.X - Location.Map.HorizontalCells / 2, 0, position.Y - Location.Map.VerticalCells / 2) * BlockSize;
            }

            return nextActor;
        }
    }
}