using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class ExecuteNodeArgs : YamlConfig
    {
        public string? Next { get; init; }
        public required string Action { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            var dict = new Dictionary<string, object?>
            {
                { "action", Action }
            };

            if (Next != null)
            {
                dict["next"] = Next;
            }

            return dict;
        }
    }
}