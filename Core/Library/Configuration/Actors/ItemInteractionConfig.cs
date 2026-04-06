using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Actors
{
    public class ItemInteractionConfig : ItemActionConfig
    {
        public required string Actor { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            var dict = base.AsDict();

            dict["actor"] = Actor;

            return dict;
        }
    }
}