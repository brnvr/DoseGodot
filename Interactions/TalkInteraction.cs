using Dose.Library.Actors;
using Dose.Library.Common;
using Godot;

namespace Dose.Interactions
{
    public class TalkInteraction<TInteractor>(TInteractor interactor) : IInteraction<TInteractor>
    {
        public string DisplayName => "talk";

        public TInteractor Interactor { get; } = interactor;

        public void Perform(IActor subject)
        {
            if (subject is ITalkable talkable)
            {
                talkable.Talk();
            }
        }
    }
}