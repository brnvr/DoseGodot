using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Props
{
    public class PropConfig : YamlConfig
    {
        public string? TexturePath { get; init; }
        public string? ModelPath { get; init; }
        public bool IsBillboard => TexturePath != null;

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { "texturePath", TexturePath },
                { "modelPath", ModelPath }
            };
        }
    }
}
