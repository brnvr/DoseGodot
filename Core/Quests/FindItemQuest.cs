using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.Quests;
using Dose.Core.QuestArgs;
using Godot;

namespace Dose.Core.Quests
{
    public class FindItemQuest(ItemContext itemContext) : Quest<FindItemQuestArgs>
    {
        public ItemContext ItemContext { get; } = itemContext;
        public INotificator? Notificator { get; init; }

        public override bool Equals(Quest<FindItemQuestArgs>? other)
        {
            if (other is FindItemQuest findItemQuest)
            {
                return ItemContext == findItemQuest.ItemContext;
            }

            return false;
        }

        protected override bool HasCompleted(FindItemQuestArgs args)
        {
            return ItemContext == args.ItemContext;
        }

        protected override void OnComplete(FindItemQuestArgs args)
        {
            Notificator?.Notify($"Completed quest: Find {ItemContext.DisplayName}", 2f);
        }
    }
}