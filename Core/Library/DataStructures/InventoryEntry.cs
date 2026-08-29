using Dose.Core.Library.Actors;

namespace Dose.Core.Library.DataStructures
{
    public readonly record struct InventoryEntry(ItemContext Item, int Quantity);
}
