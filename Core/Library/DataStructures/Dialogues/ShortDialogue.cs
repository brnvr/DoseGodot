namespace Dose.Core.Library.DataStructures.Dialogues
{
    public class ShortDialogue(string text) : IDialogue
    {
        readonly DialogueLine _line = new(text);

        public IDialogueNode GetNode(int index)
        {
            return _line;
        }

        public int GetNodeCount()
        {
            return 1;
        }
    }
}