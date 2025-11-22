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

			AudioCollector.InitializeESpeak();

            var speaker = new Speaker(new AudioCollector(ProcessESpeakAudio));
			
			var controlPlane = GetNode<ControlPlane>("ControlPlane").Initialize(ui, player, npc);

			speaker.Synth("test testing some test something.");
		}
		
		void ProcessESpeakAudio(byte[] data)
		{
			var stream = new AudioStreamWav
			{
				MixRate = 22050,
				Data = data,
				Format = AudioStreamWav.FormatEnum.Format16Bits
			};

			var player = new AudioStreamPlayer
			{
				Stream = stream
			};

			AddChild(player);

			player.Play();
		}
	}
}