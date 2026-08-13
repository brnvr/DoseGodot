using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Configuration.Actors
{
    public class ItemConfigConverter : YamlConfigConverter
    {
        public override ItemConfig FromDict(Dictionary<string, object?> dict)
        {
            var identifier = GetRequired<string>(dict, "identifier");
            var texturePath = GetRequired<string>(dict, "texturePath");

            return new ItemConfig
            {
                Identifier = identifier,
                DisplayName = GetOptional<string>(dict, "displayName") ?? identifier,
                TexturePath = texturePath,
                EquipedTexturePath = GetOptional<string>(dict, "equipedTexturePath") ?? texturePath,
                Rarity = GetRequired<string>(dict, "rarity"),
                MaxQuantity = GetOptional<int?>(dict, "maxQuantity"),
                Execute = GetOptional<List<object>>(dict, "execute", []).Select(o => {
                    var dict = (Dictionary<object, object?>)o;
                    var action = dict["action"] ?? "";
                    var args = (Dictionary<object, object?>?)dict["args"] ?? [];

                    return new ItemActionConfig
                    {
                        Action = (string)action,
                        Args = args is null ? [] : args.ToDictionary(kvp => (string)kvp.Key, kvp => kvp.Value)
                    };
                }),
                InteractWith = GetOptional<List<object>>(dict, "interactWith", []).Select(o =>
                {
                    var dict = (Dictionary<object, object?>)o;
                    var actor = dict["actor"] ?? "";
                    var action = dict["action"] ?? "";
                    var args = (Dictionary<object, object?>?)dict["args"] ?? [];

                    return new ItemInteractionConfig
                    {
                        Actor = (string)actor,
                        Action = (string)action,
                        Args = args is null ? [] : args.ToDictionary(kvp => (string)kvp.Key, kvp => kvp.Value)
                    };
                }),
                KeepAfterUse = GetOptional<bool?>(dict, "keepAfterUse", false) ?? false
            };
        }
    }
}