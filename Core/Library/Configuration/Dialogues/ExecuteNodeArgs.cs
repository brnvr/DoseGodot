using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class ExecuteNodeArgs : YamlConfig
    {
        public string? Next { get; init; }
        public required string Action { get; init; }
        public required Dictionary<string, object?> Args;

        public override Dictionary<string, object?> AsDict()
        {
            var dict = new Dictionary<string, object?>
            {
                { nameof(Action), Action },
                { nameof(Args), Args }
            };

            if (Next != null)
            {
                dict[nameof(Next)] = Next;
            }

            return dict;
        }
    }
}
