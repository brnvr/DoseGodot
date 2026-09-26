using Dose.Core.Library.Locations;
using Dose.Core.Library.Randomness;
using Dose.Core.Library.GameEvents;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Dose.Core.Scenes
{
    public abstract partial class Location : Node3D, ILocation
    {
        const string LIGHT_POINTS_NODE_NAME = "LampFloorOverlayMeshInstance";
        const float DARK_OVERLAY_OPACITY = 0.5f;

        readonly List<ILocationEvent> _gameEvents = [];
        bool _active;
        ColorRect? _darkOverlay;
        IEnumerable<MeshInstance3D> LightPoints => FindChildren(
            LIGHT_POINTS_NODE_NAME,
            recursive: true,
            owned: false
        ).OfType<MeshInstance3D>();

        public Guid Id { get; } = Guid.NewGuid();
        public NavigationRegion3D? NavigationRegion { get; private set; }
        public bool LightsOn { get; private set; } = true;

        public enum LocationType
        {
            Room, Corridor
        }

        public abstract LocationType? Type { get; }

        public override void _Ready()
        {
            var canvasLayer = new CanvasLayer
            {
                Name = "DarkOverlayLayer",
                Layer = 0
            };
            AddChild(canvasLayer);

            _darkOverlay = new ColorRect
            {
                Name = "DarkOverlay",
                Color = new Color(0f, 0f, 0f, DARK_OVERLAY_OPACITY),
                MouseFilter = Godot.Control.MouseFilterEnum.Ignore,
                Visible = false
            };
            canvasLayer.AddChild(_darkOverlay);
            _darkOverlay.SetAnchorsAndOffsetsPreset(Godot.Control.LayoutPreset.FullRect);
        }

        public override void _Process(double delta)
        {
            if (!_active)
            {
                return;
            }

            foreach (var gameEvent in _gameEvents)
            {
                if (gameEvent.ShouldTrigger(this))
                {
                    gameEvent.OnTrigger(this);
                }
            }
        }

        public virtual void OnEntered()
        {
            _active = true;

            foreach (var gameEvent in _gameEvents)
            {
                gameEvent.Activate(this);
            }
        }

        public virtual void OnLeft()
        {
            foreach (var gameEvent in _gameEvents)
            {
                gameEvent.Deactivate(this);
            }

            _active = false;
        }

        protected void AddGameEvent(ILocationEvent gameEvent)
        {
            _gameEvents.Add(gameEvent);

            if (_active)
            {
                gameEvent.Activate(this);
            }
        }

        public void TurnLightsOn()
        {
            LightsOn = true;

            if (_darkOverlay != null)
            {
                _darkOverlay.Visible = false;
            }

            foreach (var lightPoint in LightPoints)
            {
                lightPoint.Visible = true;
            }
        }

        public void TurnLightsOff()
        {
            LightsOn = false;

            if (_darkOverlay != null)
            {
                _darkOverlay.Visible = true;
            }

            foreach (var lightPoint in LightPoints)
            {
                lightPoint.Visible = false;
            }
        }

        public virtual Location InitializeNavigationRegion(string floorNodeName = "FloorMeshInstance")
        {
            var floorNode = FindChild(floorNodeName, owned: false);
            
            if (floorNode == null) 
            {
                GD.PushWarning($"Child not found: {floorNodeName}");
                return this;
            }

            if (NavigationRegion != null)
            {
                GD.PushWarning($"{nameof(NavigationRegion)} has already been initialized.");
                return this;
            }
    
            NavigationRegion = new NavigationRegion3D
            { 
                NavigationMesh = new NavigationMesh
                {
                    GeometryParsedGeometryType = NavigationMesh.ParsedGeometryType.StaticColliders,
                    GeometryCollisionMask = LocationCollisionLayer.NavigationSource
                }
            };

            AddChild(NavigationRegion);
            
            var sourceData = new NavigationMeshSourceGeometryData3D();

            NavigationServer3D.ParseSourceGeometryData(
                NavigationRegion.NavigationMesh,
                sourceData,
                floorNode
            );

            NavigationServer3D.BakeFromSourceGeometryData(
                NavigationRegion.NavigationMesh,
                sourceData
            );

            // GD.Print($"Source has data: {sourceData.HasData()}");
            // GD.Print($"Source vertices: {sourceData.GetVertices().Length}");
            // GD.Print($"Source indices: {sourceData.GetIndices().Length}");

            return this;
        }

        public TPortal SelectRandomPortal<TPortal>(Func<TPortal, bool>? predicate = null) where TPortal : IPortal
        {
            var portals = GetChildren().OfType<TPortal>();

            if (predicate != null)
            {
                portals = portals.Where(predicate);
            }

            return new RandomSelector<TPortal>(portals).Pick();
        }

        public void Clear(int keepLevels)
        {
            Clear(keepLevels, null);
        }

        void Clear(int keepLevels, Location? previous)
        {
            var doors = GetChildren().OfType<Door>();
            var stairs = GetChildren().OfType<Stairs>();

            Clear(doors, keepLevels, previous);
            Clear(stairs, keepLevels, previous);
        }

        void Clear(IEnumerable<IPortal> portals, int keepLevels, Location? previous)
        {
            foreach (var portal in portals)
            {
                var otherLocation = portal.Linked?.Location;
                
                if (otherLocation is Location _otherLocation && _otherLocation != previous)
                {
                    _otherLocation.Clear(keepLevels-1, this); 

                    if (keepLevels <= 0)
                    {
                        portal.Unlink();
                        _otherLocation.QueueFree();
                    }
                }
            }
        }
    }

}
