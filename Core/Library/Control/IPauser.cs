namespace Dose.Core.Library.Control
{
    public interface IPauser
    {
        bool IsPaused { get; }
        void Pause();
        void Unpause();
    }
}