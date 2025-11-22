using Dose.Core.Library.Control;
using Dose.Core.Library.UI;

namespace Dose.Core.Library.DataStructures
{
    public interface IDialogueNode
    {
        public bool Process();
        IDialogueNode? GetNextNode(Dialogue currentDialogue, int currentNodeIndex, out Dialogue? nextDialogue, out int nextNodeIndex);
    }
}