using Dose.Core.Actors.Npcs;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Control;
using Godot;

namespace Dose.Core.Scenes
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
