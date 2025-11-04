using Dose.Core.Library.Common;
using Godot;

namespace Dose.Core.Library.Actors
{
    public interface IInteraction<TInteractor> : INameable
    {
        TInteractor Interactor { get; }

        void Perform(IActor subject);
    }
}