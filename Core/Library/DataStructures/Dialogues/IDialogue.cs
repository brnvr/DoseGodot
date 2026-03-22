namespace Dose.Core.Library.DataStructures.Dialogues
{
    public interface IDialogue
    {
        IDialogueNode GetNode(int index);
        public int GetNodeCount();
    }
}