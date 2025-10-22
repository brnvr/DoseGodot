using Godot;
using System;

public partial class Door : Node3D
{
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
        // RotateY(Mathf.DegToRad(5));

        // switch (_state)
        // {
        //     case State.Open:
        //         switch (OpenDirection)
        //         {
        //             case 
        //         }

        //     case State.Closed:
        //         break;
        // }


        base._Process(delta);
    }
}
