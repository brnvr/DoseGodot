using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;

namespace Dose.Core.Library.Configuration.Packs
{
    public class PackConfigBuilder : IObjectFromConfigBuilder<PackConfig>
    {
        public PackConfig Build(YamlConfig config) => (PackConfig)config;
    }
}
