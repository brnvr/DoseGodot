using System.Linq;
using Dose.Core.Library.Common;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.Locations;
using Dose.Core.Scenes;

namespace Dose.Core.Library.Control
{
    public class CustomRoomController(
        Pool<CustomRoomContext?> customRoomPool,
        ILoader<ILocation?, CustomRoomLoadArgs> customRoomLoader,
        ILocationManager locationManager,
        LocationLoadRegistry loadRegistry
    )
    {
        readonly object _lock = new();

        public LocationLoadRegistry LoadRegistry { get; } = loadRegistry;
        public Pool<CustomRoomContext?> CustomRoomPool { get; } = customRoomPool;
        public ILoader<ILocation?, CustomRoomLoadArgs> CustomRoomLoader { get; } = customRoomLoader;
        public ILocationManager LocationManager { get; } = locationManager;

        public bool TryLoadCustomRoom(int number, out ILocation? customRoom)
        {
            var ctx = CustomRoomPool.Content.FirstOrDefault(c => c != null && c.RoomNumber == number);

            if (ctx == null)
            {
                customRoom = null;
                return false;
            }

            customRoom = CustomRoomLoader.Load(new CustomRoomLoadArgs
            {
                Scene = ctx.Scene,
                LocationManager = LocationManager
            });

            if (customRoom != null)
            {
                lock(_lock)
                {
                    LoadRegistry.Add(customRoom);
                }
            }

            return true;
        }
    }
}