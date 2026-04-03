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

        public IDialogue BuildDialogue(DialogueConfig config, string type)
        {
            var nodeMap = CreateNodeMap(config);

            var dialogue = Dialogue.Start();
            var currentNodes = nodeMap[type];

            while (true)
            {
                foreach (var node in currentNodes.Nodes)
                {
                    dialogue = dialogue.WithNode(node);
                }

                if (currentNodes.NextNode == null)
                {
                    break;
                }

                currentNodes = nodeMap[currentNodes.NextNode];
            }

            return dialogue;
        }

        Dictionary<string, NodeMapEntry> CreateNodeMap(DialogueConfig config)
        {
            return config.Nodes.ToDictionary(c => c.Key, c =>
            {
                var builder = NodeBuilderMap[c.Value.Type];
                var context = new DialogueBuildContext(this, config);
                var nodes = builder.GetDialogueNodes(context, c.Key);
                var nodeConfig = config.Nodes[c.Key];

                return new NodeMapEntry(nodes, nodeConfig.NextNode);
            });
        }

        readonly struct NodeMapEntry(IEnumerable<IDialogueNode> nodes, string? nextNode)
        {
            public IEnumerable<IDialogueNode> Nodes { get; } = nodes;
            public string? NextNode { get; } = nextNode;
        }
    }
}