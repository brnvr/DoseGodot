using System;
using System.Collections.Generic;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration.Dialogues;
using Dose.Core.Scenes;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public class DialogueActionBuilder(Dictionary<string, IActionPerformer> dialogueActionMap, IActionExecuter actionExecuter, Action onFinish) : IDialogueNodesBuilder
    {
        public IActionExecuter ActionExecuter { get; } = actionExecuter;
        public Action OnFinish { get; } = onFinish;
        public Dictionary<string, IActionPerformer> ActionPerformerMap { get; } = dialogueActionMap;

        public IEnumerable<IDialogueNode> GetDialogueNodes(DialogueBuildContext context, string key)
        {
            var nodeConfig = (ExecuteNode)context.DialogueConfig.Nodes[key];
            var actionPerformer = ActionPerformerMap[nodeConfig.Args.Action];

            return [
                new DialogueAction<Dictionary<string, object?>>(
                    (args, onFinish) =>
                    {
                        if (ActionExecuter.ExecuteAction(actionPerformer, args))
                        {
                            onFinish();
                        }
                    },
                    nodeConfig.Args.Args,
                    OnFinish
                )
            ];
        }
    }
}