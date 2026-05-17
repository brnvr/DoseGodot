using Dose.Core.Library.Configuration;

namespace Dose.Core.Library.Common
{
    public interface IContextBuilder<TContext>
    {
        TContext BuildContext(YamlConfig config);
    }
}