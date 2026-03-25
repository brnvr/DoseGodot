using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.DataStructures.Dialogues;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class DialogueLinesBuilder : IDialogueNodesBuilder
    {
        public IEnumerable<IDialogueNode> GetDialogueNodes(DialogueBuilder dialogueBuilder, string key)
        {
            var nodeConfig = dialogueBuilder.DialogueConfig.Nodes[key];
            return ((SpeakNode)nodeConfig).Args.Lines.Select(line => new DialogueLine(line));
        }
    }
}