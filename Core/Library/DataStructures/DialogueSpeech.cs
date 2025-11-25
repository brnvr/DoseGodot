using System;
using Dose.Core.Library.UI;

namespace Dose.Core.Library.DataStructures
{
    public class DialogueSpeech(string text) : IDialogueNode, IUIComponent
    {
        public string Text { get; } = text;

        public IDialogueNode? GetNextNode(IDialogue currentDialogue, int currentNodeIndex, out IDialogue? nextDialogue, out int nextNodeIndex)
        {
            nextDialogue = currentDialogue;

            if (currentNodeIndex >= currentDialogue.GetNodeCount() - 1)
            {
                nextNodeIndex = currentNodeIndex;

                return null;
            }

            nextNodeIndex = currentNodeIndex + 1;

            return currentDialogue.GetNode(nextNodeIndex);
        }

        public void OnUIEvent(IUI ui)
        {
            ui.SetCaption(Text);
        }

        public bool Process()
        {
            return true;
        }
    }
}