using Dose.Library.Actors;
using Dose.Library.Common;
using Dose.Library.DataStructures;

namespace Dose.Library.Control
{
    public interface IControlPlane
    {
        void OnTargetChange(IActor? target);
    }
}