using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Configuration.Actors
{
    public class ItemConfigConverter : YamlConfigConverter
    {
        public override ItemConfig FromDict(Dictionary<string, object?> dict)
        {
            var identifier = GetRequired<string>(dict, "identifier");

            return new ItemConfig
            {
                Identifier = identifier,
                DisplayName = GetOptional<string>(dict, "displayName") ?? identifier,
                TexturePath = GetRequired<string>(dict, "texturePath"),
                MaxQuantity = GetOptional<int?>(dict, "maxQuantity"),
                Execute = GetOptional<List<object>>(dict, "execute", []).Select(o => {
                    var dict = (Dictionary<string, object?>)o;
                    var action = dict["action"] ?? "";
                    var args = dict["args"];

                    return new ItemActionConfig
                    {
                        Action = (string)action,
                        Args = args is null ? [] : (Dictionary<string, object?>)args
                    };
                }),
                InteractWith = GetOptional<List<object>>(dict, "interactWith", []).Select(o =>
                {
                    var dict = (Dictionary<string, object?>)o;
                    var actor = dict["actor"] ?? "";
                    var action = dict["action"] ?? "";
                    var args = dict["args"];

                    return new ItemInteractionConfig
                    {
                        Actor = (string)actor,
                        Action = (string)action,
                        Args = args is null ? [] : (Dictionary<string, object?>)args
                    };
                })
            };
        }
    }
}