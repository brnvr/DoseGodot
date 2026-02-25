namespace Dose.Core.Library.Actors
{
    public interface IInitializableActor : IActor
    {
        IActor Initialize(IActorContext context);
    }
}