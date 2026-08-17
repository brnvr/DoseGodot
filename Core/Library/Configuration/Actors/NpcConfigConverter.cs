using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Numbers;

namespace Dose.Core.Library.Configuration.Actors
{
    public class NpcConfigConverter : YamlConfigConverter
    {
        public override NpcConfig FromDict(Dictionary<string, object?> dict)
        {
            var identifier = GetRequired<string>(dict, nameof(NpcConfig.Identifier));
            var scale = new ScaleFactor(GetOptionalFloat(dict, nameof(NpcConfig.Scale), 1f));

            return new NpcConfig
            {
                Identifier = identifier,
                DisplayName = GetOptional<string>(dict, nameof(NpcConfig.DisplayName)) ?? identifier,
                Rarity = GetRequired<string>(dict, nameof(NpcConfig.Rarity)),
                PackId = GetRequired<string>(dict, nameof(NpcConfig.PackId)),
                TexturePath = GetRequired<string>(dict, nameof(NpcConfig.TexturePath)),
                Voice = GetRequired<string>(dict, nameof(NpcConfig.Voice)),
                DialoguesDir = GetRequired<string>(dict, nameof(NpcConfig.DialoguesDir)),
                AvailableDialogues = GetRequired<List<object>>(dict, nameof(NpcConfig.AvailableDialogues)).Select(o => (string)o),
                InteractWith = GetOptional<List<object>>(dict, nameof(NpcConfig.InteractWith), []).Select(o =>
                {
                    var interactionDict = (Dictionary<object, object?>)o;
                    var item = interactionDict[nameof(NpcItemInteractionConfig.Item)] ?? "";
                    var action = interactionDict[nameof(ItemActionConfig.Action)] ?? "";
                    var args = (Dictionary<object, object?>?)interactionDict[nameof(ItemActionConfig.Args)] ?? [];

                    return new NpcItemInteractionConfig
                    {
                        Item = (string)item,
                        Action = (string)action,
                        Args = args.ToDictionary(kvp => (string)kvp.Key, kvp => kvp.Value)
                    };
                }),
                Scale = scale
            };
        }
    }
}
