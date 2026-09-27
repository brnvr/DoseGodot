namespace Dose.Core.Library.GameEvents
{
    public interface IGameEvent<in TTarget>
    {
        void Activate(TTarget target);
        void Deactivate(TTarget target);
        void OnTrigger(TTarget target);
        bool ShouldTrigger(TTarget target);
    }
}
