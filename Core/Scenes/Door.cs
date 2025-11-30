using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
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

    public partial class Door : Node3D, IDoor, IActor<Player>, IToggleable<DoorState>
    {
        const double MAX_OPENING = Math.PI / 2;

        float _yRotationStart;
        float _opening;
        readonly float _rotateSpeed = Mathf.DegToRad(5);
        ILocationManager? _locationManager;

        [Export] public ClockDirection OpenDirection { get; set; } = ClockDirection.Clockwise;
        public ILocation? Location { get; private set; }
        public IDoor? LinkedDoor { get; private set; }
        public IActorContext? Context { get; private set; }
        public DoorState State { get; private set; }
        public IReadOnlyDictionary<DoorState, string> DisplayNameForState { get; } = new Dictionary<DoorState, string>
        {
          { DoorState.Open, "close" },  
          { DoorState.Opening, "close" },
          { DoorState.Closed, "open" },
          { DoorState.Closing, "open" },
        };

        public void Initialize(DoorContext context, ILocationManager locationManager, ILocation? location, ClockDirection openDirection)
        {
            _opening = 0;
            State = DoorState.Closed;
            _yRotationStart = Rotation.Y;
            Context = context;
            Location = location;
            OpenDirection = openDirection;
            _locationManager = locationManager;
        }

        public void LinkDoor(IDoor other)
        {
            if (LinkedDoor == other)
            {
                return;
            }
            else if (LinkedDoor != null)
            {
                GD.PushError($"{nameof(Door)}.{nameof(LinkedDoor)} has already been set.");
                return;
            }

            LinkedDoor = other;
            other.LinkDoor(this);
        }

        public void UnlinkDoor()
        {
            LinkedDoor = null;
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

                    if (_opening <= 0)
                    {
                        State = DoorState.Closed;
                    }

                    UpdateRotation();

                    break;
            }


            base._Process(delta);
        }

        void UpdateRotation()
        {
            Rotation = new Vector3(0, _yRotationStart + _opening, 0);
        }

        bool TryOpen()
        {
            if (State == DoorState.Open || State == DoorState.Opening)
            {
                return false;
            }

            State = DoorState.Opening;
            return true;
        }
        
        bool TryClose()
        {
            if (State == DoorState.Closed || State == DoorState.Closing)
            {
                return false;
            }

            State = DoorState.Closing;
            return true;
        }
    }
}