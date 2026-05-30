using System;

namespace Dose.Core.Library.Locations
{
    public interface ILocationManager
    {
        ILocation? CurrentLocation { get; set; }
        ILocation? BuildNextLocation(IPortal portal);
        void OnPortalAccessed(IPortal portal);
    }
}