using System;
using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Locations;
using Dose.Core.Library.Randomness;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.Library.Control
{
    public class DoorNumberController
    {
        const int DOOR_COUNT = 999;

        IEnumerable<int> _availableNumbers = [];
        bool _isPlayerRoomNumberAvailable = true;

        public int PlayerRoomNumber { get; private set; }
        public CustomRoomPool CustomRoomPool { get; }
        // public Dictionary<int, IDoor> PredefinedDoors { get; }

        public DoorNumberController(int? playerRoomNumber, CustomRoomPool customRoomPool)
        {
            Reset(playerRoomNumber);
            CustomRoomPool = customRoomPool;
            // PredefinedDoors = predefinedDoors;
        }

        public Dictionary<int, IDoor> GetPredefinedDoors()
        {
            return CustomRoomPool.AvailableContexts.ToDictionary(ctx => ctx.RoomNumber, ctx => ctx.Door);
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

        public int PullPlayerRoomNumber()
        {
            if (!_isPlayerRoomNumberAvailable)
            {
                GD.PushWarning("Player room number has already been popped.");
                return 0;
            }

            _isPlayerRoomNumberAvailable = false;

            return PlayerRoomNumber;
        }

        public int PullNumber(int preference = 0)
        {
            if (preference < 0 || preference > DOOR_COUNT)
            {
                GD.PushError($"{nameof(preference)} must be an integer between 0 and {DOOR_COUNT}.");
                return 0;
            }

            if (!_availableNumbers.Any())
            {
                if (_isPlayerRoomNumberAvailable)
                {
                    return PullPlayerRoomNumber();
                }

                GD.PushError("No available door numbers.");

                return 0;
            }

            var number = preference > 0 && _availableNumbers.Contains(preference) ?
            preference :
            new RandomSelector<int>(_availableNumbers).Pick();

            _availableNumbers = _availableNumbers.Except([number]);

            return number;
        }
    }
}