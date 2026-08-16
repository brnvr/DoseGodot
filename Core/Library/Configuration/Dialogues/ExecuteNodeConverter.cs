using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class ExecuteNodeConverter : YamlConfigConverter
    {
        public override ExecuteNode FromDict(Dictionary<string, object?> dict)
        {
            return new ExecuteNode
            {
                Args = new ExecuteNodeArgs
                {
                    Action = GetRequired<string>(dict, nameof(ExecuteNodeArgs.Action)),
                    Next = GetOptional<string>(dict, nameof(ExecuteNodeArgs.Next)),
                    Args = GetOptional<Dictionary<string, object?>>(dict, nameof(ExecuteNodeArgs.Args)) ?? []
                }
            };
        }
    }
}
