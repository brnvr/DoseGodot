using Dose.Core.ActorArgs;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Location;
using Dose.Core.Library.Locations;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class Stairs : Node3D, IActor<Player>, IStairs, IInitializableActor<StairsContext, StairsArgs>
    {
        public IStairs? Linked { get; private set; }
        public ILocation? Location { get; private set; }
        public StairsContext? Context { get; private set; }
        public StairsDirection? Direction { get; private set; }

        IActorContext? IActor.Context => Context;

        IPortal? IPortal.Linked => Linked;


        public void Link(Stairs other)
        {
            if (Linked == other)
            {
                return;
            }

            Linked = other;
            other.Link(this);
        }

        public IInteraction<Player> GetDefaultInteraction(Player interactor)
        {
            return new UseStairsInteraction<Player>(interactor);
        }

        public void OnInteract(IInteraction<Player> interaction)
        {
            interaction.Perform(this);
        }

        void IPortal.Link(IPortal other) => Link((Stairs)other);

        public void Unlink()
        {
            Linked = null;
        }

        public void Initialize(StairsContext context, StairsArgs args)
        {
            Context = context;
            Location = args.Location;
        }
    }
}
