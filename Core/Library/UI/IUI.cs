using System.Collections.Generic;
using Dose.Core.Library.Control;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.Actors;

namespace Dose.Core.Library.UI
{
    public interface IUI : IMenuContainer
    {
        void SetLabelValue(string labelName, string text);
        void SetCursorVisible(bool visible);
        void SetActorName(string? name);
        void SetActorNameVisible(bool visible);
        void ShowDialogueLine(string text);
        void HideDialogue();
        void UpdateInventory(
            IEnumerable<InventoryEntry> entries,
            ItemContext? selectedItem,
            ItemContext? equipedItem
        );
    }
}
