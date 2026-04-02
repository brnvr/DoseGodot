using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Configuration.Dialogues;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public class DialogueOptionsBuilder : IDialogueNodesBuilder
    {
        public IEnumerable<IDialogueNode> GetDialogueNodes(DialogueBuildContext context, string key)
        {
            var nodeConfig = context.DialogueConfig.Nodes[key];

            var options = ((DisplayOptionsNode)nodeConfig).Args.Options.Select(option => {
                return new DialogueOption(option.Text, () =>
                {
                    var dialogue = context.DialogueBuilder.BuildDialogue(context.DialogueConfig, option.Next);

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