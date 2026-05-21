namespace Dose.Core.Library.GameEvents
{
    public interface IGameEvent<TTarget>
    {
        void OnTrigger(TTarget target);
        bool ShouldTrigger(TTarget target);
    }
}