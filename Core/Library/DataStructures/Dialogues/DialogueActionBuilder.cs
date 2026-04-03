using System.Collections.Generic;
using Dose.Core.Library.Configuration.Dialogues;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public class DialogueActionBuilder(Dictionary<string, DialogueAction> dialogueActionMap) : IDialogueNodesBuilder
    {
        public Dictionary<string, DialogueAction> DialogueActionMap { get; } = dialogueActionMap;

        public IEnumerable<IDialogueNode> GetDialogueNodes(DialogueBuildContext context, string key)
        {
            var nodeConfig = (ExecuteNode)context.DialogueConfig.Nodes[key];

            return [DialogueActionMap[nodeConfig.Args.Action]];
        }
    }
}