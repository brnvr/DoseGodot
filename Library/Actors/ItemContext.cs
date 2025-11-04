using Dose.Library.Common;
using Godot;

namespace Dose.Library.Actors
{
    public abstract class ItemContext : IActorContext
    {
        public abstract string DisplayName { get; }
    }
}