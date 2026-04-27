using Godot;
using System.Collections.Generic;
using Dose.Core.ActionPerformers;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration.Actors;
using Dose.Core.Library.Configuration.Dialogues;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.ESpeak;

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
			var player = GetNode<Player>("Player");
			var controlPlane = GetNode<ControlPlane>("ControlPlane");
			var locationManager = GetNode<LocationManager>("LocationManager");

			var itemPool = new ActorPool<ItemContext>(
				configPath: "Configuration/Actors/Items",
				actorScene: GD.Load<PackedScene>("res://Configuration/Scenes/Item.tscn"),
				configConverter: new ItemConfigConverter(),
				contextBuilder: new ItemContextBuilder(new Dictionary<string, IActionPerformer>
				{
					{ "movePlayer", new MovePlayerPerformer(player) },
					{ "receive", new ReceivePerformer(player) }
				})
			).LoadContexts();
			
			var npcPool = new ActorPool<NpcContext>(
				configPath: "Configuration/Actors/Npcs",
				actorScene: GD.Load<PackedScene>("res://Configuration/Scenes/Npc.tscn"),
				configConverter: new NpcConfigConverter(),
				contextBuilder: new NpcContextBuilder([
					new("speak", new SpeakNodeConverter(), new DialogueLinesBuilder()),
					new ("execute", new ExecuteNodeConverter(), new DialogueActionBuilder(
						dialogueActionMap: new Dictionary<string, IActionPerformer>
						{
							{ "exitGame", new MovePlayerPerformer(player) }
						},
						actionExecuter: player,
						onFinish: player.ProgressDialogue
					)),
					new("displayOptions", new DisplayOptionsNodeConverter(), new DialogueOptionsBuilder())
				])
			).LoadContexts();

			controlPlane.Initialize(ui, player, locationManager);
			locationManager.Initialize(controlPlane, player, itemPool, npcPool);
			player.Initialize(controlPlane);

			AudioCollector.InitializeESpeak();
		}
	}
}