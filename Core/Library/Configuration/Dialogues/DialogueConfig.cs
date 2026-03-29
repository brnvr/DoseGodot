using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class DialogueConfig : YamlConfig
    {
        public required string StartAt { get; init; }
        public required Dictionary<string, DialogueConfigNode> Nodes { get; init; }
        public DialogueConfigNode FirstNode => Nodes[StartAt];

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
              { "startAt", StartAt },
              { "nodes", Nodes.ToDictionary(n => n.Key, n => n.Value.AsDict()) }  
            };
        }
    }
}