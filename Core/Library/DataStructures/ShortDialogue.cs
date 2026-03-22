using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.DataStructures
{
    public class ShortDialogue(string text) : IDialogue
    {
        readonly DialogueLine _line = new(text);

        public IDialogueNode GetNode(int index)
        {
            return _line;
        }

        public int GetNodeCount()
        {
            return 1;
        }
    }
}