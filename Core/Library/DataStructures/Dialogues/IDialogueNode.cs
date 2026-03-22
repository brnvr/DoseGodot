namespace Dose.Core.Library.DataStructures.Dialogues
{
    public interface IDialogueNode
    {
        public bool Process();
        IDialogueNode? GetNextNode(IDialogue currentDialogue, int currentNodeIndex, out IDialogue? nextDialogue, out int nextNodeIndex);
    }
}