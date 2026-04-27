using System.Collections.Generic;

namespace Dose.Core.Library.Common
{
    public class Executable
    {
        public required IActionPerformer Performer { get; init; }
        public Dictionary<string, object?> Args { get; init; } = [];

        public virtual void Execute(object sender)
        {
            Performer.Perform(sender, Args);
        }
    }
}