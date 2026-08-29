using System;
using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class DisplayOptionsNodeConverter() : YamlConfigConverter
    {
        public override DisplayOptionsNode FromDict(Dictionary<string, object?> dict)
        {
            var optionsRaw = GetRequired<IEnumerable<object>>(dict, nameof(DisplayOptionsNodeArgs.Options));

            var options = optionsRaw.Select(o => (Dictionary<string, object?>)o);

            return new DisplayOptionsNode
            {
                Args = new DisplayOptionsNodeArgs
                {
                    Options = options.Select(option => new DisplayOptionsNodeOption
                    {
                        Text = option[nameof(DisplayOptionsNodeOption.Text)] as string ?? throw new FormatException($"Invalid text in {nameof(DisplayOptionsNode)}"),
                        Next = option[nameof(DisplayOptionsNodeOption.Next)] as string ?? throw new FormatException($"Invalid next in {nameof(DisplayOptionsNode)}")
                    })
                }
            };
        }

        
    }
}
