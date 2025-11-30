using Dose.Core.Library.Actors;
using Dose.Core.Library.Interactivity;

namespace Dose.Core.Interactions
{
    public class ToggleInteraction<TInteractor, TState>(TInteractor interactor) : IInteraction<TInteractor> where TState : struct
    {
        IToggleable<TState>? _currentSubject;

        public string DisplayName { get => GetDisplayName(); }
        public TInteractor Interactor { get; } = interactor;

        public virtual void Perform(IActor subject)
        {
            if (subject is IToggleable<TState> toggleable)
            {
                _currentSubject = toggleable;

                toggleable.Toggle();
            }
        }

        string GetDisplayName()
        {
            if (_currentSubject is null)
            {
                return "";
            }

            if (_currentSubject.DisplayNameForState.TryGetValue(_currentSubject.State, out string? name))
            {
                return name ?? "";
            }

            return "";
        }
    }
}