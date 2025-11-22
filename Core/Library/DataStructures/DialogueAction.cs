using System;

namespace Dose.Core.Library.DataStructures
{
    public class DialogueAction<TOther>(TOther other, Func<TOther, bool> Action) : IDialogueNode
    {
        public TOther Other { get; } = other;
        public Func<TOther, bool> Action { get; } = Action;

        public bool Process()
        {
            return Action(Other);
        }

        public IDialogueNode? GetNextNode(Dialogue currentDialogue, int currentNodeIndex, out Dialogue? nextDialogue, out int nextNodeIndex)
        {
            nextDialogue = currentDialogue;
            nextNodeIndex = currentNodeIndex + 1;

            return currentDialogue.GetNode(nextNodeIndex);
        }
    }
}