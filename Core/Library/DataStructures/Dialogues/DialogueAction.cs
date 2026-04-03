using System;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public abstract class DialogueAction(Action onFinish) : IDialogueNode
    {
        public Action OnFinish { get; } = onFinish;
  
        public int? GetNextNodeIndex(IDialogue currentDialogue, int currentNodeIndex, out IDialogue? nextDialogue)
        {
            nextDialogue = currentDialogue;

            if (currentNodeIndex >= currentDialogue.GetNodeCount() - 1)
            {
                return null;
            }

            return currentNodeIndex + 1;
        }

        public abstract void Process();
    }

    public class DialogueAction<TArgs>(Action<TArgs, Action> action, TArgs args, Action onFinish) : DialogueAction(onFinish)
    {
        public TArgs Args { get; } = args;
        public Action<TArgs, Action> Action { get; } = action;


        public override void Process()
        {
            Action(Args, OnFinish);
        }
    }
}