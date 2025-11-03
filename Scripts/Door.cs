using Dose.Library.Actors;
using Dose.Library.Common;
using Dose.Library.Numbers;
using Godot;
using System;

namespace Dose.Scripts
{
    public partial class Door : Node3D, IActor<Player>, IInitializable
    {
        const double MAX_OPENING = Math.PI / 2;

        State _state;
        float _yRotationStart;
        float _opening;
        readonly float _rotateSpeed = Mathf.DegToRad(5);

        [Export] public ClockDirection OpenDirection = ClockDirection.Clockwise;

        public IActorController Controller => new TestDoorController();

        enum State
        {
            Open,
            Opening,
            Closed,
            Closing
        }

        public void Initialize()
        {
            _opening = 0;
            _state = State.Closed;
            _yRotationStart = Rotation.Y;
        }

        public override void _Process(double delta)
        {
            switch (_state)
            {
                case State.Opening:
                    var target = OpenDirection == ClockDirection.Clockwise ? MAX_OPENING : -MAX_OPENING;

                    _opening = (float)new LinearInterpolator(_opening, target, _rotateSpeed).Interpolate();

                    if (_opening >= MAX_OPENING)
                    {
                        _state = State.Open;
                    }

                    UpdateRotation();

                    break;

                case State.Closing:
                    _opening = (float)new LinearInterpolator(_opening, 0, _rotateSpeed).Interpolate();

                    if (_opening <= 0)
                    {
                        _state = State.Closed;
                    }

                    UpdateRotation();

                    break;
            }


            base._Process(delta);
        }

        public void HandleInteraction(Player player)
        {
            var couldOpen = TryOpen();

            if (!couldOpen)
            {
                TryClose();
            }
        }

        void UpdateRotation()
        {
            Rotation = new Vector3(0, _yRotationStart + _opening, 0);
        }

        bool TryOpen()
        {
            if (_state == State.Open || _state == State.Opening)
            {
                return false;
            }

            _state = State.Opening;
            return true;
        }
        
        bool TryClose()
        {
            if (_state == State.Closed || _state == State.Closing)
            {
                return false;
            }

            _state = State.Closing;
            return true;
        }
    }

}