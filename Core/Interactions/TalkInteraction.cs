using System;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.Control;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Interactions
{
    public class TalkInteraction<TInteractor>(
        TInteractor interactor,
        IDialogue? dialogue = null
    ) : IInteraction<TInteractor>
    {
        public string DisplayName => "talk";

        public TInteractor Interactor { get; } = interactor;
        public IDialogue? Dialogue { get; } = dialogue;

        public void Perform(IActor subject)
        {
            if (Interactor is ITalker talker)
            {
                if (subject is ITalkable talkable)
                {
                    var canTalk = true;
                    var selectedDialogue = Dialogue ?? talkable.GetDialogue();

                    if (selectedDialogue == null)
                    {
                        return;
                    }

                    var dialogueId = Guid.NewGuid().ToString();

                    var dialogueRunner = new DialogueRunner(selectedDialogue, (sender, e) =>
                    {
                        if (subject is ISpeakable speakable)
                        {
                            if (e.Node is DialogueLine line)
                            {
                                if (sender is DialogueRunner runner && runner.Dialogue is ShortDialogue)
                                {
                                    if (speakable.Speaker?.IsSpeaking == true)
                                    {
                                        canTalk = false;
                                    }
                                }
                                
                                if (canTalk)
                                {
                                    speakable.Speaker?.Speak(line.Text);
                                }
                            } else
                            {
                                speakable.Speaker?.Stop();
                            }  
                        }
                    });

                    if (canTalk)
                    {
                        talker.OnDialogueBegin(dialogueRunner);
                    }
                }

                return;
            }

            GD.PushWarning("Interactor is not an ITalker. Doing nothing.");
        }
    }
}
