using System.Linq;
using Dose.Core.Library.Quests;
using Dose.Core.Library.UI;

namespace Dose.Core.Library.Control
{
    public class QuestBookController(QuestBook questBook, IUI ui)
    {
        public IUI UI { get; } = ui;
        public QuestBook QuestBook { get; } = questBook;
        public string QuestBookLabelName { get; init; } = "QuestBook";

        public void OnQuestBookOpened()
        {
            var questLog = string.Join("\n", QuestBook.Select(q => q.Name));
            UI.SetLabelValue(QuestBookLabelName, questLog);
        }

        public void OnQuestBookClosed()
        {
            UI.SetLabelValue(QuestBookLabelName, "");
        }
    }
}