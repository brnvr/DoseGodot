using System.Threading.Tasks;
using Dose.Core.Library.Locations;
using Godot;

namespace Dose.Core.Library.Control
{
    public class LocationController(ILocationManager locationManager, int maxLayers = 3)
    {
        public int MaxLayers { get; } = maxLayers;
        public ILocationManager LocationManager { get; } = locationManager;

        public async Task UpdateLocation<TPortal>(TPortal portal) where TPortal : IPortal
        {
            if (portal.Linked is null)
            {
                var location = LocationManager.BuildNextLocation(portal);
    
                if (location != null)
                {
                    if (location is Node locationNode && locationNode.IsInsideTree())
                    {
                        await locationNode.ToSignal(
                            locationNode.GetTree(),
                            SceneTree.SignalName.ProcessFrame
                        );
                    }

                    var other = location.SelectRandomPortal<TPortal>(linkedPortal => {
                        if (linkedPortal.Linked != null) return false;
                        if (portal is not IStairs stairs) return true;
                        if (linkedPortal is not IStairs linkedStairs) return true;
                        return stairs.Direction != linkedStairs.Direction;
                    });
                
                    if (other != null)
                    {
                        portal.Link(other);
                    }

                    LocationManager.CurrentLocation = location;
                }
            }
            else
            {
                LocationManager.CurrentLocation = portal.Linked.Location;
            }

            portal.Linked?.Location?.Clear(MaxLayers);
            
        }
    }
}
