using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Control;
using Dose.Core.Library.UI;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public class DialogueOptions(params DialogueOption[] options) : IDialogueNode, IUIComponent
    {
        IUI? _currentUI;

        public IEnumerable<DialogueOption> Options { get; } = options;
        public DialogueOption? Selected { get; set; }

        public int? GetNextNodeIndex(IDialogue currentDialogue, int currentNodeIndex, out IDialogue? nextDialogue)
        {
            if (Selected == null)
            {
                nextDialogue = currentDialogue;
                return currentNodeIndex;
            }

            if (currentNodeIndex >= currentDialogue.GetNodeCount())
            {
                nextDialogue = null;
                return null;
            }

            nextDialogue = Selected.GetNext();

            _currentUI?.HideMenu();

            return 0;
        }

        public void OnUIEvent(IUI ui)
        {
            _currentUI = ui;

            var menuOptions = Options.Select(o => new MenuOption(
                text: o.Text,
                onHover: () => Selected = o,
                onExit: () => Selected = null,
                onButtonDown: () => Selected = o
            ));

            ui.HideDialogue();
            ui.DisplayMenu(menuOptions);
        }

        public void Process()
        {
        }
    }
}
