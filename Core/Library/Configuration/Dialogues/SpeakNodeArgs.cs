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
                { "lines", Lines }
            };

            if (Next != null)
            {
                dict.Add("next", Next);
            }

            return dict;
        }
    }
}