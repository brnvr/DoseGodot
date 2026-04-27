using Dose.Core.Library.Actors;
using Dose.Core.Library.Quests;
using Dose.Core.QuestArgs;
using Godot;

namespace Dose.Core.Quests
{
    public class TalkToNpcQuest(NpcContext npcContext) : Quest<TalkToNpcQuestArgs>
    {
        public override string Name => $"Talk to {NpcContext.DisplayName}";
        public NpcContext NpcContext { get; } = npcContext;

        public override bool Equals(Quest<TalkToNpcQuestArgs>? other)
        {
            if (other is TalkToNpcQuest talkToNpcQuest)
            {
                return NpcContext == talkToNpcQuest.NpcContext;
            }

            return false;
        }

        protected override bool HasCompleted(TalkToNpcQuestArgs args)
        {
            return NpcContext == args.NpcContext;
        }
    }
}