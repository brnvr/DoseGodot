using Dose.Core.Library.Common;

namespace Dose.Core.Library.Packs
{
    public interface IPackResource : IHasRarity
    {
        string PackId { get; }
        LocationRestriction LocationRestriction { get; }
    }
}
