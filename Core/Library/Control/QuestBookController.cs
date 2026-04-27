using Dose.Core.Library.UI;

namespace Dose.Core.Library.Control
{
    public class QuestBookController(IUI ui)
    {
        public IUI UI { get; } = ui;

        public void OnQuestBookOpened()
        {
            UI.SetLabelValue("QuestBook", "abcd");
        }

        public void OnQuestBookClosed()
        {
            UI.SetLabelValue("QuestBook", "");
        }
    }
}