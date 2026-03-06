using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Dose.Core.Library.Quests
{
    public class QuestBook : IEnumerable<Quest>
    {
        private readonly List<Quest> _quests = [];

        public void Add<TArgs>(Quest<TArgs> quest)
        {
            if (_quests.Contains(quest))
            {
                GD.Print($"Quest already exists: {quest}");
                return;
            }

            GD.Print("Quest added: " + quest);

            _quests.Add(quest);
        }

        public bool TestCompleted<TQuest, TArgs>(TArgs args) where TQuest : Quest<TArgs>
        {
            foreach (var quest in _quests.Where(q => q is Quest<TArgs>).Cast<Quest<TArgs>>())
            {
                if (quest.TestCompleted(args))
                {
                    _quests.Remove(quest);
                    return true;
                }
            }
            return false;
        }
    
        public IEnumerator<Quest> GetEnumerator()
        {
            return _quests.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}