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
        private int _iterations;

        public LocationPopulatorSelector<TActor, TActorContext> Selector { get; } = selector;
        public Probability ProbabilityPopulate { get; } = probabilityPopulate;
        public int BlockSize { get; } = blockSize;

        protected override void OnStart()
        {
            _iterations = 0;

            var decider = new RandomDecider(ProbabilityPopulate);

            while (decider.Roll())
            {
                _iterations++;
            }

            base.OnStart();
        }

        protected override void OnBuildCell(Vector2I position)
        {
            if (_iterations <= 0) return;

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

            _iterations--;
            //_allActorContexts.Remove(nextActor);
        }
    }
}