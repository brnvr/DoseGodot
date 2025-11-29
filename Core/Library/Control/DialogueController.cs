using Dose.Core.Library.DataStructures;
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
            UI?.SetLabelValue("DialogueCaption", "");
            UI?.SetLabelVisible("NameCaption", true);
        }
    }
}