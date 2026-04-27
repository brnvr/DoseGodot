using Dose.Core.Library.UI;

namespace Dose.Core.Library.Control
{
    public class QuestBookController(IUI ui)
    {
        public IUI UI { get; } = ui;
        public string QuestBookLabelName { get; init; } = "QuestBook";

        public void OnQuestBookOpened()
        {
            UI.SetLabelValue(QuestBookLabelName, "abcd");
        }

        public void OnQuestBookClosed()
        {
            UI.SetLabelValue(QuestBookLabelName, "");
        }
    }
}