using Dose.Actors.Npcs;
using Dose.Library.Actors;
using Dose.Library.Control;
using Godot;

namespace Dose.Scripts
{
    public partial class ControlPlane : Node, IControlPlane
    {
        UI? _ui;
        readonly Agnes _agnes = new();


        public ControlPlane Initialize(UI ui, Player player, Npc npc)
        {
            _ui = ui;
            
            npc.Initialize(_agnes);
            player.Initialize(this);

            return this;
        }

        public void OnTargetChange(IActor? target)
        {
            if (_ui == null) return;

            _ui.SetActorName(target?.Context?.DisplayName ?? "");
        }
    }
}
