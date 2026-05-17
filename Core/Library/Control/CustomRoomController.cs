using System.Linq;
using Dose.Core.Library.Common;
using Dose.Core.Library.Locations;

namespace Dose.Core.Library.Control
{
    public class CustomRoomController(
        CustomRoomPool customRoomPool,
        ILoader<ILocation?, CustomRoomLoadArgs> customRoomLoader,
        ILocationManager locationManager,
        LocationLoadRegistry loadRegistry
    )
    {
        readonly object _lock = new();

        public LocationLoadRegistry LoadRegistry { get; } = loadRegistry;
        public CustomRoomPool CustomRoomPool { get; } = customRoomPool;
        public ILoader<ILocation?, CustomRoomLoadArgs> CustomRoomLoader { get; } = customRoomLoader;
        public ILocationManager LocationManager { get; } = locationManager;

        public bool TryLoadCustomRoom(int number, out ILocation? customRoom)
        {
            var ctx = CustomRoomPool.AvailableContexts.FirstOrDefault(c => c.RoomNumber == number);

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