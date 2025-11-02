using Dose.SceneScripts;
using Godot;

namespace Dose.Library.Interactivity
{
    public interface IDoorOpener
    {
        void CanOpenDoor(Door door);
    }
}