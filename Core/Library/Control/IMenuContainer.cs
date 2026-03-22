using System.Collections.Generic;

namespace Dose.Core.Library.Control
{
    public interface IMenuContainer
    {
        void DisplayMenu(IEnumerable<MenuOption> options);
        void HideMenu();
    }
}