using System;
using Dose.Core.Library.DataStructures;
using Godot;

namespace Dose.Core.Library.Control
{
    public class DialogueTravesser
    {
        IDialogue _currentDialogue;
        int _currentNodeIndex;

        public IDialogue Dialogue { get; }
        public Action<IDialogueNode>? OnNodeSet { get; }

        public DialogueTravesser(IDialogue dialogue, Action<IDialogueNode>? onNodeSet)
        {
            _currentDialogue = dialogue;

            Dialogue = dialogue;
            OnNodeSet = onNodeSet;

            OnNodeSet?.Invoke(GetCurrentNode());
        }

        public void Reset()
        {
            _currentDialogue = Dialogue;
            _currentNodeIndex = 0;

            OnNodeSet?.Invoke(GetCurrentNode());
        }

        public IDialogueNode GetCurrentNode()
        {
            return _currentDialogue.GetNode(_currentNodeIndex);
        }

        public bool GoToNextNode()
        {
            var currentNode = GetCurrentNode();

            var nextNode = currentNode.GetNextNode(_currentDialogue, _currentNodeIndex, out var nextDialogue, out var nextNodeIndex);

            if (nextNode == null)
            {
                return false;
            }

            if (nextDialogue == null)
            {
                GD.PushError("Next dialogue not defined.");

                return false;
            }

            _currentDialogue = nextDialogue;
            _currentNodeIndex = nextNodeIndex;

            OnNodeSet?.Invoke(nextNode);

            return true;
        }
    }

    public class DialogueNode
    {
    }
}