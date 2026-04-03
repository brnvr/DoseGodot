using System.Collections.Generic;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Dialogues;
using Dose.Core.Library.DataStructures.Dialogues;

namespace Dose.Core.Library.Actors
{
    public readonly struct NpcContextBuilderAction(string key, YamlConfigConverter dialogueNodeConverter, IDialogueNodesBuilder dialogueNodeBuilder)
    {
        public string Key { get; } = key;
        public YamlConfigConverter DialogueNodeConverter { get; } = dialogueNodeConverter;
        public IDialogueNodesBuilder DialogueNodeBuilder { get; } = dialogueNodeBuilder;
    }
}