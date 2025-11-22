using Dose.Core.Library.DataStructures;
using Godot;

namespace Dose.Core.Library.Control
{
    public class DialogueTravesser(Dialogue dialogue)
    {
        Dialogue _currentDialogue = dialogue;
        int _currentNodeIndex;

        public Dialogue Dialogue { get; } = dialogue;

        public void Reset()
        {
            _currentDialogue = Dialogue;
            _currentNodeIndex = 0;
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
                Reset();

                return false;
            }

            if (nextDialogue == null)
            {
                GD.PushError("Next dialogue not defined.");
                Reset();

                return false;
            }

            _currentDialogue = nextDialogue;
            _currentNodeIndex = nextNodeIndex;

            return true;
        }
    }
}