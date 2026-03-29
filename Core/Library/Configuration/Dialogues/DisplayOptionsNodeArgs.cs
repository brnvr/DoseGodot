using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class DisplayOptionsNodeArgs : YamlConfig
    {
        public required IEnumerable<DisplayOptionsNodeOption> Options { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { "options", Options.Select(o => o.AsDict()) }
            };
        }
    }
}