using Dose.Core.Library.Quests;
using Dose.Core.QuestArgs;
using Godot;

namespace Dose.Core.Quests
{
    public class FindRoomQuest(int roomNumber) : Quest<FindRoomQuestArgs>
    {
        public override string Name => $"Find room {RoomNumber}";
        public int RoomNumber { get; } = roomNumber;

        public override bool Equals(Quest<FindRoomQuestArgs>? other)
        {
            if (other is FindRoomQuest findRoomQuest)
            {
                return RoomNumber == findRoomQuest.RoomNumber;
            }

            return false;
        }

        protected override bool HasCompleted(FindRoomQuestArgs args)
        {
            return RoomNumber == args.RoomNumber;
        }
    }
}