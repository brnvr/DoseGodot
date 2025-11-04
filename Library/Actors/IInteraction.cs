using Dose.Library.Common;
using Godot;

namespace Dose.Library.Actors
{
    public interface IInteraction<TInteractor> : INameable
    {
        TInteractor Interactor { get; }

        void Perform(IActor subject);
    }
}