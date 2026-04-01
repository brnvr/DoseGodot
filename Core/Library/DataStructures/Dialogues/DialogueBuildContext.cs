using Dose.Core.Library.Configuration.Dialogues;

namespace Dose.Core.Library.DataStructures.Dialogues
{
    public readonly struct DialogueBuildContext(DialogueBuilder builder, DialogueConfig config)
    {
        public DialogueBuilder DialogueBuilder { get; } = builder;
        public DialogueConfig DialogueConfig { get; } = config;
    }
}