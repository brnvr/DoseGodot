using Dose.Core.Library.ESpeak;

using Godot;

namespace Dose.Core.Scenes
{
    public partial class Main : Node3D
	{
		public override void _Ready()
		{
            var light = new DirectionalLight3D
            {
                RotationDegrees = new Vector3(-45, 0, 0)
            };

            AddChild(light);

			var ui = GetNode<UI>("UI");
			var npc = GetNode<Npc>("Npc");
			var player = GetNode<Player>("Player");
			var controlPlane = GetNode<ControlPlane>("ControlPlane").Initialize(ui, player, npc);
			var locationManager = GetNode<LocationManager>("LocationManager").Initialize(controlPlane);

			AudioCollector.InitializeESpeak();
			
			
		}
		
		
	}
}