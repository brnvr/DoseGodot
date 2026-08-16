using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Numbers;

namespace Dose.Core.Library.Configuration.Actors
{
    public class ItemConfigConverter : YamlConfigConverter
    {
        public override ItemConfig FromDict(Dictionary<string, object?> dict)
        {
            var identifier = GetRequired<string>(dict, nameof(ItemConfig.Identifier));
            var texturePath = GetRequired<string>(dict, nameof(ItemConfig.TexturePath));
            var scale = new ScaleFactor(GetOptionalFloat(dict, nameof(ItemConfig.Scale), 1f));

            return new ItemConfig
            {
                Identifier = identifier,
                DisplayName = GetOptional<string>(dict, nameof(ItemConfig.DisplayName)) ?? identifier,
                TexturePath = texturePath,
                EquipedTexturePath = GetOptional<string>(dict, nameof(ItemConfig.EquipedTexturePath)) ?? texturePath,
                Rarity = GetRequired<string>(dict, nameof(ItemConfig.Rarity)),
                PackId = GetRequired<string>(dict, nameof(ItemConfig.PackId)),
                MaxQuantity = GetOptional<int?>(dict, nameof(ItemConfig.MaxQuantity)),
                Execute = GetOptional<List<object>>(dict, nameof(ItemConfig.Execute), []).Select(o => {
                    var dict = (Dictionary<object, object?>)o;
                    var action = dict[nameof(ItemActionConfig.Action)] ?? "";
                    var args = (Dictionary<object, object?>?)dict[nameof(ItemActionConfig.Args)] ?? [];

                    return new ItemActionConfig
                    {
                        Action = (string)action,
                        Args = args is null ? [] : args.ToDictionary(kvp => (string)kvp.Key, kvp => kvp.Value)
                    };
                }),
                InteractWith = GetOptional<List<object>>(dict, nameof(ItemConfig.InteractWith), []).Select(o =>
                {
                    var dict = (Dictionary<object, object?>)o;
                    var actor = dict[nameof(ItemInteractionConfig.Actor)] ?? "";
                    var action = dict[nameof(ItemActionConfig.Action)] ?? "";
                    var args = (Dictionary<object, object?>?)dict[nameof(ItemActionConfig.Args)] ?? [];

                    return new ItemInteractionConfig
                    {
                        Actor = (string)actor,
                        Action = (string)action,
                        Args = args is null ? [] : args.ToDictionary(kvp => (string)kvp.Key, kvp => kvp.Value)
                    };
                }),
                KeepAfterUse = GetOptional<bool?>(dict, nameof(ItemConfig.KeepAfterUse), false) ?? false,
                Scale = scale
            };
        }
    }
}
