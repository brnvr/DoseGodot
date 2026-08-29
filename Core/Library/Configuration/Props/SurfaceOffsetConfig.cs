using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Props
{
    public class SurfaceOffsetConfig
    {
        public float Horizontal { get; init; }
        public float Vertical { get; init; }

        public Dictionary<string, object?> AsDict() => new()
        {
            { nameof(Horizontal), Horizontal },
            { nameof(Vertical), Vertical }
        };
    }
}
