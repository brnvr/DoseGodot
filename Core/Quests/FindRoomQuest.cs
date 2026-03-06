using Dose.Core.Library.Quests;
using Dose.Core.QuestArgs;
using Godot;

namespace Dose.Core.Quests
{
    public class FindRoomQuest(int roomNumber) : Quest<RoomQuestArgs>
    {
        public int RoomNumber { get; } = roomNumber;

        public override bool Equals(Quest<RoomQuestArgs>? other)
        {
            if (other is FindRoomQuest findRoomQuest)
            {
                return RoomNumber == findRoomQuest.RoomNumber;
            }

            return false;
        }

        protected override bool HasCompleted(RoomQuestArgs args)
        {
            return RoomNumber == args.RoomNumber;
        }

        protected override void OnComplete(RoomQuestArgs args)
        {
            GD.Print($"Completed quest: Find room {RoomNumber}");
        }
    }
}