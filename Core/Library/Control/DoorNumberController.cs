using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Randomness;
using Godot;

namespace Dose.Core.Library.Control
{
    public class DoorNumberController
    {
        IEnumerable<int> _avaliableNumbers = [];

        public DoorNumberController()
        {
            Reset();
        }

        public void Reset()
        {
            _avaliableNumbers = Enumerable.Range(1, 999);
        }

        public int PopRandomNumber()
        {
            if (!_avaliableNumbers.Any())
            {
                GD.PushError("No avaliable door numbers.");

                return 0;
            }

            var number = new RandomSelector<int>(_avaliableNumbers).Pick();

            _avaliableNumbers = _avaliableNumbers.Except([number]);

            return number;
        }
    }
}