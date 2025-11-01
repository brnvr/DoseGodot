using Dose.Library.Interactivity;
using Godot;
using System;

namespace Dose.Library.Scenes
{
    public partial class Door : Node3D, IInteractable<Player>
    {
        const double MAX_OPENING = System.Math.PI / 2;
        State _state;
        float _rotationYStart;

        [Export] public ClockDirection OpenDirection = ClockDirection.Clockwise;

        enum State
        {
            Open,
            Opening,
            Closed,
            Closing
        }

        public override void _Ready()
        {
            _state = State.Closed;
            _rotationYStart = Rotation.Y;

            base._Ready();
        }


        public override void _Process(double delta)
        {
            

            switch (_state)
            {
                case State.Opening:
                    RotateY(Mathf.DegToRad(5));

                    if (System.Math.Abs(Rotation.Y - _rotationYStart) >= MAX_OPENING)
                    {
                        _state = State.Open;
                    }

                    break;

                case State.Closed:
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