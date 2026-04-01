using System.Collections.Generic;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Dialogues;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.ESpeak;

using Godot;

namespace Dose.Core.Scenes
{
    public partial class Main : Node3D
	{
		public override void _Ready()
		{
			var loaded = (Dictionary<string, object?>)YamlConfig.FromYaml("D:/Godot/Dose/test.yml");

			// var converter = new DialogueConfigConverter(new Dictionary<string, YamlConfigConverter>
			// {
			// 	{ "speak", new SpeakNodeConverter() },
			// 	{ "displayOptions", new DisplayOptionsNodeConverter() }
			// });

			// var config = converter.FromDict(loaded);

			// var builder = new DialogueBuilder(new Dictionary<string, IDialogueNodesBuilder>
			// {
			// 	{ "speak", new DialogueLinesBuilder() },
			// 	{ "displayOptions", new DialogueOptionsBuilder() }
			// });

			// var dialogue = builder.BuildDialogue(config);

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