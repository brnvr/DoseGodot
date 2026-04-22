using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Randomness;
using Godot;

namespace Dose.Core.Library.Control
{
    public class DoorNumberController
    {
        IEnumerable<int> _availableNumbers = [];

        public DoorNumberController()
        {
            Reset();
        }

        public void Reset()
        {
            _availableNumbers = Enumerable.Range(1, 999);
        }

        public int PopRandomNumber()
        {
            if (!_availableNumbers.Any())
            {
                GD.PushError("No available door numbers.");

                return 0;
            }

            var number = new RandomSelector<int>(_availableNumbers).Pick();

            _availableNumbers = _availableNumbers.Except([number]);

            return number;
        }
    }
}