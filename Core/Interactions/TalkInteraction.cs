using Dose.Core.Library.Actors;
using Dose.Core.Library.Control;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Interactions
{
    public class TalkInteraction<TInteractor>(TInteractor interactor) : IInteraction<TInteractor>
    {
        public string DisplayName => "talk";

        public TInteractor Interactor { get; } = interactor;

        public void Perform(IActor subject)
        {
            if (Interactor is ITalker talker)
            {
                if (subject is ITalkable talkable)
                {
                    var dialogue = talkable.GetDialogue();

                    if (dialogue == null)
                    {
                        return;
                    }

                    var travesser = new DialogueTravesser(dialogue);

                    talker.OnDialogueBegin(travesser);
                }

                return;
            }

            GD.PushWarning("Interactor is not an ITalker. Doing nothing.");
        }
    }
}