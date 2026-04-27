namespace Dose.Core.Library.Control
{
    public interface IControlPlane
    {
        LocationController? LocationController { get;}
        InteractivityController? InteractivityController {get; }
        InventoryController? InventoryController { get; }
        DialogueController? DialogueController  { get; }
        DoorNumberController? DoorNumberController { get; }
        PauseController? PauseController { get; }
        QuestBookController? QuestBookController { get; }
    }
}