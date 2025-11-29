using Dose.Core.Library.DataStructures;

namespace Dose.Core.Library.Events
{
    public class DialogueNodeSetEventArgs(IDialogueNode node) : System.EventArgs
    {
        public IDialogueNode Node { get; } = node;
    }
}