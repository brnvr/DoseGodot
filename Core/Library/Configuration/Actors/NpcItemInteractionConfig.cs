using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Actors
{
    public class NpcItemInteractionConfig : ItemActionConfig
    {
        public required string Item { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            var dict = base.AsDict();

            dict[nameof(Item)] = Item;

            return dict;
        }
    }
}
