using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Configuration.Actors
{
    public class ItemActionConfig : YamlConfig
    {
        public required string Action { get; init; }
        public Dictionary<string, object?> Args { get; init; } = [];

        public override Dictionary<string, object?> AsDict()
        {
            var dict = new Dictionary<string, object?>
            {
                { "action", Action }
            };

            if (Args.Count > 0)
            {
                dict.Add("args", Args);
            }

            return dict;
        }
    }
}