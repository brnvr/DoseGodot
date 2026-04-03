using Dose.Core.Library.UI;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public class DialogueLine(string text) : IDialogueNode, IUIComponent
    {
        public string Text { get; } = text;

        public int? GetNextNodeIndex(IDialogue currentDialogue, int currentNodeIndex, out IDialogue? nextDialogue)
        {
            nextDialogue = currentDialogue;

            if (currentNodeIndex >= currentDialogue.GetNodeCount() - 1)
            {
                return null;
            }

            return currentNodeIndex + 1;
        }

        public void OnUIEvent(IUI ui)
        {
            ui.SetLabelValue("DialogueCaption", Text);
            ui.SetLabelVisible("NameCaption", false);
        }

        public void Process()
        {
        }
    }
}