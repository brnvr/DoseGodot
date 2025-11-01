using Dose.Library.Scenes;
using Godot;

namespace Dose.Library.Interactivity
{
    public interface IDoorOpener
    {
        void CanOpenDoor(Door door);
    }
}