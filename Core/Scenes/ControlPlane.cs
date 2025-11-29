using Dose.Core.Actors.Npcs;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Control;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.UI;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class ControlPlane : Node, IControlPlane
    {
        UI? _ui;
        Agnes? _agnes;
        Player? _player;

        public DialogueController? DialogueController { get; private set; }
        public InventoryController? InventoryController  { get; private set; }
        public InteractivityController? InteractivityController { get; private set; }
        public DoorNumberController DoorNumberController { get; } = new DoorNumberController();

        public ControlPlane Initialize(UI ui, Player player, Npc npc)
        {
            _agnes = new(player);
            _ui = ui;
            _player = player;
            
            npc.Initialize(_agnes, player);
            player.Initialize(this);

            DialogueController = new DialogueController(_ui);
            InventoryController = new InventoryController(_player.Inventory);
            InteractivityController = new InteractivityController(_ui);

            return this;
        }
    }
}
