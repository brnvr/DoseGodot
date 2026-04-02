using System;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public class DialogueOption(string text, Func<IDialogue> getNext)
    {
        public string Text { get; } = text;
        public Func<IDialogue> GetNext { get; } = getNext;
    }
}