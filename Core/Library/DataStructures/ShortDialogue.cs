using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.DataStructures
{
    public class ShortDialogue(string text) : IDialogue
    {
        readonly DialogueSpeech _speech = new(text);

        public IDialogueNode GetNode(int index)
        {
            return _speech;
        }

        public int GetNodeCount()
        {
            return 1;
        }
    }
}