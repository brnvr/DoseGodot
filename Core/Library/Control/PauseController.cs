using System;

namespace Dose.Core.Library.Control
{
    public class PauseController(IMenuContainer menuContainer)
    {
        public IMenuContainer MenuContainer { get; } = menuContainer;
        public EventHandler? Paused;
        public EventHandler? Unpaused;
        public EventHandler? Quit;

        public void Pause()
        {
            MenuContainer.DisplayMenu([
                new MenuOption("Resume", Unpause),
                new MenuOption("Quit to Windows", () => Quit?.Invoke(this, EventArgs.Empty))
            ]);
            Paused?.Invoke(this, EventArgs.Empty);
        }

        public void Unpause()
        {
            MenuContainer.HideMenu();
            Unpaused?.Invoke(this, EventArgs.Empty);
        }
    }
}