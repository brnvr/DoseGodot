using Dose.Core.Library.Actors;
using Dose.Core.Library.Quests;
using Dose.Core.QuestArgs;
using Godot;

namespace Dose.Core.Quests
{
    public class TalkToNpc(NpcContext npcContext) : Quest<FindNpcQuestArgs>
    {
        public NpcContext NpcContext { get; } = npcContext;

        public override bool Equals(Quest<FindNpcQuestArgs>? other)
        {
            if (other is TalkToNpc talkToNpc)
            {
                return NpcContext == talkToNpc.NpcContext;
            }

            return false;
        }

        protected override bool HasCompleted(FindNpcQuestArgs args)
        {
            return NpcContext == args.NpcContext;
        }

        protected override void OnComplete(FindNpcQuestArgs args)
        {
            GD.Print($"Completed quest: Talk to {NpcContext.DisplayName}");
        }
    }
}