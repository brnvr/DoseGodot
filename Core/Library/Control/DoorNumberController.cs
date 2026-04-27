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

        bool _isPlayerRoomNumberAvailable = true;

        public int PlayerRoomNumber { get; private set; }
        IEnumerable<int> _availableNumbers = [];

        public DoorNumberController(int? playerRoomNumber)
        {
            Reset(playerRoomNumber);
        }

        public void Reset(int? playerRoomNumber)
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

            _isPlayerRoomNumberAvailable = true;
            _availableNumbers = Enumerable.Range(1, DOOR_COUNT).Except([PlayerRoomNumber]);
        }

        public int PopPlayerRoomNumber()
        {
            if (!_isPlayerRoomNumberAvailable)
            {
                GD.PushWarning("Player room number has already been popped.");
                return 0;
            }

            _isPlayerRoomNumberAvailable = false;

            return PlayerRoomNumber;
        }

        public int PopRandomNumber()
        {
            if (!_availableNumbers.Any())
            {
                if (_isPlayerRoomNumberAvailable)
                {
                    return PopPlayerRoomNumber();
                }

                GD.PushError("No available door numbers.");

                return 0;
            }

            var number = new RandomSelector<int>(_availableNumbers).Pick();

            _availableNumbers = _availableNumbers.Except([number]);

            return number;
        }
    }
}