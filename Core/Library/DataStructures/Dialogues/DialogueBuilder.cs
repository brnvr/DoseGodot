using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Configuration.Dialogues;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public class DialogueBuilder(Dictionary<string, IDialogueNodesBuilder> nodeBuilderMap)
    {
        public Dictionary<string, IDialogueNodesBuilder> NodeBuilderMap { get; } = nodeBuilderMap;

        public IDialogue BuildDialogue(DialogueConfig config)
        {
            return BuildDialogue(config, config.StartAt);
        }

        public IDialogue BuildDialogue(DialogueConfig config, string action)
        {
            var nodeMap = CreateNodeMap(config);

            var dialogue = Dialogue.Start();
            var currentNodes = nodeMap[action];

            while (true)
            {
                foreach (var node in currentNodes.Nodes)
                {
                    dialogue = dialogue.WithNode(node);
                }

                if (currentNodes.NextAction == null)
                {
                    break;
                }

                currentNodes = nodeMap[currentNodes.NextAction];
            }

            return dialogue;
        }

        Dictionary<string, NodeMapEntry> CreateNodeMap(DialogueConfig config)
        {
            return config.Nodes.ToDictionary(c => c.Key, c =>
            {
                var builder = NodeBuilderMap[c.Value.Action];
                var context = new DialogueBuildContext(this, config);
                var nodes = builder.GetDialogueNodes(context, c.Key);
                var nodeConfig = config.Nodes[c.Key];

                return new NodeMapEntry(nodes, nodeConfig.NextAction);
            });
        }

        readonly struct NodeMapEntry(IEnumerable<IDialogueNode> nodes, string? nextAction)
        {
            public IEnumerable<IDialogueNode> Nodes { get; } = nodes;
            public string? NextAction { get; } = nextAction;
        }
    }
}