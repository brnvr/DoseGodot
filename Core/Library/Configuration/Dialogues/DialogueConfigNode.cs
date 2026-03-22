using System.Collections.Generic;
using Dose.Core.Library.Common;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public abstract class DialogueConfigNode : YamlConfig { } 

    public abstract class DialogueConfigNode<TArgs>  : DialogueConfigNode where TArgs : YamlConfig
    {
        public abstract string Action { get; }
        public abstract required TArgs Args { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { "action", Action },
                { "args", Args.AsDict() }
            };
        }
    }
}