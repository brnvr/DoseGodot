using Dose.Core.Library.DataStructures.Dialogues;

namespace Dose.Core.Library.Events
{
    public class DialogueNodeSetEventArgs(IDialogueNode node) : System.EventArgs
    {
        public IDialogueNode Node { get; } = node;
    }
}