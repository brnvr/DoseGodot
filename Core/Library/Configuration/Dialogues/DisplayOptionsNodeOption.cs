using System.Collections.Generic;

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
                { nameof(Text), Text },
                { nameof(Next), Next }
            };
        }
    }
}
