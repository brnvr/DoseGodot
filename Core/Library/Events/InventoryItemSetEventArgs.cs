using Dose.Core.Library.Actors;

namespace Dose.Core.Library.Events
{
    public class InventoryItemSetEventArgs(ItemContext? itemContext) : System.EventArgs
    {
        public ItemContext? ItemContext { get; } = itemContext;
    }
}