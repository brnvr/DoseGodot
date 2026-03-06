using Dose.Core.Library.Actors;
using Dose.Core.Library.Quests;
using Dose.Core.QuestArgs;
using Godot;

namespace Dose.Core.Quests
{
    public class FindItemQuest(ItemContext itemContext) : Quest<ItemQuestArgs>
    {
        public ItemContext ItemContext { get; } = itemContext;

        public override bool Equals(Quest<ItemQuestArgs>? other)
        {
            if (other is FindItemQuest findItemQuest)
            {
                return ItemContext == findItemQuest.ItemContext;
            }

            return false;
        }

        protected override bool HasCompleted(ItemQuestArgs args)
        {
            return ItemContext == args.ItemContext;
        }

        protected override void OnComplete(ItemQuestArgs args)
        {
            GD.Print($"Completed quest: Find {ItemContext.DisplayName}");
        }
    }
}