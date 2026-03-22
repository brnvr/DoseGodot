using System.Collections.Generic;
using Dose.Core.Library.Common;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class DisplayOptionsNodeOption : YamlConfig
    {
        public required string Text { get; init; }
        public required string Next { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { "text", Text },
                { "next", Next }
            };
        }
    }
}