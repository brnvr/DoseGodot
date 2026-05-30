using System.Linq;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.Locations;
using Godot;

namespace Dose.Core.Library.Control
{
    public abstract class CustomRoomController(
        Pool<CustomRoomContext?> customRoomPool,
        ILocationManager locationManager,
        LocationLoadRegistry loadRegistry
    )
    {
        public ILocationManager LocationManager { get; } = locationManager;
        public LocationLoadRegistry LoadRegistry { get; } = loadRegistry;
        public Pool<CustomRoomContext?> CustomRoomPool { get; } = customRoomPool;
    
        public abstract bool TryLoadCustomRoom(int number, out ICustomRoom? customRoom);
    }

    public class CustomRoomController<TCustomRoom>
    (
        Pool<CustomRoomContext?> customRoomPool,
        ILocationManager locationManager,
        LocationLoadRegistry loadRegistry,
        CustomRoomLoader<TCustomRoom> customRoomLoader
    ) : CustomRoomController(customRoomPool, locationManager, loadRegistry) where TCustomRoom : Node, ICustomRoom
    {
        readonly object _lock = new();

        
        public CustomRoomLoader<TCustomRoom> CustomRoomLoader { get; } = customRoomLoader;
        

        public override bool TryLoadCustomRoom(int number, out ICustomRoom? customRoom)
        {
            var ctx = CustomRoomPool.Content.FirstOrDefault(c => c != null && c.RoomNumber == number);

            if (ctx == null)
            {
                customRoom = null;
                return false;
            }

            customRoom = CustomRoomLoader.Load(ctx.Scene, LocationManager);

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