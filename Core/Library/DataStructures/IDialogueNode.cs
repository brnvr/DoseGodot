using Dose.Core.Library.Control;
using Dose.Core.Library.UI;

namespace Dose.Core.Library.DataStructures
{
    public interface IDialogueNode
    {
        public bool Process();
        IDialogueNode? GetNextNode(IDialogue currentDialogue, int currentNodeIndex, out IDialogue? nextDialogue, out int nextNodeIndex);
    }
}