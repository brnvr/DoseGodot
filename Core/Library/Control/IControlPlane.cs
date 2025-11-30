using Dose.Core.Library.Actors;
using Dose.Core.Library.DataStructures;

namespace Dose.Core.Library.Control
{
    public interface IControlPlane
    {
        LocationController? LocationController { get;}
        InteractivityController? InteractivityController {get; }
        InventoryController? InventoryController { get; }
        DialogueController? DialogueController  { get; }
        DoorNumberController? DoorNumberController { get; }
    }
}