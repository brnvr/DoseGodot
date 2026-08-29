using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;

namespace Dose.Core.Library.Interactivity
{
    public enum ItemInteractionResult
    {
        NotApplicable,
        Accepted,
        Rejected
    }

    public class ItemInteractionOperator
    {
        public required string ItemIdentifier { get; init; }
        public required IActionPerformer Performer { get; init; }
        public Dictionary<string, object?> Args { get; init; } = [];

        public ItemInteractionResult Interact(ItemContext item, IActor actor)
        {
            if (item.Identifier != ItemIdentifier)
            {
                return ItemInteractionResult.NotApplicable;
            }

            var defaultArgs = new Dictionary<string, object?>
            {
                { "actor", actor },
                { "actorContext", actor.Context }
            };

            var accepted = Performer.Perform(
                item,
                defaultArgs.Concat(Args).ToDictionary()
            );

            return accepted
                ? ItemInteractionResult.Accepted
                : ItemInteractionResult.Rejected;
        }
    }
}
