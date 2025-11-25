namespace Dose.Core.Library.DataStructures
{
    public interface IDialogue
    {
        IDialogueNode GetNode(int index);
        public int GetNodeCount();
    }
}