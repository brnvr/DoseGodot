using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Configuration.Dialogues;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public class DialogueLinesBuilder : IDialogueNodesBuilder
    {
        public IEnumerable<IDialogueNode> GetDialogueNodes(DialogueBuildContext context, string key)
        {
            var nodeConfig = context.DialogueConfig.Nodes[key];
            return ((SpeakNode)nodeConfig).Args.Lines.Select(line => new DialogueLine(line));
        }
    }
}