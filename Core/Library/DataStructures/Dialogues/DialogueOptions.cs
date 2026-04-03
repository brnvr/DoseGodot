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

        public IDialogueNode? GetNextNode(IDialogue currentDialogue, int currentNodeIndex, out IDialogue? nextDialogue, out int nextNodeIndex)
        {
            if (Selected == null)
            {
                nextDialogue = currentDialogue;
                nextNodeIndex = currentNodeIndex;

                return this;
            }

            if (currentNodeIndex >= currentDialogue.GetNodeCount())
            {
                nextDialogue = null;
                nextNodeIndex = currentNodeIndex;

                return null;
            }

            nextDialogue = Selected.GetNext();
            nextNodeIndex = 0;

            _currentUI?.HideMenu();

            return nextDialogue.GetNode(0);
        }

        public void OnUIEvent(IUI ui)
        {
            _currentUI = ui;

            var menuOptions = Options.Select(o => new MenuOption(text: o.Text, onExit: () => Selected = null, onHover: () => Selected = o));

            ui.DisplayMenu(menuOptions);
        }        public bool Process()
        {
            return true;
        }
    }
}