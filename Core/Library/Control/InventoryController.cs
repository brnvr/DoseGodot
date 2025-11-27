using System.Linq;
using Dose.Core.Library.Actors;
using Dose.Core.Library.DataStructures;

namespace Dose.Core.Library.Control
{
    public class InventoryController(Inventory inventory)
    {
        int _currentItemIndex;

        public Inventory Inventory { get; } = inventory;
        public ItemContext? CurrentItem { get => GetNextItem(0); }
        
        public void GoToNextItem()
        {
            _currentItemIndex = _currentItemIndex < Inventory.Count() - 1 ? _currentItemIndex + 1 : 0;
        }

        public void GoToPreviousItem()
        {
            _currentItemIndex = _currentItemIndex > 0 ? _currentItemIndex - 1 : Inventory.Count() - 1;
        }

        public ItemContext? GetNextItem(int offset)
        {
            int count = Inventory.Count();

            if (count == 0)
                return null;
                
            int index = (_currentItemIndex + offset) % count;

            if (index < 0)
                index += count;

            return Inventory[index];
        }
    }
}