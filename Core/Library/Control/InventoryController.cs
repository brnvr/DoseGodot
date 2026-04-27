using System;
using System.Linq;
using System.Text.Json;
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

        public Inventory Inventory { get; }
        public ItemContext? CurrentItem { get => GetNextItem(0); }
        public IUI UI { get; }
        public INotificator? Notificator { get; }
        public EventHandler<InventoryItemSetEventArgs>? ItemSet { get; set; }

        public string InventoryLabelName { get; init; } = "Inventory";
        public string CurrentItemLabelName { get; init; } = "CurrentItem";
        public string ActiveItemLabelName { get; init; } = "ActiveItem";

        public InventoryController(Inventory inventory, IUI ui, INotificator? notificator)
        {
            UI = ui;
            Inventory = inventory;
            Notificator = notificator;

            Inventory.ItemAdded += (sender, e) =>
            {
                GD.Print($"Added {e.ItemContext.DisplayName} to inventory.");

                if (UI != null)
                {
                    string json = JsonSerializer.Serialize(Inventory);
                    UI.SetLabelValue(InventoryLabelName, json);
                }

                Notificator?.Notify($"Added {e.ItemContext.DisplayName} to inventory.", 2f);
            };

            Inventory.ItemDropped += (sender, e) =>
            {
                GD.Print($"Dropped {e.ItemContext.DisplayName} from inventory.");

                if (UI != null)
                {
                    string json = JsonSerializer.Serialize(Inventory);
                    UI.SetLabelValue(InventoryLabelName, json);
                }

                UI?.SetLabelValue(CurrentItemLabelName, e.ItemContext?.DisplayName ?? "");
            };

            ItemSet += (sender, e) =>
            {
                UI?.SetLabelValue(CurrentItemLabelName, e.ItemContext?.DisplayName ?? "");
            };

            Inventory.ItemAdded += (sender, e) =>
            {
                if (Inventory.Count() == 1)
                {
                    UI?.SetLabelValue(CurrentItemLabelName, e.ItemContext?.DisplayName ?? "");
                }
            };
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

            UI.SetLabelValue(ActiveItemLabelName, CurrentItem.DisplayName);
        }

        public void OnActiveItemUnselected()
        {
            UI.SetLabelValue(ActiveItemLabelName, "");
        }
    }
}