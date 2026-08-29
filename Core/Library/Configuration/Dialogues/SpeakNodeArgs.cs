using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class SpeakNodeArgs : YamlConfig
    {
        public required IEnumerable<string> Lines { get; init; }
        public string? Next { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            var dict = new Dictionary<string, object?>
            {
                { nameof(Lines), Lines }
            };

            if (Next != null)
            {
                dict.Add(nameof(Next), Next);
            }

            return dict;
        }
    }
}
