using System;
using System.Collections.Generic;
using Godot;

namespace Dose.Core.Library.DataStructures
{
    public class DialogueOptions(params DialogueOption[] options) : IDialogueNode
    {
        public IEnumerable<DialogueOption> Options { get; } = options;
        public DialogueOption? Selected { get; set; }

        public IDialogueNode? GetNextNode(Dialogue currentDialogue, int currentNodeIndex, out Dialogue? nextDialogue, out int nextNodeIndex)
        {
            if (Selected == null)
            {
                nextDialogue = currentDialogue;
                nextNodeIndex = currentNodeIndex;

                return this;
            }

            if (currentNodeIndex >= currentDialogue.GetNodeCount())
            {
                nextDialogue = null;
                nextNodeIndex = currentNodeIndex;

                return null;
            }

            nextDialogue = Dialogue.Start();
            nextNodeIndex = 0;

            return Selected.GetNext(nextDialogue).GetNode(0);
        }

        public bool Process()
        {
            return true;
        }
    }
}