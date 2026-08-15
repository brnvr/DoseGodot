using System;
using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Props
{
    public class PropConfigConverter : YamlConfigConverter
    {
        public override PropConfig FromDict(Dictionary<string, object?> dict)
        {
            var texturePath = GetOptional<string>(dict, "texturePath");
            var modelPath = GetOptional<string>(dict, "modelPath");
            var hasTexturePath = !string.IsNullOrWhiteSpace(texturePath);
            var hasModelPath = !string.IsNullOrWhiteSpace(modelPath);

            if (hasTexturePath == hasModelPath)
            {
                throw new ArgumentException(
                    "A prop config must define exactly one of texturePath or modelPath."
                );
            }

            return new PropConfig
            {
                TexturePath = hasTexturePath ? texturePath : null,
                ModelPath = hasModelPath ? modelPath : null
            };
        }
    }
}
