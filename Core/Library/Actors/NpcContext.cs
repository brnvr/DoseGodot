namespace Dose.Core.Library.Actors
{
    public abstract record NpcContext : IActorContext
    {
        public abstract string DisplayName { get; }
    }
}