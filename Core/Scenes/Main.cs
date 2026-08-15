using Godot;
using System.Collections.Generic;
using System.Linq;
using Dose.Core.ActionPerformers;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration.Actors;
using Dose.Core.Library.Configuration.Dialogues;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.ESpeak;
using Dose.Core.Library.Locations;
using Dose.Core.Library.Configuration.Locations;
using Dose.Core.Library.GameEvents;
using Dose.Core.Library.DataStructures;
using System.IO;
using Dose.Core.Library.Loaders;
using Dose.Core.Library.Configuration.Props;
using Dose.Core.Library.Props;

namespace Dose.Core.Scenes
{
    public partial class Main : Node3D
	{
		const string TEXTURE_PATH = "Configuration/Textures";
		public override void _Ready()
		{
            List<DirectionalLight3D> lights = [
				new DirectionalLight3D
				{
					RotationDegrees = new Vector3(0, 0, 0)
				},
				new DirectionalLight3D
				{
					RotationDegrees = new Vector3(0, 180, 0)
				},
				new DirectionalLight3D
				{
					RotationDegrees = new Vector3(0, 90, 0), 
					LightEnergy = 0.25f
				},
				new DirectionalLight3D
				{
					RotationDegrees = new Vector3(0, 270, 0), 
					LightEnergy = 0.25f
				},
			];

            foreach (var light in lights)
			{
				AddChild(light);
			}

			var ui = GetNode<UI>("UI");
			var player = GetNode<Player>("Player");
			var postShader = GetNode<PostShader>("PostShader");
			var controlPlane = GetNode<ControlPlane>("ControlPlane");
			var locationManager = GetNode<LocationManager>("LocationManager");
			var locationOccluder = GetNode<LocationOccluder>("LocationOccluder");

            void SyncCursorVisibility()
            {
                ui.SetCursorVisible(player.CanInteract() && player.SelectedItem == null);
            }

            player.SelectedItemChanged += (sender, e) =>
            {
                ui.SetEquipedItem(e.Item?.EquipedTexture);
                ui.SetEquipedItemTargeted(e.Item != null && player.HasInteractionTarget);
                SyncCursorVisibility();
            };
            player.InteractionAvailabilityChanged += (sender, e) =>
            {
                SyncCursorVisibility();
            };
            player.InteractionTargetChanged += (sender, e) =>
            {
                ui.SetEquipedItemTargeted(e.HasTarget);
            };
            player.ItemInteractionRejected += (sender, e) =>
            {
                ui.PlayRejectedItemInteractionFeedback();
            };

			List<IGameEvent<Npc>> npcEvents = [
				new DisappearOnDistanceEvent<Npc>(player, 10)
			];

			List<IGameEvent<Location>> locationEvents = [];

			var palette = new JascPalLoader("Configuration/Palettes/miri16.pal").Load();

			var wallTexturePool = new WeightedPool<TextureDefinition>
			(
				configPath: Path.Join(TEXTURE_PATH, "Environment/Walls"),
				builder: new TextureDefinitionBuilder(),
				configConverter: new TextureConfigConverter()
			).Load();

			var lampTexturePool = new WeightedPool<TextureDefinition>
			(
				configPath: Path.Join(TEXTURE_PATH, "Environment/Lamps"),
				builder: new TextureDefinitionBuilder(),
				configConverter: new TextureConfigConverter()
			).Load();

			var doorTexturePool = new WeightedPool<TextureDefinition>
			(
				configPath: Path.Join(TEXTURE_PATH, "Environment/Doors"),
				builder: new TextureDefinitionBuilder(),
				configConverter: new TextureConfigConverter()
			).Load();

			var wallOverlayTexturePool = new Pool<WallOverlayTextureDefinition>
			(
				configPath: Path.Join(TEXTURE_PATH, "Environment/WallOverlays"),
				builder: new WallOverlayTextureBuilder(),
				configConverter: new WallOverlayTextureConfigConverter()
			).Load();

			var itemPool = new ActorPool<ItemContext>
			(
				configPath: "Configuration/Actors/Items",
				actorScene: GD.Load<PackedScene>("res://Configuration/Scenes/Item.tscn"),
				configConverter: new ItemConfigConverter(),
				contextBuilder: new ItemContextBuilder(new Dictionary<string, IActionPerformer>
				{
					{ "movePlayer", new MovePlayerPerformer(player) },
					{ "receive", new ReceivePerformer(player) }
				})
			).Load();

            var currantSyrup = itemPool.Content.Single(item => item.Identifier == "currant syrup");
			
			var npcPool = new ActorPool<NpcContext>
			(
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
			).Load();

			var customRoomPool = new Pool<CustomRoomContext?>
			(
				configPath: "Configuration/Locations/CustomRooms",
				configConverter: new CustomRoomConfigConverter(),
				builder: new CustomRoomContextBuilder()
			).Load();

			var propPool = new PropPool
			(
				configPath: "Configuration/Props",
				propScene: GD.Load<PackedScene>("res://Configuration/Scenes/Prop.tscn"),
				configConverter: new PropConfigConverter(),
				builder: new PropDefinitionBuilder()
			).Load();

			controlPlane.Initialize(ui, player, locationManager, customRoomPool, new CustomRoomLoader<CustomRoom>());
			locationManager.Initialize
			(
				controlPlane,
				player,
				itemPool,
				npcPool,
				wallTexturePool,
				doorTexturePool,
				lampTexturePool,
				wallOverlayTexturePool,
				propPool,
				npcEvents,
				locationEvents
			);
			player.Initialize(controlPlane);
            player.Inventory.Add(currantSyrup);
			postShader.Initialize(palette);

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
