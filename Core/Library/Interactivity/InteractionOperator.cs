using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;

namespace Dose.Core.Library.Interactivity
{
    public class InteractionOperator
    {
        public required string ActorIdentifier { get; init; }
        public required IActionPerformer Performer { get; init; }
        public Dictionary<string, object?> Args { get; init; } = [];

        public bool TryInteract(object sender, IActorContext actorContext)
        {
            if (actorContext.Identifier != ActorIdentifier) return false;

            var defaultArgs = new Dictionary<string, object?>
            {
                { "actorContext", actorContext }
            };

            new Executable {
                Performer = Performer,
                Args = defaultArgs.Concat(Args).ToDictionary()
            }.Execute(sender);

            return true;
        }
    }
}