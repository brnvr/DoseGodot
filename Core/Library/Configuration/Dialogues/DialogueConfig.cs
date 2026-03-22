using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class DialogueConfig : YamlConfig
    {
        public required string StartAt { get; init; }
        public required Dictionary<string, DialogueConfigNode> Nodes { get; init; }

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