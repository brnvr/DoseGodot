using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.UI;

namespace Dose.Core.Library.Control
{
    public class DialogueController(IUI? ui)
    {
        public IUI? UI { get; } = ui;

        public void OnDialogueChange(IDialogueNode node)
        {
            if (UI == null)
            {
                return;
            }

            if (node is IUIComponent uiComponent)
            {
                uiComponent.OnUIEvent(UI);
            }
        }

        public void OnDialogueEnd()
        {
            UI?.HideDialogue();
            UI?.SetActorNameVisible(true);
        }
    }
}
