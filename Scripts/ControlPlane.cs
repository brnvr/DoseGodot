using Dose.Actors.Npcs;
using Dose.Library.Actors;
using Dose.Library.Common;
using Dose.Library.Control;
using Dose.Library.DataStructures;
using Godot;

namespace Dose.Scripts
{
    public partial class ControlPlane : Node, IControlPlane
    {
        UI? _ui;
        readonly AgnesController _agnesController = new();


        public ControlPlane Initialize(UI ui, Player player, Npc npc)
        {
            _ui = ui;
            
            npc.Initialize(_agnesController);
            player.Initialize(this);

            return this;
        }

        public void OnTargetChange(IActor? target)
        {
            if (_ui == null) return;

            _ui.SetActorName(target?.Controller?.DisplayName ?? "");
        }
    }
}
