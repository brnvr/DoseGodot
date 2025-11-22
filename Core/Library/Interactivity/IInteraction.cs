using Dose.Core.Library.Common;

namespace Dose.Core.Library.Actors
{
    public interface IInteraction<TInteractor> : INameable
    {
        TInteractor Interactor { get; }

        void Perform(IActor subject);
    }
}