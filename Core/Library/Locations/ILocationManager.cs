using System;
using Dose.Core.Library.Location;

namespace Dose.Core.Library.Locations
{
    public interface ILocationManager
    {
        ILocation? CurrentLocation { get; set; }
        ILocation? BuildNextLocation(IPortal portal);
        bool PortalPredicate(IPortal portal, IPortal linkedPortal);
    }
}