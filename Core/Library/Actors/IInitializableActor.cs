namespace Dose.Core.Library.Actors
{
    public interface IInitializableActor : IActor { }

    public interface IInitializableActor<TActorContext, TArgs> : IInitializableActor
    {
        void Initialize(TActorContext context, TArgs args);
    }
}