using Dose.Library.Actors;
using Dose.Library.DataStructures;

namespace Dose.Library.Control
{
    public interface IControlPlane
    {
        ActorRegistry? ActorRegistry { get; }
        void OnTargetChange(IActor? target);
    }
}