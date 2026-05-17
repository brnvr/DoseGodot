using System;
using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Randomness;
using Godot;

namespace Dose.Core.Library.Control
{
    public class DoorNumberController
    {
        const int DOOR_COUNT = 999;

        readonly object _lock = new();
        IEnumerable<int> _availableNumbers = [];

        public int PlayerRoomNumber { get; private set; }

        public DoorNumberController(int? playerRoomNumber)
        {
            Reset(playerRoomNumber);
        }


        public void Reset(int? playerRoomNumber)
        {
            lock (_lock)
            {
                if (playerRoomNumber == null)
                {
                    PlayerRoomNumber = new Random().Next(1, DOOR_COUNT + 1);
                }
                else
                {
                    if (playerRoomNumber < 1 || playerRoomNumber > DOOR_COUNT)
                    {
                        GD.PushError($"{playerRoomNumber} must be an integer between 1 and {DOOR_COUNT}.");
                        return;
                    }

                    PlayerRoomNumber = (int)playerRoomNumber;
                }

                _availableNumbers = Enumerable.Range(1, DOOR_COUNT);
            }
        }

        public int PullNumber(int preference = 0)
        {
            lock (_lock)
            {
                if (preference < 0 || preference > DOOR_COUNT)
                {
                    GD.PushError($"{nameof(preference)} must be an integer between 0 and {DOOR_COUNT}.");
                    return 0;
                }

                if (!_availableNumbers.Any())
                {
                    GD.PushError("No available door numbers.");

                    return 0;
                }

                if (_availableNumbers.Count() == 1)
                {
                    return PlayerRoomNumber;
                }

                var number = preference > 0 && _availableNumbers.Contains(preference) ?
                preference :
                new RandomSelector<int>(_availableNumbers.Except([PlayerRoomNumber])).Pick();

                _availableNumbers = _availableNumbers.Except([number]);

                return number;
            }
        }
    }
}