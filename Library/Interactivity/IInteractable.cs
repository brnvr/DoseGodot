using Godot;

namespace Dose.Library.Interactivity
{
    public interface IInteractable<T> where T : Node
    {
        void HandleInteraction(T other);
    }
}