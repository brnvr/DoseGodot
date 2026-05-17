using Godot;
using System.Collections.Generic;
using Dose.Core.ActionPerformers;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration.Actors;
using Dose.Core.Library.Configuration.Dialogues;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.ESpeak;
using Dose.Core.Library.Locations;
using Dose.Core.Library.Configuration.Locations;

namespace Dose.Core.Scenes
{
    public partial class Main : Node3D
	{
		readonly CustomRoomPool? _customRoomPool;

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

			var customRoomPool = new CustomRoomPool(
				configPath: "Configuration/Locations/CustomRooms",
				configConverter: new CustomRoomConfigConverter(),
				contextBuilder: new CustomRoomContextBuilder()
			).LoadContexts();

			controlPlane.Initialize(ui, player, locationManager, customRoomPool);
			locationManager.Initialize(controlPlane, player, itemPool, npcPool);
			player.Initialize(controlPlane);

			AudioCollector.InitializeESpeak();
		}

        // public override void _ExitTree()
        // {
		// 	var controlPlane = GetNode<ControlPlane>("ControlPlane");

		// 	if (controlPlane.DoorNumberController == null) return;

		// 	foreach (var kvp in controlPlane.DoorNumberController.PredefinedDoors)
		// 	{
		// 		kvp.Value.Location
		// 	}
        // }
	}
}