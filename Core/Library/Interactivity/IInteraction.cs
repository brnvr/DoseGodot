using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;

namespace Dose.Core.Library.Interactivity
{
    public interface IInteraction<TInteractor> : INameable
    {
        TInteractor Interactor { get; }

        void Perform(IActor subject);
    }
}