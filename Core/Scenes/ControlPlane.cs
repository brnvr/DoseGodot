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
            LocationController = new LocationController(locationManager);
            DialogueController = new DialogueController(ui);
            InventoryController = new InventoryController(player.Inventory, ui, ui);
            InteractivityController = new InteractivityController(ui);

            return this;
        }

        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Always;
        }

        public override void _Process(double delta)
        {
            if (Input.IsActionJustPressed("Pause"))
                GetTree().Paused = !GetTree().Paused;

            base._Process(delta);
        }

    }
}
