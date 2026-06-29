using Dose.Core.ActorArgs;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Interactivity;
using Dose.Core.Library.Locations;
using Dose.Core.Library.Numbers;
using Godot;
using System;
using System.Collections.Generic;

namespace Dose.Core.Scenes
{
    public enum DoorState
    {
        Open,
        Opening,
        Closed,
        Closing
    }

    public partial class Door : Node3D, IDoor, IActor<Player>, IToggleable<DoorState>, IInitializableActor<DoorContext, DoorArgs>
    {
        const double MAX_OPENING = Math.PI / 2;

        MeshInstance3D? _mesh;
        StaticBody3D? _body;
        CollisionShape3D? _collisionShape;
        float _yRotationStart;
        float _opening;
        readonly float _rotateSpeed = Mathf.DegToRad(5);
        ILocationManager? _locationManager;

        [Export] public ClockDirection OpenDirection { get; set; } = ClockDirection.Clockwise;
        public ILocation? Location { get; private set; }
        public IDoor? Linked { get; private set; }
        IActorContext? IActor.Context => Context;
        public DoorContext? Context { get; private set; }
        public DoorState State { get; private set; }
        public IReadOnlyDictionary<DoorState, string> DisplayNameForState { get; } = new Dictionary<DoorState, string>
        {
          { DoorState.Open, "close" },  
          { DoorState.Opening, "close" },
          { DoorState.Closed, "open" },
          { DoorState.Closing, "open" },
        };

        IPortal? IPortal.Linked => Linked;


        public override void _Ready()
        {
            _mesh = GetNode<MeshInstance3D>("Mesh");
            _body = GetNode<StaticBody3D>("Body");
            _collisionShape = _body.GetNode<CollisionShape3D>("CollisionShape");
            
            _yRotationStart = _mesh.Rotation.Y;
        }

        public void Link(Door other)
        {
            if (Linked == other)
            {
                return;
            }
            else if (Linked != null)
            {
                GD.PushError($"{nameof(Door)}.{nameof(Linked)} has already been set.");
                return;
            }

            Linked = other;
            other.Link(this);
        }

        public void Unlink()
        {
            Linked = null;
        }

        public IInteraction<Player> GetDefaultInteraction(Player interactor)
        {
            if (_locationManager == null)
            {
                return new ToggleInteraction<Player, DoorState>(interactor);
            }

            return new OpenDoorInteraction<Player>(interactor, _locationManager);
        }

        public void OnInteract(IInteraction<Player> interaction)
        {
            interaction.Perform(this);
        }

        public void Toggle()
        {
            var couldOpen = TryOpen();

            if (!couldOpen)
            {
                TryClose();
            }
        }

        public override void _Process(double delta)
        {
            switch (State)
            {
                case DoorState.Opening:
                    var target = OpenDirection == ClockDirection.Clockwise ? MAX_OPENING : -MAX_OPENING;

                    _opening = (float)new LinearInterpolator(_opening, target, _rotateSpeed).Interpolate();

                    if (_opening >= MAX_OPENING)
                    {
                        State = DoorState.Open;
                    }

                    UpdateRotation();

                    break;

                case DoorState.Closing:
                    _opening = (float)new LinearInterpolator(_opening, 0, _rotateSpeed).Interpolate();

                    if (_opening == 0)
                    {
                        State = DoorState.Closed;
                    }

                    UpdateRotation();

                    break;
            }


            base._Process(delta);
        }

        public bool TryOpen(bool disableCollision = true)
        {
            if (State == DoorState.Open || State == DoorState.Opening)
            {
                return false;
            }

            State = DoorState.Opening;
            if (disableCollision) DisableCollision();
            
            return true;
        }

        
        public bool TryClose()
        {
            if (State == DoorState.Closed || State == DoorState.Closing)
            {
                return false;
            }

            EnableCollision();
            State = DoorState.Closing;

            return true;
        }

        public Vector3 GetHingeCenter()
        {
            if (_mesh is null)
                return Vector3.Zero;

            var mesh = _mesh.Mesh;
            var arrays = mesh.SurfaceGetArrays(0);
            var vertices = (Vector3[])arrays[(int)Mesh.ArrayType.Vertex];

            float hingeY = _mesh.Transform.Origin.Y;   // origin = hinge
            const float tolerance = 0.01f; // accepts vertices close to hinge height

            float minX = float.MaxValue;
            float maxX = float.MinValue;

            foreach (var v in vertices)
            {
                if (Mathf.Abs(v.Y - hingeY) <= tolerance)
                {
                    minX = Mathf.Min(minX, v.X);
                    maxX = Mathf.Max(maxX, v.X);
                }
            }

            // midpoint at hinge-level
            float centerX = (minX + maxX) * 0.5f;

            // convert local to global
            return _mesh.GlobalTransform * new Vector3(centerX, hingeY, 0);
        }


        void UpdateRotation()
        {
            if (_mesh == null) return;

            _mesh.Rotation = new Vector3(0, _yRotationStart + _opening, 0);
        }

        void EnableCollision()
        {
            if (_collisionShape == null) return;

            _collisionShape.Disabled = false;
        }
        
        void DisableCollision()
        {
            if (_collisionShape == null) return;

            _collisionShape.Disabled = true;
        }

        void IPortal.Link(IPortal other) => Link((Door)other);

        public void Initialize(DoorContext context, DoorArgs args)
        {
            _opening = 0;
            State = DoorState.Closed;
            Context = context;
            Location = args.Location;
            OpenDirection = args.OpenDirection;
            _locationManager = args.LocationManager;
        }
    }
}