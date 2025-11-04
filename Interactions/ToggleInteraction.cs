using Dose.Library.Actors;
using Dose.Library.Common;
using Godot;

namespace Dose.Interactions
{
    public class ToggleInteraction<TInteractor>(TInteractor interactor) : IInteraction<TInteractor>
    {
        public string DisplayName => "open";

        public TInteractor Interactor { get; } = interactor;

        public void Perform(IActor subject)
        {
            if (subject is IToggleable toggleable)
            {
                toggleable.Toggle();
            }
        }
    }
}