using System.Collections.Generic;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Dialogues;
using Dose.Core.Library.ESpeak;

using Godot;

namespace Dose.Core.Scenes
{
    public partial class Main : Node3D
	{
		public override void _Ready()
		{
			DialogueConfig actions = new()
			{
				StartAt = "asdasdas",
				Nodes = new()
				{
					{ "asdasdas", new SpeakNode {
						Args = new SpeakNodeArgs {
							Lines = [
								"sadasdasda",
								"teste 123"
							],
							Next = "asdasdasd",
						}
					}},
					{ "as sadasd", new DisplayOptionsNode {
						Args = new DisplayOptionsNodeArgs {
							Options = [
								new DisplayOptionsNodeOption {
									Text = "Testing",
									Next = "asdasdasd"
								}
							]
						}
					}}
				}
			};

			actions.SaveYaml("D:/Godot/Dose/test.yml");

			var loaded = (Dictionary<string, object>)YamlConfig.FromYaml("D:/Godot/Dose/test.yml");

			var converter = new DialogueConfigConverter();

			var loadedAsObject = converter.FromDict(loaded);

            var light = new DirectionalLight3D
            {
                RotationDegrees = new Vector3(-45, 0, 0)
            };

            AddChild(light);

			var ui = GetNode<UI>("UI");
			//var npc = GetNode<Npc>("Npc");
			var player = GetNode<Player>("Player");
			var controlPlane = GetNode<ControlPlane>("ControlPlane");
			var locationManager = GetNode<LocationManager>("LocationManager").Initialize(controlPlane, player);

			player.Initialize(controlPlane);
			controlPlane.Initialize(ui, player, locationManager);

			AudioCollector.InitializeESpeak();
		}
		
		
	}
}