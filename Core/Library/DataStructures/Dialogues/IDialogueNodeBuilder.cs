using System.Collections.Generic;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public interface IDialogueNodesBuilder
    {
        IEnumerable<IDialogueNode> GetDialogueNodes(DialogueBuildContext context, string key);
    }
}