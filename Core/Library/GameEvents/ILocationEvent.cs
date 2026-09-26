using Dose.Core.Scenes;

namespace Dose.Core.Library.GameEvents
{
    public interface ILocationEvent : IGameEvent<Location>
    {
        void Activate(Location location);
        void Deactivate(Location location);
    }
}
