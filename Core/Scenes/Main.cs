using System.Collections.Generic;
using Dose.Core.ActionPerformers;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration.Actors;
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
            var light = new DirectionalLight3D
            {
                RotationDegrees = new Vector3(-45, 0, 0)
            };

            AddChild(light);

			var ui = GetNode<UI>("UI");
			var player = GetNode<Player>("Player");
			var controlPlane = GetNode<ControlPlane>("ControlPlane");

			var itemPool = new ActorPool<ItemContext>(
				configPath: "Configuration/Actors/Items",
				actorScene: GD.Load<PackedScene>("res://Configuration/Scenes/Item.tscn"),
				configConverter: new ItemConfigConverter(),
				contextBuilder: new ItemContextBuilder(new Dictionary<string, IActionPerformer>
				{
					{ "movePlayer", new MovePlayerPerformer(player) }
				})
			).LoadContexts();
			
			var npcPool = new ActorPool<NpcContext>(
				configPath: "Configuration/Actors/Npcs",
				actorScene: GD.Load<PackedScene>("res://Configuration/Scenes/Npc.tscn"),
				configConverter: new NpcConfigConverter(),
				contextBuilder: new NpcContextBuilder([
					new("speak", new SpeakNodeConverter(), new DialogueLinesBuilder()),
					new ("execute", new ExecuteNodeConverter(), new DialogueActionBuilder(
						new Dictionary<string, DialogueAction> {
							{ "exitGame", new DialogueAction<Player>((player, onFinish) => {
								player.Position = new Vector3(player.Position.X + 1, player.Position.Y, player.Position.Z);
							}, player, player.ProgressDialogue) }
						}
					)),
					new("displayOptions", new DisplayOptionsNodeConverter(), new DialogueOptionsBuilder())
				])
			).LoadContexts();

			var locationManager = GetNode<LocationManager>("LocationManager").Initialize(controlPlane, player, itemPool, npcPool);

			player.Initialize(controlPlane);
			controlPlane.Initialize(ui, player, locationManager);

			AudioCollector.InitializeESpeak();
		}
	}
}