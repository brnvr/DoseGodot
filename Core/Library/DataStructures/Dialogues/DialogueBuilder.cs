using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Configuration.Dialogues;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public class DialogueBuilder(DialogueConfig dialogueConfig, Dictionary<string, IDialogueNodesBuilder> nodeBuilderMap)
    {
        public DialogueConfig DialogueConfig { get; } = dialogueConfig;
        public Dictionary<string, IDialogueNodesBuilder> NodeBuilderMap { get; } = nodeBuilderMap;

        public Dialogue BuildDialogue()
        {
            return BuildDialogue(DialogueConfig.StartAt);
        }

        public Dialogue BuildDialogue(string action)
        {
            var nodeMap = CreateNodeMap();

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

        Dictionary<string, NodeMapEntry> CreateNodeMap()
        {
            return DialogueConfig.Nodes.ToDictionary(c => c.Key, c =>
            {
                var builder = NodeBuilderMap[c.Value.Action];
                var nodes = builder.GetDialogueNodes(this, c.Key);
                var nodeConfig = DialogueConfig.Nodes[c.Key];

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