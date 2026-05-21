using Dose.Core.Library.Interactivity;

namespace Dose.Core.Library.Actors
{
    public interface IActor
    {
        IActorContext? Context { get; }
    }

    public interface IActor<TInteractor> : IActor
    {
        void OnInteract(IInteraction<TInteractor> interaction);
        IInteraction<TInteractor> GetDefaultInteraction(TInteractor interactor);
    }
}