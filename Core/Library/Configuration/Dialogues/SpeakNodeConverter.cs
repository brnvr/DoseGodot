using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class SpeakNodeConverter : YamlConfigConverter
    {
        public override SpeakNode FromDict(Dictionary<string, object?> dict)
        {
            return new SpeakNode
            {
                Args = new SpeakNodeArgs
                {
                    Lines = GetRequired<List<object>>(dict, "lines").Select(l => (string)l),
                    Next = GetOptional<string>(dict, "next")
                }
            };
        }
    }
}