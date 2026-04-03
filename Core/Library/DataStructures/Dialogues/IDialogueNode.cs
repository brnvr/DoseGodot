namespace Dose.Core.Library.DataStructures.Dialogues
{
    public interface IDialogueNode
    {
        public void Process();
        int? GetNextNodeIndex(IDialogue currentDialogue, int currentNodeIndex, out IDialogue? nextDialogue);
    }
}