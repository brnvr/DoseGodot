using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Configuration.Actors
{
    public class ItemConfigConverter : YamlConfigConverter
    {
        public override ItemConfig FromDict(Dictionary<string, object?> dict)
        {
            return new ItemConfig
            {
                DisplayName = GetRequired<string>(dict, "displayName"),
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
                })
            };
        }
    }
}