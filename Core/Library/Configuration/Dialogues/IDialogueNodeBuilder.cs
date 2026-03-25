using System.Collections.Generic;
using Dose.Core.Library.DataStructures.Dialogues;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public interface IDialogueNodesBuilder
    {
        IEnumerable<IDialogueNode> GetDialogueNodes(DialogueBuilder dialogueBuilder, string key);
    }
}