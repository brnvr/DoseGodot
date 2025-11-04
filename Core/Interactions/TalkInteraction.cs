using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Godot;

namespace Dose.Core.Interactions
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