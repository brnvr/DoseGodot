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
using Dose.Core.Library.Configuration.Packs;
using Dose.Core.Library.Packs;

namespace Dose.Core.Scenes
{
	public partial class Main : Node3D
	{
		const string TEXTURE_PATH = "Configuration/Textures";
		UI? _ui;
		Player? _player;
		LocationManager? _locationManager;
		WorldMap? _worldMap;
		Vector2? _lastRenderedPlayerPosition;

		public override void _Ready()
		{
            var worldLightRig = new Node3D { Name = "WorldLightRig" };
            AddChild(worldLightRig);

            List<DirectionalLight3D> lights = [
				new DirectionalLight3D
				{
					RotationDegrees = new Vector3(-90, 0, 0),
					LightEnergy = 1.25f
				},
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
				worldLightRig.AddChild(light);
			}

			var ui = GetNode<UI>("UI");
			var player = GetNode<Player>("Player");
			var postShader = GetNode<PostShader>("PostShader");
			var controlPlane = GetNode<ControlPlane>("ControlPlane");
			var locationManager = GetNode<LocationManager>("LocationManager");
			var worldMap = new WorldMap(locationManager.MapRenderQueue);
			_ui = ui;
			_player = player;
			_locationManager = locationManager;
			_worldMap = worldMap;
			locationManager.CurrentLocationChanged += async (previous, current) =>
			{
				worldMap.Visit(current, previous);
				_lastRenderedPlayerPosition = null;
				UpdateMapDisplay();
				var image = await worldMap.GetImageAsync(current.Id);
				if (image != null)
				{
					var locationId = current.Id;
					Callable.From(() => DisplayRenderedMap(locationId, image)).CallDeferred();
				}
			};

            void SyncCursorType()
            {
                ui.SetCursorType(
                    player.CanInteract() && player.SelectedItem == null
                        ? Cursor.Types.Dot
                        : null
                );
            }

            player.SelectedItemChanged += (sender, e) =>
            {
                ui.SetEquipedItem(e.Item?.EquipedTexture, e.Item?.EquipedScale ?? 1f);
                ui.SetEquipedItemTargeted(e.Item != null && player.HasInteractionTarget);
                SyncCursorType();
            };
            player.DoorTransitionStarted += (sender, e) =>
            {
                worldLightRig.RotateY(e.RotationDelta);
            };
            player.InteractionAvailabilityChanged += (sender, e) =>
            {
                SyncCursorType();
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

			var lampTexturePool = new WeightedPool<LampTextureDefinition>
			(
				configPath: Path.Join(TEXTURE_PATH, "Environment/Lamps"),
				builder: new LampTextureDefinitionBuilder(),
				configConverter: new LampTextureConfigConverter()
			).Load();

			var doorTexturePool = new WeightedPool<TextureDefinition>
			(
				configPath: Path.Join(TEXTURE_PATH, "Environment/Doors"),
				builder: new TextureDefinitionBuilder(),
				configConverter: new TextureConfigConverter()
			).Load();

			var wallOverlayTexturePool = new WeightedPool<WallOverlayTextureDefinition>
			(
				configPath: Path.Join(TEXTURE_PATH, "Environment/WallOverlays"),
				builder: new WallOverlayTextureBuilder(),
				configConverter: new WallOverlayTextureConfigConverter()
			).Load();

			var floorOverlayTexturePool = new WeightedPool<FloorOverlayTextureDefinition>
			(
				configPath: Path.Join(TEXTURE_PATH, "Environment/FloorOverlays"),
				builder: new FloorOverlayTextureBuilder(),
				configConverter: new FloorOverlayTextureConfigConverter()
			).Load();

			var itemPool = new ActorPool<ItemContext>
			(
				configPath: "Configuration/Actors/Items",
				actorScene: GD.Load<PackedScene>("res://Configuration/Scenes/Item.tscn"),
				configConverter: new ItemConfigConverter(),
				contextBuilder: new ItemContextBuilder(new Dictionary<string, IActionPerformer>
				{
					{ "MovePlayer", new MovePlayerPerformer(player) }
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
				], new Dictionary<string, IActionPerformer>
				{
					{ "Receive", new ReceivePerformer(player) },
					{ "RejectWithDialogue", new RejectWithDialoguePerformer(player) }
				})
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

			var packConfigs = new Pool<PackConfig>
			(
				configPath: "Configuration/Packs",
				configConverter: new PackConfigConverter(),
				builder: new PackConfigBuilder()
			).Load();

			var packPool = new PackPoolBuilder(
				packConfigs.Content,
				itemPool,
				npcPool,
				wallTexturePool.Content,
				doorTexturePool.Content,
				lampTexturePool.Content,
				wallOverlayTexturePool.Content,
				floorOverlayTexturePool.Content,
				propPool
			).Build();

			controlPlane.Initialize(ui, player, locationManager, customRoomPool, new CustomRoomLoader<CustomRoom>());
			locationManager.Initialize
			(
				controlPlane,
				player,
				packPool,
				npcEvents,
				locationEvents
			);
			player.Initialize(controlPlane);
            player.Inventory.Add(currantSyrup);
			postShader.Initialize(palette);

			AudioCollector.InitializeESpeak();
		}

		void DisplayRenderedMap(System.Guid locationId, Image image)
		{
			if (_locationManager?.CurrentLocation?.Id == locationId)
			{
				_ui?.DisplayMap(image);
			}
		}

		public override void _Process(double delta)
		{
			UpdateMapDisplay();
		}

		void UpdateMapDisplay()
		{
			if (_ui == null || _player == null || _worldMap == null ||
				_locationManager?.CurrentLocation is not IBuildableLocation { BlockSize: > 0 })
			{
				return;
			}

			var blockSize = ((IBuildableLocation)_locationManager.CurrentLocation).BlockSize;
			var playerPosition = new Vector2(_player.GlobalPosition.X, _player.GlobalPosition.Z) / blockSize;
			if (_lastRenderedPlayerPosition is Vector2 previous && previous.IsEqualApprox(playerPosition))
			{
				return;
			}

			_lastRenderedPlayerPosition = playerPosition;
			_ui.CenterMapOn(_worldMap.GetCurrentPlayerPixel(playerPosition));
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
