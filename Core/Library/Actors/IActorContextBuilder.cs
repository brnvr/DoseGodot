using Dose.Core.Library.Configuration;

namespace Dose.Core.Library.Actors
{
    public interface IActorContextBuilder
    {
        IActorContext BuildContext(YamlConfig config);
    }
}