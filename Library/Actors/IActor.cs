using Godot;

namespace Dose.Library.Actors
{
    public interface IActor { }

    public interface IActor<T> : IActor
    {
        void HandleInteraction(T other);
    }
}