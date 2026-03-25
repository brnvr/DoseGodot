using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.DataStructures.Dialogues;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class DialogueOptionsBuilder : IDialogueNodesBuilder
    {
        public IEnumerable<IDialogueNode> GetDialogueNodes(DialogueBuilder dialogueBuilder, string key)
        {
            var nodeConfig = dialogueBuilder.DialogueConfig.Nodes[key];

            var options = ((DisplayOptionsNode)nodeConfig).Args.Options.Select(option => {
                return new DialogueOption(option.Text, current =>
                {
                    var next = dialogueBuilder.DialogueConfig.Nodes[option.Next];

                    var dialogue = dialogueBuilder.BuildDialogue(next.Action);

                    return dialogue;
                });
            });

            return new List<DialogueOptions>
            {
                new([.. options])
            };
        }
    }
}