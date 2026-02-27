using System.Linq;
using System.Text.Json;
using Dose.Core.Library.Actors;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.UI;
using Godot;

namespace Dose.Core.Library.Control
{
    public class InventoryController
    {
        int _currentItemIndex;

        public Inventory Inventory { get; }
        public ItemContext? CurrentItem { get => GetNextItem(0); }
        IUI? UI { get; }

        public InventoryController(Inventory inventory, IUI? ui)
        {
            UI = ui;
            Inventory = inventory;

            Inventory.ItemAdded += (sender, e) =>
            {
                GD.Print($"Added {e.ItemContext.DisplayName} to inventory.");

                if (UI != null)
                {
                    string json = JsonSerializer.Serialize(Inventory);

                    UI.SetLabelValue("Inventory", json);
                }
            };

            Inventory.ItemDropped += (sender, e) =>
            {
                GD.Print($"Dropped {e.ItemContext.DisplayName} from inventory.");

                if (e.ItemContext == CurrentItem)
                {
                    GoToPreviousItem();
                }
            };
        }

        public void GoToFirstItem()
        {
            _currentItemIndex = 0;
        }

        public void GoToLastItem()
        {
            if (!Inventory.Any())
                return;

            _currentItemIndex = Inventory.Count() - 1;
        }

        public void GoToItem(ItemContext itemContext)
        {
            var index = Inventory
                .Select((item, index) => new { item, index })
                .FirstOrDefault(x => x.item == itemContext)
                ?.index;

            if (index != null)
            {
                _currentItemIndex = (int)index;
            }
        }
        
        public void GoToNextItem()
        {
            if (!Inventory.Any())
                return;

            _currentItemIndex = _currentItemIndex < Inventory.Count() - 1 ? _currentItemIndex + 1 : 0;
        }

        public void GoToPreviousItem()
        {
            if (!Inventory.Any())
                return;

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