namespace Dose.Core.Library.Packs
{
    public enum LocationRestriction
    {
        AnyLocation,
        Rooms,
        Corridors
    }

    public static class LocationRestrictionExtensions
    {
        public static bool CanPopulate(
            this IPackResource resource,
            LocationRestriction locationRestriction
        )
        {
            return resource.LocationRestriction == LocationRestriction.AnyLocation
                || resource.LocationRestriction == locationRestriction;
        }
    }
}
