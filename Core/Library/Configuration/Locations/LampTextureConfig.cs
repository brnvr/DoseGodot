using System.Collections.Generic;
using Dose.Core.Library.Numbers;

namespace Dose.Core.Library.Configuration.Locations
{
    public class LampTextureConfig : TextureConfig
    {
        public ScaleFactor Scale { get; init; } = 1f;

        public override Dictionary<string, object?> AsDict()
        {
            var dict = base.AsDict();
            dict.Add(nameof(Scale), Scale.Value);
            return dict;
        }
    }
}
