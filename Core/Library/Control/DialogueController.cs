using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.UI;

namespace Dose.Core.Library.Control
{
    public class DialogueController(IUI? ui)
    {
        public IUI? UI { get; } = ui;
        public string DialogueCaptionLabelName { get; init; } = "DialogueCaption";
        public string NameCaptionLabelName { get; init; } = "NameCaption";

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
            UI?.SetLabelValue(DialogueCaptionLabelName, "");
            UI?.SetLabelVisible(NameCaptionLabelName, true);
        }
    }
}