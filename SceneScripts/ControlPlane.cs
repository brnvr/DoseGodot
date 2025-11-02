using Dose.Library.Actors;
using Dose.Library.Control;
using Dose.Library.DataStructures;
using Godot;

namespace Dose.SceneScripts
{
    public partial class ControlPlane : Node, IControlPlane
    {
        UI? _ui;

        public ActorRegistry? ActorRegistry { get; private set; }


        public ControlPlane Initialize(UI ui)
        {
            _ui = ui;
            ActorRegistry = new ActorRegistry();

            ActorRegistry.Register<Door>("door");
            ActorRegistry.Register<Npc>("NPC");

            return this;
        }

        public void OnTargetChange(IActor? target)
        {
            if (_ui == null) return;

            var record = target is null ? null : ActorRegistry?.Get(target);

            _ui.SetActorName(record?.ActorName);
        }
    }
}
