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
        public PauseController? PauseController { get; private set; }
        public DoorNumberController DoorNumberController { get; } = new DoorNumberController();

        public ControlPlane Initialize(UI ui, Player player, LocationManager locationManager)
        {
            LocationController = new LocationController(locationManager);
            DialogueController = new DialogueController(ui);
            InventoryController = new InventoryController(player.Inventory, ui, ui);
            InteractivityController = new InteractivityController(ui);
            PauseController = new PauseController(ui);

            if (PauseController != null)
            {
                PauseController.Paused += (s, e) => {
                    GetTree().Paused = true;
                    Input.MouseMode = Input.MouseModeEnum.Visible;
                };
                PauseController.Unpaused += (s, e) => {
                    GetTree().Paused = false;
                    Input.MouseMode = Input.MouseModeEnum.Captured;
                };
                PauseController.Quit += (s, e) => {
                    GetTree().Quit();
                };
            }

            return this;
        }

        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Always;
        }

        public override void _Process(double delta)
        {
            if (Input.IsActionJustPressed("Pause"))
            {
                if (GetTree().Paused)
                {
                    PauseController?.Unpause();
                }
                else
                {
                    PauseController?.Pause();
                }
            }

            base._Process(delta);
        }

    }
}
