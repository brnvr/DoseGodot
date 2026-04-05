using System.Collections.Generic;
using Dose.Core.Library.Common;
using Godot;

namespace Dose.Core.Library.Actors
{
    public class Executable
    {
        public required IActionPerformer Performer { get; init; }
        public Dictionary<string, object?> Args { get; init; } = [];

        public void Execute()
        {
            Performer.Perform(Args);
        }
    }
}