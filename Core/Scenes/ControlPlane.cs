using Dose.Core.Library.Control;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class ControlPlane : Node, IControlPlane
    {
        public LocationController? LocationController { get; private set; }
        public DialogueController? DialogueController { get; private set; }
        public InventoryController? InventoryController  { get; private set; }
        public InteractivityController? InteractivityController { get; private set; }
        public DoorNumberController DoorNumberController { get; } = new DoorNumberController();

        public ControlPlane Initialize(UI ui, Player player, LocationManager locationManager)
        {
            player.Initialize(this);

            LocationController = new LocationController(locationManager);
            DialogueController = new DialogueController(ui);
            InventoryController = new InventoryController(player.Inventory, ui, ui);
            InteractivityController = new InteractivityController(ui);

            return this;
        }

        public void Pause()
        {
            GetTree().Paused = true;
        }

        public void Unpause()
        {
            GetTree().Paused = false;
        }
    }
}
