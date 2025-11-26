using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.DataStructures;

namespace Dose.Core.Library.Control
{
    public interface IControlPlane
    {
        void OnTargetChange(IActor? target);
        void OnDialogueChange(IDialogueNode node);
        void OnDialogueEnd();
    }
}