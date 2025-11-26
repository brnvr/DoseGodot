using Dose.Core.Actors.Npcs;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Control;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.ESpeak;
using Dose.Core.Library.UI;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class ControlPlane : Node, IControlPlane
    {
        UI? _ui;
        Agnes? _agnes;
        Player? _player;

        public ControlPlane Initialize(UI ui, Player player, Npc npc)
        {
            _agnes = new(player);
            _ui = ui;
            _player = player;
            
            npc.Initialize(_agnes, player);
            player.Initialize(this);

            return this;
        }

        public void OnTargetChange(IActor? target)
        {
            _ui?.SetLabelValue("NameCaption", target?.Context?.DisplayName ?? "");
        }

        public void OnDialogueChange(IDialogueNode node)
        {
            if (_ui == null)
            {
                return;
            }

            if (node is IUIComponent uiComponent)
            {
                uiComponent.OnUIEvent(_ui);
            }
        }

        public void OnDialogueEnd()
        {
            _ui?.SetLabelValue("DialogueCaption", "");
            _ui?.SetLabelVisible("NameCaption", true);
        }
    }
}
