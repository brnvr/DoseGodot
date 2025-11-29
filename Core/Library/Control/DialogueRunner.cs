using System;
using Dose.Core.Library.DataStructures;
using Godot;

namespace Dose.Core.Library.Control
{
    public class DialogueRunner
    {
        IDialogue _currentDialogue;
        int _currentNodeIndex;

        public IDialogue Dialogue { get; }
        public EventHandler<NodeSetEventArgs>? NodeSet { get; set; }

        public DialogueRunner(IDialogue dialogue, EventHandler<NodeSetEventArgs>? nodeSet)
        {
            _currentDialogue = dialogue;

            Dialogue = dialogue;
            NodeSet = nodeSet;

            NodeSet?.Invoke(this, new NodeSetEventArgs(GetCurrentNode()));
        }

        public void Reset()
        {
            _currentDialogue = Dialogue;
            _currentNodeIndex = 0;

            NodeSet?.Invoke(this, new NodeSetEventArgs(GetCurrentNode()));
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

            NodeSet?.Invoke(this, new NodeSetEventArgs(GetCurrentNode()));

            return true;
        }
    }

    public class DialogueNode
    {
    }
}