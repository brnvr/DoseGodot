using Godot;

namespace Dose.Core.Scenes
{
    public partial class CustomRoom : Location
    {
        int _blockSize;
        ClockDirection _doorsClockDirection = ClockDirection.Clockwise;

        public override LocationType? Type { get => LocationType.Room; }
        public override int BlockSize { get => _blockSize; }
        public override ClockDirection DoorsClockDirection { get => _doorsClockDirection; }

        public CustomRoom Initialize(int blockSize, ClockDirection doorsClockDirection)
        {
            _blockSize = blockSize;
            _doorsClockDirection = doorsClockDirection;

            return this;
        }
    }
}
