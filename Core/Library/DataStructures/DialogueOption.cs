using System;

namespace Dose.Core.Library.DataStructures
{
    public class DialogueOption(string text, Func<Dialogue, Dialogue> getNext)
    {
        public string Text { get; } = text;
        public Func<Dialogue, Dialogue> GetNext { get; } = getNext;
    }
}