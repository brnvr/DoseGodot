using Dose.Core.Library.Configuration;

namespace Dose.Core.Library.Common
{
    public interface IObjectFromConfigBuilder<T>
    {
        T Build(YamlConfig config);
    }
}