using System;
using System.Linq;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.Events;
using Dose.Core.Library.UI;
using Godot;

namespace Dose.Core.Library.Control
{
    public class InventoryController
    {
        int _currentItemIndex;
        ItemContext? _equipedItem;

        public Inventory Inventory { get; }
        public ItemContext? CurrentItem { get => GetNextItem(0); }
        public IUI UI { get; }
        public INotificator? Notificator { get; }
        public EventHandler<InventoryItemSetEventArgs>? ItemSet { get; set; }

        public InventoryController(Inventory inventory, IUI ui, INotificator? notificator)
        {
            UI = ui;
            Inventory = inventory;
            Notificator = notificator;

            Inventory.ItemAdded += (sender, e) =>
            {
                GD.Print($"Added {e.ItemContext.DisplayName} to inventory.");
                UpdateInventoryUI(e.ItemContext);
                Notificator?.Notify($"Added {e.ItemContext.DisplayName} to inventory.", 2f);
            };

            Inventory.ItemDropped += (sender, e) =>
            {
                GD.Print($"Dropped {e.ItemContext.DisplayName} from inventory.");
                UpdateInventoryUI();
            };

            ItemSet += (sender, e) => UpdateInventoryUI();
        }

        public void GoToFirstItem()
        {
            _currentItemIndex = 0;

            ItemSet?.Invoke(this, new InventoryItemSetEventArgs(CurrentItem));
        }

        public void GoToLastItem()
        {
            if (!Inventory.Any())
                return;

            _currentItemIndex = Inventory.Count() - 1;
            
            ItemSet?.Invoke(this, new InventoryItemSetEventArgs(CurrentItem));
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

            ItemSet?.Invoke(this, new InventoryItemSetEventArgs(CurrentItem));
        }
        
        public void GoToNextItem()
        {
            if (!Inventory.Any())
                return;

            _currentItemIndex = _currentItemIndex < Inventory.Count() - 1 ? _currentItemIndex + 1 : 0;

            ItemSet?.Invoke(this, new InventoryItemSetEventArgs(CurrentItem));
        }

        public void GoToPreviousItem()
        {
            if (!Inventory.Any())
                return;

            _currentItemIndex = _currentItemIndex > 0 ? _currentItemIndex - 1 : Inventory.Count() - 1;

            ItemSet?.Invoke(this, new InventoryItemSetEventArgs(CurrentItem));
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

        public void OnActiveItemSet()
        {
            if (CurrentItem == null)
            {
                GD.PushWarning($"{nameof(CurrentItem)} is null. Skipping {nameof(OnActiveItemSet)}...");
                return;
            }

            _equipedItem = CurrentItem;
            UpdateInventoryUI();
        }

        public void OnActiveItemUnselected()
        {
            _equipedItem = null;
            UpdateInventoryUI();
        }

        void UpdateInventoryUI(ItemContext? pickedItem = null)
        {
            UI.UpdateInventory(Inventory.Entries, CurrentItem, _equipedItem, pickedItem);
        }
    }
}
