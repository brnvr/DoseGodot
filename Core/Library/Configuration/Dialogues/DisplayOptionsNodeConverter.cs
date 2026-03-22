using System;
using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class DisplayOptionsNodeConverter : YamlConfigConverter
    {
        public override DialogueConfigNode<DisplayOptionsNodeArgs> FromDict(Dictionary<string, object?> dict)
        {
            var optionsRaw = GetRequired<IEnumerable<object>>(dict, "options");

            var options = optionsRaw.Select(o => (Dictionary<string, object?>)o);

            return new DisplayOptionsNode
            {
                Args = new DisplayOptionsNodeArgs
                {
                    Options = options.Select(option => new DisplayOptionsNodeOption
                    {
                        Text = option["text"] as string ?? throw new FormatException($"Invalid text in {nameof(DisplayOptionsNode)}"),
                        Next = option["next"] as string ?? throw new FormatException($"Invalid next in {nameof(DisplayOptionsNode)}")
                    })
                }
            };
        }
    }
}