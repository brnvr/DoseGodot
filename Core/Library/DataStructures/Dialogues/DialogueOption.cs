using System;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public class DialogueOption(string text, Func<IDialogue, IDialogue> getNext)
    {
        public string Text { get; } = text;
        public Func<IDialogue, IDialogue> GetNext { get; } = getNext;
    }
}