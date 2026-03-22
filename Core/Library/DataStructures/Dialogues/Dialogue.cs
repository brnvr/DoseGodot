using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public class Dialogue : IDialogue
    {
        readonly List<IDialogueNode> _nodes = [];

        public static Dialogue Start()
        {
            return new Dialogue([]);
        }

        Dialogue(IEnumerable<IDialogueNode> nodes)
        {
            _nodes = [.. nodes];
        }

        public Dialogue WithNode(IDialogueNode node)
        {
            return new Dialogue(_nodes.Concat([node]));
        }

        public IDialogueNode GetNode(int index)
        {
            return _nodes[index];
        }

        public int GetNodeCount()
        {
            return _nodes.Count;
        }
    }
}