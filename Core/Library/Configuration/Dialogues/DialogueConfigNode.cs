using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public abstract class DialogueConfigNode : YamlConfig
    {
        public abstract string Type { get; }
        public abstract string? NextNode { get; }
    } 

    public abstract class DialogueConfigNode<TArgs>  : DialogueConfigNode where TArgs : YamlConfig
    {
        public abstract TArgs Args { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { nameof(Type), Type },
                { nameof(Args), Args.AsDict() }
            };
        }
    }
}
